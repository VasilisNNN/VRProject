using Nav3D.API;
using Nav3D.LocalAvoidance;
using Nav3D.Pathfinding;
using System;
using UnityEngine;

namespace Nav3D.Common
{
    public class Nav3DAgentMover : IMovable
    {
        #region Constants

        readonly string LOG_BEGIN_MOVE_TO = $"{nameof(Nav3DAgentMover)}.{nameof(BeginMoveTo)}: {{0}}";
        readonly string LOG_PATH_INIT = $"{nameof(Nav3DAgentMover)}.{nameof(InitGlobalPath)}";
        readonly string LOG_GET_FOLLOW_DATA = $"{nameof(Nav3DAgentMover)}.{nameof(TryGetFollowData)}";
        readonly string LOG_PATH_UPDATE_FAIL = $"{nameof(Nav3DAgentMover)}.{nameof(InitGlobalPath)}: {nameof(Path)}.UpdatePath: Fail: {{0}}";

        #endregion

        #region Constructors

        public Nav3DAgentMover(Vector3 _Position, Nav3DAgentDescription _Description, Nav3DAgent _Agent)
        {
            Agent = _Agent;
            m_LastPosition = _Position;

            SetDescription(_Description);
        }

        #endregion

        #region Attributes

        bool m_IsNeighborMovablesDirty = true;
        bool m_IsNeighborObstaclesDirty = true;

        Vector3 m_AccumulatedVelocity;
        Vector3 m_TargetPoint;
        Vector3 m_LastPosition;
        Vector3 m_LastVelocity;
        Vector3 m_LastNonZeroVelocity;

        Path m_GlobalPath;
        PathFollowData m_CurrFollowData;

        protected LocalAvoidanceSolver m_MoverLASolver;
        protected StaticLocalAvoidanceSolver m_StaticLASolver;

        protected Nav3DAgentDescription m_Description;

        Action<PathfindingError> m_OnPathfindingFail;

        Vector3 m_PathNextPoint;
        Vector3 m_LocalAvoidanceVelocity;
        Vector3 m_BlendedVelocity;

        DateTime m_LastPathUpdateFireTime;

        Log m_Log;

        #endregion

        #region Properties

        Vector3 LastVelocity
        {
            get => m_LastVelocity;
            set
            {
                m_LastVelocity = value;

                if (value != Vector3.zero)
                    m_LastNonZeroVelocity = value;
            }
        }
        
        public Nav3DAgent Agent { get; private set; }
        float VelocityDangerDistance => m_Description.VelocityRadius * 2f;

        Vector3 VPref
        {
            get
            {
                if (m_Description.MotionNavigationType == MotionNavigationType.LOCAL)
                    return (m_TargetPoint - m_LastPosition).normalized * m_Description.Speed;
                else
                    return (m_PathNextPoint - m_LastPosition).normalized * m_Description.Speed;
            }
        }

        public Vector3 LastPosition => m_LastPosition;

        public BehaviorType BehaviorType => m_Description.BehaviorType;
        public StaticLocalAvoidanceSolver StaticLocalAvoidanceSolver => m_StaticLASolver;

        public float UpdateStaticObstaclesSqrDistanceThreshold => m_Description.VelocityRadiusSqr;

        public bool AvoidanceInterrupt { get; private set; }

        #endregion

        #region Events

        public event Action<IMovable, Vector3> OnPositionChanged;
        public event Action<float> OnVelocityDangerDistanceChanged;

        #endregion

        #region Public methods

