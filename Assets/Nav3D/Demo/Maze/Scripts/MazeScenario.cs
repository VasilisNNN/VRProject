using Nav3D.API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Nav3D.Obstacles;

namespace Nav3D.Demo
{
    public class MazeScenario : MonoBehaviour
    {
        #region Serialized fields

        [Space, Header("GameObjects")]
        [SerializeField] Nav3DObstacle m_Obstacle;
        [SerializeField] Transform m_PointATransform;
        [SerializeField] Transform m_PointBTransform;

        [Space, Header("Positions")]
        [SerializeField] List<Transform> m_APositions = new List<Transform>();
        [SerializeField] List<Transform> m_BPositions = new List<Transform>();

        [Space, Header("Debug drawing")]
        [SerializeField] bool m_DrawResolutionRegions;
        [SerializeField] bool m_DrawOccupiedLeaves;
        [SerializeField] int m_FreeLeavesLayerNumber;
        [SerializeField] bool m_DrawGraph;

        [Space, Header("UI")]
        [SerializeField] Text m_LoaderText;
        [SerializeField] Button m_MoveRedButton;
        [SerializeField] Button m_MoveGreenButton;

        #endregion

        #region Attributes

        bool m_PathInited = false;

        int m_APosIndex = 0;
        int m_BPosIndex = 0;

        Vector3 m_PointAPrePos;
        Vector3 m_PointBPrePos;
        LineRenderer m_PathRenderer;
        Path m_Path;

        #endregion

        #region Unity events

        void Start()
        {
            m_PointATransform.position = m_APositions.First().position;
            m_PointBTransform.position = m_BPositions.First().position;

            BindUIHandlers();

            m_PathRenderer = GetComponent<LineRenderer>();

            m_PointAPrePos = m_PointATransform.position;
            m_PointBPrePos = m_PointBTransform.position;

            //process obstacles group, create path on succeed
            m_Obstacle.OnObstacleAdded += () =>
            {
                m_LoaderText.gameObject.SetActive(false);

                Debug.Log(m_Obstacle.AdditionProgress.GetResultStats());
            };

            Nav3DManager.OnNav3DInit += ()=>
            {
                InitPath();
                UpdatePath();
            };
        }

        void Update()
        {
            CheckPointsMoved();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled)
                return;

            if (m_DrawResolutionRegions)
                ObstacleParticularResolutionManager.Instance.Draw();
        }
#endif

#endregion

        #region Service methods

        void InitPath()
        {
            if (m_PathInited)
                return;

            //prefetch path instance and set smoothing
            m_Path = Nav3DPathfindingManager.PrefetchPath(m_PointAPrePos, m_PointBPrePos);
            m_Path.Smooth = true;
            m_Path.OnPathUpdated += OnPathUpdated;

            m_PathInited = true;
        }

        void OnPathUpdated()
        {
            m_PathRenderer.positionCount = m_Path.TrajectorySmoothed.Length;
            m_PathRenderer.SetPositions(m_Path.TrajectorySmoothed);
        }

        void UpdatePath()
        {
            //request path updating and rebuild path renderer on succeed
            m_Path?.UpdatePath(
                m_PointAPrePos,
                m_PointBPrePos,
                9000,
                true,
                _OnSuccess: () =>
                {
                    m_PathRenderer.positionCount = m_Path.TrajectorySmoothed.Length;
                    m_PathRenderer.SetPositions(m_Path.TrajectorySmoothed);
                },
                _OnFail: _Error => Debug.LogError(_Error.Msg)
            );
        }

        //if some point changed its position, then update the path
        void CheckPointsMoved()
        {
            if (m_PointAPrePos != m_PointATransform.position || m_PointBPrePos != m_PointBTransform.position)
            {
                m_PointAPrePos = m_PointATransform.position;
                m_PointBPrePos = m_PointBTransform.position;

                UpdatePath();
            }
        }

        void BindUIHandlers()
        {
            m_MoveRedButton.onClick.AddListener(MoveRed);
            m_MoveGreenButton.onClick.AddListener(MoveGreen);
        }

        #endregion

        #region UI handlers

        //moves red point to next position 
        void MoveRed()
        {
            m_APosIndex++;

            if (m_APosIndex >= m_APositions.Count)
                m_APosIndex = 0;

            m_PointATransform.position = m_APositions[m_APosIndex].position;

            CheckPointsMoved();
        }

        //moves green point to next position
        void MoveGreen()
        {
            m_BPosIndex++;

            if (m_BPosIndex >= m_BPositions.Count)
                m_BPosIndex = 0;

            m_PointBTransform.position = m_BPositions[m_BPosIndex].position;

            CheckPointsMoved();
        }

        #endregion
    }
}