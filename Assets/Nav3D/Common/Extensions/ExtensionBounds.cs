using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Plane = Nav3D.LocalAvoidance.SupportingMath.Plane;
using Straight = Nav3D.LocalAvoidance.SupportingMath.Straight;

namespace Nav3D.Common
{
    public static class ExtensionBounds
    {
        #region Constants

        const string UNION_NO_PARAMS_ERROR = "There is no any parameters for the Bounds.Union() method.";

        #endregion

        #region Static methods

        public static void Draw(this Bounds _Bounds)
        {
            Gizmos.DrawWireCube(_Bounds.center, _Bounds.size);
        }
        
        public static void DrawSolid(this Bounds _Bounds)
        {
            Gizmos.DrawCube(_Bounds.center, _Bounds.size);
        }

        public static float GetMaxSize(this Bounds _Bounds)
        {
            return Mathf.Max(_Bounds.size.x, _Bounds.size.y, _Bounds.size.z);
        }
        
        public static float GetMinSize(this Bounds _Bounds)
        {
            return Mathf.Min(_Bounds.size.x, _Bounds.size.y, _Bounds.size.z);
        }

        public static bool DoesNotIntersects(this Bounds _Bounds, Bounds _Other)
        {
            Vector3 boundsMin = _Bounds.min;
            Vector3 boundsMax = _Bounds.max;

            Vector3 otherMin = _Other.min;
            Vector3 otherMax = _Other.max;

            return boundsMin.x > otherMax.x ||
                boundsMax.x < otherMin.x ||
                boundsMin.y > otherMax.y ||
                boundsMax.y < otherMin.y ||
                boundsMin.z > otherMax.z ||
                boundsMax.z < otherMin.z;
        }

        public static Bounds TrianglesBounds(List<Triangle> _Triangles)
        {
            Bounds targetBounds = _Triangles.First().Bounds;

            if (_Triangles.Count == 1)
                return targetBounds;

            for (int i = 1; i < _Triangles.Count; i++)
            {
                targetBounds = targetBounds.Union(_Triangles[i].Bounds);
            }

            return targetBounds;
        }

        public static Bounds PointsBounds(List<Vector3> _Points)
        {
            float xMin = float.MaxValue;
            float yMin = float.MaxValue;
            float zMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMax = float.MinValue;
            float zMax = float.MinValue;

            for(int i =0; i< _Points.Count; i++)
            {
                Vector3 point = _Points[i];

                xMin = Mathf.Min(xMin, point.x);
                yMin = Mathf.Min(yMin, point.y);
                zMin = Mathf.Min(zMin, point.z);
                xMax = Mathf.Max(xMax, point.x);
                yMax = Mathf.Max(yMax, point.y);
                zMax = Mathf.Max(zMax, point.z);
            }

            float sizeX = xMax - xMin;
            float sizeY = yMax - yMin;
            float sizeZ = zMax - zMin;

            Vector3 center = new Vector3(xMin + sizeX * 0.5f, yMin + sizeY * 0.5f, zMin + sizeZ * 0.5f);

            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            return new Bounds(center, size);
        }

        public static bool Embrace(this Bounds _Bounds, Bounds _OtherBounds)
        {
            return _OtherBounds.GetCornerPoints().All(_Point => _Bounds.Contains(_Point));
        }

        public static Bounds MinMax(Vector3 _A, Vector3 _B)
        {
            float xMin = Mathf.Min(_A.x, _B.x);
            float yMin = Mathf.Min(_A.y, _B.y);
            float zMin = Mathf.Min(_A.z, _B.z);
            float xMax = Mathf.Max(_A.x, _B.x);
            float yMax = Mathf.Max(_A.y, _B.y);
            float zMax = Mathf.Max(_A.z, _B.z);

            float sizeX = xMax - xMin;
            float sizeY = yMax - yMin;
            float sizeZ = zMax - zMin;

            Vector3 center = new Vector3(xMin + sizeX * 0.5f, yMin + sizeY * 0.5f, zMin + sizeZ * 0.5f);

            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            return new Bounds(center, size);
        }

        ///                Bounds A
        ///  _____________ /                         ___________________ 
        /// |             |                         |                   |
        /// |      _______|_____                    |                   |
        /// |     |       |     |-- Bounds B   ==>  |                   |
        /// |_____|_______|     |                   |                   |
        ///       |             |                   |                   |
        ///       |_____________|                   |___________________|
        public static Bounds Union(this Bounds _BoundsA, Bounds _BoundsB)
        {
            _BoundsA.Encapsulate(_BoundsB.min);
            _BoundsA.Encapsulate(_BoundsB.max);

            return _BoundsA;
        }

