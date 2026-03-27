using Nav3D.Common;
using Nav3D.LocalAvoidance;
using System;
using System.Collections.Generic;
using UnityEngine;
using Log = Nav3D.Common.Log;

namespace Nav3D.API
{
    public class Nav3DAgent : MonoBehaviour
    {
        #region Constants

        readonly string LOG_DESCRIPTION_SET = $"{nameof(Nav3DAgent)}.{nameof(SetDescription)}";
        readonly string LOG_MOVE_TO = $"{nameof(Nav3DAgent)}.{nameof(MoveTo)}: {{0}}";
        readonly string LOG_MOVE_TO_ENQUEUE = $"{nameof(Nav3DAgent)}.{nameof(MoveToEnqueue)}: {{0}}";
        readonly string LOG_FOLLOW_TARGET = $"{nameof(Nav3DAgent)}.{nameof(FollowTarget)}: Transfrom: {{0}}:{{1}}, ToleranceUpdate: {{2}}, Reach dist: {{3}}";
        readonly string LOG_STOP = $"{nameof(Nav3DAgent)}.{nameof(StopInternal)}";
        readonly string LOG_BEGIN_MOVE_TO = $"{nameof(Nav3DAgent)}.{nameof(BeginMoveTo)}: {{0}}";

        const string LOG_DESABLED_ERROR = "Logging is disabled for agent.";

        readonly string MOVE_TO_YIELDING_EX = $"Impossible execution. It is not possible to call the {nameof(MoveTo)}, {nameof(MoveToEnqueue)}, " +
            $"{nameof(FollowTarget)} methods on an agent with {nameof(BehaviorType)} equal to {nameof(BehaviorType.YIELDING)}.";
        readonly string MOVE_TO_INDIFFERENT_EX = $"Impossible execution. It is not possible to call the {nameof(MoveTo)}, {nameof(MoveToEnqueue)}, " +
            $"{nameof(FollowTarget)} methods on an agent with {nameof(BehaviorType)} equal to {nameof(BehaviorType.INDIFFERENT)}.";

        readonly string ON_INIT_UNSUBSCRIBE_ERROR = $"There is no need to unsubscribe from the {nameof(OnAgentInit)} event. " +
            $"All subscriptions will be unsubscribed after {nameof(Nav3DAgent)} is initialized.";

        #endregion

        #region Events

        event Action OnAgentInitInternal
        {
            add
            {
                if (value == null)
                    return;

                if (Inited)
                {
                    value.Invoke();

                    return;
                }

                m_OnInitInternalSubscribers.Add(value);

                m_OnAgentInitInternal += value;
            }
            remove
            {
                m_OnInitInternalSubscribers.Remove(value);

                m_OnAgentInitInternal -= value;
            }
        }

        public event Action OnAgentInit
        {
            add
            {
                if (value == null)
                    return;

                if (Inited)
                {
                    ThreadDispatcher.BeginInvoke(value);

                    return;
                }

                Action subscriber = () => ThreadDispatcher.BeginInvoke(value);

                m_OnInitSubscribers.Add(subscriber);

                m_OnAgentInit += subscriber;
            }
            remove
            {
                Debug.LogError(ON_INIT_UNSUBSCRIBE_ERROR);
            }
        }

        #endregion

        #region Serialized fields

        [SerializeField] Nav3DAgentDescription m_Description;

        [Space, Header("Draw agent operating properties")]
        [SerializeField] bool m_DrawRadius;
        [SerializeField] bool m_DrawVelocities;
        [SerializeField] bool m_DrawCurrentPath;

        [Space, Header("Show agent nearest avoidance targets")]
        [SerializeField] bool m_ShowAgents;
        [SerializeField] bool m_ShowStaticTriangles;

        #endregion

        #region Attributes common

        List<Action> m_OnInitSubscribers = new List<Action>();
        List<Action> m_OnInitInternalSubscribers = new List<Action>();

        event Action m_OnAgentInit;
        event Action m_OnAgentInitInternal;

        //cached transform
        protected Transform m_CachedTransform;

        // ReSharper disable once MemberCanBePrivate.Global
        protected Nav3DAgentDescription m_DescriptionInternal;
        protected Nav3DAgentMover m_Mover;

