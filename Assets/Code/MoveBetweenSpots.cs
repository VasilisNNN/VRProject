using Nav3D.LocalAvoidance;
using Nav3D.LocalAvoidance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveBetweenSpots : MonoBehaviour
{
    public Transform[] waypoints;

    public float speed = 3f; // Speed of movement

    public float attackspeedbuff = 1.8f; // Speed of movement
    public float StopDistance = 0; 

    private float attackincrease =2;
    public float rotationSpeed = 2f; // Speed of rotation
    public float obstacleAvoidanceDistance = 2f; // Distance to detect obstacles
    [HideInInspector]
    private int currentWaypointIndex = 0; // Index of the current waypoint


    private bool Walk;
    public bool Attacking { get; set; }

    public float speedBuff { get; set; }
    private float slowdown { get; set; }
    private float slowdownCount { get; set; }
    private float StunTimer;
    private float DelayBetweenHits;

    private Player pl;

    private Vector3 avoidDirection = Vector3.zero;
    private    RaycastHit hit;
    public float BodyWidth = 4;
    public NavMeshAgent Agent { get; private set; }
    
    public Transform targetWaypoint;
    public Transform targetWaypointRotation;
    private float SetNexPointTimer;

    public Vector3 NavMeshAgentVelocity { get; set; }

    public bool Dashing { get; set; }
    public Vector3 DashingPosition;
    public bool IgnorePause;
    private void Start()
    {
   
        pl = InitializeOnAwake.pl;
        Agent = GetComponent<NavMeshAgent>();


        Resume();
        speedBuff = 1;

    }

    void Update()
    {

        if (pl._Menu.MenuONOFF || pl._gameover || ((pl.InDialog || pl.inv.showinvent || pl.inv.showjournal) && !IgnorePause))
        {
           
                if (Agent != null && Agent.isOnNavMesh)
                    Agent.isStopped = true;

                return;
            
        }


        if (Mathf.Abs(transform.position.x-pl._transform.position.x) > pl.UpdateRange || 
            Mathf.Abs(transform.position.z- pl._transform.position.z) > pl.UpdateRange) return;


        SlowDownAndStun();
        if (Agent != null)
        {
           
            MoveAgentObject();
            return;
        }
        

      
    }

    void AttackMultiplierCalculation()
    {

        if (!isWalking())
        {
            attackincrease = 1;
            return;
        }

        if (Attacking)
            attackincrease = attackspeedbuff;
        else attackincrease = 1;
        
        
    }



    void MoveAgentObject()
    {

        if (Agent == null) return;


        if (Dashing)
        {
            if (Mathf.Abs(transform.position.x - targetWaypoint.transform.position.x) > 0.1 ||
                Mathf.Abs(transform.position.z - targetWaypoint.transform.position.z) > 0.1)
            {
                Agent.SetDestination(DashingPosition);
                Agent.speed=10;


                return;
            }

        }


        NavMeshAgentVelocity = Agent.velocity;
      

        if (!Attacking)
        {
            if (waypoints.Length > 0)
            {
                targetWaypoint = waypoints[currentWaypointIndex];
                targetWaypointRotation = waypoints[currentWaypointIndex];
            }
        }
        else currentWaypointIndex = 0;

        // Agent.stoppingDistance = 1;

        if (targetWaypoint == null) return;
        if (Agent != null)
        {
         if(Agent.isOnNavMesh)
            Agent.SetDestination(targetWaypoint.position);
        }  
        
        AttackMultiplierCalculation();



        if (Agent != null)
        {
         
                Agent.speed = speed * attackincrease * speedBuff * slowdown;

                Agent.stoppingDistance = StopDistance;



                if (isWalking())
                {

                    Agent.isStopped = false;

                    if (targetWaypoint != null)
                    {
                        if (targetWaypointRotation == null) targetWaypointRotation = targetWaypoint;
                        RotateTowards(targetWaypointRotation.position);
                    }
                }
                else Agent.isStopped = true;
            
        }
        if (SetNexPointTimer < Time.fixedTime && Mathf.Abs(Vector3.Distance(transform.position, new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z))) < 0.5f)
        {
            if (waypoints.Length <= 1)
            {
                if (!Attacking)
                {
                    Stop();
               
                }
            }
            else
            {


                if (currentWaypointIndex < waypoints.Length - 1)
                    currentWaypointIndex++;
                else currentWaypointIndex = 0;


                SetNexPointTimer = Time.fixedTime + 0.5f;
            }
        }

    }


   
   public void RotateTowards(Vector3 targetPosition)
    {
        // Calculate the direction to the target
        Vector3 direction = targetPosition - transform.position;

        // Calculate the rotation step
        float step = rotationSpeed * Time.deltaTime;

        // Rotate towards the target
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    bool CheckRay(Vector3 StartPos, Vector3 _direction, out RaycastHit hit, LayerMask mask)
    {
        if (Physics.Raycast(StartPos, _direction, out hit, obstacleAvoidanceDistance, mask))
        {
            
            avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            _direction = Vector3.RotateTowards(_direction, avoidDirection, Mathf.PI, 0.0f);

            return true;
        }
        else
        {
          
           
            return false;
        }
    }



    private void SlowDownAndStun()
    {
        if (slowdownCount >= 3)
        {
            Stun();
        }

        if (StunTimer > 0)
        {
            if (StunTimer < Time.deltaTime * 2)
                Resume();

                StunTimer -= Time.deltaTime;
      
            slowdown = 0;
        }
        else
            slowdown += Time.deltaTime;


        if (DelayBetweenHits > 0) DelayBetweenHits -= Time.deltaTime;
        else slowdownCount = 0;

        if (slowdown > 1) slowdown = 1;


    }



    public void ResetAllSpeed()
    {
        attackincrease = 1;
        speedBuff = 1;
        slowdown = 1;
    }


    public void AddSlowdown(int slow)
    {
      
        if(slowdown >= 0.4f)
        slowdown -= 0.2f;



        DelayBetweenHits = 3;
        slowdownCount += slow;


        print("AddSlowdown " + slowdownCount);
    }


    public void Stop()
    {
        if(Agent!=null)
        Agent.velocity = Vector3.zero;
        
        Walk = false;

    }



    public void Resume()
    {
        if (waypoints.Length <= 0 && !Attacking)
        {
            Walk = false;
            return;
        }


        Walk = true;
      
    }

    public void Stun()
    {
        slowdownCount = 0;
        StunTimer = 1.5f;


        Stop();

    }



    public bool isStunned()
    {
        if (StunTimer > 0) return true;

        return false;

    }


    public bool isWalking()
    {
        if(StunTimer>0) return false;
        if(waypoints.Length<=0) return Walk;

        if(targetWaypoint == null) return false;

        if (Mathf.Abs(targetWaypoint.position.x - transform.position.x) <= 0 &&
            Mathf.Abs(targetWaypoint.position.y - transform.position.y) <= 0 &&
            Mathf.Abs(targetWaypoint.position.z - transform.position.z) <= 0)
        {
         
            return false;
        }

        return Walk;

    }


}