        public static Bounds Union(params Bounds[] _Params)
        {
            if (!_Params.Any())
                throw new Exception(UNION_NO_PARAMS_ERROR);

            if (_Params.Length == 1)
                return _Params.First();

            Bounds result = _Params.First();

            for (int i = 1; i < _Params.Length; i++)
            {
                result = result.Union(_Params[i]);
            }

            return result;
        }

        public static Vector3[] GetCornerPoints(this Bounds _Bounds)
        {
            Vector3 center = _Bounds.center;
            Vector3 extents = _Bounds.extents;

            Vector3[] corners = new Vector3[8];

            corners[0] = center + Vector3.Scale(extents, new Vector3(-1, -1, -1));
            corners[1] = center + Vector3.Scale(extents, new Vector3(-1, -1, 1));
            corners[2] = center + Vector3.Scale(extents, new Vector3(-1, 1, -1));
            corners[3] = center + Vector3.Scale(extents, new Vector3(1, -1, -1));
            corners[4] = center + Vector3.Scale(extents, new Vector3(-1, 1, 1));
            corners[5] = center + Vector3.Scale(extents, new Vector3(1, -1, 1));
            corners[6] = center + Vector3.Scale(extents, new Vector3(1, 1, -1));
            corners[7] = center + Vector3.Scale(extents, new Vector3(1, 1, 1));

            return corners;
        }

        /// <summary>
        /// Returns the bounds which size components is a smallest multiple of multiplier greater to or equal to source bounds size components.
        /// </summary>
        public static Bounds CeilSizeToMultiple(this Bounds _Bounds, float _Multiple)
        {
            float sizeX = _Bounds.size.x;
            float sizeY = _Bounds.size.y;
            float sizeZ = _Bounds.size.z;

            int ceilX = (int)Mathf.Ceil(sizeX / _Multiple);
            int ceilY = (int)Mathf.Ceil(sizeY / _Multiple);
            int ceilZ = (int)Mathf.Ceil(sizeZ / _Multiple);

            sizeX = (ceilX + 2) * _Multiple;
            sizeY = (ceilY + 2) * _Multiple;
            sizeZ = (ceilZ + 2) * _Multiple;

            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            return new Bounds(_Bounds.center, size);
        }

        /// <summary>
        /// Intersection cases:
        /// 1) The ray penetrates one or two faces of the bounds;
        /// 2) The ray is inside of bounds;
        /// </summary> 
        public static bool IntersectRay(this Bounds _Bounds, Ray _Ray)
        {
            if (_Bounds.Contains(_Ray.Start) || _Bounds.Contains(_Ray.End))
                return true;

            float boundsMinX = Mathf.Min(_Bounds.min.x, _Bounds.max.x);
            float boundsMaxX = Mathf.Max(_Bounds.min.x, _Bounds.max.x);
            float boundsMinY = Mathf.Min(_Bounds.min.y, _Bounds.max.y);
            float boundsMaxY = Mathf.Max(_Bounds.min.y, _Bounds.max.y);
            float boundsMinZ = Mathf.Min(_Bounds.min.z, _Bounds.max.z);
            float boundsMaxZ = Mathf.Max(_Bounds.min.z, _Bounds.max.z);

            Plane xMinPlane = new Plane(Vector3.right, new Vector3(boundsMinX, 0, 0));
            Plane xMaxPlane = new Plane(Vector3.left, new Vector3(boundsMaxX, 0, 0));
            Plane yMinPlane = new Plane(Vector3.up, new Vector3(0, boundsMinY, 0));
            Plane yMaxPlane = new Plane(Vector3.down, new Vector3(0, boundsMaxY, 0));
            Plane zMinPlane = new Plane(Vector3.forward, new Vector3(0, 0, boundsMinZ));
            Plane zMaxPlane = new Plane(Vector3.back, new Vector3(0, 0, boundsMaxZ));

            Straight rayStraight = new Straight(_Ray.DirectionMagn, _Ray.Origin);

            if (_Ray.Start.x <= boundsMinX && _Ray.End.x >= boundsMinX ||
               _Ray.Start.x >= boundsMinX && _Ray.End.x <= boundsMinX)
            {
                Vector3 intersectionPoint = xMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                    return true;
            }

            if (_Ray.Start.x <= boundsMaxX && _Ray.End.x >= boundsMaxX ||
               _Ray.Start.x >= boundsMaxX && _Ray.End.x <= boundsMaxX)
            {
                Vector3 intersectionPoint = xMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                    return true;
            }

            if (_Ray.Start.y <= boundsMinY && _Ray.End.y >= boundsMinY ||
               _Ray.Start.y >= boundsMinY && _Ray.End.y <= boundsMinY)
            {
                Vector3 intersectionPoint = yMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                    return true;
            }

            if (_Ray.Start.y <= boundsMaxY && _Ray.End.y >= boundsMaxY ||
               _Ray.Start.y >= boundsMaxY && _Ray.End.y <= boundsMaxY)
            {
                Vector3 intersectionPoint = yMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                    return true;
            }

            if (_Ray.Start.z <= boundsMinZ && _Ray.End.z >= boundsMinZ ||
               _Ray.Start.z >= boundsMinZ && _Ray.End.z <= boundsMinZ)
            {
                Vector3 intersectionPoint = zMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY)
                    return true;
            }

