using Nav3D.Common;
using Nav3D.Pathfinding;
using System.Linq;
using System;
using System.Threading;
using UnityEngine;

namespace Nav3D.API
{
    public class Path : IDisposable, ICurve
    {
        #region Constants

        const int DEFAULT_SEARCH_TIMEOUT = 2000;
        const int DEFAULT_SMOOTH_RATIO = 3;

        #endregion

        #region Constants : Log

        readonly string LOG_PATH_CTOR = $"{nameof(Path)}.ctor from: {{0}} to: {{1}}";
        readonly string LOG_PATH_UPDATE = $"{nameof(Path)}.{nameof(UpdatePath)}: from: {{0}} to: {{1}}";
        readonly string LOG_PATH_SUCCESS = $"{nameof(Path)}.{nameof(DoOnSuccess)}: from: {{0}} to: {{1}}";
        readonly string LOG_PATH_FAIL = $"{nameof(Path)}.{nameof(DoOnFail)}: from: {{0}} to: {{1}}";
        readonly string LOG_PATH_DISPOSE = $"{nameof(Path)}.{nameof(Dispose)}: from: {{0}} to: {{1}}";

        #endregion

        #region Attributes

        protected int m_SmoothRatio = DEFAULT_SMOOTH_RATIO;
        int m_Timeout = DEFAULT_SEARCH_TIMEOUT;

        protected Vector3 m_Start;
        protected Vector3 m_Goal;

        Vector3[] m_TrajectoryOriginal;
        Vector3[] m_TrajectoryOptimized;
        Vector3[] m_TrajectoryFinal;

        Bounds m_PathBounds;
        Log m_Log;

        protected CancellationTokenSource m_UpdateTokenSource;

        PathFollowData m_LastGivenFollowData;

        #endregion

        #region Properties

        /// <summary>
        /// Final trajectory processed according to pathfinding parameters.
        /// </summary>
        //The internal logic is such that m_TrajectorySmoothed contains the actual trajectory in accordance with all requirements.
        public Vector3[] Trajectory => m_TrajectoryFinal;

        /// <summary>
        /// Initial trajectory obtained after searching for A* in octrees.
        /// </summary>
        public Vector3[] TrajectoryOriginal => m_TrajectoryOriginal;

        /// <summary>
        /// Optimized initial trajectory.
        /// </summary>
        public Vector3[] TrajectoryOptimized => m_TrajectoryOptimized;

        /// <summary>
        /// Smoothed optimized trajectory.
        /// </summary>
        public Vector3[] TrajectorySmoothed => m_TrajectoryFinal;

        public Bounds Bounds => m_PathBounds;

        public Vector3 Start
        {
            get => m_Start;
            set
            {
                if (value == m_Start)
                    return;

                m_Start = value;
                IsValid = false;
            }
        }

        public Vector3 Goal
        {
            get => m_Goal;
            set
            {
                if (value == m_Goal)
                    return;

                m_Goal = value;
                IsValid = false;
            }
        }

        /// <summary>
        /// Smooth samples per min bucket volume.
        /// </summary>
        public int SmoothRatio
        {
            get => m_SmoothRatio;
            set
            {
                if (value < 1 || value == m_SmoothRatio)
                    return;

                m_SmoothRatio = value;
            }
        }

        public int Timeout
        {
            get => m_Timeout;
            set
            {
                if (value < 1 || value == m_Timeout)
                    return;

                m_Timeout = value;
            }
        }

        public bool Smooth { get; set; }
        public bool IsValid { get; private set; }
        public bool IsPathUpdating { get; private set; }

        public Common.Ray[] Segments { get; private set; }

        #endregion

        #region Events

        public event Action OnPathUpdated;

        #endregion

        #region Constructors

        public Path(Vector3 _Start, Vector3 _Goal, int? _Timeout = null, Log _Log = null)
        {
            m_Start = _Start;
            m_Goal = _Goal;
            m_Log = _Log;

            m_Log?.WriteFormat(LOG_PATH_CTOR, m_Start.ToStringExt(), m_Goal.ToStringExt());

            if (_Timeout.HasValue)
                Timeout = _Timeout.Value;
        }

