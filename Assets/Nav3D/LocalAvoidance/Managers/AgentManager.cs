using Nav3D.API;
using Nav3D.Common;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System;
using UnityEngine;

namespace Nav3D.LocalAvoidance
{
    public class AgentManager : MonoBehaviour
    {
        #region Nested types

        enum AgentOperationType
        {
            ADD,
            REMOVE
        }

        #endregion

        #region Attributes

        float m_DangerDistance = float.MinValue;
        float m_DangerDistanceSqr = float.MinValue;

        HashSet<Nav3DAgent> m_Agents = new HashSet<Nav3DAgent>();
        SpatialHashMap<Nav3DAgentMover> m_AgentsStorage;

        ConcurrentQueue<(Nav3DAgent, AgentOperationType)> m_AgentsOperations = new ConcurrentQueue<(Nav3DAgent, AgentOperationType)>();

        #endregion

        #region Properties

        public static AgentManager Instance => Singleton<AgentManager>.Instance;
        public static bool Doomed { get; private set; } = false;

        public float DangerDistance
        {
            get => m_DangerDistance;
            set
            {
                if (m_DangerDistance == value)
                    return;

                m_DangerDistance = value;
                m_DangerDistanceSqr = m_DangerDistance * m_DangerDistance;

                m_AgentsStorage.SetBucketSize(m_DangerDistance * 2);
            }
        }

        public float DangerDistanceSqr => m_DangerDistanceSqr;

        public static DateTime Now { get; private set; }

        public float StorageBucketSize => m_AgentsStorage.BucketSize;

        #endregion

        #region Public methods

        public void Initialize(List<Nav3DAgentMover> _Agents)
        {
            m_AgentsStorage = new SpatialHashMap<Nav3DAgentMover>(m_DangerDistance * 2, _Agents);
        }

        public void Uninitialize(bool _NeedDestroy = true)
        {
            if (!_NeedDestroy)
                return;

            UtilsCommon.SmartDestroy(this);
        }

        public void RegisterAgent(Nav3DAgent _Agent)
        {
            m_AgentsOperations.Enqueue((_Agent, AgentOperationType.ADD));
        }

        public void UnregisterAgent(Nav3DAgent _Agent)
        {
            m_AgentsOperations.Enqueue((_Agent, AgentOperationType.REMOVE));
        }

        public void RegisterAgentMover(Nav3DAgentMover _Agent, float _DangerDistance)
        {
            if (!Nav3DManager.Inited)
                Initialize(new List<Nav3DAgentMover> { _Agent });
            else
                m_AgentsStorage.Insert(_Agent);

            UpdateCellSize(_DangerDistance);

            _Agent.OnVelocityDangerDistanceChanged += UpdateCellSize;
        }

        public void UnregisterAgentMover(Nav3DAgentMover _Agent)
        {
            m_AgentsStorage?.Remove(_Agent);

            _Agent.OnVelocityDangerDistanceChanged -= UpdateCellSize;
        }

        /// <summary>
        /// Determines agent bucket, returns all agents from bucket.
        /// </summary>
        /// <param name="_Agent">Agent.</param>
        /// <returns>Agents in bucket.</returns>
        public HashSet<Nav3DAgentMover> GetAgentBucketAgents(Nav3DAgentMover _Agent)
        {
            return m_AgentsStorage.GetElementBucketElements(_Agent);
        }

        public List<Nav3DAgent> GetAgentsInBounds(Bounds _Bounds)
        {
            return m_AgentsStorage.GetMovablesInBounds(_Bounds).Select(_Mover => _Mover.Agent).ToList();
        }

        public List<Nav3DAgent> GetAgentsInSphere(Vector3 _Center, float _Radius)
        {
            float diameter = _Radius * 2;
            float sqrRadius = _Radius * _Radius;

            List<Nav3DAgentMover> result = m_AgentsStorage.GetMovablesInBounds(new Bounds(_Center, new Vector3(diameter, diameter, diameter)));
            result.RemoveAll(_Mover => (_Mover.LastPosition - _Center).sqrMagnitude > sqrRadius);

            return result.Select(_Mover => _Mover.Agent).ToList();
        }

        public bool HasNeighbors(Nav3DAgentMover _Agent)
        {
            return m_AgentsStorage.HasNeighbors(_Agent);
        }

        public bool HasNearestObstacles(Nav3DAgentMover _Agent, out Bounds _DangerBounds)
        {
            return m_AgentsStorage.HasNearestObstacles(_Agent, out _DangerBounds);
        }

        public void SetMovablesInBoundsObstacleDirty(Bounds _Bounds)
        {
            m_AgentsStorage.SetMovablesInBoundsObstacleDirty(_Bounds);
        }

#if UNITY_EDITOR
        public void VisualizeStorage()
        {
            using (Common.Debug.UtilsGizmos.ColorPermanence)
            {
                Gizmos.color = Color.green;

                m_AgentsStorage.Visualize();
                m_Agents.ForEach(_Agent => _Agent.VisualizeNearestObstacleTriangles());
            }
        }
#endif

        #endregion

        #region Service methods

        void UpdateCellSize(float _Value)
        {
            if (DangerDistance < _Value)
                DangerDistance = _Value;
        }

        #endregion

        #region Unity events
        private void Awake()
        {
            Doomed = false;
        }

        void OnDestroy()
        {
            Doomed = true;

            Uninitialize(false);
        }

        private void Update()
        {
            Now = DateTime.Now;
        }

        void FixedUpdate()
        {
            while (m_AgentsOperations.Any())
            {
                if (!m_AgentsOperations.TryDequeue(out (Nav3DAgent agent, AgentOperationType operation) agentOperation))
                    continue;

                if (agentOperation.operation == AgentOperationType.ADD)
                {
                    m_Agents.Add(agentOperation.agent);

                    continue;
                }

                m_Agents.Remove(agentOperation.agent);
            }

            foreach (Nav3DAgent agent in m_Agents)
            {
                agent.DoFixedUpdate();
            }
        }

        #endregion
    }
}