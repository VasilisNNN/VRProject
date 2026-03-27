using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Nav3D.Common;
using Nav3D.Obstacles.Serialization;
using Plane = Nav3D.LocalAvoidance.SupportingMath.Plane;

namespace Nav3D.Obstacles
{
    public class Leaf : Node
    {
        #region Constructors

        public Leaf(
            Octree _Octree,
            SpatialGrid _SpatialGrid,
            Bounds _Bounds,
            int _GridLayer,
            bool _Occupied,
            Func<int> _GetID)
            :
            base(
                _Octree,
                _Bounds,
                _GridLayer,
                _Occupied,
                _GetID)
        {
            m_Index = _SpatialGrid.AddLeafOnLayer(_GridLayer, this);
            _SpatialGrid.SubstractLeafFromProgress(_GridLayer, true);
        }

        public Leaf(
            Bounds _Bounds,
            int _GridLayer,
            bool _Occupied,
            int _ID)
            : base(
                  _Bounds,
                  _GridLayer,
                  _Occupied,
                  _ID)
        {
        }

        #endregion

        #region Attributes

        HashSet<Leaf> m_FreeAdjacents = new HashSet<Leaf>();
        Vector3Int m_Index;

        #endregion

        #region Properties

        public bool Occupied => m_Occupied;
        public HashSet<Leaf> FreeAdjacents => m_FreeAdjacents;
        public float Size => Bounds.size.x;
        public List<Vector3Int> FacesDirections { get; } = new List<Vector3Int> {
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.forward,
            Vector3Int.back
        };

        #endregion

        #region Public methods

        public override Leaf GetEmbracingLeaf(Vector3 _Point)
        {
            //default case
            if (!m_Bounds.Contains(_Point))
                return null;
            
            //rare case
            //it's possible that due to inaccurate bounds edge detection occupied leaf was determined as containing the _Point
            //in such case wee need to find adjacent free leaf closest to _Point
            if (m_Occupied)
            {
                //determines the closest face
                Plane closestFace = null;
                float minToFaceDistance = float.MaxValue;

                float halfSize = Bounds.extents.x;

                //find bounds face closest to _Point
                foreach (Vector3Int faceNormal in UtilsMath.BucketFacesNormals)
                {
                    Vector3 origin = Bounds.center + new Vector3(faceNormal.x * halfSize, faceNormal.y * halfSize, faceNormal.z * halfSize);
                    Plane face = new Plane(faceNormal, origin);
                    float toFaceDistance = Mathf.Abs(face.Distance(_Point));

                    if (toFaceDistance < minToFaceDistance)
                    {
                        minToFaceDistance = toFaceDistance;
                        closestFace = face;
                    }
                }

                //get mirrored outside point for found face
                Vector3 outsidePoint = closestFace!.GetClosestPoint(_Point) + closestFace.Normal * minToFaceDistance;
                
                //find adjacent free leaf, that contains outside point
                Leaf result = FreeAdjacents.FirstOrDefault(_AdjacentLeaf => _AdjacentLeaf.Bounds.Contains(outsidePoint));
                
                return result;
            }

            return this;
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
            base.FillGizmosDrawData(
                _GizmosDrawData,
                _DrawOccupiedLeaves,
                _DrawFreeLeaves,
                _DrawGraph,
                _DrawOccupiedLeavesAll,
                _DrawOccupiedLeavesLayerNumber,
                _DrawFreeLeavesAll,
                _DrawFreeLeavesLayerNumber,
                _DrawGraphNodesAll,
                _DrawGraphLayerNumber);

            if (m_Occupied || !_DrawGraph)
                return;

            if (_DrawGraphNodesAll == 0 || _DrawGraphLayerNumber == m_GridLayer)
            {
                _GizmosDrawData.Add(new Common.Debug.GizmosSphereSolid(m_Bounds.center, m_Bounds.size.x * 0.05f, Color.magenta));

                foreach (Leaf leaf in m_FreeAdjacents)
                {
                    Vector3 contactPoint = GetAdjacentContactPointInternal(leaf);
                    _GizmosDrawData.Add(new Common.Debug.GizmosLine(m_Bounds.center, contactPoint, Color.magenta));
                    _GizmosDrawData.Add(new Common.Debug.GizmosLine(contactPoint, leaf.m_Bounds.center, Color.magenta));
                }
            }
        }
#endif

        public void AddFreeAdjacent(Leaf _Adjacent)
        {
            m_FreeAdjacents.Add(_Adjacent);
        }

