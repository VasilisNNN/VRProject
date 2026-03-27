using Nav3D.API;
using Nav3D.Common;
using Nav3D.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Nav3D.Obstacles.Serialization;
using ConstructionStatus = Nav3D.Obstacles.GraphConstructionProgress.ConstructionStatus;

namespace Nav3D.Obstacles
{
    public partial class Octree
    {
        #region Constants

        readonly string EMPTY_LEAVES_ERROR = $"[{nameof(Octree)}]:{nameof(FindPathInternal)}: There is no embracing leaf: A: {0}, ALeaf is null: {1}, B: {2}, BLeaf is null: {3}";

        #endregion

        #region Constructors

        public Octree(ObstacleInfoBase _ObstacleInfo, float _MinBucketSize, CancellationToken _CancellationToken, GraphConstructionProgress _ConstructionProgress = null)
        {
            ObstacleInfo = _ObstacleInfo;
            ConstructionProgress = _ConstructionProgress;

            BuildTree(_MinBucketSize, _CancellationToken);
        }

        public Octree(ObstacleInfoBase _ObstacleInfo, Node[] _Roots, int _LayersCount, float _MinBucketSizeBase, float _MinBucketSizeReal, int _NodesCount)
        {
            ObstacleInfo = _ObstacleInfo;
            m_Roots = _Roots;

            LayersCount = _LayersCount;
            m_MinBucketSizeBase = _MinBucketSizeBase;
            m_MinBucketSizeReal = _MinBucketSizeReal;
            
            NodeCount = _NodesCount;

            ConstructionProgress = GraphConstructionProgress.COMPLETED;
        }

        #endregion

        #region Attributes

        Node[] m_Roots;

        //min bucket size given at the octree init;
        float m_MinBucketSizeBase;
        //factual min bucket size taking into account particular regions min bucket size
        float m_MinBucketSizeReal;

        volatile int m_CurNodeID = -1;

        #endregion

        #region Properties

        public ObstacleInfoBase ObstacleInfo { get; private set; }
        public int LayersCount { get; private set; }
        public int NodeCount { get; private set; }

        public GraphConstructionProgress ConstructionProgress { get; private set; }

        int MaxLayer => LayersCount - 1;

        #endregion

        #region Public methods : Runtime operating

        public OctreePathfindingResult FindPath(Vector3 _PointA, Vector3 _PointB, CancellationToken _CancellationTokenExternal, CancellationToken _CancellationTokenTimeout)
        {
            return FindPathInternal(_PointA, _PointB, _CancellationTokenExternal, _CancellationTokenTimeout);
        }

