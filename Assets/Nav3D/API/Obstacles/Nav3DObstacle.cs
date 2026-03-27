using System.Collections.Generic;
using UnityEngine;
using Nav3D.Obstacles;
using Nav3D.Common;
using System.Linq;
using System;

namespace Nav3D.API
{
    [ExecuteInEditMode]
    public class Nav3DObstacle : MonoBehaviour
    {
        #region Constants

        const float AUTO_UPDATE_MIN_PERIOD = 1f;
        readonly string NAV3D_INITIALIZER_NOT_FOUND_ERROR = $"Serialization error. {nameof(Nav3DInitializer)} component is not found on scene.";
        readonly string UNKNOWN_OBSTACLE_DATA_SOURCE_TYPE = $"Unknown {nameof(ObstacleDataSource)} value: {{0}}";

        const string ADD_REMOVAL_UNAVALABLE_TIP = "Addition/Removal operations is unavailable at runtime for pre-baked obstacles.";
        readonly static string CROSS_STATIC_OBSTACLE_ERROR = $"An intersection with a static obstacle has been detected (GameObject name: {{0}})! " +
            "(Static obstacles - loaded on scene from binary). Runtime addition of obstacles that cross static obstacles on the scene is not allowed.";
        readonly static string HAS_NO_OBSTACLE_INFOS_ERROR = $"There is no obstacle data components found for GameObject: {{0}}. Make sure that the game object or its children in the transform hierarchy " +
            $"has attached {nameof(MeshFilter)} component with the selected {nameof(Mesh)}, or {nameof(Terrain)} component. ";

        #endregion

        #region Exceptions

        public class IntersectStaticObstacleException : Exception
        {
            #region Constructors

            public IntersectStaticObstacleException(string _Name) : base(string.Format(CROSS_STATIC_OBSTACLE_ERROR, _Name))
            {

            }

            #endregion
        }

        #endregion

        #region Events

        public event Action OnObstacleAdded;

        #endregion

        #region Nested types

        public enum ObstacleDataSource
        {
            RUNTIME_PROCESSING,
            LOAD_FROM_BINARY
        }

        #endregion

        #region Serialized fields

        [SerializeField] bool m_AutoAddChilds;
        [SerializeField] ObstacleDataSource m_ObstacleDataSource;

        [SerializeField] bool m_AutoUpdateIfChanged = false;
        [SerializeField] float m_AutoUpdateMinPeriod = AUTO_UPDATE_MIN_PERIOD;

        [SerializeField] bool m_DrawOccupiedLeaves;
        [SerializeField] bool m_DrawFreeLeaves;
        [SerializeField] bool m_DrawGraph;

        [SerializeField] int m_OccupiedLeavesDrawingMode;         //0-all layers, 1-specific layer
        [SerializeField] int m_OccupiedLeavesDrawingLayerNumber;

        [SerializeField] int m_FreeLeavesDrawingMode;         //0-all layers, 1-specific layer
        [SerializeField] int m_FreeLeavesDrawingLayerNumber;

        [SerializeField] int m_GraphNodesDrawingMode;         //0-all layers, 1-specific layer
        [SerializeField] int m_GraphNodesDrawingLayerNumber;

        #endregion

        #region Attributes

        int m_InstanceID;

        //cached transform
        Transform m_Transform;
        DateTime m_LastAutoUpdateTime;

        List<ObstacleInfoBase> m_ObstacleInfos;

        ObstacleAdditionProgress m_RuntimeAdditionProgress;

        Nav3DObstacleLoader m_Loader;

#if UNITY_EDITOR
        Common.Debug.GizmosDrawData m_GizmosData;
#endif

        #endregion

        #region Properties

        public int InstanceID => m_InstanceID;

        public int OctreeLayersCount => ObstacleManager.Instance.GetObstacleOctreeLayersMaxCount(InstanceID);
        public ObstacleDataSource DataSource => m_ObstacleDataSource;
        public IObstacleAdditionProgress AdditionProgress =>
            (DataSource == ObstacleDataSource.RUNTIME_PROCESSING) ?
            (IObstacleAdditionProgress)m_RuntimeAdditionProgress :
            (m_Loader != null ? m_Loader.DeserializingProgress : null);

        //Is transform changed since the obstacle was last baked?
        public bool TransformHasChanged => GetTarsnformHasChanged();