        public Vector3 GetAdjacentContactPoint(Leaf _OtherLeaf)
        {
            return GetAdjacentContactPointInternal(_OtherLeaf);
        }

        public void RemoveFaceConsideration(Vector3Int _Face)
        {
            FacesDirections.Remove(_Face);
        }

        public void SetFreeAdjacents(Dictionary<int, Node> _NodeMap, int[] _FreeAdjacentIDs)
        {
            foreach (int id in _FreeAdjacentIDs)
                if (_NodeMap.TryGetValue(id, out Node node))
                    m_FreeAdjacents.Add((Leaf)node);
        }

        public override void SetOctreeReference(Octree _Octree)
        {
            base.SetOctreeReference(_Octree);

            m_Index = m_Octree.GetBucketIndexOnLayer(m_GridLayer, this);
        }

        #endregion

        #region Service methods

        Vector3 GetAdjacentContactPointInternal(Leaf _OtherLeaf)
        {
            float size = m_Bounds.size.x;

            if (m_GridLayer == _OtherLeaf.m_GridLayer)
            {
                if (m_Index.x != _OtherLeaf.m_Index.x)
                {
                    if (m_Index.x < _OtherLeaf.m_Index.x)
                        return m_Bounds.center + new Vector3(size * 0.5f, 0, 0);
                    else
                        return m_Bounds.center + new Vector3(-size * 0.5f, 0, 0);
                }

                if (m_Index.y != _OtherLeaf.m_Index.y)
                {
                    if (m_Index.y < _OtherLeaf.m_Index.y)
                        return m_Bounds.center + new Vector3(0, size * 0.5f, 0);
                    else
                        return m_Bounds.center + new Vector3(0, -size * 0.5f, 0);
                }

                if (m_Index.z != _OtherLeaf.m_Index.z)
                {
                    if (m_Index.z < _OtherLeaf.m_Index.z)
                        return m_Bounds.center + new Vector3(0, 0, size * 0.5f);
                    else
                        return m_Bounds.center + new Vector3(0, 0, -size * 0.5f);
                }

                return m_Bounds.center;
            }

            Leaf smallerLeaf;
            Leaf biggerLeaf;

            Vector3Int smallerIndex;
            Vector3Int biggerIndex;

            int layersDelta = Mathf.Abs(m_GridLayer - _OtherLeaf.m_GridLayer);

            if (m_GridLayer < _OtherLeaf.m_GridLayer)
            {
                smallerLeaf = _OtherLeaf;
                biggerLeaf = this;
            }
            else
            {
                smallerLeaf = this;
                biggerLeaf = _OtherLeaf;
            }

            smallerIndex = smallerLeaf.m_Index;
            biggerIndex = biggerLeaf.m_Index;

            //equalize layers for leaves
            for (int i = 0; i < layersDelta; i++)
                smallerIndex = SpatialGrid.GetLevelUpIndex(smallerIndex);

            float smallerLeafSize = smallerLeaf.Size;

            if (smallerIndex.x != biggerIndex.x)
            {
                if (smallerIndex.x < biggerIndex.x)
                    return smallerLeaf.m_Bounds.center + new Vector3(smallerLeafSize * 0.5f, 0, 0);
                else
                    return smallerLeaf.m_Bounds.center + new Vector3(-smallerLeafSize * 0.5f, 0, 0);
            }

            if (smallerIndex.y != biggerIndex.y)
            {
                if (smallerIndex.y < biggerIndex.y)
                    return smallerLeaf.m_Bounds.center + new Vector3(0, smallerLeafSize * 0.5f, 0);
                else
                    return smallerLeaf.m_Bounds.center + new Vector3(0, -smallerLeafSize * 0.5f, 0);
            }

            if (smallerIndex.z != biggerIndex.z)
            {
                if (smallerIndex.z < biggerIndex.z)
                    return smallerLeaf.m_Bounds.center + new Vector3(0, 0, smallerLeafSize * 0.5f);
                else
                    return smallerLeaf.m_Bounds.center + new Vector3(0, 0, -smallerLeafSize * 0.5f);
            }

            Debug.LogError($"{nameof(GetAdjacentContactPointInternal)}: Positive infinity. {smallerLeaf.m_GridLayer} {biggerLeaf.m_GridLayer}, {smallerIndex} {biggerIndex}");

            return Vector3.positiveInfinity;
        }

        protected override NodeSerializable GetSerializableInstance()
        {
            return new LeafSerializable(m_Bounds, (byte)m_GridLayer, Occupied, m_FreeAdjacents.Select(_Adjacent => _Adjacent.ID).ToArray(), m_ID);
        }

        #endregion
    }
}