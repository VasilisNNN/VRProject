using System;
using Nav3D.API;
using Nav3D.Common;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using PathfindingMethod =
    System.Func<
        UnityEngine.Vector3,
        UnityEngine.Vector3,
        bool,
        int,
        System.Threading.CancellationToken,
        System.Threading.CancellationToken,
        Nav3D.API.PathfindingResult>;

namespace Nav3D.Pathfinding
{
    struct PathfindingOrder : IExecutable
    {
        #region Constants

        const string FAIL_TIMEOUT = "Pathfinding from {0} to {1} failed; Reason = TIMEOUT: {2} ms was taken, time limit is: {3} ms";
        const string FAIL_NO_PATH = "Pathfinding from {0} to {1} failed; Reason = PATH DOES NOT EXIST: {2} ms was taken";
        const string FAIL_CANCELLED = "Pathfinding from {0} to {1} failed; Reason = CANCELLED BY EXTERNAL CONTROLLER: {2} ms was taken";
        const string FAIL_INSIDE_OBSTACLE = "Pathfinding from {0} to {1} failed; Reason = ONE OF THE POINTS IS INSIDE OF THE OBSTACLE: {2} ms was taken";
        const string FAIL_UNKNOWN = "Pathfinding from {0} to {1} canceled; Reason = UNKNOWN: {2} ms was taken";

        #endregion

        #region Attributes

        Vector3 m_PointA;
        Vector3 m_PointB;
        bool m_Smooth;
        int m_PerMinBucketSmoothSamples;
        CancellationToken m_CancellationTokenExternal;
        int m_Timeout;

        Action<PathfindingResult> m_OnSuccess;
        Action<PathfindingError> m_OnFail;
        Action m_OnExecuteStart;
        Action m_OnExecuteFinish;
        PathfindingMethod m_PathfindingMethod;

        Log m_Log;

        #endregion

        #region Constructors

        public PathfindingOrder(
            Vector3 _PointA,
            Vector3 _PointB,
            bool _Smooth,
            int _PerMinBucketSmoothSamples,
            CancellationToken _CancellationToken,
            int _Timeout,
            Action<PathfindingResult> _OnSuccess,
            Action<PathfindingError> _OnFail,
            PathfindingMethod _PathfindingMethod,
            Action _OnExecuteStart,
            Action _OnExecuteFinish,
            Log _Log = null)
        {
            m_PointA = _PointA;
            m_PointB = _PointB;
            m_Smooth = _Smooth;
            m_PerMinBucketSmoothSamples = _PerMinBucketSmoothSamples;
            m_CancellationTokenExternal = _CancellationToken;
            m_Timeout = _Timeout;
            m_OnSuccess = _OnSuccess;
            m_OnFail = _OnFail;
            m_PathfindingMethod = _PathfindingMethod;
            m_Log = _Log;

            m_OnExecuteStart = _OnExecuteStart;
            m_OnExecuteFinish = _OnExecuteFinish;

            m_Log?.Write($"{nameof(PathfindingOrder)}.ctor (ID:{GetHashCode()}): from {_PointA.ToStringExt()} to {_PointB.ToStringExt()}");
        }

        #endregion

        #region Public methods

        public void Execute(Action _OnResolve)
        {
            m_OnExecuteStart?.Invoke();

            m_Log?.Write($"{nameof(PathfindingOrder)}.{nameof(Execute)}: from {m_PointA.ToStringExt()} to {m_PointB.ToStringExt()}");

            if (m_CancellationTokenExternal.IsCancellationRequested)
            {
                m_OnExecuteFinish?.Invoke();

                return;
            }

            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellationTokenTimeout = timeoutCancellationTokenSource.Token;
            CancellationToken cancellationTokenExternal = m_CancellationTokenExternal;

            //copying values to local scope variables
            //because it's impossible to use closure at lambda code with variables from structure instance
            int timeout = m_Timeout;
            Vector3 pointA = m_PointA;
            Vector3 pointB = m_PointB;
            bool smooth = m_Smooth;
            int perMinBucketSmoothSamples = m_PerMinBucketSmoothSamples;
            Action<PathfindingError> onFail = m_OnFail;
            Action<PathfindingResult> onSuccess = m_OnSuccess;

            PathfindingMethod pathfindingMethod = m_PathfindingMethod;
            Log log = m_Log;
            Action onFinish = m_OnExecuteFinish + _OnResolve;

            Task.Run(() =>
            {
                log?.Write($"{nameof(PathfindingOrder)}: Pathfinding task started: from {pointA.ToStringExt()} to {pointB.ToStringExt()}");
                timeoutCancellationTokenSource.CancelAfter(timeout);

                if (cancellationTokenExternal.IsCancellationRequested)
                {
                    onFinish?.Invoke();

                    return;
                }

                PathfindingResult result = pathfindingMethod(
                    pointA,
                    pointB,
                    smooth,
                    perMinBucketSmoothSamples,
                    cancellationTokenExternal,
                    cancellationTokenTimeout);

                if (result.Failed)
                {
                    onFail?.Invoke(new PathfindingError(
                        result.Result,
                        result.Result == PathfindingResultCode.TIMEOUT ? string.Format(FAIL_TIMEOUT, pointA, pointB, result.PathfindingDuration.TotalMilliseconds, timeout) :
                        result.Result == PathfindingResultCode.PATH_DOES_NOT_EXIST ? string.Format(FAIL_NO_PATH, pointA, pointB, result.PathfindingDuration.TotalMilliseconds) :
                        result.Result == PathfindingResultCode.CANCELED ? string.Format(FAIL_CANCELLED, pointA, pointB, result.PathfindingDuration.TotalMilliseconds) :
                        result.Result == PathfindingResultCode.START_POINT_INSIDE_OBSTACLE ? string.Format(FAIL_INSIDE_OBSTACLE, pointA, pointB, result.PathfindingDuration.TotalMilliseconds) :
                        string.Format(FAIL_UNKNOWN, pointA, pointB, result.PathfindingDuration)
                        ));

                    onFinish?.Invoke();

                    return;
                }

                if (cancellationTokenExternal.IsCancellationRequested)
                    onFail?.Invoke(new PathfindingError(PathfindingResultCode.CANCELED, string.Format(FAIL_CANCELLED, pointA, pointB, result.PathfindingDuration.TotalMilliseconds)));
                else if (cancellationTokenTimeout.IsCancellationRequested)
                    onFail?.Invoke(new PathfindingError(PathfindingResultCode.TIMEOUT, string.Format(FAIL_TIMEOUT, pointA, pointB, result.PathfindingDuration.TotalMilliseconds, timeout)));
                else
                    onSuccess.Invoke(result);

                onFinish?.Invoke();
            });
        }

        #endregion
    }
}
