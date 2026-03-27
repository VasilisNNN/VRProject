using Nav3D.Obstacles.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine;

namespace Nav3D.Obstacles
{
    public class Fork : Node
    {
        #region Constructors

        public Fork(
            Octree _Octree,
            SpatialGrid _SpatialGrid,
            TriangleStorage _ObstacleTriangles,
            Bounds _Bounds,
            int _GridLayer,
            bool _Occupied,
            Func<int> _GetID,
            int _MaxGridLayer,
            bool _NeedCheckMaxLayer,
            CancellationToken _CancellationToken,
            int _ParallelFactor)
            :
            base(
                _Octree,
                _Bounds,
                _GridLayer,
                _Occupied,
                _GetID)
        {
            if (_CancellationToken.IsCancellationRequested)
                return;

            ProduceChilds(_MaxGridLayer, _GetID, _SpatialGrid, _ObstacleTriangles, _CancellationToken, _NeedCheckMaxLayer, _ParallelFactor);
        }
        public Fork(
            Bounds _Bounds,
            int _GridLayer,
            bool _Occupied,
            int _ID)
            : base(
                  _Bounds,
                  _GridLayer,
                  _Occupied,
                  _ID)
        { }

        #endregion

        #region Attributes

        List<Node> m_Childs = new List<Node>(8);

        #endregion

        #region Properties

        public List<Node> ChildNodes => m_Childs;

        #endregion

        #region Public methods

        public override Leaf GetEmbracingLeaf(Vector3 _Point)
        {
            if (!m_Bounds.Contains(_Point))
                return null;

            foreach(Node child in m_Childs)
            {
                if (!child.Bounds.Contains(_Point))
                    continue;

                return child.GetEmbracingLeaf(_Point);
            }

            return null;
        }

#if UNITY_EDITOR
        public override void FillGizmosDrawData(
            Common.Debug.GizmosDrawData _GizmosDrawData,
            bool _DrawOccupiedLeaves,
            bool _DrawFreeLeaves,
            bool _DrawGraph,
            int _DrawOccupiedLeavesAll,
            int _DrawOccupiedLeavesLayerNumber,
            int _DrawFreeLeavesAll,
            int _DrawFreeLeavesLayerNumber,
            int _DrawGraphNodesAll,
            int _DrawGraphLayerNumber)
        {
            m_Childs.ForEach(
                _Bucket => _Bucket.FillGizmosDrawData(
                    _GizmosDrawData,
                    _DrawOccupiedLeaves,
                    _DrawFreeLeaves,
                    _DrawGraph,
                    _DrawOccupiedLeavesAll,
                    _DrawOccupiedLeavesLayerNumber,
                    _DrawFreeLeavesAll,
                    _DrawFreeLeavesLayerNumber,
                    _DrawGraphNodesAll,
                    _DrawGraphLayerNumber)
                );
        }
#endif

        public override void GetSerializableInstances(List<NodeSerializable> _Nodes, ObstacleSerializingProgress _Progress)
        {
            base.GetSerializableInstances(_Nodes, _Progress);

            if (_Progress.CancellationToken.IsCancellationRequested)
                return;

            _Progress.SetNodesPackingProgress(_Nodes.Count, m_Octree.NodeCount);

            foreach (Node child in m_Childs)
                child.GetSerializableInstances(_Nodes, _Progress);
        }

        public void SetChilds(Dictionary<int, Node> _NodeMap, int[] _ChildIDs)
        {
            foreach (int id in _ChildIDs)
                if (_NodeMap.TryGetValue(id, out Node node))
                    m_Childs.Add(node);
        }

        #endregion

        #region Service methods

        void ProduceChilds(int _MaxGridLayer, Func<int> _GetID, SpatialGrid _SpatialGrid, TriangleStorage _ObstacleTriangles, CancellationToken _CancellationToken, bool _NeedCheckMaxLayer, int _ParallelFactor)
        {
            bool parallelChildProduce;

            if (parallelChildProduce = _ParallelFactor > 0)
                _ParallelFactor--;

            List<Task> taskSet = new List<Task>();

            int inParentIndex = 0;

            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 halfSize = m_Bounds.size * 0.5f;

                        Vector3Int indices = new Vector3Int(x, y, z);

                        Vector3 offset = indices;
                        offset.Scale(m_Bounds.size);
                        offset *= 0.25f;

                        Vector3 center = m_Bounds.center + offset;

                        Bounds newBounds = new Bounds(center, halfSize);

                        //check if bucket is out of obstacle bounds
                        if (!m_Octree.ObstacleInfo.Bounds.Intersects(newBounds))
                        {
                            _SpatialGrid.SubstractLeafFromProgress(m_GridLayer + 1, false);
                            continue;
                        }

                        bool needCheckMaxLayer = _NeedCheckMaxLayer && ObstacleParticularResolutionManager.Instance.HasCrossingBoundables(newBounds);
                        int maxLayer = _MaxGridLayer;

                        if (needCheckMaxLayer)
                        {
                            if (m_Octree.TryGetEmbracingRegionsMinRes(newBounds, out float bucketSize, out int regionsCount))
                            {
                                //if whole node is inside of all regions then stop checking for
                                if (ObstacleParticularResolutionManager.Instance.GetCrossingBoundablesCount(newBounds) == regionsCount)
                                {
                                    maxLayer = m_Octree.GetLayersCount(bucketSize, out _) - 1;
                                    needCheckMaxLayer = false;
                                }
                            }
                        }

                        if (!parallelChildProduce)
                            m_Childs.Add(
                                Create(
                                    m_Octree,
                                    _SpatialGrid,
                                    _ObstacleTriangles,
                                    newBounds,
                                    m_GridLayer + 1,
                                    IsOccupy(_ObstacleTriangles, newBounds),
                                    _GetID,
                                    maxLayer,
                                    needCheckMaxLayer,
                                    _CancellationToken,
                                    _ParallelFactor));
                        else
                        {
                            taskSet.Add(Task.Run(() =>
                            {
                                Node newNode = Create(
                                    m_Octree,
                                    _SpatialGrid,
                                    _ObstacleTriangles,
                                    newBounds,
                                    m_GridLayer + 1,
                                    IsOccupy(_ObstacleTriangles, newBounds),
                                    _GetID,
                                    maxLayer,
                                    needCheckMaxLayer,
                                    _CancellationToken,
                                    _ParallelFactor);

                                lock (m_Childs)
                                    m_Childs.Add(newNode);
                            }));
                        }

                        inParentIndex++;
                    }

            if (parallelChildProduce)
                Task.WaitAll(taskSet.ToArray());
        }

        protected override NodeSerializable GetSerializableInstance()
        {
            return new ForkSerializable(m_Bounds, (byte)m_GridLayer, m_Occupied, m_Childs.Select(_Child => _Child.ID).ToArray(), m_ID);
        }

        #endregion
    }
}