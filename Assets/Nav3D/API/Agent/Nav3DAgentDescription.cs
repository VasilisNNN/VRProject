using Nav3D.Common;
using System;
using System.Text;
using UnityEngine;

namespace Nav3D.API
{
    [CreateAssetMenu(fileName = "Agent description", menuName = "Nav3D/Agent description", order = 150)]
    public class Nav3DAgentDescription : ScriptableObject
    {
        #region Constants : Debug

        readonly string ORCA_TAU_INVALID = $"There is invalid {nameof(ORCATau)} parameter value: {{0}}. Correct it, or perform project update.";
        readonly string VELOCITY_WEIGHTS_INVALID = $"Some of the weight values are set incorrectly. " +
            $" Check the current values in the description. You probably set an incorrect value, or you didn't update the project.";

        #endregion

        #region Constants : Defaults

        const float DEFAULT_RADIUS = 0.25f;
        const float DEFAULT_SPEED = 0.01f;
        const float DEFAULT_SPEED_TO_MAX_SPEED_MULTIPLIER = 1.15f;
        const float DEFAULT_REACH_DISTANCE = 0f;

        const int DEFAULT_RADIUS_OBTAIN_MODE = 0;
        const int DEFAULT_SPEED_OBTAIN_MODE = 0;
        const int DEFAULT_MAX_SPEED_OBTAIN_MODE = 0;

        const bool DEFAULT_USE_CONSIDERED_AGENTS_NUMBER_LIMIT = false;
        const int DEFAULT_CONSIDERED_AGENTS_NUMBER_LIMIT = 3;

        const int DEFAULT_PATHFINDING_TIMEOUT = 2000; //ms
        const bool DEFAULT_SMOOTH_PATH = false;
        const int DEFAULT_SMOOTH_PATH_RATIO = 3;
        const MotionNavigationType DEFAULT_MOTION_NAV_TYPE = MotionNavigationType.GLOBAL_AND_LOCAL;
        const BehaviorType DEFAULT_BEHAVIOR_TYPE = BehaviorType.DEFAULT;
        const bool DEFAULT_AUTOUPDATE_PATH = true;
        const int DEFAULT_AUTOUPDATE_PATH_COOLDOWN = 2000;
        const float MAX_DEGREES_ROTATION_PER_TICK = 5f;

        const float DEFAULT_ORCA_TAU = 2.5f;

        //velocity blending weights for case when pathfinding performing and there are both agents and obstacles near
        const float DEFAULT_PATH_VELOCITY_WEIGHT = 1f;
        const float DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT = 10f;
        const float DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT = 5f;

        //velocity blending weights for case when pathfinding performing and there is only obstacles near
        const float DEFAULT_PATH_VELOCITY_WEIGHT1 = 1;
        const float DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT1 = 3f;

        //velocity blending weights for case when pathfinding performing and there is only agents near
        const float DEFAULT_PATH_VELOCITY_WEIGHT2 = 1f;
        const float DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT1 = 10f;

        //velocity blending weights for case when only local avoidance performing and there are both agents and obstacles near
        const float DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT2 = 1f;
        const float DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT2 = 1f;

        const bool DEFAULT_USE_LOG = false;
        const int DEFAULT_LOG_SIZE = 50;

        #endregion

        #region Events

        public event Action<MotionNavigationType> OnMotionNavTypeChanged;
        public event Action<BehaviorType> OnBehaviorTypeChanged;
        public event Action<float> OnMaxSpeedChanged;
        public event Action<float> OnSpeedChanged;
        public event Action<float> OnRadiusChanged;
        public event Action<int> OnSmoothRatioChanged;
        public event Action<bool> OnUseLogChanged;
        public event Action<int> OnLogSizeChanged;

        #endregion

        #region Attributes

        [SerializeField] float m_Radius;
        [SerializeField] float m_RadiusMin;
        [SerializeField] float m_RadiusMax;
        [SerializeField] int m_RadiusRandomRangeMode;    //0-uniform distribution, 1-normal distribution
        [SerializeField] int m_RadiusObtainMode;         //0-specific value, 1-random value