        Action m_OnReach;
        Action<PathfindingError> m_OnPathfindingFail;

        OperatingMode m_CurOperatingMode;
        Vector3 m_CurMoveToPoint;

        Log m_Log;

        #endregion

        #region Attributes: follow target mode

        Transform m_TargetInternal;
        Vector3 m_LastTargetPosition;
        float m_SqrOffsetToleranceUpdate;
        float m_SqrGapToReach;

        Action m_OnCancelPersecution;

        #endregion

        #region Attributes: points queue following

        Queue<(Vector3 _Point, Action _OnReach, Action<PathfindingError> _OnFail)> m_PointsQueue = new Queue<(Vector3 _Point, Action _OnReach, Action<PathfindingError> _OnFail)>();

        #endregion

        #region Properties

        public bool Inited { get; private set; }
        public OperatingMode OperatingMode => m_CurOperatingMode;
        /// <summary>
        /// Cached agent's transform
        /// </summary>
        public Transform Transform => m_CachedTransform;
        /// <summary>
        /// The agent's radius taken from it's description
        /// </summary>
        public float Radius => m_DescriptionInternal?.Radius ?? 0;
        /// <summary>
        /// Agent description instance.
        /// </summary>
        public Nav3DAgentDescription Description => m_DescriptionInternal;

        #endregion

        #region Public methods

        /// <summary>
        /// Sets specific description to an agent.
        /// </summary>
        /// <param name="_Description"></param>
        public void SetDescription(Nav3DAgentDescription _Description)
        {
            Nav3DManager.CheckInitedSoft();

            OnAgentInitInternal += () =>
            {
                UnsubscribeDescriptionEvents();

                m_DescriptionInternal = m_Description = _Description;

                //Workaround for the case when the project update was not performed or some parameters in the description have invalid values
                m_DescriptionInternal.FixInvalidParams();

                SubscribeDescriptionEvents();

                OnUseLogChanged(m_DescriptionInternal.UseLog);

                m_Mover.SetDescription(m_DescriptionInternal);

                if (m_DescriptionInternal.UseLog)
                    m_Log.Write(LOG_DESCRIPTION_SET);
            };
        }

        /// <summary>
        /// Order to immediately follow to the point.
        /// </summary>
        /// <param name="_Point">Point to follow.</param>
        /// <param name="_OnReach">On point reach.</param>
        /// <param name="_OnPathfindingFail">On pathfinding failed action.</param>
        public void MoveTo(Vector3 _Point, Action _OnReach = null, Action<PathfindingError> _OnPathfindingFail = null)
        {
            Nav3DManager.CheckInitedSoft();

            OnAgentInitInternal += () =>
            {

                if (m_DescriptionInternal.BehaviorType == BehaviorType.YIELDING)
                    throw new Exception(MOVE_TO_YIELDING_EX);

                if (m_DescriptionInternal.BehaviorType == BehaviorType.INDIFFERENT)
                    throw new Exception(MOVE_TO_INDIFFERENT_EX);

                if (m_CurOperatingMode != OperatingMode.POINT_FOLLOWING)
                {
                    StopInternal();

                    m_CurOperatingMode = OperatingMode.POINT_FOLLOWING;
                }

                if (m_DescriptionInternal.UseLog)
                    m_Log.WriteFormat(LOG_MOVE_TO, _Point.ToStringExt());

                BeginMoveTo(_Point, _OnReach, _OnPathfindingFail);
            };
        }

