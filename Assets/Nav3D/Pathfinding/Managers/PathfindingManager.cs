using Nav3D.API;
using Nav3D.Common;
using Nav3D.Obstacles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Nav3D.Pathfinding
{
    public partial class PathfindingManager : MonoBehaviour
    {
        #region Constants

        //ms
        const int ORDERS_QUEUE_UPDATE_TASK_LIFETIME = 1000;
        const float SQRDISTANCE_EPSILON = 0.00001f;

        #endregion

        #region Constants : Log

        readonly string LOG_REQUEST_PATHFINDING = $"{nameof(PathfindingManager)}.{nameof(RequestPathfinding)}: from {{0}} to {{1}}";

        #endregion

        #region Properties

        public static bool Doomed { get; private set; } = false;
        public static PathfindingManager Instance => Singleton<PathfindingManager>.Instance;

        public int PathFindingTasksMaxCount
        {
            get => m_PathFindingTasksMaxCount;
            set
            {
                if (value == m_PathFindingTasksMaxCount)
                    return;

                m_PathFindingTasksMaxCount = Mathf.Max(value, 1);
                
                RecreateOrdersExecutor(m_PathFindingTasksMaxCount);
            }
        }
        public int PathFindingTasksCount => m_PathfindingTasksOperatingCount;

        #endregion

        #region Attributes

        int m_PathFindingTasksMaxCount = Environment.ProcessorCount - 1;

        float m_StorageBucketSize;

        //Paths that were found in scene space.
        //Need to be stored to determine if any update to the obstacle will invalidate some path.
        //In this case, the invalid path needs to be updated.
        CurvesSpatialHashMap<Path> m_PathsStorage; 

        OrdersExecutor<Path, PathfindingOrder> m_PathfindingOrdersExecutor;

        #endregion

        #region Serialized fields

        [SerializeField]
        volatile int m_PathfindingTasksOperatingCount = 0;
        [SerializeField]
        bool m_DrawPathsStorage;

        public int DebugRegisteredPathsCount;

        #endregion

        #region Public methods

        public void Initialize(float _StorageBucketSize)
        {
            RecreateOrdersExecutor(m_PathFindingTasksMaxCount);

            m_StorageBucketSize = _StorageBucketSize;

            m_PathsStorage = new CurvesSpatialHashMap<Path>(m_StorageBucketSize);
        }

        public void Uninitialize(bool _NeedDestroy = true)
        {
            m_PathfindingOrdersExecutor?.Dispose();

            if (!_NeedDestroy)
                return;

            UtilsCommon.SmartDestroy(this);
        }

        public void RequestPathfinding(
            Path _Requester,
            Vector3 _PointA,
            Vector3 _PointB,
            bool _Smooth,
            int _PerMinBucketSmoothSamples,
            CancellationToken _CancellationToken,
            int _Timeout,
            Action<PathfindingResult> _OnSuccess,
            Action<PathfindingError> _OnFail = null,
            Action _OnFinish = null,
            Log _Log = null)
        {
            _Log?.WriteFormat(LOG_REQUEST_PATHFINDING, _PointA.ToStringExt(), _PointB.ToStringExt());

            m_PathfindingOrdersExecutor.EnqueueOrder(
                _Requester,
                new PathfindingOrder(
                    _PointA,
                    _PointB,
                    _Smooth,
                    _PerMinBucketSmoothSamples,
                    _CancellationToken,
                    _Timeout,
                    _OnSuccess,
                    _OnFail,
                    FindPath,
                    () => Interlocked.Increment(ref m_PathfindingTasksOperatingCount),
                    _OnFinish + (() => Interlocked.Decrement(ref m_PathfindingTasksOperatingCount)),
                    _Log
                ),
                _Log
                );
        }

        public void UpdateAllBoundsCrossingPaths(Bounds _Bounds)
        {
            if (!m_PathsStorage.TryGetIntersectingCurves(_Bounds, out HashSet<Path> intersectingPaths))
                return;

            intersectingPaths.ForEach(_Path => _Path.UpdatePath());
        }

        public Path PrefetchPath(Vector3 _PointA, Vector3 _PointB, Log _Log = null)
        {
            return new Path(_PointA, _PointB, _Log: _Log);
        }

        public void UpdatePathInStorage(Path _Path)
        {
            m_PathsStorage.Update(_Path);
        }

        public void DisposePath(Path _Path)
        {
            m_PathfindingOrdersExecutor.TryRemoveOrder(_Path);
            m_PathsStorage.Unregister(_Path);
        }

        #endregion

        #region Service methods

        void RecreateOrdersExecutor(int _MaxAliveOrders)
        {
            m_PathfindingOrdersExecutor = new OrdersExecutor<Path, PathfindingOrder>(_MaxAliveOrders);
        }
        
        PathfindingResult FindPath(
            Vector3 _PointA,
            Vector3 _PointB,
            bool _Smooth,
            int _PerMinBucketSmoothSamples,
            CancellationToken _CancellationTokenExternal,
            CancellationToken _CancellationTokenTimeout)
        {
            DateTime start = DateTime.Now;
            TimeSpan pathfindingDuration;
            TimeSpan optimizingDuration;
            TimeSpan smoothingDuration;

            try
            {
                if ((_PointA - _PointB).sqrMagnitude < SQRDISTANCE_EPSILON)
                {
                    return new PathfindingResult(
                        new[] { _PointA, _PointB },
                        new[] { _PointA, _PointB },
                        new[] { _PointA, _PointB },
                        TimeSpan.Zero,
                        TimeSpan.Zero,
                        TimeSpan.Zero
                    );
                }

                List<Obstacle> obstacles =
                    ObstacleManager.Doomed ?
                    new List<Obstacle>() :
                    ObstacleManager.Instance.GetObstaclesCrossingTheLine(_PointA, _PointB);

                //sort obstacles by increasing distance from A
                if (obstacles.Count > 1)
                    obstacles = obstacles.OrderBy(
                    _Obstacle =>
                    {
                        return _Obstacle.Bounds.GetIntersection(new Common.Ray(_PointA, _PointB)).
                        Min(_Point => Vector3.SqrMagnitude(_PointA - _Point));
                    })
                    .ToList();

                List<Vector3> path = new List<Vector3> { _PointA };
                List<Vector3> pathSmoothed = new List<Vector3> { _PointA };

                Common.Ray ray = new Common.Ray(_PointA, _PointB);

                foreach (Obstacle obstacle in obstacles)
                {
                    if (_CancellationTokenExternal.IsCancellationRequested || _CancellationTokenTimeout.IsCancellationRequested)
                        break;

                    //get intersection points and sort by distance from start
                    List<Vector3> intersections = obstacle.Bounds
                        .GetIntersection(ray)
                        .OrderBy(_Point => Vector3.SqrMagnitude(_PointA - _Point))
                        .ToList();

                    OctreePathfindingResult pathfindingResult;

                    /*
                    * Possible obstacle bounds intersection cases:
                    * 1) All inside.
                    *  ___________ 
                    * |       B   |
                    * |      /    |
                    * |     /     |
                    * |    A      |
                    * |___________|
                    */
                    if (intersections.Count == 0)
                    {
                        pathfindingResult = obstacle.FindPath(_PointA, _PointB, _CancellationTokenExternal, _CancellationTokenTimeout);

                        if (pathfindingResult.Failed)
                            return new PathfindingResult(pathfindingResult.ResultCode);

                        path = pathfindingResult.Path;
                    }
                    /*
                    * 2) All outside.
                    *           B
                    *  ________/___ 
                    * |       /   |
                    * |      /    |
                    * |     /     |
                    * |    /      |
                    * |___/_______|
                    *    /
                    *   A
                    */
                    else if (intersections.Count == 2)
                    {
                        pathfindingResult = obstacle.FindPath(intersections.First(), intersections.Last(), _CancellationTokenExternal, _CancellationTokenTimeout);

                        if (pathfindingResult.Failed)
                            return new PathfindingResult(pathfindingResult.ResultCode);

                        path.AddRange(pathfindingResult.Path);
                    }
                    /*
                    * 3) A inside, or B inside
                    *  ___________   ___________ 
                    * |       A   | |       B   |
                    * |      /    | |      /    |
                    * |     /     | |     /     |
                    * |    /      | |    /      |
                    * |___/_______| |___/_______|
                    *    /             /
                    *   B             A
                    */
                    else if (obstacle.Bounds.Contains(_PointA))
                    {
                        pathfindingResult = obstacle.FindPath(_PointA, intersections.First(), _CancellationTokenExternal, _CancellationTokenTimeout);

                        if (pathfindingResult.Failed)
                            return new PathfindingResult(pathfindingResult.ResultCode);

                        path.AddRange(pathfindingResult.Path);
                    }
                    else if (obstacle.Bounds.Contains(_PointB))
                    {
                        pathfindingResult = obstacle.FindPath(intersections.First(), _PointB, _CancellationTokenExternal, _CancellationTokenTimeout);

                        if (pathfindingResult.Failed)
                            return new PathfindingResult(pathfindingResult.ResultCode);

                        path.AddRange(pathfindingResult.Path);
                    }
                    //oops, something goes wrong
                    else
                    {
                        Vector3 boundsMin = obstacle.Bounds.min;
                        Vector3 boundsMax = obstacle.Bounds.max;

                        string errorData =
                            $"Bounds: min:{{{boundsMin.x}, {boundsMin.y}, " +
                            $"{boundsMin.z}}}, max:{{{boundsMax.x}, {boundsMax.y}, {boundsMax.z}}}" +
                            $"Ray: A: {_PointA.ToStringExt()}, {_PointB.ToStringExt()}";

                        Debug.LogError(errorData);

                        return new PathfindingResult(PathfindingResultCode.UNKNOWN);
                    }
                }

                //record pathfinding duration
                pathfindingDuration = DateTime.Now - start;
                start = DateTime.Now;

                path.Add(_PointB);

                Vector3[] initialPath;
                Vector3[] optimizedPath;
                Vector3[] smoothedPath;

                initialPath = path.ToArray();

                //increase points density to provide efficient shorten procedure
                DetailPath(path);
                //prune redundant path pieces
                ShortenPath(path, obstacles);
                path.Reverse();

                //do the same in reverse
                DetailPath(path);
                ShortenPath(path, obstacles);
                path.Reverse();

                optimizedPath = path.ToArray();

                //record path optimizing duration
                optimizingDuration = DateTime.Now - start;
                start = DateTime.Now;

                if (_Smooth)
                {
                    //Here we need to expand the area of obstacles that will be taken into account during the smoothing procedure.
                    //This is due to the fact that the resulting spline will deviate in space from the original array of points.
                    //And it may intersect any other obstacles that were not taken into account due pathfinding.

                    //Firstly get the source points array extended with extreme points
                    List<Vector3> extendedPointsArray = AddExtremePointsToCatmullRomArray(path);
                    //Then get the bounds for array
                    Bounds pointsBounds = ExtensionBounds.PointsBounds(extendedPointsArray);
                    //Obtain the list of all crossing obstacles for extended bounds
                    List<Obstacle> newObstaclesSet =
                        ObstacleManager.Doomed ?
                        new List<Obstacle>() :
                        ObstacleManager.Instance.GetObstaclesCrossingTheBounds(pointsBounds);
                    //Execute smothing procedure for extended obstacles list
                    SmoothPath(path, newObstaclesSet, _PerMinBucketSmoothSamples);

                    smoothedPath = path.ToArray();
                    //record path smoothing duration
                    smoothingDuration = DateTime.Now - start;
                }
                else
                {
                    smoothedPath = optimizedPath;
                    smoothingDuration = TimeSpan.Zero;
                }

                return new PathfindingResult(initialPath, optimizedPath, smoothedPath, pathfindingDuration, optimizingDuration, smoothingDuration);
            }
            catch (Exception _Exception)
            {
                Debug.LogError(_Exception);

                return new PathfindingResult(PathfindingResultCode.UNKNOWN);
            }
        }

        //Add excessive points forming convex hull for each elementary spline.
        List<Vector3> AddExtremePointsToCatmullRomArray(List<Vector3> _InputArray)
        {
            if (_InputArray.Count < 4)
                return _InputArray;

            int sourceCount = _InputArray.Count;

            List<Vector3> result = new List<Vector3>((sourceCount - 2) * 3 + 2);
            result.AddRange(_InputArray);

            for (int i = 1; i < sourceCount - 2; i += 2)
            {
                Vector3 p1 = _InputArray[i];
                Vector3 p2 = _InputArray[i + 1];
                Vector3 p0 = _InputArray[i - 1];
                Vector3 p3 = _InputArray[i + 2];

                result.Add(p1 + (p2 - p0));
                result.Add(p1 + (p1 - p3));
                result.Add(p2 + (p2 - p0));
                result.Add(p2 + (p1 - p3));
            }

            return result;
        }

        void DetailPath(List<Vector3> _Path)
        {
            int actualLength = _Path.Count;

            for (int i = 1; i < actualLength; i++)
            {
                Vector3 curPoint = _Path[i];
                Vector3 prePoint = _Path[i - 1];

                if (!ObstacleManager.Doomed && (curPoint - prePoint).sqrMagnitude > ObstacleManager.Instance.MinBucketSizeSqr)
                {
                    //insert median point
                    _Path.Insert(i, prePoint + (curPoint - prePoint) * 0.5f);

                    actualLength++;
                    i = 1;
                }
            }
        }

        void ShortenPath(List<Vector3> _Path, List<Obstacle> _InfluencingObstacles)
        {
            int actualLength = _Path.Count;

            for (int i = 0; i < actualLength; i++)
            {
                Vector3 curPoint = _Path[i];

                for (int j = 2; j < actualLength - i; j++)
                {
                    Vector3 checkPoint = _Path[i + j];

                    if (!RayIntersectOccupiedLeaf(curPoint, checkPoint))
                    {
                        _Path.RemoveAt(i + j - 1);
                        actualLength--;
                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void SmoothPath(List<Vector3> _Path, List<Obstacle> _InfluencingObstacles, int _PerMinBucketSmoothSamples)
        {
            if (ObstacleManager.Doomed)
                return;

            float sampleLength = ObstacleManager.Instance.MinBucketSize / _PerMinBucketSmoothSamples;

            List<Vector3> sourceTrajectory = new List<Vector3>(_Path);
            List<Vector3> smoothedTrajectory = new List<Vector3>();

            //accessory extreme points adding
            sourceTrajectory.Insert(0, sourceTrajectory[0] + sourceTrajectory[0] - sourceTrajectory[1]);
            sourceTrajectory.Add(sourceTrajectory.Last() + (sourceTrajectory.Last() - sourceTrajectory[sourceTrajectory.Count - 2]));

            for (int i = 1; i <= sourceTrajectory.Count - 3; i++)
            {
                smoothedTrajectory.Add(sourceTrajectory[i]);

                int smoothRatio = (int)(Vector3.Distance(sourceTrajectory[i], sourceTrajectory[i + 1]) / sampleLength);
                if (smoothRatio == 0)
                    continue;

                float paramStep = 1f / smoothRatio;
                float t = 0;

                Vector3 lastPoint = sourceTrajectory[i];

                for (int pass = 0; pass < smoothRatio; pass++)
                {
                    t += paramStep;

                    Vector3 newPoint = CatmullRomEq(sourceTrajectory[i - 1], sourceTrajectory[i], sourceTrajectory[i + 1], sourceTrajectory[i + 2], t);

                    if (lastPoint != newPoint && RayIntersectOccupiedLeaf(lastPoint, newPoint))
                    {
                        if (i != sourceTrajectory.Count - 3)
                        {
                            Vector3 accessoryPoint3 = sourceTrajectory[i + 1] + (sourceTrajectory[i + 2] - sourceTrajectory[i + 1]) * 0.5f;
                            _Path.Insert(i + 1, accessoryPoint3);
                        }

                        Vector3 accessoryPoint2 = sourceTrajectory[i] + (sourceTrajectory[i + 1] - sourceTrajectory[i]) * 0.5f;
                        _Path.Insert(i, accessoryPoint2);

                        if (i != 1)
                        {
                            Vector3 accessoryPoint1 = sourceTrajectory[i - 1] + (sourceTrajectory[i] - sourceTrajectory[i - 1]) * 0.5f;
                            _Path.Insert(i - 1, accessoryPoint1);
                        }

                        SmoothPath(_Path, _InfluencingObstacles, _PerMinBucketSmoothSamples);

                        return;
                    }

                    smoothedTrajectory.Add(newPoint);

                    lastPoint = newPoint;
                }
            }

            smoothedTrajectory.Add(_Path.Last());

            _Path.Clear();
            _Path.AddRange(smoothedTrajectory);
        }

        Vector3 CatmullRomEq(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            return .5f * (
                -t * (1 - t) * (1 - t) * P0
                + (2 - 5 * t * t + 3 * t * t * t) * P1
                + t * (1 + 4 * t - 3 * t * t) * P2
                - t * t * (1 - t) * P3
            );
        }

        bool RayIntersectOccupiedLeaf(Vector3 _Start, Vector3 _End)
        {
            return ObstacleManager.Instance.GetObstaclesCrossingTheLine(_Start, _End).Any(_Obstacle => _Obstacle.RayIntersectOccupiedLeaf(new Common.Ray(_Start, _End)));
        }

        #endregion

        #region Unity events

        private void Awake()
        {
            Doomed = false;
        }

        void OnDestroy()
        {
            Doomed = true;

            Uninitialize(false);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            if (!m_DrawPathsStorage)
                return;

            m_PathsStorage.Draw();
        }

        private void Update()
        {
            DebugRegisteredPathsCount = m_PathsStorage.Count;
        }

        #endregion
    }
}
