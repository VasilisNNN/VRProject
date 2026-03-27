using System;
using UnityEngine;

namespace Nav3D.Common
{
    public interface IMovable
    {
        #region Events

        event Action<IMovable, Vector3> OnPositionChanged;
        event Action<float> OnVelocityDangerDistanceChanged;

        #endregion

        #region Properties

        //Whether the movable seeks to avoid a collision.
        public bool Avoiding { get; }
        public bool IsNeighborMoversDirty { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// The current position of the agent after the last move (usually the same as transform.position).
        /// </summary>
        public Vector3 GetPosition();
        /// <summary>
        /// Velocity vector used at las fixed update tick. Used by other movables to compute avoidance velocity.
        /// </summary>
        public Vector3 GetLastFrameVelocity();
        /// <summary>
        /// Average velocity vector over the last few frames.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAccumulatedVelocity();
        /// <summary>
        /// Time horizon in which collision avoidance considered.
        /// </summary>
        public float GetTimeHorizon();
        /// <summary>
        /// Radius in world units. Used by other movables to compute avoidance velocity.
        /// </summary>
        public float GetRadius();
        /// <summary>
        /// The maximum allowed movement speed per fixed update tick 
        /// </summary>
        public float GetMaxSpeed();
        /// <summary>
        /// A value showing the possible occupied volume when moving in any direction during TAU time horizon. Usually it is radius + max speed * ORCATAU
        /// </summary>
        public float GetDangerRadius();

        public float GetStaticObstaclesDangerDistance();

        public void SetNeighborMovablesDirty(bool _Dirty);
        public void SetNeighborObstaclesDirty(bool _Dirty);

        #endregion
    }
}