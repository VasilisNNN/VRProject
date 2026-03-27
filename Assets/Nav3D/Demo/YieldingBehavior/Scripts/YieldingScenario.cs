using Nav3D.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nav3D.Demo
{
    public class YieldingScenario : MonoBehaviour
    {
        #region Constants

        const int AGENTS_COUNT = 100;
        const float SPAWN_DELAY = 0.5f;

        #endregion

        #region Serialized fields

        [Space, Header("Agent descriptions")]
        [SerializeField] Nav3DAgentDescription m_YieldingDescr;
        [SerializeField] Nav3DAgentDescription m_AgentDescription;

        [Space, Header("Scenario transforms")]
        [SerializeField] Transform m_YieldingsRoot;
        [SerializeField] Transform m_PointA;

        [Space, Header("Debug draw")]
        [SerializeField] bool m_VisualizeAgents;
        [SerializeField] bool m_VisualizeSpatialStorage;

        #endregion

        #region Attributes

        List<Nav3DAgent> m_Agents = new List<Nav3DAgent>();

        #endregion

        #region Unity events

        void Start()
        {
            Nav3DManager.OnNav3DInit += () =>
            {
                //init yielding agents
                SetupYieldings();

                //start spawn agents coroutine
                StartCoroutine(SpawnAgents(AGENTS_COUNT, SPAWN_DELAY));
            };
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            if (m_VisualizeAgents)
            {
                foreach (Nav3DAgent agent in m_Agents)
                    agent.Visualize();
            }

            if (m_VisualizeSpatialStorage)
                Nav3DAgentManager.VisualizeAgentsStorage();
        }
#endif

        #endregion

        #region Service methods

        //add Nav3DAgent component and set description to yielding agents 
        void SetupYieldings()
        {
            foreach (Transform yielding in m_YieldingsRoot)
            {
                Nav3DAgent agentYielding = yielding.gameObject.AddComponent<Nav3DAgent>();
                agentYielding.SetDescription(m_YieldingDescr);
            }
        }

        IEnumerator SpawnAgents(int _Count, float _Delay)
        {
            GameObject agentsRoot = new GameObject("AgentsRoot");

            for (int i = 0; i < _Count; i++)
            {
                //get agent description variant
                Nav3DAgentDescription descrVariant = m_AgentDescription.GetDescriptionVariant();

                //create agent's GameObject
                GameObject agentGO = DemoHelper.InstantiateAgentConeBody($"agent_{i}", descrVariant.Radius, out _);

                agentGO.transform.SetParent(agentsRoot.transform);

                Nav3DAgent agent = agentGO.AddComponent<Nav3DAgent>();

                //attach controller inherited from Nav3DAgent
                FoolfishController foolfishController = agentGO.AddComponent<FoolfishController>();

                m_Agents.Add(agent);

                //apply description variant
                agent.SetDescription(descrVariant);

                //set controller's parameters and start following to random point
                foolfishController.transform.position = m_PointA.position;
                foolfishController.FollowPointSpawnRadius = 10;
                foolfishController.Origin = m_PointA.transform.position;
                foolfishController.FollowRandomPoint();

                yield return new WaitForSeconds(_Delay);
            }
        }

        #endregion
    }
}