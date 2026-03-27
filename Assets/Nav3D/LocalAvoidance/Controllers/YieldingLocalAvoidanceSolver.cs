using Nav3D.API;
using Nav3D.LocalAvoidance;
using Nav3D.Obstacles;
using Nav3D.LocalAvoidance.SupportingMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Plane = Nav3D.LocalAvoidance.SupportingMath.Plane;

namespace Nav3D.Common
{
    class YieldingLocalAvoidanceSolver : LocalAvoidanceSolver
    {
        #region Constants

        const float YIELDING_SPEED_DECAY_FACTOR = 0.25f;

        #endregion

        #region Attributes

        HashSet<Nav3DAgentMover> m_NearestMovers = new HashSet<Nav3DAgentMover>();
        List<Triangle> m_NearestTriangles = new List<Triangle>();

        #endregion

        #region Constructors

        public YieldingLocalAvoidanceSolver(Nav3DAgentMover _Mover, Nav3DAgentDescription _Description) : base(_Mover, _Description)
        {
        }

        #endregion

        #region Public methods

#if UNITY_EDITOR

        public override void VisualizeNearestMovers()
        {
            Gizmos.color = Color.red;

            foreach (Nav3DAgentMover mover in m_NearestMovers)
            {
                Gizmos.DrawLine(m_Mover.GetPosition(), mover.GetPosition());
                Gizmos.DrawWireSphere(mover.GetPosition(), mover.GetRadius());
            }
        }
#endif

        #endregion

        #region Service methods

        protected override Vector3 ResolveVelocity(Vector3 _LastPosition, Vector3 _PreVelocity, Vector3 _VPref, out float _ProximityScore)
        {
            Vector3 avoidingSumVector = Vector3.zero;
            int collisionsCount = 0;

            //update neighbor movers list if necessary
            if (m_Mover.IsNeighborMoversDirty)
            {
                m_NearestMovers = AgentManager.Instance.GetAgentBucketAgents(m_Mover);

                //reset flag
                m_Mover.SetNeighborMovablesDirty(false);
            }

            //update neighbor obstacles list if necessary
            if (m_Mover.IsNeighborObstaclesDirty)
            {
                if (AgentManager.Instance.HasNearestObstacles(m_Mover, out Bounds dangerBounds))
                    m_NearestTriangles = ObstacleManager.Instance.GetIntersectedObstaclesTriangles(dangerBounds);

                //reset flag
                m_Mover.SetNeighborObstaclesDirty(false);
            }

            List<IVO> nearestVOs = new List<IVO>();

            foreach (Nav3DAgentMover otherMover in m_NearestMovers)
            {
                //skip self mover
                if (otherMover == m_Mover)
                    continue;

                if (otherMover.BehaviorType == BehaviorType.YIELDING && !m_Mover.IsCollidingWithOther(otherMover))
                    continue;

                Vector3 inVector = m_Mover.LastPosition - otherMover.LastPosition;
                float dist = inVector.magnitude;
                float radiusSum = m_Description.Radius + otherMover.GetRadius();

                if (dist < radiusSum + (m_Description.MaxSpeed + otherMover.GetMaxSpeed()) * m_Description.ORCATau)
                {
                    avoidingSumVector += inVector * (radiusSum / dist);
                    collisionsCount++;

                    VOAgent vOAgent = new VOAgent(m_Mover, otherMover, m_Description.ORCATau);
                    vOAgent.ComputeVO();
                    nearestVOs.Add(vOAgent);
                }
            }

            foreach (Triangle triangle in m_NearestTriangles)
            {
                float distance = new UnityEngine.Plane(triangle.V1, triangle.V2, triangle.V3).GetDistanceToPoint(_LastPosition);


                if (Mathf.Abs(distance) < m_Description.VelocityRadius * m_Description.ORCATau && triangle.InsideOfSolidHunk(m_Mover.LastPosition))
                {
                    nearestVOs.Add(new VOObstacle(m_Mover, triangle, m_Description.ORCATau));
                }
            }

            Vector3 newVelocity = Vector3.zero;

            if (nearestVOs.Any())
            {
                List<Plane> ORCAs = new List<Plane>(nearestVOs.Count);

                foreach (VOAgent vo in nearestVOs)
                    ORCAs.Add(vo.GetORCA());

                newVelocity = LPSolver.Instance.SolveMax(
                    ORCAs,
                    new Sphere(Vector3.zero, m_Description.MaxSpeed),
                    collisionsCount == 0 ? Vector3.zero : (avoidingSumVector / collisionsCount).normalized * m_Description.Speed * (m_Description.Speed / m_Description.Radius));
            }
            else
            {
                Vector3 lastVelocity = m_Mover.GetLastFrameVelocity();

                if (lastVelocity != Vector3.zero)
                    newVelocity = Vector3.Lerp(lastVelocity, Vector3.zero, YIELDING_SPEED_DECAY_FACTOR);
            }

            _ProximityScore = 0;

            return newVelocity;
        }

        #endregion
    }
}