        /// <summary>
        /// Queues the next point to follow.
        /// </summary>
        /// <param name="_Point">Point to follow.</param>
        /// <param name="_OnReach">On point reach.</param>
        /// <param name="_OnPathfindingFail">On pathfinding failed action.</param>
        public void MoveToEnqueue(Vector3 _Point, Action _OnReach = null, Action<PathfindingError> _OnPathfindingFail = null)
        {
            Nav3DManager.CheckInitedSoft();

            OnAgentInitInternal += () =>
            {
                if (m_DescriptionInternal.BehaviorType == BehaviorType.YIELDING)
                    throw new Exception(MOVE_TO_YIELDING_EX);

                if (m_DescriptionInternal.BehaviorType == BehaviorType.INDIFFERENT)
                    throw new Exception(MOVE_TO_INDIFFERENT_EX);

                OperatingMode preOperatingMode = m_CurOperatingMode;

                if (m_CurOperatingMode != OperatingMode.POINTS_FOLLOWING_QUEUE)
                {
                    StopInternal();

                    m_CurOperatingMode = OperatingMode.POINTS_FOLLOWING_QUEUE;
                }

                if (m_DescriptionInternal.UseLog)
                    m_Log.WriteFormat(LOG_MOVE_TO_ENQUEUE, _Point.ToStringExt());

                //if agent mode is target persecution immediately starts moving to point.
                //else add point to queue
                if (preOperatingMode == OperatingMode.MOVABLE_PERSECUTION)
                    BeginMoveTo(_Point, _OnReach, _OnPathfindingFail);
                else
                    m_PointsQueue.Enqueue((_Point, _OnReach, _OnPathfindingFail));
            };
        }

        /// <summary>
        /// Persecute a moving transform.
        /// </summary>
        /// <param name="_Target">Target transform to persecute</param>
        /// <param name="_OffsetToleranceUpdate">The transform offset needed to update the path. </param>
        /// <param name="_DistToReach">The distance to the target needed to reach.</param>
        /// <param name="_OnReach">On target reach.</param>
        /// <param name="_OnCancel">Executes if the target transform reference is null or missing.</param>
        /// <param name="_OnPathfindingFail">On pathfinding failed action (can be executed multiple times).</param>
        public void FollowTarget(
            Transform _Target,
            float _OffsetToleranceUpdate,
            float _DistToReach = 0,
            Action _OnReach = null,
            Action _OnCancel = null,
            Action<PathfindingError> _OnPathfindingFail = null
        )
        {
            Nav3DManager.CheckInitedSoft();

            OnAgentInitInternal += () =>
            {
                if (m_DescriptionInternal.BehaviorType == BehaviorType.YIELDING)
                    throw new Exception(MOVE_TO_YIELDING_EX);

                if (m_DescriptionInternal.BehaviorType == BehaviorType.INDIFFERENT)
                    throw new Exception(MOVE_TO_INDIFFERENT_EX);

                if (m_CurOperatingMode != OperatingMode.MOVABLE_PERSECUTION)
                    StopInternal();

                m_CurOperatingMode = OperatingMode.MOVABLE_PERSECUTION;

                m_SqrOffsetToleranceUpdate = _OffsetToleranceUpdate * _OffsetToleranceUpdate;
                m_SqrGapToReach = _DistToReach * _DistToReach;

                m_TargetInternal = _Target;
                m_LastTargetPosition = m_TargetInternal.position;

                m_OnCancelPersecution = _OnCancel;

                if (m_DescriptionInternal.UseLog)
                    m_Log.WriteFormat(LOG_FOLLOW_TARGET, _Target.name, _Target.GetInstanceID(), _OffsetToleranceUpdate, _DistToReach);

                BeginMoveTo(m_LastTargetPosition, _OnReach, _OnPathfindingFail);
            };
        }

        /// <summary>
        /// Stops current order execution.
        /// </summary>
        public void Stop()
        {
            Nav3DManager.CheckInitedSoft();

            OnAgentInitInternal += StopInternal;
        }

        /// <summary>
        /// Returns the list of agents distance to is less then radius.
        /// </summary>
        /// <param name="_Radius">Radius.</param>
        /// <param name="_Predicate">Predicate for agents filtering.</param>
        public List<Nav3DAgent> GetAgentsInRadius(float _Radius, Predicate<Nav3DAgent> _Predicate = null)
        {
            Nav3DManager.CheckInitedHard();

            List<Nav3DAgent> result = AgentManager.Instance.GetAgentsInSphere(m_CachedTransform.position, _Radius);

            if (_Predicate != null)
                result.RemoveAll(_Agent => !_Predicate.Invoke(_Agent));

            return result;
        }

        public string GetLogText()
        {
            return m_DescriptionInternal.UseLog ? m_Log.GetText() : LOG_DESABLED_ERROR;
        }

