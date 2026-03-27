using Nav3D.API;
using Nav3D.LocalAvoidance;
using Nav3D.LocalAvoidance.SupportingMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Plane = Nav3D.LocalAvoidance.SupportingMath.Plane;

namespace Nav3D.Common
{
    public class MoverLocalAvoidanceSolver : LocalAvoidanceSolver
    {
        #region Nested types

        class RivalsStorage
        {
            #region Attributes

            Dictionary<Nav3DAgentMover, VOAgent> m_Rivals = new Dictionary<Nav3DAgentMover, VOAgent>();

            #endregion

            #region Properties

            bool IsEmpty { get; set; }

            #endregion

            #region Construction

            public RivalsStorage() { }

            #endregion

            #region Public methods

            public void Add(Nav3DAgentMover _RivalAgent, VOAgent _RivalVO)
            {
                if (m_Rivals.ContainsKey(_RivalAgent))
                    m_Rivals[_RivalAgent] = _RivalVO;
                else
                    m_Rivals.Add(_RivalAgent, _RivalVO);

                IsEmpty = false;
            }

            public void Clear()
            {
                if (IsEmpty)
                    return;

                m_Rivals.Clear();
                IsEmpty = true;
            }

            public bool TryGetValidVO(Nav3DAgentMover _Agent, out VOAgent _VO)
            {
                m_Rivals.TryGetValue(_Agent, out VOAgent vo);
                _VO = vo;

                return _VO != null;
            }

            #endregion
        }

        #endregion

        #region Constructors

        public MoverLocalAvoidanceSolver(Nav3DAgentMover _Mover, Nav3DAgentDescription _Description) : base(_Mover, _Description)
        {
        }

        #endregion

        #region Attributes

        RivalsStorage m_Rivals = new RivalsStorage();

        bool m_HasNeighbors;
        List<Nav3DAgentMover> m_NearestMovers = new List<Nav3DAgentMover>();

        #endregion

        #region Public methods

        public override void UpdateRivalVO(Nav3DAgentMover _RivalMover, VOAgent _RivalVO)
        {
            m_Rivals.Add(_RivalMover, _RivalVO);
        }

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

        void FillNearestMoversList(HashSet<Nav3DAgentMover> _NearestMovers)
        {
            m_NearestMovers = new List<Nav3DAgentMover>(_NearestMovers.Count - 1);

            float minSqrDist = float.MaxValue;
            Vector3 moverPosition = m_Mover.GetPosition();

            //add all movers to list, sorting by distance to this mover
            foreach (Nav3DAgentMover mover in _NearestMovers)
            {
                //skip self mover
                if (mover == m_Mover)
                    continue;

                float sqrDistToMover = (mover.GetPosition() - moverPosition).sqrMagnitude;

                if (sqrDistToMover < minSqrDist)
                    m_NearestMovers.Insert(0, mover);
                else
                    m_NearestMovers.Add(mover);
            }
        }