        [SerializeField] float m_Speed;
        [SerializeField] float m_SpeedMin;
        [SerializeField] float m_SpeedMax;
        [SerializeField] float m_MaxSpeed;
        [SerializeField] float m_SpeedToMaxSpeedMultiplier;
        [SerializeField] int m_SpeedObtainMode;                  //0-specific value, 1-random value
        [SerializeField] int m_SpeedRandomRangeMode;             //0-uniform distribution, 1-normal distribution
        [SerializeField] int m_MaxSpeedObtainMode;               //0-multiplier, 1-absolute value
        [SerializeField] bool m_UseConsideredAgentsNumberLimit;
        [SerializeField] int m_ConsideredAgentsNumberLimit;
        // ReSharper disable once InconsistentNaming
        [SerializeField] float m_ORCATau;

        [SerializeField] int m_PathfindingTimeout;   //ms
        [SerializeField] bool m_SmoothPath;
        [SerializeField] int m_SmoothRatio;
        [SerializeField] MotionNavigationType m_MotionNavigationType;
        [SerializeField] BehaviorType m_BehaviorType;
        [SerializeField] float m_TargetReachDistance;
        [SerializeField] float m_TargetReachDistanceSqr;
        [SerializeField] bool m_AutoUpdatePath;
        [SerializeField] int m_PathAutoUpdateCooldown;           //ms
        [SerializeField] float m_MaxAgentDegreesRotationPerTick;

        //velocities blending rules:
        //1) Following global path and there is some agents and obstacles near
        [SerializeField] float m_PathVelocityWeight;
        [SerializeField] float m_AgentsAvoidanceVelocityWeight;
        [SerializeField] float m_ObstacleAvoidanceVelocityWeight;

        //2) Following global path and there is some obstacles near
        [SerializeField] float m_PathVelocityWeight1;
        [SerializeField] float m_ObstacleAvoidanceVelocityWeight1;

        //3) Following global path and there is some agents near
        [SerializeField] float m_PathVelocityWeight2;
        [SerializeField] float m_AgentsAvoidanceVelocityWeight1;

        //4) We just use local avoidance and there is agents and obstacles near
        [SerializeField] float m_AgentsAvoidanceVelocityWeight2;
        [SerializeField] float m_ObstacleAvoidanceVelocityWeight2;

        [SerializeField] bool m_UseLog;
        [SerializeField] int m_LogSize;

        float m_VelocityDangerRadiusTau;
        float m_VelocityDangerRadius;
        float m_VelocityDangerRadiusSqr;
        bool m_VelocityDangerRadiusTauIsDirty = true;
        bool m_VelocityDangerRadiusIsDirty = true;

        #endregion

        #region Properties

        /// <summary>
        /// Agent radius used for local avoidance.
        /// </summary>
        public float Radius
        {
            get => m_Radius;
            set
            {
                if (value <= 0 || Mathf.Approximately(m_Radius, value))
                    return;

                m_Radius = value;

                m_VelocityDangerRadiusTauIsDirty = true;
                m_VelocityDangerRadiusIsDirty = true;

                OnRadiusChanged?.Invoke(m_Radius);
            }
        }
        public float RadiusMin
        {
            get => m_RadiusMin;
            set
            {
                m_RadiusMin = value;
                RegenerateRadius();
            }
        }
        public float RadiusMax
        {
            get => m_RadiusMax;
            set
            {
                m_RadiusMax = value;
                RegenerateRadius();
            }
        }
        public int RadiusRandomRangeMode
        {
            get => m_RadiusRandomRangeMode;
            set
            {
                m_RadiusRandomRangeMode = value;
                RegenerateRadius();
            }
        }
        public int RadiusObtainMode
        {
            get => m_RadiusObtainMode;
            set => m_RadiusObtainMode = value;
        }
        /// <summary>
        /// Desired speed in Unity distance per tick of the FixedUpdate event.
        /// </summary>
        public float Speed
        {
            get => m_Speed;
            set
            {
                if (value <= 0 || Mathf.Approximately(m_Speed, value))
                    return;

                if (m_MaxSpeedObtainMode == 0)
                {
                    m_Speed = value;

                    MaxSpeed = m_Speed * m_SpeedToMaxSpeedMultiplier;
                }
                else if (m_MaxSpeedObtainMode == 1)
                {
                    m_Speed = Mathf.Clamp(value, 0, m_MaxSpeed);
                }
                else
                {
                    throw new Exception($"Unknown m_MaxSpeedObtainMode type {value}");
                }

                m_VelocityDangerRadiusTauIsDirty = true;
                m_VelocityDangerRadiusIsDirty = true;

                OnSpeedChanged?.Invoke(m_Speed);
            }
        }
        public float SpeedMin
        {
            get => m_SpeedMin;
            set
            {
                m_SpeedMin = value;
                RegenerateSpeed();
            }
        }
        public float SpeedMax
        {
            get => m_SpeedMax; set
            {
                m_SpeedMax = value;
                RegenerateSpeed();
            }
        }
        /// <summary>
        /// The maximum speed allowed for performing local avoidance.
        /// </summary>
        public float MaxSpeed
        {
            get => m_MaxSpeed;
            set
            {
                float valueToSet = Mathf.Max(value, m_Speed);

                if (Mathf.Approximately(m_MaxSpeed, valueToSet))
                    return;

                m_MaxSpeed = valueToSet;

                m_VelocityDangerRadiusTauIsDirty = true;
                m_VelocityDangerRadiusIsDirty = true;

                OnMaxSpeedChanged?.Invoke(m_MaxSpeed);
            }
        }