        public void DoFixedUpdate()
        {
            if (!Inited)
                return;

            if (m_CurOperatingMode != OperatingMode.IDLE || m_DescriptionInternal.BehaviorType != BehaviorType.DEFAULT)
            {
                CheckTarget();

                Move();
            }

            m_Mover.SetLastPosition(m_CachedTransform.position);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Visualizes agent with Gizmos.
        /// </summary>
        public void Visualize(bool _DrawRadius = true, bool _DrawPath = true, bool _DrawVelocities = true)
        {
            m_Mover.Visualize(_DrawRadius, _DrawPath, _DrawVelocities);
        }

        public void VisualizeNearestMovers()
        {
            m_Mover?.VisualizeNearestMovers();
        }

        public void VisualizeNearestObstacleTriangles()
        {
            m_Mover?.VisualizeNearestObstacleTriangles();
        }
#endif

        #endregion

        #region Service methods

        void DoAwake()
        {
            m_CachedTransform = transform;
        }

        void Initialize()
        {
            if (Inited)
                return;

            AgentManager.Instance.RegisterAgent(this);

            Nav3DManager.OnNav3DInitInternal += () =>
            {
                SetDefaultDescription();

                CreateMover(m_DescriptionInternal);

                OnUseLogChanged(m_DescriptionInternal.UseLog);

                MarkAsInited();
            };
        }

        void Uninitialize()
        {
            Inited = false;

            if (!AgentManager.Doomed)
                AgentManager.Instance.UnregisterAgent(this);

            UnsubscribeDescriptionEvents();

            m_Mover?.Uninitialize();

            m_Log?.Clear();
        }

        void SetDefaultDescription()
        {
            //Use default description, or take default
            m_DescriptionInternal ??= m_Description?.GetDescriptionVariant() ?? Nav3DAgentDescription.DefaultDescription;
        }

        protected virtual void CreateMover(Nav3DAgentDescription _Description)
        {
            m_Mover = new Nav3DAgentMover(m_CachedTransform.position, _Description, this);
        }

        void OnUseLogChanged(bool _UseLog)
        {
            if (_UseLog)
                m_Log = new Log(m_DescriptionInternal.LogSize);
            else
                m_Log = null;

            m_Mover.SetLogPtr(m_Log);
        }

        void MarkAsInited()
        {
            Inited = true;

            m_OnAgentInitInternal?.Invoke();
            m_OnAgentInit?.Invoke();

            UnsubscribeOnInitInternalSubscribers();
            UnsubscribeOnInitSubscribers();
        }

        void UnsubscribeOnInitInternalSubscribers()
        {
            foreach (Action subscriber in m_OnInitInternalSubscribers)
            {
                m_OnAgentInitInternal -= subscriber;
            }

            m_OnInitInternalSubscribers.Clear();
        }

        void UnsubscribeOnInitSubscribers()
        {
            foreach (Action subscriber in m_OnInitSubscribers)
            {
                m_OnAgentInit -= subscriber;
            }

            m_OnInitSubscribers.Clear();
        }



        void BeginMoveTo(Vector3 _Point, Action _OnReach, Action<PathfindingError> _OnPathfindingFail)
        {
            if (m_DescriptionInternal.UseLog)
                m_Log.WriteFormat(LOG_BEGIN_MOVE_TO, _Point.ToStringExt());

            if (!isActiveAndEnabled)
                return;

            m_Mover.SetLastPosition(m_CachedTransform.position);
            m_Mover.BeginMoveTo(_Point, _OnPathfindingFail);

            m_CurMoveToPoint = _Point;
            m_OnReach = _OnReach;
            m_OnPathfindingFail = _OnPathfindingFail;
        }

        void StopInternal()
        {
            m_CurOperatingMode = OperatingMode.IDLE;
            m_PointsQueue.Clear();

            if (m_DescriptionInternal.UseLog)
                m_Log.Write(LOG_STOP);
        }

        void CheckTarget()
        {
            if (m_CurOperatingMode != OperatingMode.MOVABLE_PERSECUTION)
                return;

            if (m_TargetInternal == null || !m_TargetInternal.gameObject.activeInHierarchy)
            {
                m_CurOperatingMode = OperatingMode.IDLE;
                m_OnCancelPersecution?.Invoke();

                return;
            }

            Vector3 curTargetPos = m_TargetInternal.position;

            if ((m_LastTargetPosition - curTargetPos).sqrMagnitude >= m_SqrOffsetToleranceUpdate || m_DescriptionInternal.MotionNavigationType == MotionNavigationType.LOCAL)
            {
                m_LastTargetPosition = curTargetPos;
                BeginMoveTo(m_LastTargetPosition, m_OnReach, m_OnPathfindingFail);
            }
        }

        void Move()
        {
            Vector3 frameVelocity;

            if (m_DescriptionInternal.BehaviorType != BehaviorType.INDIFFERENT)
            {
                frameVelocity = m_Mover.GetFrameVelocity();
                m_CachedTransform.position += frameVelocity;
            }
            else
            {
                frameVelocity = m_CachedTransform.position - m_Mover.LastPosition;
            }

            if (frameVelocity != Vector3.zero)
                m_CachedTransform.rotation = Quaternion.RotateTowards(
                    m_CachedTransform.rotation,
                    Quaternion.LookRotation(frameVelocity),
                    m_DescriptionInternal.MaxAgentDegreesRotationPerTick
                );

            if (m_DescriptionInternal.BehaviorType == BehaviorType.DEFAULT)
            {
                float reachThreshold = m_CurOperatingMode == OperatingMode.MOVABLE_PERSECUTION ? m_SqrGapToReach : m_DescriptionInternal.FactualMaxSpeed;

                if ((m_CachedTransform.position - m_CurMoveToPoint).magnitude - m_DescriptionInternal.TargetReachDistance <= reachThreshold)
                    OnGoalReach();
            }
        }

        void OnGoalReach()
        {
            void MoveToNextPoint()
            {
                (Vector3 Point, Action OnReach, Action<PathfindingError> OnFail) pointOrder = m_PointsQueue.Dequeue();

                //follow to the next point, if pathfinding fails - follow to the next and so on.
                BeginMoveTo(pointOrder.Point, pointOrder.OnReach, _Error =>
                {
                    if (m_PointsQueue.Count > 0)
                        MoveToNextPoint();

                    pointOrder.OnFail?.Invoke(_Error);
                });
            }

            if (m_CurOperatingMode == OperatingMode.POINTS_FOLLOWING_QUEUE && m_PointsQueue.Count > 0)
            {
                MoveToNextPoint();
            }
            else
            {
                m_CurOperatingMode = OperatingMode.IDLE;
            }

            m_OnReach?.Invoke();
        }

        void OnLogSizeChanged(int _LogSize)
        {
            if (m_DescriptionInternal.UseLog)
            {
                m_Log = new Log(_LogSize);
                m_Mover.SetLogPtr(m_Log);
            }
        }

        void SubscribeDescriptionEvents()
        {
            m_DescriptionInternal.OnUseLogChanged += OnUseLogChanged;
            m_DescriptionInternal.OnLogSizeChanged += OnLogSizeChanged;
        }

        void UnsubscribeDescriptionEvents()
        {
            if (m_DescriptionInternal == null)
                return;

            m_DescriptionInternal.OnUseLogChanged -= OnUseLogChanged;
            m_DescriptionInternal.OnLogSizeChanged -= OnLogSizeChanged;
        }

        #endregion

        #region Unity events

        void Awake()
        {
            DoAwake();
        }

        void OnEnable()
        {
            Initialize();
        }

        void OnDestroy()
        {
            Uninitialize();
        }

        void OnDisable()
        {
            Uninitialize();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!enabled)
                return;

            if (!Application.isPlaying)
            {
                Gizmos.color = Color.white;

                if (m_DrawRadius && m_Description != null)
                    Gizmos.DrawWireSphere(transform.position, m_Description.Radius);
            }
            else
            {
                using (Common.Debug.UtilsGizmos.ColorPermanence)
                {
                    //draw operating parameters
                    if (m_DrawRadius || m_DrawVelocities || m_DrawCurrentPath)
                        Visualize(m_DrawRadius, m_DrawCurrentPath, m_DrawVelocities);

                    //draw nearest agents
                    if (m_ShowAgents)
                        VisualizeNearestMovers();

                    //draw nearest obstacle's triangles
                    if (m_ShowStaticTriangles)
                        VisualizeNearestObstacleTriangles();
                }
            }
        }
#endif

        #endregion
    }
}
