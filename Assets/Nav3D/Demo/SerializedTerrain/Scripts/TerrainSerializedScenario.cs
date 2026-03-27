using UnityEngine;
using Nav3D.API;
using System;

namespace Nav3D.Demo
{
    public class TerrainSerializedScenario : MonoBehaviour
    {
        #region Constants

        const int AGENTS_COUNT = 50;

        #endregion

        #region Serialized fields

        [SerializeField] Nav3DObstacleLoader m_ObstaclesLoader;
        [SerializeField] Nav3DAgentDescription m_Description;
        [SerializeField] bool m_DrawAgentsStorage;

        #endregion

        #region Unity events

        private void Start()
        {
            m_ObstaclesLoader.OnLoadingFinished += () =>
            {
                Debug.Log(m_ObstaclesLoader.DeserializingProgress.GetResultStats());

                //set pathfinding tasks count limitation
                Nav3DPathfindingManager.MaxPathfindingTasks = (int)(Environment.ProcessorCount * 0.6f);

                SpawnAgents();
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            if (m_DrawAgentsStorage && Nav3DManager.Inited)
                Nav3DAgentManager.VisualizeAgentsStorage();
        }
#endif

        #endregion

        #region Service methods

        void SpawnAgents()
        {
            //create root object for miners on scene
            GameObject agentsRoot = new GameObject("AgentsRoot");

            for (int i = 0; i < AGENTS_COUNT; i++)
            {
                //yield return new WaitForSeconds(0.125f);
                //obtain miner description variant
                Nav3DAgentDescription descrVariant = m_Description.GetDescriptionVariant();
                //instantiate miner prefab
                GameObject agentGO = DemoHelper.InstantiateAgentConeBody($"agent_{i}", descrVariant.Radius, out _, _BodyColor: Color.green);
                float x = 500 * (i + 1) / AGENTS_COUNT;
                agentGO.transform.position = new Vector3(500 * (i + 1) / AGENTS_COUNT, 25, 0);
                agentGO.transform.SetParent(agentsRoot.transform);
                //add controller to miner prefab
                Nav3DAgent agent = agentGO.AddComponent<Nav3DAgent>();
                //attach miner description variant
                agent.SetDescription(descrVariant);
                //init controller
                agent.MoveTo(new Vector3(500 - x, 25, 500), _OnPathfindingFail: _Error => Debug.LogError(_Error.Msg));
            }
        }

        #endregion
    }
}