        public void BeginMoveTo(Vector3 _Point, Action<PathfindingError> _OnPathfindingFail = null)
        {
            if (m_Description.UseLog)
                m_Log.WriteFormat(LOG_BEGIN_MOVE_TO, _Point.ToStringExt());

            m_TargetPoint = _Point;
            m_OnPathfindingFail = _OnPathfindingFail;

            switch (m_Description.MotionNavigationType)
            {
                case MotionNavigationType.GLOBAL:
                    {
                        DisposeGlobalPath();
                        InitGlobalPath();

                        break;
                    }
                case MotionNavigationType.GLOBAL_AND_LOCAL:
                    {
                        DisposeGlobalPath();
                        InitGlobalPath();

                        if (m_MoverLASolver == null)
                            InitLocalAvoidanceSolvers();

                        break;
                    }
                case MotionNavigationType.LOCAL:
                    {
                        if (m_MoverLASolver == null)
                            InitLocalAvoidanceSolvers();
                        else
                        {
                            m_MoverLASolver.SetFollowPoint(m_TargetPoint);
                            m_StaticLASolver.SetFollowPoint(m_TargetPoint);
                        }

                        break;
                    }
                default:
                    throw new Exception($"Unknown MotionNavigationType: {m_Description.MotionNavigationType}");
            }
        }

        // ReSharper disable once InconsistentNaming
        public void UpdateRivalVO(Nav3DAgentMover _RivalMover, VOAgent _RivalVO)
        {
            m_MoverLASolver?.UpdateRivalVO(_RivalMover, _RivalVO);
        }

        public Vector3 GetFrameVelocity()
        {
            Vector3 velocity = m_Description.BehaviorType == BehaviorType.INDIFFERENT ? Vector3.zero : GetVelocity();

            m_AccumulatedVelocity = Vector3.Slerp(m_AccumulatedVelocity, velocity, 0.5f);

            return velocity;
        }

        public void SetLogPtr(Log _LogPtr)
        {
            m_Log = _LogPtr;
        }

        public bool IsCollidingWithOther(Nav3DAgentMover _Other)
        {
            Vector3 agentsDeltaPos = _Other.GetPosition() - m_LastPosition;
            float radiusSum = m_Description.Radius + _Other.GetRadius();

            return agentsDeltaPos.magnitude < radiusSum;
        }

        public void SetLastPosition(Vector3 _Position)
        {
            m_LastPosition = _Position;

            OnPositionChanged?.Invoke(this, m_LastPosition);

            if (m_Description.BehaviorType == BehaviorType.YIELDING)
                m_MoverLASolver?.SetFollowPoint(m_LastPosition);
        }

        public void SetDescription(Nav3DAgentDescription _Description)
        {
            UnsubscribeDescriptionEvents();

            m_Description = _Description;

            SetMotionNavType(m_Description.MotionNavigationType);

            DisposeLocalAvoidanceSolvers();
            InitLocalAvoidanceSolvers();

            m_MoverLASolver?.SetDescription(m_Description);
            m_StaticLASolver?.SetDescription(m_Description);

            SubscribeDescriptionEvents();
        }

        public void Uninitialize()
        {
            UnsubscribeDescriptionEvents();

            DisposeGlobalPath();

            DisposeLocalAvoidanceSolvers();
        }