        public float SqrMaxSpeed => MaxSpeed * MaxSpeed;

        public float FactualMaxSpeed
        {
            get
            {
                if (MotionNavigationType == MotionNavigationType.GLOBAL)
                    return Speed;

                return MaxSpeed;
            }
        }
        public float SpeedToMaxSpeedMultiplier
        {
            get => m_SpeedToMaxSpeedMultiplier;
            set => m_SpeedToMaxSpeedMultiplier = Mathf.Max(1 + float.Epsilon, value);
        }
        public int SpeedObtainMode
        {
            get => m_SpeedObtainMode;
            set => m_SpeedObtainMode = value;
        }
        public int SpeedRandomRangeMode
        {
            get => m_SpeedRandomRangeMode;
            set
            {
                m_SpeedRandomRangeMode = value;
                RegenerateSpeed();
            }
        }
        public int MaxSpeedObtainMode
        {
            get => m_MaxSpeedObtainMode;
            set => m_MaxSpeedObtainMode = value;
        }
        public bool UseConsideredAgentsNumberLimit
        {
            get => m_UseConsideredAgentsNumberLimit;
            set => m_UseConsideredAgentsNumberLimit = value;
        }
        public int ConsideredAgentsNumberLimit
        {
            get => m_ConsideredAgentsNumberLimit;
            set => m_ConsideredAgentsNumberLimit = Mathf.Max(1, value);
        }
        public int PathfindingTimeout
        {
            get => m_PathfindingTimeout;
            set
            {
                if (value <= 0 || m_PathfindingTimeout == value)
                    return;

                m_PathfindingTimeout = value;
            }
        }
        public bool SmoothPath
        {
            get => m_SmoothPath;
            set => m_SmoothPath = value;
        }
        public int SmoothRatio
        {
            get => m_SmoothRatio;
            set
            {
                if (value <= 0 || m_SmoothRatio == value)
                    return;

                m_SmoothRatio = value;

                OnSmoothRatioChanged?.Invoke(m_SmoothRatio);
            }
        }
        public MotionNavigationType MotionNavigationType
        {
            get => m_MotionNavigationType;
            set
            {
                if (value == MotionNavigationType.GLOBAL)
                {
                    if (BehaviorType == BehaviorType.YIELDING)
                        BehaviorType = BehaviorType.DEFAULT;
                }
                else if (value == MotionNavigationType.GLOBAL_AND_LOCAL)
                {
                    if (BehaviorType == BehaviorType.YIELDING)
                        BehaviorType = BehaviorType.DEFAULT;
                }

                m_MotionNavigationType = value;

                m_VelocityDangerRadiusTauIsDirty = true;
                m_VelocityDangerRadiusIsDirty = true;

                OnMotionNavTypeChanged?.Invoke(m_MotionNavigationType);
            }
        }
        public BehaviorType BehaviorType
        {
            get => m_BehaviorType;
            set
            {
                if (value == BehaviorType.YIELDING)
                    MotionNavigationType = MotionNavigationType.LOCAL;
                else if (value == BehaviorType.INDIFFERENT)
                    MotionNavigationType = MotionNavigationType.GLOBAL;

                m_BehaviorType = value;

                OnBehaviorTypeChanged?.Invoke(m_BehaviorType);
            }
        }
        public float TargetReachDistance
        {
            get => m_TargetReachDistance;
            set
            {
                if (value < 0)
                    return;

                m_TargetReachDistance = value;
                m_TargetReachDistanceSqr = m_TargetReachDistance * m_TargetReachDistance;
            }
        }
        public float TargetReachDistanceSqr => m_TargetReachDistanceSqr;
        public bool AutoUpdatePath
        {
            get => m_AutoUpdatePath;
            set => m_AutoUpdatePath = value;
        }
        public int PathAutoUpdateCooldown
        {
            get => m_PathAutoUpdateCooldown;
            set => m_PathAutoUpdateCooldown = Mathf.Max(1, value);
        }
        public float MaxAgentDegreesRotationPerTick
        {
            get => m_MaxAgentDegreesRotationPerTick;
            set => m_MaxAgentDegreesRotationPerTick = Mathf.Max(0f, value);
        }
        public float PathVelocityWeight
        {
            get => m_PathVelocityWeight;
            set => m_PathVelocityWeight = Mathf.Max(value, float.Epsilon);
        }
        public float PathVelocityWeight1
        {
            get => m_PathVelocityWeight1;
            set => m_PathVelocityWeight1 = Mathf.Max(value, float.Epsilon);
        }
        public float PathVelocityWeight2
        {
            get => m_PathVelocityWeight2;
            set => m_PathVelocityWeight2 = Mathf.Max(value, float.Epsilon);
        }
        public float AgentsAvoidanceVelocityWeight
        {
            get => m_AgentsAvoidanceVelocityWeight;
            set => m_AgentsAvoidanceVelocityWeight = Mathf.Max(value, float.Epsilon);
        }
        public float AgentsAvoidanceVelocityWeight1
        {
            get => m_AgentsAvoidanceVelocityWeight1;
            set => m_AgentsAvoidanceVelocityWeight1 = Mathf.Max(value, float.Epsilon);
        }
        public float AgentsAvoidanceVelocityWeight2
        {
            get => m_AgentsAvoidanceVelocityWeight2;
            set => m_AgentsAvoidanceVelocityWeight2 = Mathf.Max(value, float.Epsilon);
        }

