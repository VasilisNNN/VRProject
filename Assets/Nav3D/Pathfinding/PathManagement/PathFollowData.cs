using Nav3D.LocalAvoidance.SupportingMath;
using UnityEngine;

namespace Nav3D.Pathfinding
{
    public class PathFollowData
    {
        #region Attributes

        Vector3[] m_Trajectory;
        int m_NextIndex;
        public Vector3 m_NextPoint;

        #endregion

        #region Properties

        public bool IsValid { get; private set; } = true;

        #endregion

        #region Constructors

        public PathFollowData(Vector3[] _Trajectory, Vector3 _FollowerPoint)
        {
            m_Trajectory = _Trajectory;

            GetClosestPoint(_FollowerPoint, out m_NextPoint, out m_NextIndex);
        }

        #endregion

        #region Public methods

        public Vector3 GetMovePoint(float _Speed, float _Radius, Vector3 _CurrentFollowerPosition)
        {
            int nextIndex = m_NextIndex;

            Vector3 next = m_Trajectory[nextIndex];
            Vector3 curToNext = next - _CurrentFollowerPosition;
            float toNextDistance = curToNext.magnitude;

            while (toNextDistance <= _Speed + _Radius && m_NextIndex < m_Trajectory.Length - 1)
            {
                _Speed -= toNextDistance;
                _CurrentFollowerPosition = next;

                m_NextIndex++;
                next = m_Trajectory[m_NextIndex];
                curToNext = next - _CurrentFollowerPosition;
                toNextDistance = curToNext.magnitude;
            }

            return next;
        }

        public void Invalidate()
        {
            IsValid = false;
        }

        public void GetDistToClosestOnPath(Vector3 _Point, out Vector3 _Closest, out float _Magnitude)
        {
            int nextIndex = m_NextIndex;
            int preIndex;

            if (m_NextIndex == m_Trajectory.Length)
                nextIndex--;

            preIndex = nextIndex - 1;

            if (nextIndex == 0)
            {
                _Closest = m_Trajectory[nextIndex];
                _Magnitude = (_Closest - _Point).magnitude;

                return;
            }

            Vector3 next = m_Trajectory[nextIndex];
            Vector3 pre = m_Trajectory[preIndex];

            //The case when the path contains only two equal points
            if (next == pre)
            {
                _Closest = next;
                _Magnitude = 0;

                return;
            }

            Vector3 closestOnLine = new Straight(next - pre, pre).ClosestPoint(_Point);

            float preMagn = (pre - _Point).magnitude;
            float nextMagn = (next - _Point).magnitude;
            float closestMagn = (closestOnLine - _Point).magnitude;

            if (preMagn < nextMagn && preMagn < closestMagn)
            {
                _Closest = pre;
                _Magnitude = preMagn;

                return;
            }

            if (nextMagn < preMagn && nextMagn < closestMagn)
            {
                _Closest = next;
                _Magnitude = nextMagn;

                return;
            }

            _Closest = closestOnLine;
            _Magnitude = closestMagn;
        }

        #endregion

        #region Service methods

        void GetClosestPoint(Vector3 _Point, out Vector3 _ClosestPoint, out int _Index)
        {
            float minSqrMagn = float.MaxValue;

            Vector3 min = Vector3.positiveInfinity;
            int minIndex = int.MaxValue;

            for (int i = 0; i < m_Trajectory.Length; i++)
            {
                Vector3 curPathPoint = m_Trajectory[i];

                float curSqrMagn = (curPathPoint - _Point).sqrMagnitude;

                if (curSqrMagn < minSqrMagn)
                {
                    minIndex = i;
                    min = curPathPoint;
                    minSqrMagn = curSqrMagn;
                }
            }
            _ClosestPoint = min;
            _Index = minIndex;
        }

        #endregion
    }
}