        protected override Vector3 ResolveVelocity(Vector3 _LastPosition, Vector3 _PreVelocity, Vector3 _VPref, out float _ProximityScore)
        {
            Vector3 velocity = _VPref;
            float proximityScore = 0;

            if (m_Mover.AvoidanceInterrupt)
            {
                m_Mover.SetAvoidanceInterrupt(false);
                _ProximityScore = 0;
                        
                return Vector3.zero;
            }
            
            //update neighbor movers list if necessary
            if (m_Mover.IsNeighborMoversDirty)
            {
                m_HasNeighbors = AgentManager.Instance.HasNeighbors(m_Mover);
                FillNearestMoversList(AgentManager.Instance.GetAgentBucketAgents(m_Mover));

                //reset flag
                m_Mover.SetNeighborMovablesDirty(false);
            }

            if (!m_HasNeighbors)
            {
                VPreOpt = velocity;
                _ProximityScore = proximityScore;

                return velocity;
            }

            List<IVO> nearestMoverVOs = new List<IVO>();

            //add error to preferred velocity to avoid stagnant forehead-duel condition
            if (m_NearestMovers.Count == 1)
            {
                Nav3DAgentMover mover = m_NearestMovers.First(_Mover => _Mover != m_Mover);

                if (Vector3.SqrMagnitude(mover.GetPosition() - m_Mover.GetPosition()) < AgentManager.Instance.DangerDistanceSqr)
                {
                    float dotProduct = Vector3.Dot(
                        _PreVelocity.normalized,
                        m_NearestMovers.First().GetLastFrameVelocity().normalized
                    );

                    //agents on a head-on collision course
                    if (1 - Mathf.Abs(dotProduct) < 0.2f)
                    {
                        //small error addition to avoid stagnant behavior
                        _PreVelocity += UtilsMath.GetRandomVector(m_Description.Speed);
                    }
                }
            }

            int consideredNumber = 0;
            int consideredMoversNumberLimit = m_Description.UseConsideredAgentsNumberLimit ? m_Description.ConsideredAgentsNumberLimit : m_NearestMovers.Count;

            foreach (Nav3DAgentMover mover in m_NearestMovers)
            {
                if (mover.GetLastFrameVelocity() == Vector3.zero)
                    continue;

                Vector3 otherMoverPos = mover.GetPosition();
                Vector3 moverPos = m_Mover.GetPosition();

                Vector3 deltaPos = otherMoverPos - moverPos;
                float sqrProximity = Vector3.SqrMagnitude(deltaPos);
                float dangerDistanceSqr = UtilsMath.Sqr(mover.GetDangerRadiusOneFrame() + m_Mover.GetDangerRadiusOneFrame());

                //skip if proximity greater than next frame colliding distance
                if (sqrProximity > dangerDistanceSqr)
                    continue;

                float radiiSqrSum = UtilsMath.Sqr(m_Mover.GetRadius() + mover.GetRadius());

                //5 degrees angle threshold (in rads.)
                const float VELOCITY_VECTORS_CO_DIRECTIONALITY_THRESHOLD = 0.349066f;
                Vector3 thisMoverLastVelocity = m_Mover.GetLastNonZeroVelocity();
                Vector3 otherMoverLastVelocity = mover.GetLastNonZeroVelocity();
                
                float velocitiesAngle = Mathf.Acos(Vector3.Dot(thisMoverLastVelocity.normalized, otherMoverLastVelocity.normalized));

                //agents intersects
                if (sqrProximity < radiiSqrSum)
                {
                    if (velocitiesAngle < VELOCITY_VECTORS_CO_DIRECTIONALITY_THRESHOLD &&
                        new Plane(thisMoverLastVelocity, moverPos).GetSide(otherMoverPos))
                    {
                        m_Mover.SetAvoidanceInterrupt(true);
                        _ProximityScore = 0;

                        return Vector3.zero;
                    }
                }
                //agents does not intersects
                else
                {
                    //skip if movement vectors are co-directional
                    if (velocitiesAngle < VELOCITY_VECTORS_CO_DIRECTIONALITY_THRESHOLD)
                        continue;

                    //skip if velocity trajectories will not lead to collision in the next frame
                    if (!UtilsMath.VelocityTrailIntersects(
                            otherMoverPos, mover.GetLastFrameVelocityProjected(), mover.GetRadius(),
                            moverPos, m_Mover.GetLastFrameVelocityProjected(), m_Mover.GetRadius())
                       )
                        continue;
                }

                float radiSum = m_Description.Radius + mover.GetRadius();
                float maxSpeedSum = m_Description.MaxSpeed + mover.GetMaxSpeed();

                float dangerSqrProximity = UtilsMath.Sqr(radiSum + maxSpeedSum * m_Description.ORCATau);
                float contactProximity = UtilsMath.Sqr(radiSum + maxSpeedSum);

                proximityScore += m_Description.AgentsAvoidanceVelocityWeight * (dangerSqrProximity - sqrProximity) / (dangerSqrProximity - contactProximity);

                // ReSharper disable once InconsistentNaming
                if (!m_Rivals.TryGetValidVO(mover, out VOAgent agentVO))
                {
                    agentVO = CreateAgentVO(m_Mover, mover, m_Description.ORCATau);
                    agentVO.ComputeVO();

                    mover.UpdateRivalVO(m_Mover, agentVO.Flipped);
                }

                nearestMoverVOs.Add(agentVO);

                consideredNumber++;

                if (consideredNumber == consideredMoversNumberLimit)
                    break;
            }

            if (nearestMoverVOs.Any())
            {
                List<Plane> moversPlanes = new List<Plane>(nearestMoverVOs.Count);

                moversPlanes.AddRange(nearestMoverVOs.Select(_VO => _VO.GetORCA()));

                velocity = SolveLP(moversPlanes, new Sphere(Vector3.zero, m_Description.MaxSpeed), _PreVelocity);

                if (velocity.sqrMagnitude > m_Description.SqrMaxSpeed)
                    velocity = Vector3.ClampMagnitude(velocity, m_Description.MaxSpeed);
            }

            m_Rivals.Clear();

            VPreOpt = velocity;
            _ProximityScore = proximityScore;

            return velocity;
        }

        protected virtual Vector3 SolveLP(List<Plane> _ConstraintPlanes, Sphere _VelocitySphere, Vector3 _PreVelocity)
        {
            return LPSolver.Instance.SolveMax(_ConstraintPlanes, _VelocitySphere, _PreVelocity);
        }

        protected virtual VOAgent CreateAgentVO(IMovable _SolverAgent, IMovable _OtherAgent, float _Tau)
        {
            return new VOAgent(_SolverAgent, _OtherAgent, _Tau);
        }

        #endregion
    }
}