            if (_Ray.Start.z <= boundsMaxZ && _Ray.End.z >= boundsMaxZ ||
               _Ray.Start.z >= boundsMaxZ && _Ray.End.z <= boundsMaxZ)
            {
                Vector3 intersectionPoint = zMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Here we assume that intersection exist.
        /// -----------------------------------------------------
        ///          \  intersection 1
        ///       ____\/______ 
        ///      |     \      |
        ///      |      \     |
        ///      |       \    |
        ///      |________\___|
        ///               /\
        /// intersection 2  \
        /// </summary>
        public static List<Vector3> GetIntersection(this Bounds _Bounds, Ray _Ray)
        {
            List<Vector3> intersections = new List<Vector3>();

            float boundsMinX = Mathf.Min(_Bounds.min.x, _Bounds.max.x);
            float boundsMaxX = Mathf.Max(_Bounds.min.x, _Bounds.max.x);
            float boundsMinY = Mathf.Min(_Bounds.min.y, _Bounds.max.y);
            float boundsMaxY = Mathf.Max(_Bounds.min.y, _Bounds.max.y);
            float boundsMinZ = Mathf.Min(_Bounds.min.z, _Bounds.max.z);
            float boundsMaxZ = Mathf.Max(_Bounds.min.z, _Bounds.max.z);

            Plane xMinPlane = new Plane(Vector3.right, new Vector3(boundsMinX, 0, 0));
            Plane xMaxPlane = new Plane(Vector3.left, new Vector3(boundsMaxX, 0, 0));
            Plane yMinPlane = new Plane(Vector3.up, new Vector3(0, boundsMinY, 0));
            Plane yMaxPlane = new Plane(Vector3.down, new Vector3(0, boundsMaxY, 0));
            Plane zMinPlane = new Plane(Vector3.forward, new Vector3(0, 0, boundsMinZ));
            Plane zMaxPlane = new Plane(Vector3.back, new Vector3(0, 0, boundsMaxZ));

            Straight rayStraight = new Straight(_Ray.DirectionMagn, _Ray.Origin);

            //ray start and end points are in opposite halfspaces of the plane 
            if (_Ray.Start.x <= boundsMinX && _Ray.End.x >= boundsMinX ||
               _Ray.Start.x >= boundsMinX && _Ray.End.x <= boundsMinX)
            {
                Vector3 intersectionPoint = xMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            if (_Ray.Start.x <= boundsMaxX && _Ray.End.x >= boundsMaxX ||
               _Ray.Start.x >= boundsMaxX && _Ray.End.x <= boundsMaxX)
            {
                Vector3 intersectionPoint = xMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            if (_Ray.Start.y <= boundsMinY && _Ray.End.y >= boundsMinY ||
               _Ray.Start.y >= boundsMinY && _Ray.End.y <= boundsMinY)
            {
                Vector3 intersectionPoint = yMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            if (_Ray.Start.y <= boundsMaxY && _Ray.End.y >= boundsMaxY ||
               _Ray.Start.y >= boundsMaxY && _Ray.End.y <= boundsMaxY)
            {
                Vector3 intersectionPoint = yMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.z <= boundsMaxZ && intersectionPoint.z >= boundsMinZ)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            if (_Ray.Start.z <= boundsMinZ && _Ray.End.z >= boundsMinZ ||
               _Ray.Start.z >= boundsMinZ && _Ray.End.z <= boundsMinZ)
            {
                Vector3 intersectionPoint = zMinPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            if (_Ray.Start.z <= boundsMaxZ && _Ray.End.z >= boundsMaxZ ||
               _Ray.Start.z >= boundsMaxZ && _Ray.End.z <= boundsMaxZ)
            {
                Vector3 intersectionPoint = zMaxPlane.Intersection(rayStraight).First();

                if (intersectionPoint.x <= boundsMaxX && intersectionPoint.x >= boundsMinX &&
                    intersectionPoint.y <= boundsMaxY && intersectionPoint.y >= boundsMinY)
                {
                    intersections.Add(intersectionPoint);
                }
            }

            return intersections;
        }

        #endregion
    }
}
