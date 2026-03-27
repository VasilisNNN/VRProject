using Nav3D.Obstacles;
using System.Collections.Generic;
using UnityEngine;
using Nav3D.API;

namespace Nav3D.Common
{
    public static class ExtensionTransform
    {
        #region Constants

        readonly static string MESH_ERROR = $"The obstacle transform (name: {{0}}, InstanceID: {{1}}) has no {nameof(MeshFilter)} component, or it's mesh has no any triangles";

        #endregion

        #region Public methods

        //Here we assume that each obstacle contains either a mesh or a terrain, and not both.
        public static bool TryGetObstacleInfo(this Transform _Transform, Nav3DObstacle _ObstacleRootController, out ObstacleInfoSingle _AdditionData)
        {
            bool result;

            if (result = GetObstacleMeshData(_Transform, _ObstacleRootController, out _AdditionData))
                return result;

            if (result = GetObstacleTerrainData(_Transform, _ObstacleRootController, out _AdditionData))
                return result;

            UnityEngine.Debug.LogWarning(string.Format(MESH_ERROR, _Transform.name, _Transform.GetInstanceID()));

            return false;
        }

        public static List<Transform> GetAllChilds(this Transform _Root, bool _CheckActive = true)
        {
            List<Transform> transformList = new List<Transform>();

            GetAllChilds(_Root, transformList, _CheckActive);

            return transformList;
        }

        #endregion

        #region Service methods

        static bool GetObstacleMeshData(this Transform _Transform, Nav3DObstacle _ObstacleRootController, out ObstacleInfoSingle _AdditionData)
        {
            if (!_Transform.TryGetComponent(out MeshFilter meshFilter) || meshFilter.sharedMesh.triangles.Length == 0)
            {
                _AdditionData = null;

                return false;
            }

            Mesh obstacleMesh = meshFilter.sharedMesh;

            _AdditionData = new ObstacleInfoMesh
            (
                _ObstacleRootController.InstanceID,
                obstacleMesh.vertices,
                obstacleMesh.triangles,
                _Transform.position,
                _Transform.lossyScale,
                _Transform.rotation
            );

            return true;
        }

        static bool GetObstacleTerrainData(this Transform _Transform, Nav3DObstacle _ObstacleRootController, out ObstacleInfoSingle _AdditionData)
        {
            if (!_Transform.TryGetComponent(out Terrain terrain))
            {
                _AdditionData = null;
                return false;
            }

            TerrainData terrainData = terrain.terrainData;
            int heightMapResolution = terrainData.heightmapResolution;

            _AdditionData = new ObstacleInfoTerrain(
                _ObstacleRootController.InstanceID,
                terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution),
                heightMapResolution,
                terrainData.size,
                _Transform.position);

            return true;
        }

        //Parses transform tree. Resulting list contains root transform and all nested childs.
        static void GetAllChilds(Transform _Transform, List<Transform> _Childs, bool _CheckActive)
        {
            if (_CheckActive && !_Transform.gameObject.activeInHierarchy)
                return;

            if (_Childs != null)
                _Childs.Add(_Transform);
            else
                _Childs = new List<Transform> { _Transform };

            foreach (Transform child in _Transform)
            {
                GetAllChilds(child, _Childs, _CheckActive);
            }
        }

        #endregion
    }
}