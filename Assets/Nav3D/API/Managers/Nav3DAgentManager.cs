using Nav3D.LocalAvoidance;
using System.Collections.Generic;
using UnityEngine;

namespace Nav3D.API
{
    static class Nav3DAgentManager
    {
        #region Public methods

        /// <summary>
        /// Draws the cells of the agent's perception space.
        /// Need to call inside MonoBehaviour.OnDrawGizmos().
        /// </summary>
        public static void VisualizeAgentsStorage()
        {
            if (!Nav3DManager.Inited)
                throw new Nav3DManager.Nav3DManagerNotInitializedException();

#if UNITY_EDITOR
            using (Common.Debug.UtilsGizmos.ColorPermanence)
            {
                AgentManager.Instance.VisualizeStorage();
            }
#endif
        }

        /// <summary>
        /// Get agents inside the bounds.
        /// </summary>
        public static List<Nav3DAgent> GetAgentsInBounds(Bounds _Bounds)
        {
            if (!Nav3DManager.Inited)
                throw new Nav3DManager.Nav3DManagerNotInitializedException();

            return AgentManager.Instance.GetAgentsInBounds(_Bounds);
        }

        /// <summary>
        /// Get agents inside the sphere.
        /// </summary>
        public static List<Nav3DAgent> GetAgentsInSphere(Vector3 _Center, float _Radius)
        {
            if (!Nav3DManager.Inited)
                throw new Nav3DManager.Nav3DManagerNotInitializedException();

            return AgentManager.Instance.GetAgentsInSphere(_Center, _Radius);
        }

        #endregion
    }
}
