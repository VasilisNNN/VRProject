using UnityEngine;

namespace Nav3D.Common
{
    public class Triangle : IBoundable, IBoundsIntersectable
    {
        #region Attributes

        Plane? m_V1Solid;
        Plane? m_V2Solid;
        Plane? m_V3Solid;

        #endregion

        #region Properties

        public Vector3 V1 { get; private set; }
        public Vector3 V2 { get; private set; }
        public Vector3 V3 { get; private set; }
        public Vector3 V12 { get; private set; }
        public Vector3 V31 { get; private set; }
        public Vector3 V23 { get; private set; }
        public Vector3 Normal { get; private set; }
        public Vector3 Center => (V1 + V2 + V3) / 3f;
        public Bounds Bounds { get; private set; }
        public Plane Plane { get; private set; }

        #endregion

        #region Constructors

        public Triangle(Vector3 _V1, Vector3 _V2, Vector3 _V3)
        {
            V1 = _V1;
            V2 = _V2;
            V3 = _V3;

            V12 = V2 - V1;
            V23 = V3 - V2;
            V31 = V1 - V3;

            Normal = Vector3.Cross(V12, V23).normalized;

            //Compute bounds
            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;
            float minZ = float.PositiveInfinity;
            float maxZ = float.NegativeInfinity;

            if (V1.x < minX) minX = V1.x;
            if (V1.x > maxX) maxX = V1.x;

            if (V2.x < minX) minX = V2.x;
            if (V2.x > maxX) maxX = V2.x;

            if (V3.x < minX) minX = V3.x;
            if (V3.x > maxX) maxX = V3.x;

            if (V1.y < minY) minY = V1.y;
            if (V1.y > maxY) maxY = V1.y;

            if (V2.y < minY) minY = V2.y;
            if (V2.y > maxY) maxY = V2.y;

            if (V3.y < minY) minY = V3.y;
            if (V3.y > maxY) maxY = V3.y;

            if (V1.z < minZ) minZ = V1.z;
            if (V1.z > maxZ) maxZ = V1.z;

            if (V2.z < minZ) minZ = V2.z;
            if (V2.z > maxZ) maxZ = V2.z;

            if (V3.z < minZ) minZ = V3.z;
            if (V3.z > maxZ) maxZ = V3.z;

            Bounds = ExtensionBounds.MinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Plane = new Plane(V1, V2, V3);

            m_V1Solid = m_V2Solid = m_V3Solid = null;
        }

        #endregion

        #region Public methods

        public void InitSolidHunkTriangles()
        {
            m_V1Solid = new Plane(V1 + Normal, V2, V1);
            m_V2Solid = new Plane(V2 + Normal, V3, V2);
            m_V3Solid = new Plane(V3 + Normal, V1, V3);
        }

        public bool FartherFromPoint(Vector3 _Point, float _DistSqrThreshold)
        {
            if ((_Point - V1).sqrMagnitude < _DistSqrThreshold)
                return false;
            
            if ((_Point - V2).sqrMagnitude < _DistSqrThreshold)
                return false;
            
            if ((_Point - V3).sqrMagnitude < _DistSqrThreshold)
                return false;

            return true;
        }

        public bool InsideOfSolidHunk(Vector3 _Point)
        {
            Plane planeA = m_V1Solid ?? (m_V1Solid = new Plane(V1 + Normal, V2, V1)).Value;
            Plane planeB = m_V2Solid ?? (m_V2Solid = new Plane(V2 + Normal, V3, V2)).Value;
            Plane planeC = m_V3Solid ?? (m_V3Solid = new Plane(V3 + Normal, V1, V3)).Value;

            return planeA.GetSide(_Point) && planeB.GetSide(_Point) && planeC.GetSide(_Point);
        }

        public void Visualize(bool DrawNormal = false)
        {
            Gizmos.DrawLine(V1, V2);
            Gizmos.DrawLine(V1, V3);
            Gizmos.DrawLine(V2, V3);

            if (DrawNormal)
            {
                float sizeFactor = Mathf.Max(
                            Vector3.Distance(V1, V2),
                            Vector3.Distance(V1, V3),
                            Vector3.Distance(V2, V3));

                Gizmos.DrawLine(Center, Center + Normal.normalized * sizeFactor);
            }
        }

        public bool Intersects(Bounds _Bounds)
        {
            if (_Bounds.DoesNotIntersects(Bounds))
                return false;

            return TriangleCubeIntersectionChecker.Inside(this, _Bounds);
        }

        #endregion
    }
}