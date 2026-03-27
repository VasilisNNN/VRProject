using Nav3D.Common;
using Nav3D.LocalAvoidance.SupportingMath;

namespace Nav3D.LocalAvoidance
{
    public class VOObstacle : IVO
    {
        #region Attributes

        Triangle m_Triangle;

        #endregion

        public VOObstacle(IMovable _SolverAgent, Triangle _Triangle, float _Tau = 2f)
        {
            m_Triangle = _Triangle;
        }

        #region Public methods

        public Plane GetORCA()
        {
            return new Plane(m_Triangle.Normal, m_Triangle.V1);
        }

        #endregion
    }
}
