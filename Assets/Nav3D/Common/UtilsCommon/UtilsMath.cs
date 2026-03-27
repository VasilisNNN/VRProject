using System;
using System.Linq;
using UnityEngine;
using Nav3D.LocalAvoidance.SupportingMath;
using Plane = Nav3D.LocalAvoidance.SupportingMath.Plane;

namespace Nav3D.Common
{
    public enum IntersectionType
    {
        BELONGING,
        INTERSECTION,
        NONINTERSECTION
    }

    public static class UtilsMath
    {
        #region Constants

        public const float PLANE_BOUNDARY_THRESHOLD = 0.000001f;

        #endregion

        #region Attributes

        static System.Random m_Random = new System.Random();

        #endregion

        #region Properties

        public static Vector3 RandomNormal => GetRandomVector().normalized;

        public static readonly Vector3Int[] NeighborBuckets = new Vector3Int[]
        {
            new Vector3Int(-1, -1, -1),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, -1, 1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, -1, -1),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, -1, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, -1, -1),
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, -1, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, 1)
        };

        public static readonly Vector3Int[] BucketFacesNormals = new Vector3Int[]
        {
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.forward,
            Vector3Int.back
        };

        #endregion

        #region Public methods

        public static float Sqr(float _Value)
        {
            return _Value * _Value;
        }

        public static Vector3 WeightedVector3Sum2(Vector3 _Vector1, Vector3 _Vector2, float _Weight1, float _Weight2)
        {
            return Vector3.Slerp(_Vector1, _Vector2, _Weight2 / (_Weight1 + _Weight2));
        }

        public static Vector3 WeightedVector3Sum3(Vector3 _Vector1, Vector3 _Vector2, Vector3 _Vector3, float _Weight1, float _Weight2, float _Weight3, out Vector3 _V23)
        {
            float w23 = _Weight2 + _Weight3;
            Vector3 v23 = Vector3.Slerp(_Vector2, _Vector3, _Weight3 / w23);

            _V23 = v23;

            float w23Mean = w23 / 2;

            Vector3 result = Vector3.Slerp(v23, _Vector1, _Weight1 / (_Weight1 + w23Mean));

            return result;
        }

        public static Vector3[] GetSphereSurfacePoints(Vector3 _Center, float _Radius, int _Count)
        {
            Vector3[] directions;

            directions = new Vector3[_Count];

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < _Count; i++)
            {
                float t = (float)i / _Count;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                directions[i] = new Vector3(x, y, z);
            }

            return directions.Select(_Direction => _Center + _Direction * _Radius).ToArray();
        }


        public static float TriangleHeronSquare(float _A, float _B, float _C)
        {
            float p = 0.5f * (_A + _B + _C);
            return Mathf.Sqrt(p * (p - _A) * (p - _B) * (p - _C));
        }

        public static Vector3 GetRandomOrthogonal(Vector3 _Vector)
        {
            const int seed = 100;
            UnityEngine.Random.InitState(seed);

            return Vector3.Cross(_Vector, RandomNormal);
        }

        public static Vector3 GetRandomVector(float _Magnitude = 1f)
        {
            return new Vector3(
                SystemRandomRange(-_Magnitude, _Magnitude),
                SystemRandomRange(-_Magnitude, _Magnitude),
                SystemRandomRange(-_Magnitude, _Magnitude)
            );
        }

        //Gaussian distribution
        public static float GetRandomNormalValue(float _Min, float _Max)
        {
            float delta = _Max - _Min;
            float halfDelta = delta * 0.5f;

            return Mathf.Clamp(_Min + halfDelta + (float)GenerateGaussianNoise().Z0 * halfDelta, _Min, _Max);
        }

        //Uniform distribution
        public static float GetRandomUniformValue(float _Min, float _Max)
        {
            return UnityEngine.Random.Range(_Min, _Max);
        }

        public static Ray[] PointSequanceToSegments(Vector3[] _Sequance)
        {
            Ray[] result = new Ray[_Sequance.Length - 1];

            for (int i = 0; i < _Sequance.Length - 1; i++)
            {
                result[i] = new Ray(_Sequance[i], _Sequance[i + 1]);
            }

            return result.ToArray();
        }

        public static Vector3Int GetBucketIndex(Vector3 _Point, float _BucketSize)
        {
            return new Vector3Int(
                Mathf.FloorToInt(_Point.x / _BucketSize),
                Mathf.FloorToInt(_Point.y / _BucketSize),
                Mathf.FloorToInt(_Point.z / _BucketSize)
            );
        }

        public static Vector3Int GetBucketIndex(Vector3 _Point, float _BucketSize, out bool xOnBorderLine, out bool yOnBorderLine, out bool zOnBorderLine)
        {
            float x = _Point.x / _BucketSize;
            float y = _Point.y / _BucketSize;
            float z = _Point.z / _BucketSize;

            xOnBorderLine = (x % 1) == 0;
            yOnBorderLine = (y % 1) == 0;
            zOnBorderLine = (z % 1) == 0;

            return new Vector3Int(
                Mathf.FloorToInt(x),
                Mathf.FloorToInt(y),
                Mathf.FloorToInt(z)
            );
        }

        public static Bounds GetBucket(Vector3Int _Index, float _BucketSize)
        {
            return new Bounds(
                new Vector3(
                    _Index.x * _BucketSize + 0.5f * _BucketSize,
                    _Index.y * _BucketSize + 0.5f * _BucketSize,
                    _Index.z * _BucketSize + 0.5f * _BucketSize
                ),
                new Vector3(_BucketSize, _BucketSize, _BucketSize)
            );
        }

        public static Bounds GetBucket(Vector3 _Point, float _BucketSize)
        {
            Vector3Int index = GetBucketIndex(_Point, _BucketSize);

            return GetBucket(index, _BucketSize);
        }

        #endregion

        #region Service methods

        static float SystemRandomRange(float _Min, float _Max)
        {
            float delta = _Max - _Min;
            float deltaMinZero = Mathf.Abs(_Min);

            float rnd = (float)m_Random.NextDouble() * delta - deltaMinZero;
            return rnd;
        }

        //Box–Muller transform
        static (double Z0, double Z1) GenerateGaussianNoise()
        {
            double x, y, s;

            do
            {
                x = UnityEngine.Random.Range(-1f, 1f);
                y = UnityEngine.Random.Range(-1f, 1f);

                s = x * x + y * y;
            } while (s > 1f || s == 0);

            double factor = Math.Sqrt(-2 * Math.Log(s) / s);

            double z0 = x * factor;
            double z1 = y * factor;

            return (z0, z1);
        }

        public static bool VelocityTrailIntersects(Vector3 _Position1, Vector3 _FrameVelocity1, float _Radius1, Vector3 _Position2, Vector3 _FrameVelocity2, float _Radius2)
        {
            Vector3 crossProduct = Vector3.Cross(_FrameVelocity1, _FrameVelocity2).normalized;

            float radiiSum = _Radius1 + _Radius2;
            float radiiSumSqr = Sqr(radiiSum);

            //collinear condition
            if (crossProduct == Vector3.zero)
            {
                {
                    Vector3 p11 = _Position1;
                    Vector3 p12 = _Position1 + _FrameVelocity1;

                    Vector3 p1ToP2Direction = _Position2 - p11;
                    Vector3 crossProduct1 = Vector3.Cross(_FrameVelocity1, p1ToP2Direction);

                    Vector3 normal = Vector3.Cross(crossProduct1, _FrameVelocity1);

                    //the distance between direction vectors
                    float distToP2 = new Plane(normal, p11).Distance(_Position2);

                    if (distToP2 >= radiiSum)
                        return false;

                    Plane p11Plane = new Plane(_FrameVelocity1, p11);

                    Vector3 p21 = _Position2;
                    Vector3 p22 = _Position2 + _FrameVelocity2;

                    bool p11p21Side = p11Plane.GetSide(p21);
                    bool p11p22Side = p11Plane.GetSide(p22);

                    //p21 and p22 lie on the opposite sides of p11Plane
                    if (p11p21Side != p11p22Side)
                        return true;

                    Plane p12Plane = new Plane(-_FrameVelocity1, p12);

                    bool p12p21Side = p12Plane.GetSide(p21);
                    bool p12p22Side = p12Plane.GetSide(p22);

                    //p21 and p22 lie on the opposite sides of p11Plane
                    if (p12p21Side != p12p22Side)
                        return true;

                    //p21 and p22 lie between p11Plane and p12Plane
                    if (p11p21Side && p12p21Side)
                        return true;

                    if (p11p21Side)
                    {
                        float p12p21SqrDist = (p12 - p21).sqrMagnitude;
                        float p12p22SqrDist = (p12 - p22).sqrMagnitude;

                        if (p12p21SqrDist < p12p22SqrDist)
                            return p12p21SqrDist < radiiSumSqr;
                        
                        return p12p22SqrDist < radiiSumSqr;
                    }

                    float p11p21SqrDist = (p11 - p21).sqrMagnitude;
                    float p11p22SqrDist = (p11 - p22).sqrMagnitude;

                    if (p11p21SqrDist < p11p22SqrDist)
                        return p11p21SqrDist < radiiSumSqr;
                    
                    return p11p22SqrDist < radiiSumSqr;
                }
            }

            Vector3 crossAndV1Cross = Vector3.Cross(_FrameVelocity1, crossProduct);
            Vector3 p2ClosestPoint = new Plane(crossAndV1Cross, _Position1).Intersection(new Straight(_FrameVelocity2, _Position2)).First();

            Vector3 crossAndV2Cross = Vector3.Cross(_FrameVelocity2, crossProduct);
            Vector3 p1ClosestPoint = new Plane(crossAndV2Cross, _Position2).Intersection(new Straight(_FrameVelocity1, _Position1)).First();

            float closestPointsDistance = Vector3.Distance(p2ClosestPoint, p1ClosestPoint);

            if (closestPointsDistance >= _Radius1 + _Radius2)
                return false;

            {
                Vector3 p11 = _Position1;
                Vector3 p12 = _Position1 + _FrameVelocity1;

                Vector3 p21 = _Position2;
                Vector3 p22 = _Position2 + _FrameVelocity2;

                Plane p11Plane = new Plane(_FrameVelocity1, p11);
                Plane p21Plane = new Plane(_FrameVelocity2, p21);
                Plane p22Plane = new Plane(-_FrameVelocity2, p22);

                if (!p11Plane.GetSide(p1ClosestPoint))
                {
                    if (!p21Plane.GetSide(p1ClosestPoint))
                        return (p11 - p21).sqrMagnitude < radiiSumSqr;

                    if (!p22Plane.GetSide(p1ClosestPoint))
                        return (p11 - p22).sqrMagnitude < radiiSumSqr;

                    return (p11 - new Straight(_FrameVelocity2, p21).ClosestPoint(p11)).sqrMagnitude < radiiSumSqr;
                }
                
                Plane p12Plane = new Plane(-_FrameVelocity1, p12);

                if (!p12Plane.GetSide(p1ClosestPoint))
                {
                    if (!p21Plane.GetSide(p1ClosestPoint))
                        return (p12 - p21).sqrMagnitude < radiiSumSqr;

                    if (!p22Plane.GetSide(p1ClosestPoint))
                        return (p12 - p22).sqrMagnitude < radiiSumSqr;

                    return (p12 - new Straight(_FrameVelocity2, p21).ClosestPoint(p12)).sqrMagnitude < radiiSumSqr;
                }

                if (!p21Plane.GetSide(p1ClosestPoint))
                    return (p21 - new Straight(_FrameVelocity1, p11).ClosestPoint(p21)).sqrMagnitude < radiiSumSqr;

                if (!p22Plane.GetSide(p1ClosestPoint))
                    return (p22 - new Straight(_FrameVelocity1, p11).ClosestPoint(p22)).sqrMagnitude < radiiSumSqr;

                return true;
            }
        }

        #endregion
    }
}