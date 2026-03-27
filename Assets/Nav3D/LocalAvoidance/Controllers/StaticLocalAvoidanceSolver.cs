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
    public class StaticLocalAvoidanceSolver : LocalAvoidanceSolver
    {
        #region Constructors

        public StaticLocalAvoidanceSolver(Nav3DAgentMover _Mover, Nav3DAgentDescription _Description) : base(_Mover, _Description)
        {
            m_LastUpdatePosition = m_Mover.LastPosition;
        }

        #endregion

        #region Attributes

        bool m_HasNeighborObstacles = false;
        List<Triangle> m_NearestTriangles = new List<Triangle>();
        List<Triangle> m_ConsideredTriangles = new List<Triangle>();
        Bounds m_ObstacleDetectionBounds;

        Vector3 m_LastUpdatePosition;

        #endregion

        #region Properties

        public List<Triangle> NearestTriangles => m_NearestTriangles;
        public Bounds ObstacleDetectionBounds => m_ObstacleDetectionBounds;

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        public void Visualize()
        {
            AgentManager.Instance.HasNearestObstacles(m_Mover, out Bounds dangerBounds);

            Gizmos.color = Color.yellow;
            dangerBounds.Draw();

            Gizmos.color = Color.magenta;
            foreach (Triangle triangle in m_ConsideredTriangles)
            {
                triangle.Visualize(true);
            }
        }

        public override void VisualizeNearestMovers()
        {
        }

#endif

        #endregion

        #region Service methods

        void UpdateConsideredTriangles()
        {
            Vector3 moverPosition = m_Mover.GetPosition();
            float dangerDistance = m_Description.VelocityRadiusTauProjection;

            m_ConsideredTriangles.Clear();

            foreach (Triangle triangle in m_NearestTriangles)
            {
                float distance = triangle.Plane.GetDistanceToPoint(moverPosition);

                if (distance <= 0 || !triangle.InsideOfSolidHunk(moverPosition) || distance > dangerDistance)
                    continue;

                m_ConsideredTriangles.Add(triangle);
            }

            m_LastUpdatePosition = moverPosition;
        }

        protected override Vector3 ResolveVelocity(Vector3 _LastPosition, Vector3 _PreVelocity, Vector3 _VPref, out float _ProximityScore)
        {
            Vector3 velocity = _VPref;
            float proximityScore = 0;

            if ((m_LastUpdatePosition - m_Mover.GetPosition()).sqrMagnitude > m_Mover.UpdateStaticObstaclesSqrDistanceThreshold)
            {
                //it's time to update considered obstacles list
                UpdateConsideredTriangles();
            }

            //update neighbor obstacles list if necessary
            if (m_Mover.IsNeighborObstaclesDirty)
            {
                m_HasNeighborObstacles = AgentManager.Instance.HasNearestObstacles(m_Mover, out m_ObstacleDetectionBounds);
                m_NearestTriangles = ObstacleManager.Instance.GetIntersectedObstaclesTriangles(m_ObstacleDetectionBounds);

                UpdateConsideredTriangles();

                //reset flag
                m_Mover.SetNeighborObstaclesDirty(false);
            }

            if (!m_HasNeighborObstacles)
            {
                VPreOpt = velocity;
                _ProximityScore = proximityScore;

                return velocity;
            }

            List<IVO> nearestStaticVOs = new List<IVO>();

            int consideredNumber = 0;

            float dangerDistance = m_Description.VelocityRadiusTauProjection;

            foreach (Triangle triangle in m_ConsideredTriangles)
            {
                float distance = triangle.Plane.GetDistanceToPoint(m_Mover.GetPosition());

                proximityScore += m_Description.ObstaclesAvoidanceVelocityWeight * (dangerDistance - distance) / (dangerDistance - m_Description.VelocityRadius);

                nearestStaticVOs.Add(new VOObstacle(m_Mover, triangle, m_Description.ORCATau));

                consideredNumber++;
            }

            if (consideredNumber > 0)
            {
                List<Plane> staticsPlanes = new List<Plane>(nearestStaticVOs.Count);

                staticsPlanes.AddRange(nearestStaticVOs.Select(_VO => _VO.GetORCA()));

                velocity = LPSolver.Instance.SolveMax(staticsPlanes, new Sphere(Vector3.zero, m_Description.MaxSpeed), _PreVelocity);

                if (velocity.sqrMagnitude > m_Description.SqrMaxSpeed)
                    velocity = Vector3.ClampMagnitude(velocity, m_Description.MaxSpeed);
            }

            VPreOpt = velocity;
            _ProximityScore = proximityScore;

            return velocity;
        }

        #endregion
    }
}