        public void SetAvoidanceInterrupt(bool _Value)
        {
            AvoidanceInterrupt = _Value;
        }

#if UNITY_EDITOR
        public void Visualize(bool _DrawRadius = true, bool _DrawPath = true, bool _DrawVelocities = true)
        {
            void drawPath()
            {
                if (m_GlobalPath != null)
                {
                    Vector3[] agentPath = m_GlobalPath?.Trajectory;

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (_DrawPath && agentPath != null)
                    {
                        Gizmos.color = Color.cyan;

                        if (agentPath.Length > 0)
                            for (int i = 0; i < agentPath.Length - 1; i++)
                            {
                                Gizmos.DrawLine(agentPath[i], agentPath[i + 1]);
                            }

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(m_LastPosition, m_PathNextPoint);
                        Gizmos.DrawSphere(m_PathNextPoint, 0.025f);
                    }
                }
            }

            if (_DrawRadius)
                Gizmos.DrawWireSphere(m_LastPosition, m_Description.Radius);

            switch (m_Description.MotionNavigationType)
            {
                case MotionNavigationType.GLOBAL:
                    {
                        if (_DrawPath)
                            drawPath();

                        break;
                    }
                case MotionNavigationType.GLOBAL_AND_LOCAL:
                    {
                        if (_DrawPath)
                            drawPath();

                        if (_DrawVelocities)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawLine(m_LastPosition, m_LastPosition + VPref.normalized);

                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(m_LastPosition, m_LastPosition + m_LocalAvoidanceVelocity.normalized);

                            Gizmos.color = Color.blue;
                            Gizmos.DrawLine(m_LastPosition, m_LastPosition + m_BlendedVelocity.normalized);
                        }

                        break;
                    }
                case MotionNavigationType.LOCAL:
                    {
                        if (_DrawPath)
                        {
                            Gizmos.color = Color.cyan;

                            Gizmos.DrawLine(m_LastPosition, m_TargetPoint);
                        }

                        if (_DrawVelocities)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawLine(m_LastPosition, m_LastPosition + VPref.normalized);

                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(m_LastPosition, m_LastPosition + m_LocalAvoidanceVelocity.normalized);
                        }

                        break;
                    }
                default:
                    throw new Exception($"Unknown MotionNavigationType: {m_Description.MotionNavigationType}");
            }
        }

        public void VisualizeNearestMovers()
        {
            m_MoverLASolver?.VisualizeNearestMovers();
        }

        public void VisualizeNearestObstacleTriangles()
        {
            m_StaticLASolver?.Visualize();
        }
