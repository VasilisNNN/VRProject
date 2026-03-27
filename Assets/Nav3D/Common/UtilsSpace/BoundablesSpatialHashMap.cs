using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

namespace Nav3D.Common
{
    /// <summary>
    /// Spatial storage for objects described by a prismatic boundary (Bounds).
    /// The storage consists of a spatial bucket grid.Each stored object is located in the buckets that it intersects.
    /// There are connections: object -> intersecting buckets and bucket-> intersecting objects.
    /// The size of storage buckets is recalculated each time a new object is added and is taken equal to the largest size of the object's Bounds.
    /// </summary>
    /// <typeparam name="T">Object with Bounds</typeparam>
    class BoundablesSpatialHashMap<T> where T : IBoundable
    {
        #region Attributes

        float m_BucketSize = 0;
        ConcurrentDictionary<T, List<Vector3Int>> m_Boundables = new ConcurrentDictionary<T, List<Vector3Int>>();
        ConcurrentDictionary<Vector3Int, HashSet<T>> m_Buckets = new ConcurrentDictionary<Vector3Int, HashSet<T>>();

        #endregion

        #region Constructors

        public BoundablesSpatialHashMap() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the set of all boundables that cross the given bounds.
        /// </summary>
        /// <param name="_Bounds">The given bounds</param>
        public bool TryGetCrossingBoundables(Bounds _Bounds, out HashSet<T> _CrossingBoundables)
        {
            Vector3[] corners = _Bounds.GetCornerPoints();
            List<Vector3Int> cornerBuckets = corners.Select(_Corner => UtilsMath.GetBucketIndex(_Corner, m_BucketSize)).ToList();

            HashSet<T> result = new HashSet<T>();

            int xMin = cornerBuckets.Min(_Bucket => _Bucket.x);
            int yMin = cornerBuckets.Min(_Bucket => _Bucket.y);
            int zMin = cornerBuckets.Min(_Bucket => _Bucket.z);
            int xMax = cornerBuckets.Max(_Bucket => _Bucket.x);
            int yMax = cornerBuckets.Max(_Bucket => _Bucket.y);
            int zMax = cornerBuckets.Max(_Bucket => _Bucket.z);

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    for (int z = zMin; z <= zMax; z++)
                    {
                        if (m_Buckets.TryGetValue(new Vector3Int(x, y, z), out HashSet<T> boundables))
                        {
                            foreach (T boundable in boundables.Where(_Boundable => _Boundable.Bounds.Intersects(_Bounds)))
                            {
                                result.Add(boundable);
                            }
                        }
                    }

            _CrossingBoundables = result;

            return _CrossingBoundables.Any();
        }

        public bool HasCrossingBoundables(Bounds _Bounds)
        {
            Bounds obstacleBounds = _Bounds;
            
            Vector3[] corners = _Bounds.GetCornerPoints();
            List<Vector3Int> cornerBuckets = corners.Select(_Corner => UtilsMath.GetBucketIndex(_Corner, m_BucketSize)).ToList();

            int xMin = cornerBuckets.Min(_Bucket => _Bucket.x);
            int yMin = cornerBuckets.Min(_Bucket => _Bucket.y);
            int zMin = cornerBuckets.Min(_Bucket => _Bucket.z);
            int xMax = cornerBuckets.Max(_Bucket => _Bucket.x);
            int yMax = cornerBuckets.Max(_Bucket => _Bucket.y);
            int zMax = cornerBuckets.Max(_Bucket => _Bucket.z);

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    for (int z = zMin; z <= zMax; z++)
                    {
                        Vector3Int bucket = new Vector3Int(x, y, z);

                        if (m_Buckets.TryGetValue(new Vector3Int(x, y, z), out HashSet<T> boundables) &&
                            boundables.Any(_Boundable => _Boundable.Bounds.Intersects(_Bounds)))
                            return true;
                    }

            return false;
        }

        public int GetCrossingBoundablesCount(Bounds _Bounds)
        {
            Vector3[] corners = _Bounds.GetCornerPoints();
            List<Vector3Int> cornerBuckets = corners.Select(_Corner => UtilsMath.GetBucketIndex(_Corner, m_BucketSize)).ToList();

            HashSet<T> result = new HashSet<T>();

            int xMin = cornerBuckets.Min(_Bucket => _Bucket.x);
            int yMin = cornerBuckets.Min(_Bucket => _Bucket.y);
            int zMin = cornerBuckets.Min(_Bucket => _Bucket.z);
            int xMax = cornerBuckets.Max(_Bucket => _Bucket.x);
            int yMax = cornerBuckets.Max(_Bucket => _Bucket.y);
            int zMax = cornerBuckets.Max(_Bucket => _Bucket.z);

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    for (int z = zMin; z <= zMax; z++)
                    {
                        Vector3Int bucket = new Vector3Int(x, y, z);

                        if (m_Buckets.TryGetValue(new Vector3Int(x, y, z), out HashSet<T> boundables))
                            foreach (T boundable in boundables.Where(_Boundable => _Boundable.Bounds.Intersects(_Bounds)))
                            {
                                result.Add(boundable);
                            }
                    }

            return result.Count();
        }

