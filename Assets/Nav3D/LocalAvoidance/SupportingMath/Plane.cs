using Nav3D.Common;
using System;
using UnityEngine;

namespace Nav3D.LocalAvoidance.SupportingMath
{
    /// <summary>
    /// Represents plane in space.
    /// A(x-x0) + B(y-y0) + C(z-z0) = 0
    /// or
    /// Ax + By + Cz + D = 0
    /// </summary>
    public class Plane : SpatialShape
    {
        #region Attributes

        float m_A;
        float m_B;
        float m_C;
        float m_D;
        Vector3 m_Normal;

        Vector3 m_BelongingPoint;

        #endregion

        #region Properties

        public float A => m_A;
        public float B => m_B;
        public float C => m_C;
        public float D => m_D;

        public Vector3 Normal => m_Normal;

        public Vector3 BelongingPoint => m_BelongingPoint;

        #endregion

        #region Construction

        public Plane(Vector3 _Normal, Vector3 _Point)
        {
            m_Normal = _Normal.normalized;
            m_D = -Vector3.Dot(m_Normal, _Point);

            m_A = _Normal.x;
            m_B = _Normal.y;
            m_C = _Normal.z;

            m_BelongingPoint = _Point;
        }

        #endregion

        #region Public methods

        public float Distance(Vector3 _Point)
        {
            return Vector3.Dot(m_Normal, _Point) + m_D;
        }

        public void Translate(Vector3 _Translation)
        {
            m_BelongingPoint += _Translation;
            m_D = -Vector3.Dot(m_Normal, m_BelongingPoint);
        }

        public (Vector3 Point, float Distance) GetSurfaceClosestPoint(Vector3 _Point)
        {
            float distance = Distance(_Point);
            return (_Point - m_Normal * distance, distance);
        }

        public bool GetSide(Vector3 _Point)
        {
            return (double)Vector3.Dot(m_Normal, _Point) + (double)m_D > 0.0;
        }

        public bool IsBelongsHalfPlane(Vector3 _Point)
        {
            return (double)Vector3.Dot(m_Normal, _Point) + (double)m_D > -(0.0 + UtilsMath.PLANE_BOUNDARY_THRESHOLD);
        }

        public bool IsPointBelongs(Vector3 _Point)
        {
            return Mathf.Approximately(Distance(_Point), 0);
        }

        public Vector3 ProjPoint;

        public Straight Intersection(Plane _Other)
        {
            Vector3 crossProduct = Vector3.Cross(Normal, _Other.Normal);
            float dotProduct = Vector3.Dot(Normal, _Other.Normal);
            float alpha = Mathf.Acos(Mathf.Abs(dotProduct));

            Vector3 b = BelongingPoint;
            Vector3 bProj = _Other.GetClosestPoint(b);
            Vector3 bProjProj = GetClosestPoint(bProj);

            Vector3 direction = (bProjProj - b).normalized;
            float a = Vector3.Distance(b, bProj);
            float l = a / Mathf.Sin(alpha);

            Vector3 p = b + direction * l;

            return new Straight(crossProduct, p);
        }

        public Vector3[] Intersection(ILine _Line)
        {
            if (_Line is Straight straight)
            {
                var pointInfo = GetSurfaceClosestPoint(straight.Point);

                if (pointInfo.Point == straight.Point)
                    return new Vector3[] { straight.Point };

                Vector3 orthoVector = (pointInfo.Point - straight.Point).normalized;

                Vector3 point1 = straight.Point + straight.Direction;
                Vector3 point2 = straight.Point - straight.Direction;

                Vector3 direction = Mathf.Abs(GetSurfaceClosestPoint(point1).Distance) < Mathf.Abs(GetSurfaceClosestPoint(point2).Distance) ?
                    straight.Direction :
                    -straight.Direction;

                float angle = Vector3.Angle(orthoVector, direction);

                float hypotenuse = Mathf.Abs(pointInfo.Distance / Mathf.Cos(angle * Mathf.Deg2Rad));

                return new Vector3[] { straight.Point + direction * hypotenuse };
            }

            if (_Line is Circle circle)
            {
                Straight planesSecantStraight = Intersection(circle.GeneratrixPlane);
                var onStraightPointData = GetSurfaceClosestPoint(circle.GeneratrixSphere.Center);
                Vector3 onStraightPoint = onStraightPointData.Point;
                float offset = Mathf.Sqrt(circle.GeneratrixSphere.SqrRadius - onStraightPointData.Distance);

                return new Vector3[] {
                    onStraightPoint + planesSecantStraight.Direction * offset,
                    onStraightPoint - planesSecantStraight.Direction * offset
                };
            }

            throw new NotImplementedException("[Plane] Unknown intersection for type:" + _Line.GetType().FullName);
        }

        public static bool IsPlanesParallel(Plane _PlaneA, Plane _PlaneB)
        {
            return (Vector3.Cross(_PlaneA.Normal, _PlaneB.Normal) == Vector3.zero);
        }

        #endregion

        #region SpatialShape methods

        public override Vector3 GetClosestPoint(Vector3 _Point)
        {
            return GetSurfaceClosestPoint(_Point).Point;
        }

        public override IntersectionType CheckIntersection(ILine _Line)
        {
            if (_Line is Straight straight)
            {
                if (Mathf.Approximately(0, Vector3.Dot(Normal, straight.Direction)))
                    return Mathf.Approximately(0, Distance(straight.Point)) ? IntersectionType.BELONGING : IntersectionType.NONINTERSECTION;
                else
                    return IntersectionType.INTERSECTION;
            }

            if (_Line is Circle circle)
            {
                if (circle.GeneratrixPlane.IsPointBelongs(m_BelongingPoint) && IsPlanesParallel(this, circle.GeneratrixPlane))
                    return IntersectionType.BELONGING;

                Straight intersectionStraight = Intersection(circle.GeneratrixPlane);
                Vector3 closestOnStraightPoint = intersectionStraight.GetClosestPoint(circle.GeneratrixSphere.Center);

                return circle.GeneratrixSphere.IsPointInside(closestOnStraightPoint) ? IntersectionType.INTERSECTION : IntersectionType.NONINTERSECTION;
            }

            throw new NotImplementedException($"[Plane] Unknown intersection for type:{_Line.GetType().FullName}");
        }

#if UNITY_EDITOR
        public override void Visualize()
        {
            using (Common.Debug.UtilsGizmos.ColorPermanence)
            {

                Vector3 orthoVector = UtilsMath.GetRandomOrthogonal(Normal);
                Vector3 orthoVector1 = Vector3.Cross(orthoVector, Normal);

                Gizmos.color = Color.blue;

                Gizmos.DrawLine(BelongingPoint, BelongingPoint + orthoVector.normalized * 0.2f);
                Gizmos.DrawLine(BelongingPoint, BelongingPoint - orthoVector.normalized * 0.2f);
                Gizmos.DrawLine(BelongingPoint, BelongingPoint + orthoVector1.normalized * 0.2f);
                Gizmos.DrawLine(BelongingPoint, BelongingPoint - orthoVector1.normalized * 0.2f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(BelongingPoint, BelongingPoint + Normal);

            }
        }
#endif

#endregion
    }
}