#endif

        #endregion

        #region IMovable

        public bool Avoiding =>
            m_Description.MotionNavigationType != MotionNavigationType.GLOBAL &&
            !AvoidanceInterrupt &&
            Agent.OperatingMode != OperatingMode.IDLE &&
            m_Description.BehaviorType != BehaviorType.INDIFFERENT;

        public bool IsNeighborMoversDirty => m_IsNeighborMovablesDirty;
        public bool IsNeighborObstaclesDirty => m_IsNeighborObstaclesDirty;

        public Vector3 GetPosition()
        {
            return m_LastPosition;
        }

        public Vector3 GetLastFrameVelocity()
        {
            return m_LastVelocity;
        }

        public Vector3 GetLastFrameVelocityProjected()
        {
            return m_LastVelocity * m_Description.ORCATau;
        }

        public Vector3 GetLastNonZeroVelocity()
        {
            return m_LastNonZeroVelocity;
        }
        
        public Vector3 GetAccumulatedVelocity()
        {
            return m_AccumulatedVelocity;
        }

        public float GetTimeHorizon()
        {
            return m_Description.ORCATau;
        }

        public float GetRadius()
        {
            return m_Description.Radius;
        }

        public float GetMaxSpeed()
        {
            return m_Description.FactualMaxSpeed;
        }

        public float GetDangerRadiusOneFrame()
        {
            return m_Description.VelocityRadius;
        }
        
        public float GetDangerRadius()
        {
            return m_Description.VelocityRadiusTauProjection;
        }

        public float GetStaticObstaclesDangerDistance()
        {
            return m_Description.VelocityRadius;
        }

        public void SetNeighborMovablesDirty(bool _Dirty)
        {
            m_IsNeighborMovablesDirty = _Dirty;
        }

        public void SetNeighborObstaclesDirty(bool _Dirty)
        {
            m_IsNeighborObstaclesDirty = _Dirty;
        }

        #endregion

        #region Service methods

        Vector3 GetVelocity()
        {
            switch (m_Description.MotionNavigationType)
            {
                //just uses global path data to obtain velocity for motion
                case MotionNavigationType.GLOBAL:
                    {
                        if ((!m_GlobalPath?.IsValid ?? true) || (!m_CurrFollowData?.IsValid ?? true))
                            return LastVelocity = Vector3.zero;

                        m_PathNextPoint = m_CurrFollowData.GetMovePoint(m_Description.Speed, m_Description.Radius, m_LastPosition);

                        return LastVelocity = (m_PathNextPoint - m_LastPosition).normalized * m_Description.Speed;
                    }
                //just uses local avoidance velocity for motion
                case MotionNavigationType.LOCAL:
                    {
                        if (m_Description.BehaviorType == BehaviorType.YIELDING)
                            return m_MoverLASolver.GetVelocity(m_LastPosition, VPref, out _);

                        Vector3 moverLAVelocity = m_MoverLASolver.GetVelocity(m_LastPosition, VPref, out float moversProximity);
                        Vector3 staticLAVelocity = m_StaticLASolver.GetVelocity(m_LastPosition, VPref, out float staticsProximity);

                        Vector3 resultVelocity;

                        //no movers and static obstacles near - move with pref velocity
                        if (moversProximity == 0 && staticsProximity == 0)
                            resultVelocity = VPref;
                        //no movers near - move with obstacles avoidance velocity
                        else if (moversProximity == 0)
                            resultVelocity = staticLAVelocity;
                        //no obstacles near - move with movers avoidance velocity
                        else if (staticsProximity == 0)
                            resultVelocity = moverLAVelocity;
                        //has both movers and obstacles near - mix avoidance velocities according to weights
                        else
                        {
                            resultVelocity = UtilsMath.WeightedVector3Sum2(
                                moverLAVelocity, staticLAVelocity,
                                m_Description.AgentsAvoidanceVelocityWeight2, m_Description.ObstaclesAvoidanceVelocityWeight2);
                        }

                        return LastVelocity = m_LocalAvoidanceVelocity = resultVelocity;
                    }
                //uses both global path and local avoidance, and blends there velocities for perform motion
                case MotionNavigationType.GLOBAL_AND_LOCAL:
                {
                    if ((!m_GlobalPath?.IsValid ?? true) || (!m_CurrFollowData?.IsValid ?? true))
                        return LastVelocity = Vector3.zero;

                        //Update path if agent is moved away from current one far enough
                        if (m_Description.AutoUpdatePath && (AgentManager.Now - m_LastPathUpdateFireTime).TotalMilliseconds >
                            Mathf.Max(m_Description.PathAutoUpdateCooldown, m_Description.PathfindingTimeout))
                        {
                            m_CurrFollowData.GetDistToClosestOnPath(m_LastPosition, out _, out float magnitude);

                            if (magnitude > VelocityDangerDistance)
                            {
                                m_GlobalPath.UpdatePath(
                                    _Start: m_LastPosition,
                                    _Timeout: m_Description.PathfindingTimeout,
                                    _Smooth: m_Description.SmoothPath,
                                    _OnSuccess: TryGetFollowData,
                                    _OnFail: m_OnPathfindingFail);

                                m_LastPathUpdateFireTime = AgentManager.Now;

                                return Vector3.zero;
                            }
                        }

                        //obtain desirable position on the path
                        m_PathNextPoint = m_CurrFollowData.GetMovePoint(m_Description.Speed, m_Description.Radius, m_LastPosition);
                        m_MoverLASolver.SetFollowPoint(m_PathNextPoint);
                        m_StaticLASolver.SetFollowPoint(m_PathNextPoint);

                        //desired velocity to follow the global path
                        Vector3 pathVelocity = VPref;

                        Vector3 moverLAVelocity = m_MoverLASolver.GetVelocity(m_LastPosition, VPref, out float moversProximity);
                        
                        if (AvoidanceInterrupt)
                            return Vector3.zero;
                        
                        Vector3 staticLAVelocity = m_StaticLASolver.GetVelocity(m_LastPosition, VPref, out float staticsProximity);
                        
                        Vector3 resultVelocity;

                        //no movers and static obstacles near - move with pref velocity
                        if (moversProximity == 0 && staticsProximity == 0)
                        {
                            m_LocalAvoidanceVelocity = Vector3.zero;
                            resultVelocity = Vector3.Slerp(pathVelocity, m_LastVelocity, 0.1f);
                        }
                        //no movers near - mix path and obstacles avoidance velocities according to weights
                        else if (moversProximity == 0)
                        {
                            m_LocalAvoidanceVelocity = staticLAVelocity;
                            resultVelocity = UtilsMath.WeightedVector3Sum2(
                                pathVelocity, staticLAVelocity,
                                m_Description.PathVelocityWeight1, m_Description.ObstaclesAvoidanceVelocityWeight1);
                        }
                        //no obstacles near - mix path and movers avoidance velocities according to weights
                        else if (staticsProximity == 0 || m_Description.ObstaclesAvoidanceVelocityWeight == 0)
                        {
                            m_LocalAvoidanceVelocity = moverLAVelocity;

                            resultVelocity = UtilsMath.WeightedVector3Sum2(
                                pathVelocity, moverLAVelocity,
                                m_Description.PathVelocityWeight2, m_Description.AgentsAvoidanceVelocityWeight1);
                        }
                        //has both movers and obstacles near - mix avoidance velocities and path velocity according to weights
                        else
                        {
                            resultVelocity = UtilsMath.WeightedVector3Sum3(
                                pathVelocity, moverLAVelocity, staticLAVelocity,
                                m_Description.PathVelocityWeight, m_Description.AgentsAvoidanceVelocityWeight, m_Description.ObstaclesAvoidanceVelocityWeight,
                                out m_LocalAvoidanceVelocity);
                        }

                        return LastVelocity = m_BlendedVelocity = resultVelocity;
                    }
                default:
                    throw new Exception($"Unknown MotionNavigationType: {m_Description.MotionNavigationType}");
            }
        }

        void InvokeOnVelocityDangerDistanceChanged(float _Value)
        {
            OnVelocityDangerDistanceChanged?.Invoke(VelocityDangerDistance);
        }

        void SubscribeDescriptionEvents()
        {
            AgentManager.Instance.RegisterAgentMover(this, VelocityDangerDistance);

            m_Description.OnRadiusChanged += InvokeOnVelocityDangerDistanceChanged;
            m_Description.OnMaxSpeedChanged += InvokeOnVelocityDangerDistanceChanged;
            m_Description.OnSpeedChanged += InvokeOnVelocityDangerDistanceChanged;

            m_Description.OnSmoothRatioChanged += OnSmoothRatioChanged;
            m_Description.OnMotionNavTypeChanged += OnMotionNavTypeChanged;
            m_Description.OnBehaviorTypeChanged += OnBehaviorTypeChanged;
        }

        void UnsubscribeDescriptionEvents()
        {
            if (m_Description != null)
            {
                m_Description.OnRadiusChanged -= InvokeOnVelocityDangerDistanceChanged;
                m_Description.OnMaxSpeedChanged -= InvokeOnVelocityDangerDistanceChanged;
                m_Description.OnSpeedChanged -= InvokeOnVelocityDangerDistanceChanged;

                m_Description.OnSmoothRatioChanged -= OnSmoothRatioChanged;
                m_Description.OnMotionNavTypeChanged -= OnMotionNavTypeChanged;
                m_Description.OnBehaviorTypeChanged -= OnBehaviorTypeChanged;

                //check for case when scene cleanup pass occurs
                if (!AgentManager.Doomed)
                    AgentManager.Instance.UnregisterAgentMover(this);
            }
        }

        void OnMotionNavTypeChanged(MotionNavigationType _MotionNavType)
        {
            SetMotionNavType(_MotionNavType);

            InvokeOnVelocityDangerDistanceChanged(0);
        }

        void OnBehaviorTypeChanged(BehaviorType _BehaviorType)
        {
            DisposeLocalAvoidanceSolvers();
            InitLocalAvoidanceSolvers();
        }

        void OnSmoothRatioChanged(int _Ratio)
        {
            if (m_GlobalPath != null)
                m_GlobalPath.SmoothRatio = _Ratio;
        }

        void SetMotionNavType(MotionNavigationType _MotionNavType)
        {
            switch (_MotionNavType)
            {
                case MotionNavigationType.GLOBAL:
                    {
                        DisposeLocalAvoidanceSolvers();

                        if (m_GlobalPath != null && m_GlobalPath.Goal != m_TargetPoint)
                        {
                            DisposeGlobalPath();
                            InitGlobalPath();
                        }

                        return;
                    }
                case MotionNavigationType.GLOBAL_AND_LOCAL:
                    {
                        if (m_MoverLASolver == null)
                            InitLocalAvoidanceSolvers();
                        else
                        {
                            if (m_Description.BehaviorType == BehaviorType.DEFAULT)
                                m_MoverLASolver.SetFollowPoint(m_TargetPoint);

                            m_StaticLASolver.SetFollowPoint(m_TargetPoint);
                        }

                        if (m_GlobalPath != null && m_GlobalPath.Goal != m_TargetPoint)
                        {
                            DisposeGlobalPath();
                            InitGlobalPath();
                        }

                        return;
                    }
                case MotionNavigationType.LOCAL:
                    {
                        DisposeGlobalPath();

                        if (m_MoverLASolver == null)
                            InitLocalAvoidanceSolvers();
                        else
                        {
                            if (m_Description.BehaviorType == BehaviorType.DEFAULT)
                                m_MoverLASolver.SetFollowPoint(m_TargetPoint);
                            m_StaticLASolver.SetFollowPoint(m_TargetPoint);
                        }

                        return;
                    }
                default:
                    throw new Exception($"Unknown Motion navigation type {_MotionNavType}");
            }
        }

        void InitGlobalPath()
        {
            if (m_Description.UseLog)
                m_Log.Write(LOG_PATH_INIT);

            m_GlobalPath = PathfindingManager.Instance.PrefetchPath(m_LastPosition, m_TargetPoint, m_Log);

            m_GlobalPath.OnPathUpdated += TryGetFollowData;
            m_GlobalPath.UpdatePath(
                _Timeout: m_Description.PathfindingTimeout,
                _Smooth: m_Description.SmoothPath,
                _OnSuccess: TryGetFollowData,
                _OnFail: _Error =>
                {
                    if (m_Description.UseLog)
                        m_Log.WriteFormat(LOG_PATH_UPDATE_FAIL, _Error.Msg);

                    m_OnPathfindingFail?.Invoke(_Error);
                });
        }

        void TryGetFollowData()
        {
            if (m_Description.UseLog)
                m_Log.Write(LOG_GET_FOLLOW_DATA);

            if (m_GlobalPath != null && m_GlobalPath.IsValid)
                m_CurrFollowData = m_GlobalPath.GetFollowData(m_LastPosition);
        }

        void InitLocalAvoidanceSolvers()
        {
            if (m_Description.BehaviorType == BehaviorType.DEFAULT)
            {
                CreateMoverLASolver();

                m_MoverLASolver.SetFollowPoint(m_TargetPoint);
            }
            else if (m_Description.BehaviorType == BehaviorType.YIELDING)
            {
                m_MoverLASolver = new YieldingLocalAvoidanceSolver(this, m_Description);
            }

            m_StaticLASolver = new StaticLocalAvoidanceSolver(this, m_Description);

            m_StaticLASolver.SetFollowPoint(m_TargetPoint);
        }

        protected virtual void CreateMoverLASolver()
        {
            m_MoverLASolver = new MoverLocalAvoidanceSolver(this, m_Description);
        }

        void DisposeGlobalPath()
        {
            if (m_GlobalPath == null)
                return;

            m_GlobalPath.OnPathUpdated -= TryGetFollowData;
            m_GlobalPath.Dispose();
            m_GlobalPath = null;
        }

        void DisposeLocalAvoidanceSolvers()
        {
            m_MoverLASolver = null;
            m_StaticLASolver = null;
        }

        #endregion
    }
}