        public float ObstaclesAvoidanceVelocityWeight
        {
            get => m_ObstacleAvoidanceVelocityWeight;
            set => m_ObstacleAvoidanceVelocityWeight = Mathf.Max(value, float.Epsilon);
        }

        public float ObstaclesAvoidanceVelocityWeight1
        {
            get => m_ObstacleAvoidanceVelocityWeight1;
            set => m_ObstacleAvoidanceVelocityWeight1 = Mathf.Max(value, float.Epsilon);
        }

        public float ObstaclesAvoidanceVelocityWeight2
        {
            get => m_ObstacleAvoidanceVelocityWeight2;
            set => m_ObstacleAvoidanceVelocityWeight2 = Mathf.Max(value, float.Epsilon);
        }

        // ReSharper disable once InconsistentNaming
        public float ORCATau
        {
            get => m_ORCATau;
            set
            {
                if (Mathf.Approximately(m_ORCATau, value) || m_ORCATau <= 0)
                    return;

                m_ORCATau = value;

                m_VelocityDangerRadiusTauIsDirty = true;
            }
        }


        public float VelocityRadius
        {
            get
            {
                if (m_VelocityDangerRadiusIsDirty)
                {
                    m_VelocityDangerRadius = Radius + FactualMaxSpeed;
                    m_VelocityDangerRadiusSqr = m_VelocityDangerRadius * m_VelocityDangerRadius;

                    m_VelocityDangerRadiusIsDirty = false;
                }

                return m_VelocityDangerRadius;
            }
        }