        Transform CachedTransform => m_Transform = m_Transform != null ? m_Transform : transform;

        #endregion

        #region Public methods

        public void MarkTransformHasChangedTrue()
        {
            CachedTransform.hasChanged = true;
        }

        public void MarkAsSerialized()
        {
            MarkTransformHasChanged();
        }

        public void InvokeOnObstacleAdded()
        {
#if UNITY_EDITOR
            ClearGizmosCache();
#endif
            OnObstacleAdded?.Invoke();
        }

        public List<ObstacleInfoBase> PrepareObstacleInfos(out string[] _ProcessablesImprint)
        {
            Init();

            //Get obstacle infos
            List<ObstacleInfoSingle> obstacleInfos = GetObstacleProcessingInfos(out _ProcessablesImprint);

            //Merge separate obstacles in indivisible groups using bounds intersection creterio
            List<ObstacleInfoBase> groupedObstacleInfos = ObstacleInfoBase.GroupInfos(obstacleInfos.Select(_Data => (ObstacleInfoBase)_Data).ToList());

            m_ObstacleInfos = groupedObstacleInfos;

            return groupedObstacleInfos;
        }

        public void Init()
        {
            m_InstanceID = GetInstanceID();
        }

        public string[] GetImprint()
        {
            return GetAllProcessableChilds().Keys.ToArray();
        }

#if UNITY_EDITOR
        public void ClearGizmosCache()
        {
            m_GizmosData?.Clear();
        }
#endif

        #endregion

        #region Service methods

        bool GetTarsnformHasChanged()
        {
            return CachedTransform.hasChanged || CachedTransform.GetAllChilds(true).Any(_Transform => _Transform.hasChanged);
        }

        void MarkTransformHasChanged(bool _Value = false)
        {
            CachedTransform.hasChanged = _Value;
            CachedTransform.GetAllChilds(true).ForEach(_Transform => _Transform.hasChanged = _Value);
        }

        void ProcessObstacleRuntime()
        {
            if (!m_ObstacleInfos.Any())
                throw new Exception(string.Format(HAS_NO_OBSTACLE_INFOS_ERROR, gameObject.name));

            Bounds bounds = ExtensionBounds.Union(m_ObstacleInfos.Select(_Info => _Info.Bounds).ToArray());

            if (ObstacleManager.Instance.BoundsCrossStaticObstacles(bounds))
                throw new IntersectStaticObstacleException(gameObject.name);

            Nav3DManager.OnNav3DInit += () =>
            {
                if (ObstacleManager.Doomed)
                    return;

                m_RuntimeAdditionProgress = ObstacleManager.Instance.AddObstacles(m_ObstacleInfos, InvokeOnObstacleAdded);

                m_LastAutoUpdateTime = DateTime.UtcNow;
            };
        }

        void RemoveObstacle()
        {
            if (ObstacleManager.Doomed)
                return;

            ObstacleManager.Instance.RemoveObstacles(m_InstanceID);
        }

        void UpdateObstacle()
        {
            if (ObstacleManager.Doomed)
                return;

            RemoveObstacle();

            ProcessObstacleRuntime();

            m_LastAutoUpdateTime = DateTime.UtcNow;
        }

        List<ObstacleInfoSingle> GetObstacleProcessingInfos(out string[] _ProcessablesImprint)
        {
            List<ObstacleInfoSingle> obstacleProcessingDatas = new List<ObstacleInfoSingle>();

            Dictionary<string, Transform> processableTransforms = GetAllProcessableChilds();

            processableTransforms.ForEach(_Kvp =>
            {
                if (_Kvp.Value.TryGetObstacleInfo(this, out ObstacleInfoSingle obstacleInfo))
                    obstacleProcessingDatas.Add(obstacleInfo);
            });

            _ProcessablesImprint = processableTransforms.Keys.ToArray();

            return obstacleProcessingDatas;
        }

        Dictionary<string, Transform> GetAllProcessableChilds()
        {
            Dictionary<string, Transform> result = new Dictionary<string, Transform>();

            if (m_AutoAddChilds)
            {
                GetProcessableChildsRecursively(result, CachedTransform, 0, 0);
            }
            else
            {
                if (IsTransformProcessable(CachedTransform, out string imprint))
                    result.Add($"0{imprint}", CachedTransform);
            }

            return result;
        }

