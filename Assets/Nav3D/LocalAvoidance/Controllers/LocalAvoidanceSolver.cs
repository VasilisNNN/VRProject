using Nav3D.API;
using Nav3D.LocalAvoidance;
using UnityEngine;

namespace Nav3D.Common
{
    public abstract class LocalAvoidanceSolver
    {
        #region Constructors

        public LocalAvoidanceSolver(Nav3DAgentMover _Mover, Nav3DAgentDescription _Description)
        {
            m_Mover = _Mover;
            m_Description = _Description;
        }

        #endregion

        #region Attributes

        protected Nav3DAgentMover m_Mover;
        protected Nav3DAgentDescription m_Description;

        #endregion

        #region Properties

        protected Vector3 VPreOpt { get; set; } = Vector3.zero;

        #endregion

        #region Public methods

        public void SetFollowPoint(Vector3 _Point)
        {
            //Need to set V opt for first frame, then the results of the previous ones will be used on the next and so on.
            if (VPreOpt == Vector3.zero)
                VPreOpt = (_Point - m_Mover.GetPosition()).normalized * m_Description.Speed;
        }

        public void SetDescription(Nav3DAgentDescription _Description)
        {
            m_Description = _Description;
        }

        public Vector3 GetVelocity(
            Vector3 _LastPosition,
            Vector3 _VPref,
            out float _ProximityScore)
        {
            Vector3 newVelocity = ResolveVelocity(
                _LastPosition,
                VPreOpt,
                _VPref,
                out _ProximityScore
            );

            VPreOpt = newVelocity;

            return newVelocity;
        }

        public virtual void UpdateRivalVO(Nav3DAgentMover _RivalMover, VOAgent _RivalVO)
        {
        }

#if UNITY_EDITOR
        public abstract void VisualizeNearestMovers();
#endif

        #endregion

        #region Service methods

        protected abstract Vector3 ResolveVelocity(
            Vector3 _LastPosition,
            Vector3 _PreVelocity,
            Vector3 _VPref,
            out float _ProximityScore);

        #endregion
    }
}