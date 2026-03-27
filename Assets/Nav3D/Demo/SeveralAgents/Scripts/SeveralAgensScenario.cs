using Nav3D.API;
using Nav3D.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nav3D.Demo
{
    public class SeveralAgensScenario : MonoBehaviour
    {
        #region Constants

        const float DEFAULT_BLEND_FACTOR = 0.01f;
        const float DEFAULT_MAX_SPEED_FACTOR = 1.1f;
        const float DEFAULT_AGENTS_SPEED = 0.07f;
        const float DEFAULT_AGENTS_RADIUS = 0.25f;
        const int DEFAULT_AGENTS_COUNT = 4;
        readonly MotionNavigationType DEFAULT_NAV_TYPE = MotionNavigationType.GLOBAL_AND_LOCAL;

        const float MAX_BLEND_WEIGHT = 10f;
        const int MAX_AGENTS_COUNT = 50;

        const string BLEND_FACTOR_TEXT = "Avoidance vel. to global path following vel. factor: {0}\n" +
                                         "Local avoidance vel. weight: {1}\nPath following vel. weight: {2}";

        const string AGENTS_MAX_SPEED_TEXT = "Agents max speed factor: {0}\n(Max speed = {1})";
        const string AGENTS_SPEED_TEXT = "Agents speed: {0}";
        const string AGENTS_RADIUS_TEXT = "Agents radius: {0}";
        const string AGENTS_NUMBER_TEXT = "Agents count: {0}";

        #endregion

        #region Serialized fields

        [SerializeField] Button m_RespawnAgentsButton;
        [SerializeField] Button m_ResetSettingsButton;
        [SerializeField] Slider m_AgentsCountSlider;
        [SerializeField] Slider m_BlendFactorSlider;
        [SerializeField] Slider m_AgentMaxSpeedFactorSlider;
        [SerializeField] Slider m_AgentSpeedSlider;
        [SerializeField] Slider m_AgentRadiusSlider;
        [SerializeField] Dropdown m_NavTypeDropdown;

        [SerializeField] Text m_AgentsNumberText;
        [SerializeField] Text m_AgentsMaxSpeedText;
        [SerializeField] Text m_AgentsSpeedText;
        [SerializeField] Text m_AgentsBlendFactorText;
        [SerializeField] Text m_AgentsRadiusText;

        #endregion

        #region Attributes

        float m_CircleRadius = 7;
        Vector3 m_CircleCenter = Vector3.zero;

        int m_CurrAgentsCount = DEFAULT_AGENTS_COUNT;
        float m_CurrAgentBlendFactor;

        Nav3DAgentDescription m_Description;

        Nav3DAgent[] m_AgentsPrefetched = new Nav3DAgent[MAX_AGENTS_COUNT];
        List<Nav3DAgent> m_AgentsOperating = new List<Nav3DAgent>(MAX_AGENTS_COUNT);
        Dictionary<Nav3DAgent, Material> agentToBodyMat = new Dictionary<Nav3DAgent, Material>();

        #endregion

        #region Properties

        //converting agent blend factor to local avoidance weight
        float LocalAvoidanceWeight => (1 - m_CurrAgentBlendFactor) * MAX_BLEND_WEIGHT;

        //converting agent blend factor to path following weight
        float PathFollowingWeight => m_CurrAgentBlendFactor * MAX_BLEND_WEIGHT;

        #endregion

        #region Unity events

        void Start()
        {
            //init description for agents
            InitDefaultDescription();

            //reset UI values
            SetUIByDefault();

            BindUIHandlers();

            PrefetchAgents();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            //visualize each alive agent
            m_AgentsOperating.ForEach(_Agent => _Agent.Visualize());

            //draw agents storage
            Nav3DAgentManager.VisualizeAgentsStorage();
        }
#endif

        #endregion

        #region UI handlers

        void OnResetSettingsButtonClicked()
        {
            InitDefaultDescription();
            SetUIByDefault();
        }

        void OnAgentsRespawnButtonClicked()
        {
            RespawnAgents();
        }

        void OnAgentsCountSliderChanged(float _Value)
        {
            if (m_CurrAgentsCount == (int)_Value || m_CurrAgentsCount == Mathf.CeilToInt(_Value))
                return;

            m_CurrAgentsCount = Mathf.CeilToInt(_Value);

            SetUILabelsText();

            RespawnAgents();
        }

        void OnBlendFactorSliderChanged(float _Value)
        {
            m_BlendFactorSlider.value = m_CurrAgentBlendFactor = _Value;

            m_Description.AgentsAvoidanceVelocityWeight1 = LocalAvoidanceWeight;
            m_Description.PathVelocityWeight2 = PathFollowingWeight;

            SetUILabelsText();
        }

        void OnAgentMaxSpeedFactorSliderChanged(float _Value)
        {
            m_AgentMaxSpeedFactorSlider.value = m_Description.SpeedToMaxSpeedMultiplier = _Value;

            SetUILabelsText();
        }

        void OnAgentSpeedSliderChanged(float _Value)
        {
            //set new speed value to agents description.
            //Since the same description instance is set for all agents in the scene,
            //changing the speed in the description instance will change the speed of all agents
            m_AgentSpeedSlider.value = m_Description.Speed = _Value;

            SetUILabelsText();
        }

        void OnAgentRadiusSliderChanged(float _Value)
        {
            //set new radius value to agents description.
            //Since the same description instance is set for all agents in the scene,
            //changing the radius in the description instance will change the radius of all agents
            m_AgentRadiusSlider.value = m_Description.Radius = _Value;

            //change visual agent size
            OnRadiusChanged();

            SetUILabelsText();
        }

        void OnNavTypeChanged(int _Value)
        {
            m_Description.MotionNavigationType = (MotionNavigationType)_Value;
        }

        #endregion

        #region Service methods

        //create agents default description instance and set all desirable parameters
        void InitDefaultDescription()
        {
            m_Description = Nav3DAgentDescription.DefaultDescription;
            m_Description.MotionNavigationType = DEFAULT_NAV_TYPE;

            m_CurrAgentBlendFactor = DEFAULT_BLEND_FACTOR;
            m_Description.AgentsAvoidanceVelocityWeight = LocalAvoidanceWeight;
            m_Description.PathVelocityWeight = PathFollowingWeight;

            m_Description.SpeedToMaxSpeedMultiplier = DEFAULT_MAX_SPEED_FACTOR;
            m_Description.Speed = DEFAULT_AGENTS_SPEED;
            m_Description.Radius = DEFAULT_AGENTS_RADIUS;

            m_Description.MaxAgentDegreesRotationPerTick = 10;
        }

        void OnRadiusChanged()
        {
            //scale each agent body by radius
            m_AgentsPrefetched.ForEach(_Agent => { DemoHelper.ScaleAgentConeBody(_Agent.gameObject, m_Description.Radius); });
        }

        void PrefetchAgents()
        {
            for (int i = 0; i < MAX_AGENTS_COUNT; i++)
            {
                //instantiate agent GameObject
                GameObject agentGO = DemoHelper.InstantiateAgentConeBody($"agent_{i}", m_Description.Radius, out Material agentMaterial);

                //add Nav3D component
                Nav3DAgent agent = agentGO.AddComponent<Nav3DAgent>();
                agent.SetDescription(m_Description);

                agentToBodyMat.Add(agent, agentMaterial);

                agent.gameObject.SetActive(false);

                m_AgentsPrefetched[i] = agent;
            }
        }

        void RespawnAgents()
        {
            ResetAgents();

            //get agent positions distributed by sphere surface
            Vector3[] agentPositions = UtilsMath.GetSphereSurfacePoints(m_CircleCenter, m_CircleRadius, m_CurrAgentsCount);

            Color[] colors = UtilsCommon.GetNDistanColors(m_CurrAgentsCount);

            for (int i = 0; i < m_CurrAgentsCount; i++)
            {
                Nav3DAgent prefetchedAgent = m_AgentsPrefetched[i];

                TrailRenderer trailRenderer = prefetchedAgent.gameObject.GetComponent<TrailRenderer>();

                //place and activate prefetched agent
                Vector3 agentPosition = agentPositions[i];
                prefetchedAgent.transform.position = agentPosition;
                prefetchedAgent.gameObject.SetActive(true);

                //start agent moving to sphere opposite point
                Vector3 moveToPoint = m_CircleCenter + (m_CircleCenter - agentPosition) + UtilsMath.GetRandomVector(0.25f);
                prefetchedAgent.MoveTo(moveToPoint);

                m_AgentsOperating.Add(prefetchedAgent);

                RecreateTrailRendererComponent(prefetchedAgent, colors[i]);
            }
        }

        void RecreateTrailRendererComponent(Nav3DAgent _Owner, Color _Color)
        {
            if (_Owner.gameObject.TryGetComponent<TrailRenderer>(out TrailRenderer trailRenderer))
            {
                trailRenderer.Clear();
            }
            else
            {
                trailRenderer = _Owner.gameObject.AddComponent<TrailRenderer>();
            }

            Material agentBodyMat = agentToBodyMat[_Owner];
            agentBodyMat.color = _Color;

            trailRenderer.material = agentBodyMat;
            trailRenderer.startWidth = trailRenderer.endWidth = m_Description.Radius * 0.25f;
            trailRenderer.time = 10f;
            trailRenderer.emitting = true;
            trailRenderer.startColor = trailRenderer.endColor = _Color;
        }

        void ResetAgents()
        {
            m_AgentsOperating.ForEach(_Agent => _Agent.gameObject.SetActive(false));
            m_AgentsOperating.Clear();
        }

        void SetUIByDefault()
        {
            m_AgentsCountSlider.value = m_CurrAgentsCount = DEFAULT_AGENTS_COUNT;
            m_BlendFactorSlider.value = m_CurrAgentBlendFactor = DEFAULT_BLEND_FACTOR;
            m_AgentMaxSpeedFactorSlider.value = m_Description.SpeedToMaxSpeedMultiplier = DEFAULT_MAX_SPEED_FACTOR;
            m_AgentSpeedSlider.value = m_Description.Speed = DEFAULT_AGENTS_SPEED;
            m_AgentRadiusSlider.value = m_Description.Radius = DEFAULT_AGENTS_RADIUS;
            m_NavTypeDropdown.value = (int)(m_Description.MotionNavigationType = DEFAULT_NAV_TYPE);

            SetUILabelsText();
        }

        void BindUIHandlers()
        {
            m_RespawnAgentsButton.onClick.AddListener(OnAgentsRespawnButtonClicked);
            m_ResetSettingsButton.onClick.AddListener(OnResetSettingsButtonClicked);

            m_AgentsCountSlider.onValueChanged.AddListener(OnAgentsCountSliderChanged);
            m_BlendFactorSlider.onValueChanged.AddListener(OnBlendFactorSliderChanged);
            m_AgentMaxSpeedFactorSlider.onValueChanged.AddListener(OnAgentMaxSpeedFactorSliderChanged);
            m_AgentSpeedSlider.onValueChanged.AddListener(OnAgentSpeedSliderChanged);
            m_AgentRadiusSlider.onValueChanged.AddListener(OnAgentRadiusSliderChanged);
            m_NavTypeDropdown.onValueChanged.AddListener(OnNavTypeChanged);
        }

        void SetUILabelsText()
        {
            m_AgentsNumberText.text = string.Format(AGENTS_NUMBER_TEXT, m_CurrAgentsCount);
            m_AgentsMaxSpeedText.text = string.Format(AGENTS_MAX_SPEED_TEXT, m_Description.SpeedToMaxSpeedMultiplier, m_Description.MaxSpeed);
            m_AgentsSpeedText.text = string.Format(AGENTS_SPEED_TEXT, m_Description.Speed);
            m_AgentsBlendFactorText.text = string.Format(
                BLEND_FACTOR_TEXT,
                string.Format("{0:0.00}", m_CurrAgentBlendFactor),
                string.Format("{0:0.00}", LocalAvoidanceWeight),
                string.Format("{0:0.00}", PathFollowingWeight)
            );
            m_AgentsRadiusText.text = string.Format(AGENTS_RADIUS_TEXT, m_Description.Radius);
        }

        #endregion
    }
}