        int GetProcessableChildsRecursively(Dictionary<string, Transform> _Result, Transform _Transform, int _Index, int _Depth)
        {
            if (IsTransformProcessable(_Transform, out string imprint))
            {
                _Result.Add(string.Join(null, _Index, imprint, _Depth), _Transform);
                _Index++;
            }

            foreach (Transform child in _Transform)
            {
                _Index = GetProcessableChildsRecursively(_Result, child, _Index, _Depth + 1);
            }

            return _Index;
        }

        bool IsTransformProcessable(Transform _Transform, out string _TransformImprint)
        {
            if (!_Transform.gameObject.activeInHierarchy ||
                _Transform.gameObject.GetComponent<Nav3DIgnoreObstacleProcessingTag>() != null)
            {
                _TransformImprint = null;
                return false;
            }

            if (_Transform.TryGetComponent(out MeshFilter meshFilter) && meshFilter.sharedMesh.triangles.Length > 0)
            {
                _TransformImprint = "mesh" + GetTransformImprintStr(_Transform);
                return true;
            }

            if (_Transform.TryGetComponent(out Terrain terrain) && terrain.terrainData.size != Vector3.zero)
            {
                _TransformImprint = "terrain" + GetTransformImprintStr(_Transform);
                return true;
            }

            _TransformImprint = null;
            return false;
        }

        string GetTransformImprintStr(Transform _Transform)
        {
            return $"{_Transform.position.ToStringExt()}|{_Transform.rotation.ToStringExt()}|{_Transform.lossyScale.ToStringExt()}|{_Transform.gameObject.name}";
        }

        #endregion

        #region Unity events

        private void Awake()
        {
            MarkTransformHasChanged();

            if (!Application.isPlaying || !enabled)
                return;

            m_Loader = FindObjectOfType<Nav3DObstacleLoader>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || !enabled)
                return;
#endif
            if (m_ObstacleDataSource != ObstacleDataSource.RUNTIME_PROCESSING)
                return;

            if (m_AutoUpdateIfChanged && CachedTransform.hasChanged && (DateTime.UtcNow - m_LastAutoUpdateTime).TotalSeconds > m_AutoUpdateMinPeriod)
            {
                UpdateObstacle();
                CachedTransform.hasChanged = false;
            }
        }

        private void OnEnable()
        {
            Init();

#if UNITY_EDITOR
            if (!Application.isPlaying || !enabled)
                return;
#endif
            if (m_ObstacleDataSource == ObstacleDataSource.LOAD_FROM_BINARY)
            {
                Debug.LogWarning(ADD_REMOVAL_UNAVALABLE_TIP);
                return;
            }

            if (m_ObstacleDataSource == ObstacleDataSource.RUNTIME_PROCESSING)
            {
                Nav3DManager.OnNav3DInit += () =>
                {
                    if (ObstacleManager.Doomed)
                        return;

                    PrepareObstacleInfos(out _);

                    ProcessObstacleRuntime();
                };
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || !enabled)
                return;
#endif
            if (m_ObstacleDataSource == ObstacleDataSource.LOAD_FROM_BINARY)
            {
                Debug.LogWarning(ADD_REMOVAL_UNAVALABLE_TIP);
                return;
            }

            if (m_ObstacleDataSource == ObstacleDataSource.RUNTIME_PROCESSING)
                RemoveObstacle();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            if (m_GizmosData == null)
                m_GizmosData = new Common.Debug.GizmosDrawData();

            if (m_DrawOccupiedLeaves || m_DrawFreeLeaves || m_DrawGraph)
            {
                if (m_GizmosData.IsClean)
                {
                    //add data
                    ObstacleManager.Instance.FillGizmosDrawData(
                        m_GizmosData,
                        m_InstanceID,
                        m_DrawOccupiedLeaves,
                        m_DrawFreeLeaves,
                        m_DrawGraph,
                        m_OccupiedLeavesDrawingMode,
                        m_OccupiedLeavesDrawingLayerNumber,
                        m_FreeLeavesDrawingMode,
                        m_FreeLeavesDrawingLayerNumber,
                        m_GraphNodesDrawingMode,
                        m_GraphNodesDrawingLayerNumber
                        );
                }
                else
                {
                    m_GizmosData.Draw();
                }
            }
        }
#endif
        #endregion
    }
}