        /// <summary>
        /// Returns the set of all boundables that embraces the given bounds.
        /// </summary>
        /// <param name="_Bounds">Given bounds</param>
        public bool TryGetEmbracingBoundables(Bounds _Bounds, out HashSet<T> _EmbracingBoundables)
        {
            Vector3[] corners = _Bounds.GetCornerPoints();
            List<Vector3Int> cornerBuckets = corners.Select(_Corner => UtilsMath.GetBucketIndex(_Corner, m_BucketSize)).ToList();

            HashSet<T> result = new HashSet<T>();

            int xMin = cornerBuckets.Min(_Bucket => _Bucket.x);
            int yMin = cornerBuckets.Min(_Bucket => _Bucket.y);
            int zMin = cornerBuckets.Min(_Bucket => _Bucket.z);
            int xMax = cornerBuckets.Max(_Bucket => _Bucket.x);
            int yMax = cornerBuckets.Max(_Bucket => _Bucket.y);
            int zMax = cornerBuckets.Max(_Bucket => _Bucket.z);

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    for (int z = zMin; z <= zMax; z++)
                    {
                        if (m_Buckets.TryGetValue(new Vector3Int(x, y, z), out HashSet<T> boundables))
                        {
                            foreach (T boundable in boundables.Where(_Boundable => _Boundable.Bounds.Embrace(_Bounds)))
                            {
                                result.Add(boundable);
                            }
                        }
                    }

            _EmbracingBoundables = result;

            return _EmbracingBoundables.Any();
        }
        /// <summary>
        /// Returns the set of all boundables that embraced by the given bounds.
        /// </summary>
        /// <param name="_Bounds">Given bounds</param>
        public bool TryGetEmbracedBoundables(Bounds _Bounds, out HashSet<T> _EmbracingBoundables)
        {
            Vector3[] corners = _Bounds.GetCornerPoints();
            List<Vector3Int> cornerBuckets = corners.Select(_Corner => UtilsMath.GetBucketIndex(_Corner, m_BucketSize)).ToList();

            HashSet<T> result = new HashSet<T>();

            int xMin = cornerBuckets.Min(_Bucket => _Bucket.x);
            int yMin = cornerBuckets.Min(_Bucket => _Bucket.y);
            int zMin = cornerBuckets.Min(_Bucket => _Bucket.z);
            int xMax = cornerBuckets.Max(_Bucket => _Bucket.x);
            int yMax = cornerBuckets.Max(_Bucket => _Bucket.y);
            int zMax = cornerBuckets.Max(_Bucket => _Bucket.z);

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    for (int z = zMin; z <= zMax; z++)
                    {
                        if (m_Buckets.TryGetValue(new Vector3Int(x, y, z), out HashSet<T> boundables))
                        {
                            foreach (T boundable in boundables.Where(_Boundable => _Bounds.Embrace(_Boundable.Bounds)))
                            {
                                result.Add(boundable);
                            }
                        }
                    }

            _EmbracingBoundables = result;

            return _EmbracingBoundables.Any();
        }

        public void Register(T _Boundable)
        {
            Bounds bounds = _Boundable.Bounds;
            float maxBoundsSize = bounds.GetMaxSize();

            if (maxBoundsSize > m_BucketSize)
            {
                List<T> boundables = new List<T>(m_Boundables.Keys);

                Initialize(maxBoundsSize, boundables);
            }

            Vector3[] corners = bounds.GetCornerPoints();

            for (int i = 0; i < corners.Length; i++)
            {
                AddToBucket(_Boundable, UtilsMath.GetBucketIndex(corners[i], m_BucketSize));
            }
        }

        public void Unregister(T _Boundable)
        {
            RemoveFromBuckets(_Boundable);

            CheckMaxBucketSize();
        }

#if UNITY_EDITOR
        public void Draw()
        {
            using (Debug.UtilsGizmos.ColorPermanence)
            {
                foreach (Vector3Int bucket in m_Buckets.Keys)
                    Gizmos.DrawWireCube(GetBucketCenter(bucket), new Vector3(m_BucketSize, m_BucketSize, m_BucketSize));
            }
        }
#endif

#endregion

        #region Service methods

        void CheckMaxBucketSize()
        {
            float maxBoundsSize = 0;

            foreach (T boundable in m_Boundables.Keys)
            {
                float boundableMaxSize = boundable.Bounds.GetMaxSize();

                if (boundableMaxSize > maxBoundsSize)
                    maxBoundsSize = boundableMaxSize;
            }

            if (maxBoundsSize < m_BucketSize)
            {
                List<T> boundables = new List<T>(m_Boundables.Keys);

                Initialize(maxBoundsSize, boundables);
            }
        }

        void Initialize(float _BucketSize, IEnumerable<T> _Boundables = null)
        {
            m_Boundables.Clear();
            m_Buckets.Clear();

            m_BucketSize = _BucketSize;

            if (_Boundables.IsNullOrEmpty())
                return;

            foreach (T boundable in _Boundables)
                Register(boundable);
        }

        void AddToBucket(T _Boundable, Vector3Int _Bucket)
        {
            if (m_Buckets.TryGetValue(_Bucket, out HashSet<T> set))
            {
                set.Add(_Boundable);
            }
            else
            {
                m_Buckets.TryAdd(_Bucket, new HashSet<T> { _Boundable });
            }

            m_Boundables.GetOrAdd(_Boundable).Add(_Bucket);
        }

        void RemoveFromBuckets(T _Boundable)
        {
            if (!m_Boundables.TryGetValue(_Boundable, out List<Vector3Int> buckets))
                return;

            foreach (Vector3Int bucket in buckets)
            {
                if (m_Buckets.TryGetValue(bucket, out HashSet<T> bucketSet))
                {
                    bucketSet.Remove(_Boundable);

                    if (!bucketSet.Any())
                        m_Buckets.TryRemove(bucket, out _);
                }
            }

            m_Boundables.TryRemove(_Boundable, out List<Vector3Int> _);
        }

        Vector3 GetBucketCenter(Vector3Int _Bucket)
        {
            return new Vector3(_Bucket.x * m_BucketSize, _Bucket.y * m_BucketSize, _Bucket.z * m_BucketSize);
        }

        #endregion
    }
}