using System;
using UnityEngine;

namespace Nav3D.API
{
    /// <summary>
    /// Pathfinding result container for internal usage.
    /// </summary>
    public class PathfindingResult
    {
        #region Properties

        public TimeSpan PathfindingDuration { get; private set; }
        public TimeSpan OptimizingDuration { get; private set; }
        public TimeSpan SmoothingDuration { get; private set; }

        public Vector3[] Path { get; private set; }
        public Vector3[] PathOptimized { get; private set; }
        public Vector3[] PathSmoothed { get; private set; }

        public PathfindingResultCode Result { get; private set; }

        public bool Failed => Result != PathfindingResultCode.SUCCEEDED;

        #endregion

        #region Constructors

        public PathfindingResult(
            Vector3[] _Path,
            Vector3[] _PathOptimized,
            Vector3[] _PathSmoothed,
            TimeSpan _PathfindingDuration,
            TimeSpan _OptimizingDuration,
            TimeSpan _SmoothingDuration
            )
        {
            Path = _Path;
            PathSmoothed = _PathSmoothed;
            PathOptimized = _PathOptimized;

            PathfindingDuration = _PathfindingDuration;
            OptimizingDuration = _OptimizingDuration;
            SmoothingDuration = _SmoothingDuration;

            Result = PathfindingResultCode.SUCCEEDED;
        }

        /// <summary>
        /// Error case constructor.
        /// </summary>
        public PathfindingResult(PathfindingResultCode _ResultCode)
        {
            Path = null;
            PathSmoothed = null;
            PathOptimized = null;

            PathfindingDuration = default;
            OptimizingDuration = default;
            SmoothingDuration = default;

            Result = _ResultCode;
        }

        #endregion
    }
}