        public float VelocityRadiusSqr => m_VelocityDangerRadiusSqr;

        public float VelocityRadiusTauProjection
        {
            get
            {
                if (m_VelocityDangerRadiusTauIsDirty)
                {
                    m_VelocityDangerRadiusTau = Radius + FactualMaxSpeed * ORCATau;
                    m_VelocityDangerRadiusTauIsDirty = false;
                }

                return m_VelocityDangerRadiusTau;
            }
        }

        public bool UseLog
        {
            get => m_UseLog;
            set
            {
                if (value == m_UseLog)
                    return;

                m_UseLog = value;

                OnUseLogChanged?.Invoke(m_UseLog);
            }
        }

        public int LogSize
        {
            get => m_LogSize;
            set
            {
                int finalValue = Mathf.Max(value, 5);

                if (finalValue == m_LogSize)
                    return;

                m_LogSize = finalValue;

                OnLogSizeChanged?.Invoke(m_LogSize);
            }
        }

        public static Nav3DAgentDescription DefaultDescription
        {
            get
            {
                Nav3DAgentDescription newDescription = CreateInstance(typeof(Nav3DAgentDescription)) as Nav3DAgentDescription;
                // ReSharper disable once PossibleNullReferenceException
                newDescription.SetDefaultAttributes();
                newDescription.RegenerateRandomValues();

                return newDescription;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a copy of the current description with a variation of random parameters.
        /// </summary>
        public Nav3DAgentDescription GetDescriptionVariant()
        {
            Nav3DAgentDescription variant = GetDescriptionCopy();

            variant.RegenerateRandomValues();

            return variant;
        }

        /// <summary>
        /// Configures description by default.
        /// </summary>
        public void SetDefaultAttributes()
        {
            m_Radius = DEFAULT_RADIUS;
            m_RadiusObtainMode = DEFAULT_RADIUS_OBTAIN_MODE;
            m_Speed = DEFAULT_SPEED;
            m_SpeedObtainMode = DEFAULT_SPEED_OBTAIN_MODE;
            m_SpeedToMaxSpeedMultiplier = DEFAULT_SPEED_TO_MAX_SPEED_MULTIPLIER;
            m_MaxSpeedObtainMode = DEFAULT_MAX_SPEED_OBTAIN_MODE;
            m_UseConsideredAgentsNumberLimit = DEFAULT_USE_CONSIDERED_AGENTS_NUMBER_LIMIT;
            m_ConsideredAgentsNumberLimit = DEFAULT_CONSIDERED_AGENTS_NUMBER_LIMIT;
            m_TargetReachDistance = DEFAULT_REACH_DISTANCE;
            m_PathfindingTimeout = DEFAULT_PATHFINDING_TIMEOUT;
            m_SmoothPath = DEFAULT_SMOOTH_PATH;
            m_SmoothRatio = DEFAULT_SMOOTH_PATH_RATIO;
            m_MotionNavigationType = DEFAULT_MOTION_NAV_TYPE;
            m_BehaviorType = DEFAULT_BEHAVIOR_TYPE;
            m_AutoUpdatePath = DEFAULT_AUTOUPDATE_PATH;
            m_PathAutoUpdateCooldown = DEFAULT_AUTOUPDATE_PATH_COOLDOWN;
            m_MaxAgentDegreesRotationPerTick = MAX_DEGREES_ROTATION_PER_TICK;
            m_PathVelocityWeight = DEFAULT_PATH_VELOCITY_WEIGHT;
            m_PathVelocityWeight1 = DEFAULT_PATH_VELOCITY_WEIGHT1;
            m_PathVelocityWeight2 = DEFAULT_PATH_VELOCITY_WEIGHT2;
            m_AgentsAvoidanceVelocityWeight = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT;
            m_AgentsAvoidanceVelocityWeight1 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT1;
            m_AgentsAvoidanceVelocityWeight2 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT2;
            m_ObstacleAvoidanceVelocityWeight = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT;
            m_ObstacleAvoidanceVelocityWeight1 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT1;
            m_ObstacleAvoidanceVelocityWeight2 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT2;
            m_ORCATau = DEFAULT_ORCA_TAU;
            m_UseLog = DEFAULT_USE_LOG;
            m_LogSize = DEFAULT_LOG_SIZE;
        }

        // ReSharper disable once InconsistentNaming
        public void SetDefaultORCATau()
        {
            m_ORCATau = DEFAULT_ORCA_TAU;
        }

        public void SetDefaultPathVelocityWeight1()
        {
            m_PathVelocityWeight1 = DEFAULT_PATH_VELOCITY_WEIGHT1;
        }

        public void SetDefaultObstacleAvoidanceVelocityWeihgt1()
        {
            m_ObstacleAvoidanceVelocityWeight1 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT1;
        }

        public void SetDefaultPathVelocityWeight2()
        {
            m_PathVelocityWeight2 = DEFAULT_PATH_VELOCITY_WEIGHT2;
        }

        public void SetDefaultAgentsAvoidanceVelocityWeight1()
        {
            m_AgentsAvoidanceVelocityWeight1 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT1;
        }

        public void SetDefaultAgentsAvoidanceVelocityWeight2()
        {
            m_AgentsAvoidanceVelocityWeight2 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT2;
        }

        public void SetDefaultObstacleAvoidanceVelocityWeight2()
        {
            m_ObstacleAvoidanceVelocityWeight2 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT2;
        }

        public void SetDefaultVelocitiesBlendingWeights()
        {
            m_PathVelocityWeight = DEFAULT_PATH_VELOCITY_WEIGHT;
            m_PathVelocityWeight1 = DEFAULT_PATH_VELOCITY_WEIGHT1;
            m_PathVelocityWeight2 = DEFAULT_PATH_VELOCITY_WEIGHT2;

            m_ObstacleAvoidanceVelocityWeight = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT;
            m_ObstacleAvoidanceVelocityWeight1 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT1;
            m_ObstacleAvoidanceVelocityWeight2 = DEFAULT_OBSTACLES_AVOIDANCE_VELOCITY_WEIGHT2;

            m_AgentsAvoidanceVelocityWeight = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT;
            m_AgentsAvoidanceVelocityWeight1 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT1;
            m_AgentsAvoidanceVelocityWeight2 = DEFAULT_AGENTS_AVOIDANCE_VELOCITY_WEIGHT2;
        }

        public void FixInvalidParams(bool _Verbose = true)
        {
            if (m_ORCATau <= 0)
            {
                if (_Verbose)
                    Debug.LogWarning(string.Format(ORCA_TAU_INVALID, ORCATau));

                SetDefaultORCATau();
            }

            if (m_PathVelocityWeight1 == 0 || m_PathVelocityWeight2 == 0 ||
                m_AgentsAvoidanceVelocityWeight1 == 0 || m_AgentsAvoidanceVelocityWeight2 == 0 ||
                m_ObstacleAvoidanceVelocityWeight1 == 0 || m_ObstacleAvoidanceVelocityWeight2 == 0)
            {
                if (_Verbose)
                    Debug.LogWarning(VELOCITY_WEIGHTS_INVALID);

                SetDefaultVelocitiesBlendingWeights();
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"{nameof(m_Radius)}: {m_Radius}");
            stringBuilder.AppendLine($"{nameof(m_RadiusMin)}: {m_RadiusMin}");
            stringBuilder.AppendLine($"{nameof(m_RadiusMax)}: {m_RadiusMax}");
            stringBuilder.AppendLine($"{nameof(m_RadiusRandomRangeMode)}: {m_RadiusRandomRangeMode}");
            stringBuilder.AppendLine($"{nameof(m_RadiusObtainMode)}: {m_RadiusObtainMode}");

            stringBuilder.AppendLine($"{nameof(m_Speed)}: {m_Radius}");
            stringBuilder.AppendLine($"{nameof(m_SpeedMin)}: {m_SpeedMin}");
            stringBuilder.AppendLine($"{nameof(m_SpeedMax)}: {m_SpeedMax}");
            stringBuilder.AppendLine($"{nameof(m_MaxSpeed)}: {m_MaxSpeed}");
            stringBuilder.AppendLine($"{nameof(m_SpeedToMaxSpeedMultiplier)}: {m_SpeedToMaxSpeedMultiplier}");
            stringBuilder.AppendLine($"{nameof(m_SpeedObtainMode)}: {m_SpeedObtainMode}");
            stringBuilder.AppendLine($"{nameof(m_SpeedRandomRangeMode)}: {m_SpeedRandomRangeMode}");
            stringBuilder.AppendLine($"{nameof(m_MaxSpeedObtainMode)}: {m_MaxSpeedObtainMode}");
            stringBuilder.AppendLine($"{nameof(m_UseConsideredAgentsNumberLimit)}: {m_UseConsideredAgentsNumberLimit}");
            stringBuilder.AppendLine($"{nameof(m_ConsideredAgentsNumberLimit)}: {m_ConsideredAgentsNumberLimit}");

            stringBuilder.AppendLine($"{nameof(m_PathfindingTimeout)}: {m_PathfindingTimeout}");
            stringBuilder.AppendLine($"{nameof(m_SmoothPath)}: {m_SmoothPath}");
            stringBuilder.AppendLine($"{nameof(m_SmoothRatio)}: {m_SmoothRatio}");

            stringBuilder.AppendLine($"{nameof(m_MotionNavigationType)}: {m_MotionNavigationType}");
            stringBuilder.AppendLine($"{nameof(m_BehaviorType)}: {m_BehaviorType}");

            stringBuilder.AppendLine($"{nameof(m_TargetReachDistance)}: {m_TargetReachDistance}");
            stringBuilder.AppendLine($"{nameof(m_TargetReachDistanceSqr)}: {m_TargetReachDistanceSqr}");
            stringBuilder.AppendLine($"{nameof(m_AutoUpdatePath)}: {m_AutoUpdatePath}");
            stringBuilder.AppendLine($"{nameof(m_PathAutoUpdateCooldown)}: {m_PathAutoUpdateCooldown}");
            stringBuilder.AppendLine($"{nameof(m_MaxAgentDegreesRotationPerTick)}: {m_MaxAgentDegreesRotationPerTick}");

            stringBuilder.AppendLine($"{nameof(m_PathVelocityWeight)}: {m_PathVelocityWeight}");
            stringBuilder.AppendLine($"{nameof(m_PathVelocityWeight1)}: {m_PathVelocityWeight1}");
            stringBuilder.AppendLine($"{nameof(m_PathVelocityWeight2)}: {m_PathVelocityWeight2}");
            stringBuilder.AppendLine($"{nameof(m_AgentsAvoidanceVelocityWeight)}: {m_AgentsAvoidanceVelocityWeight}");
            stringBuilder.AppendLine($"{nameof(m_AgentsAvoidanceVelocityWeight1)}: {m_AgentsAvoidanceVelocityWeight1}");
            stringBuilder.AppendLine($"{nameof(m_AgentsAvoidanceVelocityWeight2)}: {m_AgentsAvoidanceVelocityWeight2}");
            stringBuilder.AppendLine($"{nameof(m_ObstacleAvoidanceVelocityWeight)}: {m_ObstacleAvoidanceVelocityWeight}");
            stringBuilder.AppendLine($"{nameof(m_ObstacleAvoidanceVelocityWeight1)}: {m_ObstacleAvoidanceVelocityWeight1}");
            stringBuilder.AppendLine($"{nameof(m_ObstacleAvoidanceVelocityWeight2)}: {m_ObstacleAvoidanceVelocityWeight2}");
            stringBuilder.AppendLine($"{nameof(m_ORCATau)}: {m_ORCATau}");

            stringBuilder.AppendLine($"{nameof(m_UseLog)}: {m_UseLog}");
            stringBuilder.AppendLine($"{nameof(m_LogSize)}: {m_LogSize}");

            return stringBuilder.ToString();
        }

        #endregion

        #region Service methods

        Nav3DAgentDescription GetDescriptionCopy()
        {
            Nav3DAgentDescription copy = CreateInstance(typeof(Nav3DAgentDescription)) as Nav3DAgentDescription;

            // ReSharper disable once PossibleNullReferenceException
            copy.m_Radius = m_Radius;
            copy.m_RadiusMin = m_RadiusMin;
            copy.m_RadiusMax = m_RadiusMax;
            copy.m_RadiusRandomRangeMode = m_RadiusRandomRangeMode;
            copy.m_RadiusObtainMode = m_RadiusObtainMode;
            copy.m_MaxSpeedObtainMode = m_MaxSpeedObtainMode;
            copy.m_SpeedToMaxSpeedMultiplier = m_SpeedToMaxSpeedMultiplier;
            copy.m_Speed = m_Speed;
            copy.m_SpeedMin = m_SpeedMin;
            copy.m_SpeedMax = m_SpeedMax;
            copy.m_MaxSpeed = m_MaxSpeed;
            copy.m_UseConsideredAgentsNumberLimit = m_UseConsideredAgentsNumberLimit;
            copy.m_ConsideredAgentsNumberLimit = m_ConsideredAgentsNumberLimit;
            copy.m_SpeedObtainMode = m_SpeedObtainMode;
            copy.m_SpeedRandomRangeMode = m_SpeedRandomRangeMode;
            copy.m_TargetReachDistance = m_TargetReachDistance;
            copy.m_PathfindingTimeout = m_PathfindingTimeout;
            copy.m_SmoothPath = m_SmoothPath;
            copy.m_SmoothRatio = m_SmoothRatio;
            copy.m_MotionNavigationType = m_MotionNavigationType;
            copy.m_BehaviorType = m_BehaviorType;
            copy.m_AutoUpdatePath = m_AutoUpdatePath;
            copy.m_PathAutoUpdateCooldown = m_PathAutoUpdateCooldown;
            copy.m_MaxAgentDegreesRotationPerTick = m_MaxAgentDegreesRotationPerTick;
            copy.m_PathVelocityWeight = m_PathVelocityWeight;
            copy.m_PathVelocityWeight1 = m_PathVelocityWeight1;
            copy.m_PathVelocityWeight2 = m_PathVelocityWeight2;
            copy.m_AgentsAvoidanceVelocityWeight = m_AgentsAvoidanceVelocityWeight;
            copy.m_AgentsAvoidanceVelocityWeight1 = m_AgentsAvoidanceVelocityWeight1;
            copy.m_AgentsAvoidanceVelocityWeight2 = m_AgentsAvoidanceVelocityWeight2;
            copy.m_ObstacleAvoidanceVelocityWeight = m_ObstacleAvoidanceVelocityWeight;
            copy.m_ObstacleAvoidanceVelocityWeight1 = m_ObstacleAvoidanceVelocityWeight1;
            copy.m_ObstacleAvoidanceVelocityWeight2 = m_ObstacleAvoidanceVelocityWeight2;
            copy.m_ORCATau = m_ORCATau;
            copy.m_UseLog = m_UseLog;
            copy.m_LogSize = m_LogSize;

            return copy;
        }

        void RegenerateRandomValues()
        {
            RegenerateRadius();
            RegenerateSpeed();
        }

        void RegenerateRadius()
        {
            if (m_RadiusObtainMode == 1)
            {
                m_Radius = m_RadiusRandomRangeMode == 0 ?
                    UtilsMath.GetRandomUniformValue(m_RadiusMin, m_RadiusMax) :
                    UtilsMath.GetRandomNormalValue(m_RadiusMin, m_RadiusMax);
            }
        }

        void RegenerateSpeed()
        {
            if (m_SpeedObtainMode == 1)
            {
                m_Speed = m_SpeedRandomRangeMode == 0 ?
                    UtilsMath.GetRandomUniformValue(m_SpeedMin, m_SpeedMax) :
                    UtilsMath.GetRandomNormalValue(m_SpeedMin, m_SpeedMax);
            }

            if (m_MaxSpeedObtainMode == 0)
                m_MaxSpeed = m_Speed * m_SpeedToMaxSpeedMultiplier;
        }

        #endregion
    }
}
