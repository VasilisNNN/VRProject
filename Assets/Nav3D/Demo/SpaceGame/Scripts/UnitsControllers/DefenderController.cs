using Nav3D.API;
using Nav3D.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nav3D.Demo
{
    [RequireComponent(typeof(Nav3DAgent))]
    public class DefenderController : MonoBehaviour
    {
        #region Constants

        const float ENEMY_DETECTION_DISTANCE = 3f;
        const float ENEMIES_CHECK_PERIOD = 0.4f;

        #endregion

        #region Attributes

        Nav3DAgent m_Agent;

        int m_CurWaypointIndex;
        int m_WaypointsCount;
        List<Transform> m_Waypoints;

        Coroutine m_EnemiesCheckRoutine;

        #endregion

        #region Public methods

        //init defender with waypoints list, start waypoint and radar prefab
        public void Init(Transform[] _Waypoints, int _StartIndex)
        {
            m_Waypoints = _Waypoints.ToList();
            m_CurWaypointIndex = _StartIndex;
            m_WaypointsCount = m_Waypoints.Count;

            int preIndex = m_CurWaypointIndex - 1 < 0 ? m_Waypoints.Count - 1 : m_CurWaypointIndex - 1;

            transform.position = m_Waypoints[preIndex].position;

            MoveToNextWaypoint();

            StartCheckEnemiesRoutine();
        }

        #endregion

        #region Service methods

        //looped game logic
        void MoveToNextWaypoint()
        {
            if (m_CurWaypointIndex < m_WaypointsCount)
            {
                //move to next waypoint from list
                m_Agent.MoveTo(m_Waypoints[m_CurWaypointIndex].position, MoveToNextWaypoint, _ => MoveToNextWaypoint());
                m_CurWaypointIndex++;
            }
            else
            {
                //if reached the last waypoint, then move to first one
                m_CurWaypointIndex = 0;
                MoveToNextWaypoint();
            }
        }

        void StartCheckEnemiesRoutine()
        {
            if (m_EnemiesCheckRoutine != null)
                StopCoroutine(m_EnemiesCheckRoutine);

            m_EnemiesCheckRoutine = StartCoroutine(CheckEnemiesRoutine());
        }

        void ContinueWaypointsFollowing()
        {
            //find closest waypoint
            Transform closest = m_Waypoints.MinBy(_Waypoint => (_Waypoint.position - m_Agent.Transform.position).sqrMagnitude);
            m_CurWaypointIndex = m_Waypoints.IndexOf(closest);

            //start move to it
            m_Agent.MoveTo(closest.position, MoveToNextWaypoint, _ => MoveToNextWaypoint());

            //start enemies check routine
            StartCheckEnemiesRoutine();
        }

        void FollowRandomWaypointSet()
        {
            //move to random waypoints list
            //after that start moving to the closet waypoint
            m_Agent.MoveToEnqueue(m_Waypoints[Random.Range(0, m_Waypoints.Count)].position);
            m_Agent.MoveToEnqueue(m_Waypoints[Random.Range(0, m_Waypoints.Count)].position);
            m_Agent.MoveToEnqueue(m_Waypoints[Random.Range(0, m_Waypoints.Count)].position, ContinueWaypointsFollowing);

            StartCheckEnemiesRoutine();
        }

        //detect the closest enemy and start pursuing it
        IEnumerator CheckEnemiesRoutine()
        {
            while (true)
            {
                List<Nav3DAgent> agents = m_Agent.GetAgentsInRadius(ENEMY_DETECTION_DISTANCE);
                List<EnemyController> enemies = new List<EnemyController>();

                //get nearest enemy agents
                //the flag 'IsVictim' means that agent has not any pursuer now
                foreach (Nav3DAgent agent in agents)
                {
                    EnemyController enemyController = agent.GetComponent<EnemyController>();
                    if (enemyController != null && !enemyController.IsVictim)
                        enemies.Add(enemyController);
                }

                if (enemies.Any())
                {
                    //get nearest enemy from list
                    EnemyController enemy = enemies.MinBy(_Enemy => (_Enemy.transform.position - m_Agent.Transform.position).sqrMagnitude);
                    //then mark it like victim
                    enemy!.MarkAsVictim();

                    //start pursuing enemy victim
                    //update path to victim if it was moved from the last position by more then 0.125
                    m_Agent.FollowTarget(
                        enemy!.transform,
                        0.125f,
                        m_Agent.Description.Radius * 1.5f,
                        //on reach destroy the enemy.
                        //then make random decision to start move to the current waypoint or to start move to the random waypoints list
                        () =>
                        {
                            if (enemy != null)
                                enemy.Destroy();

                            if (Random.Range(0, 2) == 0)
                                ContinueWaypointsFollowing();
                            else
                                FollowRandomWaypointSet();
                        },
                        //if something goes wrong, start move to the closest waypoint 
                        ContinueWaypointsFollowing,
                        _ => ContinueWaypointsFollowing()
                    );

                    yield break;
                }

                yield return new WaitForSeconds(ENEMIES_CHECK_PERIOD);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        #endregion

        #region Unity events

        void Awake()
        {
            m_Agent = GetComponent<Nav3DAgent>();
        }

        #endregion
    }
}