        ~Path()
        {
            Dispose();
        }

        #endregion

        #region ICurve

        public bool Intersects(Bounds _Bounds)
        {
            if (!IsValid)
                return false;

            foreach (Common.Ray ray in Segments)
            {
                if (Bounds.IntersectRay(ray))
                    return true;
            }

            return false;
        }

        #endregion

        #region Public methods

        public void UpdatePath(
            Vector3? _Start = null,
            Vector3? _Goal = null,
            int? _Timeout = null,
            bool? _Smooth = null,
            Action _OnSuccess = null,
            Action<PathfindingError> _OnFail = null
        )
        {
            m_UpdateTokenSource?.Cancel();
            m_UpdateTokenSource = new CancellationTokenSource();

            if (_Timeout.HasValue)
                Timeout = _Timeout.Value;

            if (_Start.HasValue)
                Start = _Start.Value;

            if (_Goal.HasValue)
                Goal = _Goal.Value;

            if (_Smooth.HasValue)
                Smooth = _Smooth.Value;

            m_Log?.WriteFormat(LOG_PATH_UPDATE, m_Start.ToStringExt(), m_Goal.ToStringExt());

            IsPathUpdating = true;

            PerformPathfinding(_OnSuccess, _OnFail);
        }

        public PathFollowData GetFollowData(Vector3 _FollowerPoint)
        {
            if (!IsValid)
                throw new Exception($"[{nameof(Path)}]: The path needs to be initialized before! (UpdatePath method)");

            return m_LastGivenFollowData = new PathFollowData(m_TrajectoryFinal, _FollowerPoint);
        }

        public void Dispose()
        {
            m_LastGivenFollowData?.Invalidate();
            m_UpdateTokenSource.Cancel();

            if (!PathfindingManager.Doomed)
                PathfindingManager.Instance.DisposePath(this);

            m_Log?.WriteFormat(LOG_PATH_DISPOSE + "\n" + Environment.StackTrace, m_Start.ToStringExt(), m_Goal.ToStringExt());
        }

        #endregion

        #region Service methods

        protected virtual void PerformPathfinding(Action _OnSuccess, Action<PathfindingError> _OnFail)
        {
            PathfindingManager.Instance.RequestPathfinding(
                this,
                m_Start,
                m_Goal,
                Smooth,
                m_SmoothRatio,
                m_UpdateTokenSource.Token,
                Timeout,
                _Result => DoOnSuccess(_Result, _OnSuccess),
                _Error => DoOnFail(_Error, _OnFail),
                () => IsPathUpdating = false,
                m_Log);
        }

        protected virtual void DoOnSuccess(PathfindingResult _Result, Action _OnSuccess)
        {
            m_LastGivenFollowData?.Invalidate();

            m_TrajectoryOriginal = _Result.Path;
            m_TrajectoryOptimized = _Result.PathOptimized;
            m_TrajectoryFinal = _Result.PathSmoothed;

            m_PathBounds = ExtensionBounds.PointsBounds(m_TrajectoryFinal.ToList());
            Segments = UtilsMath.PointSequanceToSegments(m_TrajectoryFinal);

            IsValid = true;

            PathfindingManager.Instance.UpdatePathInStorage(this);

            m_Log?.WriteFormat(LOG_PATH_SUCCESS, m_Start.ToStringExt(), m_Goal.ToStringExt());

            if (_OnSuccess != null)
                ThreadDispatcher.BeginInvoke(_OnSuccess);

            if (OnPathUpdated != null)
                ThreadDispatcher.BeginInvoke(OnPathUpdated.Invoke);
        }

        protected void DoOnFail(PathfindingError _Error, Action<PathfindingError> _OnFail)
        {
            m_Log?.WriteFormat(LOG_PATH_FAIL, m_Start.ToStringExt(), m_Goal.ToStringExt());

            if (_OnFail != null)
                ThreadDispatcher.BeginInvoke(() => _OnFail.Invoke(_Error));
        }

        #endregion
    }
}
