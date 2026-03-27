using Nav3D.API;
using Nav3D.Common;
using UnityEngine;

namespace Nav3D.Demo
{
    [RequireComponent(typeof(Nav3DAgent))]
    public class FoolfishController : MonoBehaviour
    {
        #region Attributes

        Nav3DAgent m_Agent;

        #endregion

        #region Properties

        public Vector3 InitialPos { get; set; }
        public Vector3 Origin { get; set; } = Vector3.zero;
        public float FollowPointSpawnRadius { get; set; }

        #endregion

        #region Public methods

        public void FollowRandomPoint() => m_Agent.MoveTo(
            Origin + UtilsMath.RandomNormal * FollowPointSpawnRadius,
            FollowRandomPoint,
            _ =>
            {
                transform.position = InitialPos;
                FollowRandomPoint();
            }
        );

        #endregion

        #region Unity events

        void Awake()
        {
            m_Agent = GetComponent<Nav3DAgent>();
        }

        #endregion
    }
}