        public bool RayIntersectOccupiedLeaf(Common.Ray _Ray)
        {
            return m_Roots.Any(_Root => RayIntersectOccupiedLeaf(_Root, _Ray));
        }

#if UNITY_EDITOR
        public void FillGizmosDrawData(
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
            m_Roots.ForEach(
                _Node => _Node.FillGizmosDrawData(
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

            _GizmosDrawData.Add(new Common.Debug.GizmosWireCube(ObstacleInfo.Bounds.center, ObstacleInfo.Bounds.size, Color.white));
        }
#endif

#endregion

        #region Public methods : Construction

        public Vector3Int GetBucketIndexOnLayer(int _Layer, Leaf _Leaf)
        {
            return UtilsMath.GetBucketIndex(_Leaf.Bounds.center, GetBucketSizeOnLayer(_Layer));
        }

        public float GetCrossingRegionsMinRes(Bounds _Bounds)
        {
            float particularMinBucketSize;

            if (ObstacleParticularResolutionManager.Instance.TryGetCrossingRegions(_Bounds, out HashSet<Nav3DParticularResolutionRegion> _Regions))
            {
                particularMinBucketSize = _Regions.Min(_Region => _Region.MinBucketSize);
            }
            else
            {
                return m_MinBucketSizeBase;
            }

            return GetClosestBucketSize(particularMinBucketSize, m_MinBucketSizeBase);
        }

        public bool TryGetEmbracingRegionsMinRes(Bounds _Bounds, out float _BucketSize, out int _RegionsCount)
        {
            float particularMinBucketSize;

            if (ObstacleParticularResolutionManager.Instance.TryGetEmbracingRegions(_Bounds, out HashSet<Nav3DParticularResolutionRegion> _Regions))
            {
                particularMinBucketSize = _Regions.Min(_Region => _Region.MinBucketSize);
                _RegionsCount = _Regions.Count;
            }
            else
            {
                _BucketSize = m_MinBucketSizeBase;
                _RegionsCount = 0;
                return false;
            }

            _BucketSize = GetClosestBucketSize(particularMinBucketSize, m_MinBucketSizeBase);
            return true;
        }

        public int GetLayersCount(float _MinBucketSize, out float _BoundsSize)
        {
            return GetLayersCount(ObstacleInfo.Bounds.GetMaxSize(), _MinBucketSize, out _BoundsSize);
        }

        public OctreeSerializable GetSerializableInstance(ObstacleSerializingProgress _Progress, int _ID)
        {
            NodeSerializable[] nodesSerializable = GetNodesSerializable(_Progress);

            return new OctreeSerializable(
                nodesSerializable,
                m_Roots.Select(_Root => _Root.ID).ToList(),
                LayersCount,
                m_MinBucketSizeBase,
                m_MinBucketSizeReal,
                _ID
                );
        }

        #endregion

        #region Service methods

        int GetNextNodeID()
        {
            return Interlocked.Increment(ref m_CurNodeID);
        }

        float GetBucketSizeOnLayer(int _Layer)
        {
            return m_MinBucketSizeReal * Mathf.Pow(2, LayersCount - _Layer - 1);
        }

        NodeSerializable[] GetNodesSerializable(ObstacleSerializingProgress _Progress)
        {
            //After the octree is built the m_CurNodeID stores the last created node number, so the total nodes count is m_CurNodeID + 1
            List<NodeSerializable> nodes = new List<NodeSerializable>(m_CurNodeID + 1);

            foreach (Node root in m_Roots)
                root.GetSerializableInstances(nodes, _Progress);

            return nodes.ToArray();
        }

        void BuildTree(float _MinBucketSize, CancellationToken _CancellationToken)
        {
            m_MinBucketSizeBase = _MinBucketSize;

            ConstructionProgress.SetStatus(ConstructionStatus.FILLING_TRIANGLE_STORAGE);

            TriangleStorage triangleStorage = new TriangleStorage(ObstacleInfo.Triangles, ConstructionProgress);

            ConstructionProgress.SetStatus(ConstructionStatus.TREE_CONSTRUCTION_PREPARATION);

            float particularMinBucketSize = _MinBucketSize;

            int baseLayersCount, particularLayersCount;

            particularLayersCount = baseLayersCount = GetLayersCount(m_MinBucketSizeBase, out float boundsSize);

            bool hasCrossingParticularResRegions = ObstacleParticularResolutionManager.Instance.HasCrossingBoundables(ObstacleInfo.Bounds);

            if (hasCrossingParticularResRegions)
            {
                particularMinBucketSize = GetCrossingRegionsMinRes(ObstacleInfo.Bounds);

                if (particularMinBucketSize != m_MinBucketSizeBase)
                    particularLayersCount = GetLayersCount(particularMinBucketSize, out _);
            }

            //More levels => higher detail
            if (hasCrossingParticularResRegions && particularLayersCount > baseLayersCount)
            {
                LayersCount = particularLayersCount;
                m_MinBucketSizeReal = particularMinBucketSize;
            }
            else
            {
                LayersCount = baseLayersCount;
                m_MinBucketSizeReal = m_MinBucketSizeBase;
            }

            SpatialGrid spatialGrid = new SpatialGrid(LayersCount, m_MinBucketSizeReal, ConstructionProgress);

            Bounds[] boundsInitial = GetInitialBounds(boundsSize);

            int parallelFactor = (int)(Mathf.Log(
                Mathf.ClosestPowerOfTwo((int)Mathf.Ceil(Mathf.Ceil(Mathf.Pow(2, baseLayersCount) * m_MinBucketSizeBase / m_MinBucketSizeBase) / 10f)), 2)
                ) - 1;

            ConstructionProgress.SetStatus(ConstructionStatus.TREE_CONSTRUCTION);

            BuildRoots(boundsInitial, triangleStorage, spatialGrid, baseLayersCount, parallelFactor, _CancellationToken);

            ConstructionProgress.SetStatus(ConstructionStatus.GRAPH_CONNECTIONS_BUILDING);

            spatialGrid.FormLeavesConnections(ConstructionProgress);

            NodeCount = m_CurNodeID + 1;

            ConstructionProgress.SetStatus(ConstructionStatus.FINISHED);
        }

        void BuildRoots(Bounds[] _BoundsInitial, TriangleStorage _TriangleStorage, SpatialGrid _SpatialGrid, int _BaseLayersCount, int _ParallelFactor, CancellationToken _CancellationToken)
        {
            try
            {
                ConstructionProgress.SetTotalRootsCount(GetTreeMaxPower(_BoundsInitial.Length, MaxLayer));

                int maxLayer = _BaseLayersCount - 1;

                if (_BoundsInitial.Length > 1)
                {
                    List<Task> taskSet = new List<Task>(_BoundsInitial.Length);
                    List<Node> roots = new List<Node>(_BoundsInitial.Length);

                    for (int i = 0; i < _BoundsInitial.Length; i++)
                    {
                        Bounds rootBounds = _BoundsInitial[i];

                        taskSet.Add(Task.Run(() =>
                        {
                            bool needCheckMaxLayer = ObstacleParticularResolutionManager.Instance.HasCrossingBoundables(rootBounds);

                            if (needCheckMaxLayer && TryGetEmbracingRegionsMinRes(rootBounds, out float bucketSize, out int regionsCount))
                            {
                                //if whole fork is inside of all regions then stop checking for childs
                                if (ObstacleParticularResolutionManager.Instance.GetCrossingBoundablesCount(rootBounds) == regionsCount)
                                {
                                    maxLayer = GetLayersCount(bucketSize, out _) - 1;
                                    needCheckMaxLayer = false;
                                }
                            }

                            Node root = Node.Create(
                                this,
                                _SpatialGrid,
                                _TriangleStorage,
                                rootBounds,
                                0,
                                true,
                                GetNextNodeID,
                                maxLayer,
                                needCheckMaxLayer,
                                _CancellationToken,
                                _ParallelFactor
                                );

                            lock (roots)
                                roots.Add(root);
                        }));
                    }

                    Task.WaitAll(taskSet.ToArray());

                    m_Roots = roots.ToArray();
                }
                else
                {
                    Bounds bounds = _BoundsInitial.First();

                    bool needCheckMaxLayer = ObstacleParticularResolutionManager.Instance.HasCrossingBoundables(bounds);

                    if (needCheckMaxLayer && TryGetEmbracingRegionsMinRes(bounds, out float bucketSize, out int regionsCount))
                    {
                        //if whole fork is inside of all regions then stop checking for childs
                        if (ObstacleParticularResolutionManager.Instance.GetCrossingBoundablesCount(bounds) == regionsCount)
                        {
                            maxLayer = GetLayersCount(bucketSize, out _) - 1;
                            needCheckMaxLayer = false;
                        }
                    }

                    m_Roots = new Node[] { Node.Create(
                        this,
                        _SpatialGrid,
                        _TriangleStorage,
                        bounds,
                        0,
                        true,
                        GetNextNodeID,
                        maxLayer,
                        needCheckMaxLayer,
                        _CancellationToken,
                        _ParallelFactor)
                    };
                }
            }
            catch(System.Exception _Exception)
            {
                Debug.LogException(_Exception);
            }
        }

        int GetTreeMaxPower(int _RootsCount, int _MaxLayer)
        {
            //Octree dimension is 8, layers are numbered starting from 0
            return _RootsCount * (int)Mathf.Pow(8, _MaxLayer);
        }

        public Leaf GetEmbracingLeaf(Vector3 _Point)
        {
            foreach (Node node in m_Roots)
            {
                if (!node.Bounds.Contains(_Point))
                    continue;

                return node.GetEmbracingLeaf(_Point);
            }

            return null;
        }

        OctreePathfindingResult FindPathInternal(
            Vector3 _PointA,
            Vector3 _PointB,
            CancellationToken _CancellationTokenExternal,
            CancellationToken _CancellationTokenTimeout)
        {
            Leaf startNode = GetEmbracingLeaf(_PointA);
            Leaf goalNode = GetEmbracingLeaf(_PointB);

            if (startNode == null || goalNode == null)
            {
                Debug.LogWarning(string.Format(EMPTY_LEAVES_ERROR, _PointA.ToStringExt(), startNode == null, _PointB.ToStringExt(), goalNode == null));

                return new OctreePathfindingResult(new List<Vector3> { _PointA, _PointB }, PathfindingResultCode.SUCCEEDED);
            }

            if (startNode.Occupied)
                return new OctreePathfindingResult(null, PathfindingResultCode.START_POINT_INSIDE_OBSTACLE);

            if (goalNode.Occupied)
                return new OctreePathfindingResult(null, PathfindingResultCode.GOAL_POINT_INSIDE_OBSTACLE);

            //A* execution. Obtaining adjacent leaves sequence.
            AStar pathResolver = new AStar
            {
                StartLeaf = startNode,
                GoalLeaf = goalNode,
                CancellationTokenControl = _CancellationTokenExternal,
                CancellationTokenTimeout = _CancellationTokenTimeout
            };

            PathResolverResult result = pathResolver.GetPath();

            if (result.ResultCode != PathfindingResultCode.SUCCEEDED)
                return new OctreePathfindingResult(null, result.ResultCode);

            List<Leaf> leaves = result.Path;
            List<Vector3> points = new List<Vector3> { _PointA };

            //construct path by contact points at adjacent leaves.
            if (leaves.Any())
            {
                for (int i = 0; i < leaves.Count - 1; i++)
                {
                    Leaf curLeaf = leaves[i];
                    Leaf nextLeaf = leaves[i + 1];

                    points.Add(curLeaf.Bounds.center);
                    points.Add(curLeaf.GetAdjacentContactPoint(nextLeaf));
                }

                points.Add(leaves.Last().Bounds.center);
            }

            points.Add(_PointB);

            return new OctreePathfindingResult(points, PathfindingResultCode.SUCCEEDED);
        }

        bool RayIntersectOccupiedLeaf(Node _CurrentNode, Common.Ray _Ray)
        {
            if (_CurrentNode.Bounds.IntersectRay(_Ray))
            {
                if (_CurrentNode is Leaf leaf)
                    return leaf.Occupied;

                foreach (Node child in (_CurrentNode as Fork).ChildNodes)
                {
                    if (RayIntersectOccupiedLeaf(child, _Ray))
                        return true;
                }
            }

            return false;
        }

        Bounds[] GetInitialBounds(float _BoundSize)
        {
            return ObstacleInfo.Bounds.GetCornerPoints().Select(_Corner => UtilsMath.GetBucket(_Corner, _BoundSize)).Distinct().ToArray();
        }

        //determines the number of the layer, that has greatest bucket size less then the smallest bucket size among all resolution regions that embraces the given bounds
        float GetClosestBucketSize(float _Resolution, float _MinBucketSize)
        {
            float minBucketSize = _MinBucketSize;
            float multiplier;

            if (_Resolution < minBucketSize)
            {
                multiplier = 0.5f;

                do
                {
                    minBucketSize *= multiplier;
                } while (minBucketSize > _Resolution);
            }
            else if (_Resolution > minBucketSize)
            {
                multiplier = 2f;

                while (minBucketSize < _Resolution)
                {
                    minBucketSize *= multiplier;
                }
            }

            return minBucketSize;
        }

        int GetLayersCount(float _MaxBucketSize, float _MinBucketSize, out float _BoundsSize)
        {
            int currentLayerNum = 1;
            float currentBucketSize = _MinBucketSize;

            while (currentBucketSize < _MaxBucketSize && currentLayerNum < 256)
            {
                currentBucketSize *= 2f;
                currentLayerNum++;
            }

            _BoundsSize = currentBucketSize;

            return currentLayerNum;
        }

        #endregion
    }
}
