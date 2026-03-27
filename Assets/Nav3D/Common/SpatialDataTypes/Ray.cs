using UnityEngine;

namespace Nav3D.Common
{
    /// <summary>
    /// Represents a ray of limited length, in fact a segment in space with a direction.
    /// </summary>
    public struct Ray
    {
        #region Properties

        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }
        public Vector3 Origin { get; private set; }
        public Vector3 DirectionNormal { get; private set; }
        public Vector3 DirectionMagn { get; private set; }

        #endregion

        #region Constructors

        public Ray(Vector3 _Start, Vector3 _End)
        {
            Start = _Start;
            End = _End;
            Origin = _Start;
            DirectionMagn = _End - _Start;
            DirectionNormal = DirectionMagn.normalized;
        }

        #endregion

        #region Public methods

        public UnityEngine.Ray GetUnityEngineRay()
        {
            return new UnityEngine.Ray(Origin, DirectionMagn);
        }

        #endregion
    }
}
