using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Unity.AI.Navigation;
using System;
using System.Net.NetworkInformation;
using System.Linq;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MoveBetweenSpots))]
[RequireComponent(typeof(NavMeshAgent))]


public class AnimState
{
    public bool State;
    public string Name;

    public AnimState(string _name, bool state)
    {
        Name = _name;
        State = state;
    }
}

[System.Serializable]
public class WalkingPart
{

    public Attack.WalkingType WalkType;
    public string AnimName;

    public float Speed;

    public float PartDuration = 2;

}


[System.Serializable]
public class AttackPart
{
    public Attack.AttackType AType;

    [HideInInspector]
    public GameObject Effect;

    public GameObject EffectPrefab;
    public GameObject AttackColl;

    public float AttackDuration = 2;

    public int AttackDamage = 1;
    public float AttackRange = 1;
    public AudioClip AttackClip;
    public AttackPart(Attack.AttackType aType, GameObject effectPrefab)
    {
        AType = aType;
        EffectPrefab = effectPrefab;
    }
}

public class Attack : MonoBehaviour
{

    public enum WalkingType { Normal, Forward };

    public enum AttackType { Dashing, JumpForward, Punch, Swing, Jumpback, ShootingStraight, ShootingThree, ShootingCircle, ShootLine, Lazers, SpawnEnemies, FollowBullet, Hide };


    public List<AttackPart> _Attacktype;
    public List<WalkingPart> _WalkingParts;

    private Player pl;
    private MoveBetweenSpots MoveBSpots;
    private NavMeshAgent NavAgent;
    private Rigidbody _Rigidbody;
    public AudioClip StartAttackSound;
    private AudioSource AS;

    public LayerMask AttackRayCheckMask;
    public float ChasingRange = 60;
    public float ChasingTimeDelay = 4;

    public float ResetAttackDelay = 4;

    private float ChasingTime;
  

    private float NoAttackDelay, StandupTimer, WalkTimer, PrevPosTimer, AttackDuration, DelayInAttack, DelayBetweenAttacks, AttackBuildup, StartAttackingTimer, WalkingDelay, PositionChangedTimer;


    private Animator Anim;

    private Transform _transform;
    public Transform Target { get; set; }

    private Transform Jump_transform, WalkForward_transform;

    private bool Jumping;

    private int AttackNum;
    public float buildupduration = 0.1f;
    public float DelayBetweenAttacks_MAX = 5;
    public float LyingDuration = 3;
    public float StartDelay = 0;


    [HideInInspector]
    public List<GameObject> Bullets = new List<GameObject>();
    [HideInInspector]
    private List<GameObject> Enemies = new List<GameObject>();
    [HideInInspector]
    private List<Vector3> BulletsSpeeds = new List<Vector3>();


    private List<AnimState> AnimatorStates = new List<AnimState>();

   
    private int CurrentWalkingState;

    public bool Lying { get; private set; }

    public bool Boss;
    public bool IgnoreUpdateRange;

    private CollList BackRay;

    private Vector3 PositionChanged, PositionChangedPrev;

    private Transform MoveToTransform;
    private Vector3 PrevPos;

    public bool CanBeThroughOnTheGround;
    [HideInInspector]
    public bool attacking;
    private string CurrentAnimState;

    public enum AttackState { Buildup, Attack, PauseAfterAttack, NoAttack};
    private AttackState _attackState;

    public StatsControll _Stats;
    [HideInInspector]
    public Transform StartPoint;
    public List<GameObject> rayhit = new List<GameObject>();

    private Enemies EnemiesSpawner;
    void Start()
    {
        StartAttackingTimer = StartDelay;
        EnemiesSpawner = GetComponent<Enemies>();
        AttackRayCheckMask.value = 577;

        StartPoint = Instantiate(new GameObject()).transform;
        StartPoint.name = "StartPoint";
        StartPoint.position = transform.position;

        _Rigidbody = GetComponent<Rigidbody>();
        _Stats = GetComponent<StatsControll>();


        _attackState = AttackState.NoAttack;
      
        GameObject TrGO = Instantiate<GameObject>(new GameObject());
        TrGO.name = name + "MoveToTransform";

        MoveToTransform = TrGO.GetComponent<Transform>();


        Anim = GetComponent<Animator>();
        MoveBSpots = GetComponent<MoveBetweenSpots>();
        NavAgent = GetComponent<NavMeshAgent>();
        AS = GetComponent<AudioSource>();

        pl = InitializeOnAwake.pl;
        AttackDuration = -1;

        _transform = transform;

        Jump_transform = Instantiate(new GameObject().transform);
        Jump_transform.name = "Jump_transform";

        WalkForward_transform = Instantiate(new GameObject().transform);
        WalkForward_transform.name = "WalkForward_transform";

        if (_transform.Find("BackRay") != null)
            BackRay = _transform.Find("BackRay").GetComponent<CollList>();

        Target = pl.transform;

        DelayBetweenAttacks = DelayBetweenAttacks_MAX;



        AnimatorStates.Add(new AnimState("StartTransition", false));
        AnimatorStates.Add(new AnimState("Standing", false));

        AnimatorStates.Add(new AnimState("Walking", false));
        AnimatorStates.Add(new AnimState("Walking_1", false));
        AnimatorStates.Add(new AnimState("Running", false));
        AnimatorStates.Add(new AnimState("SideRide", false));
        AnimatorStates.Add(new AnimState("BlockEyes", false));

        AnimatorStates.Add(new AnimState("Dashing", false));
        AnimatorStates.Add(new AnimState("DashingBuildup", false));
        AnimatorStates.Add(new AnimState("Hide", false));
        AnimatorStates.Add(new AnimState("HideBuildup", false));

        AnimatorStates.Add(new AnimState("DropRocks", false));
        AnimatorStates.Add(new AnimState("Punch", false));
        AnimatorStates.Add(new AnimState("Swing", false));
        AnimatorStates.Add(new AnimState("DropRocksBuildup", false));
        AnimatorStates.Add(new AnimState("FollowBullet", false));
        AnimatorStates.Add(new AnimState("FollowBulletBuildup", false));
        AnimatorStates.Add(new AnimState("ElectroSphere", false));
        AnimatorStates.Add(new AnimState("ElectroSphereBuildup", false));
        AnimatorStates.Add(new AnimState("GrowingCircle", false));
        AnimatorStates.Add(new AnimState("GrowingCircleBuildup", false));
        AnimatorStates.Add(new AnimState("Lazers", false));
        AnimatorStates.Add(new AnimState("LazersBuildup", false));
        AnimatorStates.Add(new AnimState("Volcano", false));
        AnimatorStates.Add(new AnimState("VolcanoBuildup", false));
        AnimatorStates.Add(new AnimState("SpawnEnemies", false));
        AnimatorStates.Add(new AnimState("SpawnEnemiesBuildup", false));
        AnimatorStates.Add(new AnimState("Screaming", false));
        AnimatorStates.Add(new AnimState("ScreamingBuildup", false));
        AnimatorStates.Add(new AnimState("ShootingStraight", false));
        AnimatorStates.Add(new AnimState("ShootingStraightBuildup", false));
        AnimatorStates.Add(new AnimState("Shooting3", false));
        AnimatorStates.Add(new AnimState("Shooting3Buildup", false));
        AnimatorStates.Add(new AnimState("ShootingCircle", false));
        AnimatorStates.Add(new AnimState("ShootingCircleBuildup", false));
        AnimatorStates.Add(new AnimState("Lying", false));
        AnimatorStates.Add(new AnimState("Standup", false));
        AnimatorStates.Add(new AnimState("PauseAfterAttack", false));

    }

    private void Update()
    {




        if (ChasingTime > 0)
            ChasingTime -= Time.deltaTime * pl.Game_SPEED;




        attacking = MoveBSpots.Attacking;

        if (pl.PlayerPause())
        {
            
            Anim.speed = 0;
            StopWalking();
            return;
        }


        ResetAttackAfterTime();

        MovementStartStop();

        //  Stunned();


        if (WalkingDelay > 0)
            WalkingDelay -= Time.deltaTime * pl.Game_SPEED;

        if (AttackDuration > 0)
            AttackDuration -= Time.deltaTime * pl.Game_SPEED;


        if (DelayInAttack > 0)
            DelayInAttack -= Time.deltaTime * pl.Game_SPEED;

        if (DelayBetweenAttacks > 0 && !Lying)
            DelayBetweenAttacks -= Time.deltaTime * pl.Game_SPEED;

        if (AttackBuildup > 0 && !Lying)
            AttackBuildup -= Time.deltaTime * pl.Game_SPEED;

        if (StartAttackingTimer >= 0)
            StartAttackingTimer -= Time.deltaTime * pl.Game_SPEED;



        if (AttackDuration <= 0 && DelayBetweenAttacks <= 0)
        {
            MoveBSpots.Resume();
        }



        DamagePlayer();


        AttackStateControl();

        WalkingList(CurrentWalkingState);
        Animations();

        StopDistance();
        CorrectPosition();

        // BulletControll();

        if (NavAgent != null)
        {

           
            Vector3 PosNormalize = (pl.transform.position - transform.position).normalized;
       
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + pl.GetComponent<CapsuleCollider>().height / 2, transform.position.z), new Vector3(PosNormalize.x * 100f, PosNormalize.y, PosNormalize.z * 100f), new Color(1, 1, 1, 1),1f);
        }
    }


    void AttackStateControl()
    {
        /*if (!MoveBSpots.Attacking)
        {
            StopAttacking();
            return;
        }*/

        if (_Rigidbody != null)
        {

             _Rigidbody.isKinematic = false;
        }


        if (StartAttackingTimer > 0) return;
        if (_attackState == AttackState.NoAttack)
        {
            if (_attackState != AttackState.Buildup)
            {
          
                Buildups();
            }
        }
        else if (_attackState == AttackState.Buildup)
        {
            if (AttackBuildup <= 0 && AttackDuration <= 0 && _attackState != AttackState.Attack)
            {
              
                AttackList();
              
                _attackState = AttackState.Attack;
            }
        }
        else if (_attackState == AttackState.Attack)
        {
            if (_attackState != AttackState.PauseAfterAttack)
            {
             
                StartAttackPause();

            }

        }
        else if (_attackState == AttackState.PauseAfterAttack)
        {
            if (AttackBuildup <= 0 && AttackDuration <= 0 && DelayBetweenAttacks <= 0 && _attackState != AttackState.NoAttack)
            {
                print(name + " NoAttack");
                _attackState = AttackState.NoAttack;
            }

        }


    }



    void AttackList()
    {


        if (StartAttackingTimer > 0) return;

        for (int i = 0; i < Bullets.Count; i++)
        {
            if (Bullets[i] != null)
            {
                // pl.BlowThisSmall(Bullets[i]);

            }
        }

        Bullets = new List<GameObject>();
        BulletsSpeeds = new List<Vector3>();




        if (!MoveBSpots.Attacking || Lying)
            return;




        AS.clip = _Attacktype[AttackNum].AttackClip;
        if (!AS.isPlaying)
            AS.Play();



        if (_Attacktype[AttackNum].AType == AttackType.Dashing)
            DashAttack();

        if (_Attacktype[AttackNum].AType == AttackType.Punch)
            PunchAttack();


        if (_Attacktype[AttackNum].AType == AttackType.Swing)
            SwingAttack();

        if (_Attacktype[AttackNum].AType == AttackType.ShootingStraight)
            ShootAttack();

        if (_Attacktype[AttackNum].AType == AttackType.JumpForward)
            JumpForwardAttack();

        if (_Attacktype[AttackNum].AType == AttackType.Hide)
            HideAttack();

        if (_Attacktype[AttackNum].AType == AttackType.SpawnEnemies)
            SpawnEnemies();


        if (_Attacktype[AttackNum].AType == AttackType.Lazers)
            ShootLazers();


        /*if (_Attacktype[AttackNum].AType == AttackType.ShootingThree)
            ShootAttackThree();

        if (_Attacktype[AttackNum].AType == AttackType.ShootingCircle)
            ShootCircleAttack();
        

    

      
        
        if (_Attacktype[AttackNum].AType == AttackType.FollowBullet)
            ShootFollowBullet();

        if (_Attacktype[AttackNum].AType == AttackType.ShootLine)
        {
            ShootLineBullets();

        }*/

        WalkingDelay = -1;
        NoAttackDelay = ResetAttackDelay;

        //  SetEffect(_Attacktype[AttackNum].EffectPrefab, ref _Attacktype[AttackNum].Effect);
    }

    void SpawnEnemies()
    {


        if (AttackDuration >= 0) return;

        EnemiesSpawner.enabled = true;
        Jumping = false;

        StartChasing();
        Anim.Play("Hide", 0);
        if (NavAgent != null)
            NavAgent.velocity = Vector3.zero;

        StopWalking();
        AttackDuration = _Attacktype[AttackNum].AttackDuration;

    }



    void HideAttack()
    {
 

        if (AttackDuration >= 0) return;


        Jumping = false;

        StartChasing();
        Anim.Play("Hide", 0);
        if (NavAgent != null)
            NavAgent.velocity = Vector3.zero;

        StopWalking();
        AttackDuration = _Attacktype[AttackNum].AttackDuration;

    }



    void DashAttack()
    {
        if (AttackDuration >= 0) return;

        Jumping = false;

        StartChasing();

        Anim.Play("Attack", 0);

        MoveBSpots.Attacking = true;

        StopWalking();

        MoveBSpots.Dashing = true;
        MoveBSpots.DashingPosition = Target.transform.position;


        AttackDuration = _Attacktype[AttackNum].AttackDuration;
    }



    void PunchAttack()
    {
        print(name + " Punch");

        if (AttackDuration >= 0) return;

        print(name + " Punch 1");

        Jumping = false;

        StartChasing();
        Anim.Play("Punch", 0);
        if (NavAgent != null)
            NavAgent.velocity = Vector3.zero;






        StopWalking();
        AttackDuration = _Attacktype[AttackNum].AttackDuration;

    }

    void SwingAttack()
    {

        if (AttackDuration >= 0) return;

        Jumping = false;
        StartChasing();

        MoveBSpots.Attacking = true;
        StopWalking();
        AttackDuration = _Attacktype[AttackNum].AttackDuration;


    }

    void ShootLazers()
    {

        if (AttackDuration >= 0) return;

        Jumping = false;
        StartChasing();

        MoveBSpots.Attacking = true;
        StopWalking();
        AttackDuration = _Attacktype[AttackNum].AttackDuration;


    }



    void ShootAttack()
    {



        StartChasing();

        // MoveBSpots.RotateTowards(Target.position);



        if (AttackDuration >= 0) return;


        Jumping = false;


       

        if (NavAgent != null)
            NavAgent.velocity = Vector3.zero;


       
        _attackState = AttackState.Attack;

        StopWalking();
        MoveBSpots.Attacking = true;

        AttackDuration = _Attacktype[AttackNum].AttackDuration;
    }




    void JumpAttack()
    {
        if (AttackDuration >= 0) return;

        Jumping = true;

        if (Target == null)
            Target = pl.transform;



        if (_Attacktype[AttackNum].AType == AttackType.Jumpback)
        {
            if (Mathf.Abs(Target.position.x - _transform.position.x) < 0.2f &&
                Mathf.Abs(Target.position.z - _transform.position.z) < 0.2f) JumpAway();
            else StandartAttack();
        }


        MoveBSpots.Attacking = true;
        MoveBSpots.Resume();

        AttackDuration = _Attacktype[AttackNum].AttackDuration;
    }


    void JumpForwardAttack()
    {
        if (AttackDuration >= 0) return;

        Jumping = true;



        Vector3 Point = pl.transform.forward * 10;

        if (IsPointOnNavMesh(Point))
            _transform.position = Point;
        else while (!IsPointOnNavMesh(Point))
            {
                if (!IsPointOutsideNavMesh(Point + pl.transform.forward)) break;
                Point += pl.transform.forward;

            }

        _transform.position = Point;

        MoveBSpots.Attacking = true;
        MoveBSpots.Resume();

        AttackDuration = _Attacktype[AttackNum].AttackDuration;

    }



    void Animations()
    {
       
        if (Anim == null)
        {
            return;
        }

        Anim.speed = pl.Game_SPEED;

        if (MoveBSpots.isStunned() && CanBeThroughOnTheGround)
        {
            ThroughOnTheGround();
            return;
        }

        StandingAndWalking();

        AttackAnimations();


    }
    void SetAnimStateTransition(string _name, float transition)
    {
        if (CurrentAnimState == _name) return;

        for (int i = 0; i < AnimatorStates.Count; i++)
        {
            if (AnimatorStates[i].Name == _name)
            {
                AnimatorStates[i].State = true;
                Anim.CrossFadeInFixedTime(AnimatorStates[i].Name, transition);
                CurrentAnimState = _name;

            }
            else
            {
                AnimatorStates[i].State = false;


            }
        }

    }
    void SetAnimState(string _name)
    {
        if (CurrentAnimState == _name) return;

        for (int i = 0; i < AnimatorStates.Count; i++)
        {
            if (AnimatorStates[i].Name == _name)
            {
                AnimatorStates[i].State = true;
                Anim.Play(AnimatorStates[i].Name, 0);
                CurrentAnimState = _name;

            }
            else
            {
                AnimatorStates[i].State = false;


            }
        }

    }



    void StopDistance()
    {
        if (MoveBSpots.Attacking && Boss)
            MoveBSpots.StopDistance = 0.5f;
        else MoveBSpots.StopDistance = 0;

    }

    void CorrectPosition()
    {
        CalculatePositionChanged();
        EnemyIsOnTop();
        // EnemyIsStuck();


    }


    void EnemyIsOnTop()
    {
        if (Mathf.Abs((_transform.position - Target.transform.position).magnitude) >= 2)
            return;


        if (BackRay == null) return;



        if (BackRay.HitPos.magnitude > 1 && BackRay.HitPos.magnitude < 7)
        {
            if (CheckPositionToJump(BackRay.HitPos))
            {

                _transform.position = BackRay.HitPos / 2.2f;

            }
        }
        else if (CheckPositionToJump(_transform.position + _transform.forward.normalized * -10))
        {

            _transform.position += _transform.forward.normalized * -10;

        }
        else
        if (BackRay.coll_obj.Count <= 0)
        {
            if (CheckPositionToJump(_transform.position + _transform.forward.normalized * -10))
            {

                _transform.position += _transform.forward.normalized * -10;

            }
        }


    }


    void EnemyIsStuck()
    {
        if (NavAgent == null) return;


            if (Mathf.Abs(NavAgent.velocity.magnitude) > 0.1 && Mathf.Abs(PositionChanged.magnitude) < 0.1)
            return;


        NavMeshPath P = new NavMeshPath();
        if (CheckPositionToJump(Target.transform.forward.normalized * 30))
            _transform.position = Target.transform.forward.normalized * 30;

        if (NavAgent.CalculatePath(Target.transform.forward.normalized * 30, P))
            return;

        _transform.position = Target.transform.forward.normalized * 20;

        EnemyIsStuck();
    }


    void CalculatePositionChanged()
    {
        PositionChangedTimer -= Time.deltaTime;
        PositionChanged = _transform.position - PositionChangedPrev;

        if (PositionChangedTimer > 0) return;
        PositionChangedPrev = _transform.position;

        PositionChangedTimer = 2;
    }

    void WalkingList(int RND)
    {
        if (Lying) return;

        if (PrevPosTimer < 0)
        {
            PrevPos = _transform.position;
            PrevPosTimer = 2;
        }

        PrevPosTimer -= Time.deltaTime;

        WalkTimer -= Time.deltaTime;
        if (WalkTimer < -3)
            WalkTimer = 7;

        if (_WalkingParts[RND].Speed > 0)
        {
            if (MoveBSpots.isWalking() && MoveBSpots.Attacking && _attackState == AttackState.NoAttack)
            {
                if (Boss)
                {
                    if (Mathf.Abs((_transform.position - Target.transform.position).magnitude) > 15)
                        MoveBSpots.speedBuff = 10;
                    else
                        MoveBSpots.speedBuff = Mathf.Clamp(pl.desiredVelocity.magnitude, 0.2f, 1) * _WalkingParts[RND].Speed;
                }
                else
                {
                    MoveBSpots.speedBuff = Mathf.Clamp(pl.desiredVelocity.magnitude, 0.2f, 1) * _WalkingParts[RND].Speed;

                }

               // SetAnimStateTransition("Running", 0.2f);

                return;

            }
        }

        if (_attackState == AttackState.Buildup || AttackDuration > 0 || StartAttackingTimer > 0 || DelayBetweenAttacks > 0)
            return;

     
        if (Mathf.Abs(MoveBSpots.NavMeshAgentVelocity.magnitude) <= 0) return;

        if (Boss)
        {
            if (Mathf.Abs((_transform.position - Target.transform.position).magnitude) > 15)
                MoveBSpots.speedBuff = 10;
            else
                MoveBSpots.speedBuff = Mathf.Clamp(pl.desiredVelocity.magnitude, 0.2f, 1) * _WalkingParts[RND].Speed;
        }
        else
        {
            MoveBSpots.speedBuff = Mathf.Clamp(pl.desiredVelocity.magnitude, 0.2f, 1) * _WalkingParts[RND].Speed;

        }

        if (_WalkingParts[RND].WalkType == Attack.WalkingType.Normal)
        {
           


                if (Boss)
                {

                    if (WalkTimer > 0)
                    {
                        StartChasing();
                     
                    }
                    else
                    {
                        MoveToTransform.position = new Vector3(
                            Mathf.Clamp((pl._transform.position.x - _transform.position.x) * 4, -10, 10),
                            _transform.position.y,
                            Mathf.Clamp((pl._transform.position.z - _transform.position.z) * 4, -10, 10));

                        if (!IsPointOutsideNavMesh(MoveToTransform.position)) MoveToTransform.position = PrevPos;

                        Target = MoveToTransform;
                        MoveBSpots.targetWaypoint = MoveToTransform;


                    }


                }
                else
                {
                    
                    StartChasing();
                }

            


        }


        if (_WalkingParts[RND].WalkType == Attack.WalkingType.Forward)
        {
            if (pl.OnStraightRoad)
            {
                Vector3 Piont = pl.transform.position + pl.MainCamera.transform.forward.normalized * 20;

                print("WalkingList Forward " + pl.MainCamera.transform.forward.normalized);

                if (IsPointOnNavMesh(Piont))
                    WalkForward_transform.position = new Vector3(Piont.x, transform.position.y, Piont.z);

                /*else while (!IsPointOnNavMesh(Piont))
                    {
                        if (!IsPointOutsideNavMesh(Piont*1.1f)) break;
                        Piont *= 1.1f;

                    }*/

                MoveBSpots.targetWaypointRotation = pl.transform;


                Target = WalkForward_transform;
            }
            else
            {
                StartChasing();

            }



            MoveBSpots.targetWaypoint = Target;

        }


    }




    public void StartChasing()
    {

        if (!RayCheck()) return;

            if (_attackState == AttackState.Attack || _attackState == AttackState.PauseAfterAttack) return;

    

        Jumping = false;

        Target = pl.transform;

        MoveBSpots.targetWaypoint = Target;
        MoveBSpots.targetWaypointRotation = Target;

        if (!MoveBSpots.Attacking)
        {
            AS.clip = StartAttackSound;

            if (!AS.isPlaying)
                AS.Play();
        }

        if (!pl.EnemiesAttacking.Contains(gameObject))
            pl.EnemiesAttacking.Add(gameObject);

        if (!pl.BossesAttacking.Contains(gameObject) && Boss)
            pl.BossesAttacking.Add(gameObject);

        if (!MoveBSpots.Attacking)
            ResetCurrentAttackNumber();



        WalkTimer = 6;
        MoveBSpots.Attacking = true;
        MoveBSpots.Resume();
        ChasingTime = ChasingTimeDelay;


    }


    void StandartAttack()
    {
        AS.clip = StartAttackSound;

        if (!AS.isPlaying)
            AS.Play();


        AttackNum = UnityEngine.Random.Range(0, _Attacktype.Count);
        NoAttackDelay = ResetAttackDelay;
        MoveBSpots.targetWaypoint = Target;

        Anim.Play("Attack", 0);
    }


    void ResetAttackAfterTime()
    {
        NoAttackDelay -= Time.deltaTime;

        if (NoAttackDelay < 0)
        {
            AttackNum = UnityEngine.Random.Range(0, _Attacktype.Count);
            NoAttackDelay = ResetAttackDelay;
        }

    }

    void JumpAway()
    {
        if (_attackState == AttackState.PauseAfterAttack) return;

        Jump_transform.position = new Vector3(
        Mathf.Clamp((Target.position - _transform.position).normalized.x * 3, -3, 3),
        _transform.position.y - 1,
        Mathf.Clamp((Target.position - _transform.position).normalized.z * 3, -3, 3));

        MoveBSpots.speed *= 3;
        MoveBSpots.targetWaypoint = Jump_transform;

        Anim.Play("Jump", 0);

        Jumping = true;

    }



    void AngleCalculation()
    {
        if (MoveBSpots == null) return;

        if (!MoveBSpots.Attacking) return;

        Vector3 toTarget = transform.position - pl.MainCamera.transform.position;

        float signedAngle = Vector3.SignedAngle(pl.MainCamera.transform.forward, new Vector3(0, toTarget.y, toTarget.z), pl.MainCamera.transform.right);


    }



    static double CalculateAngleBetweenVectors(double[] vectorA, double[] vectorB)
    {
        // Check if vectors have the same dimension
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must have the same dimension.");
        }

        // Calculate dot product
        double dotProduct = DotProduct(vectorA, vectorB);

        // Calculate magnitudes
        double magnitudeA = Magnitude(vectorA);
        double magnitudeB = Magnitude(vectorB);

        // Calculate the angle in radians
        double angleRadians = Math.Acos(dotProduct / (magnitudeA * magnitudeB));

        return angleRadians;
    }

    static double DotProduct(double[] vectorA, double[] vectorB)
    {
        double result = 0;
        for (int i = 0; i < vectorA.Length; i++)
        {
            result += vectorA[i] * vectorB[i];
        }
        return result;
    }

    public void PlayAttackSound()
    {
        if (StartAttackSound == null) return;
        AS = GetComponent<AudioSource>();

        if (AS == null) return;

        AS.clip = StartAttackSound;

        if (!AS.isPlaying)
            AS.Play();
    }


    void StartAttackPause()
    {

        if (_attackState == AttackState.PauseAfterAttack)
        {
            return;
        }


        if (!MoveBSpots.Attacking)
            return;

        if (AttackBuildup>0)
            return;

        if (AttackDuration <= 0.1 && AttackDuration > 0)
        {
            DelayBetweenAttacks = DelayBetweenAttacks_MAX;

            SetAnimState("PauseAfterAttack");
            StopWalking();
            MoveBSpots.Dashing = false;
            ResetCurrentAttackNumber();
            _attackState = AttackState.PauseAfterAttack;

            if (EnemiesSpawner != null)
                EnemiesSpawner.enabled = false;


        }


    }



    void StopWalking()
    {

        if (!MoveBSpots.isWalking()) return;
        WalkingDelay = -1;
        MoveBSpots.Stop();

    }


    static double Magnitude(double[] vector)
    {
        double sumOfSquares = 0;
        foreach (var component in vector)
        {
            sumOfSquares += component * component;
        }
        return Math.Sqrt(sumOfSquares);
    }

    static double RadiansToDegrees(double radians)
    {
        return radians * (180.0 / Math.PI);
    }


    public bool IsPointOnNavMesh(Vector3 point, float maxDistance = 1.0f)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(new Vector3(point.x, point.y, point.z), out hit, maxDistance, NavMesh.AllAreas);


    }

    void ThroughOnTheGround()
    {
        if (Lying) return;

        DelayBetweenAttacks = LyingDuration;

        SetAnimState("Lying");

        Lying = true;


        StopAttacking();
    }

    void MovementStartStop()
    {
        if (Lying) return;

        if (IgnoreUpdateRange)
        {
            if (AttackDuration <= 0 && DelayBetweenAttacks <= 0) MoveBSpots.Resume();
        }
        else
        {
            Vector3 pl_pos = pl.transform.position;


            if (Mathf.Abs(transform.position.x - pl_pos.x) > pl.UpdateRange ||
                   Mathf.Abs(transform.position.y - pl_pos.y) > pl.UpdateRange ||
                         Mathf.Abs(transform.position.z - pl_pos.z) > pl.UpdateRange)
            {
                if (!Boss)
                {
                    // StopWalking();
                    StopAttacking();
                }

                return;
            }
            else if (AttackDuration <= 0 && DelayBetweenAttacks <= 0) MoveBSpots.Resume();


        }

        if (Mathf.Abs(transform.position.x - pl.transform.position.x) < ChasingRange &&
            Mathf.Abs(transform.position.y - pl.transform.position.y) < ChasingRange &&
            Mathf.Abs(transform.position.z - pl.transform.position.z) < ChasingRange)
        {

           
                StartChasing();

            if (!RayCheck() && ChasingTime <= 0)
            {
                StopAttacking();

                Target = StartPoint;
         
                MoveBSpots.targetWaypoint = StartPoint;
                MoveBSpots.targetWaypointRotation = StartPoint;

            }
        }
        else
        {
            if (ChasingTime <= 0 && !RayCheck())
            {
             
                    StopAttacking();

                    Target = StartPoint;
                  
                    MoveBSpots.targetWaypoint = StartPoint;
                    MoveBSpots.targetWaypointRotation = StartPoint;
                
            }

        }

     
    }



    void StopAttacking()
    {
        if (!attacking) return;

        attacking = false;
        MoveBSpots.Attacking = false;

        if (pl.EnemiesAttacking.Contains(gameObject))
            pl.EnemiesAttacking.Remove(gameObject);

        if (pl.BossesAttacking.Contains(gameObject))
            pl.BossesAttacking.Remove(gameObject);


       
      
        _attackState = AttackState.NoAttack;

        AttackBuildup = -1;
        AttackDuration = -1;

        if (!Lying) 
        DelayBetweenAttacks = -1;

    }

    void AttackAnimations()
    {
       

        if (StandupTimer > Time.fixedTime) return;
        if (_attackState == AttackState.PauseAfterAttack) return;
        if (!MoveBSpots.Attacking) return;

     

       
        if (StartAttackingTimer > 0)
        {
            SetAnimState("StartTransition");
            return;
        }

        if (_attackState == AttackState.Buildup)
        {

            SetAnimStateTransition(_Attacktype[AttackNum].AType.ToString(), buildupduration);
            return;
        }

        if (_attackState != AttackState.Attack) return;

        if (_Attacktype[AttackNum].AType == AttackType.Punch)
        {

            SetAnimState("Punch");
        }

        if (_Attacktype[AttackNum].AType == AttackType.Hide)
        {

            SetAnimState("Hide");
        }

        if (_Attacktype[AttackNum].AType == AttackType.Dashing)
        {

            SetAnimState("Dashing");
        }

        if (_Attacktype[AttackNum].AType == AttackType.ShootingCircle)
        {

            SetAnimState("ShootingCircle");
        }

        if (_Attacktype[AttackNum].AType == AttackType.ShootingStraight)
        {
            MoveBSpots.RotateTowards(pl._transform.position);
            SetAnimState("ShootingStraight");
        }

        if (_Attacktype[AttackNum].AType == AttackType.ShootingThree)
        {
            SetAnimState("Shooting3");

        }



        if (_Attacktype[AttackNum].AType == AttackType.Lazers)
        {
            SetAnimState("Lazers");

        }


        if (_Attacktype[AttackNum].AType == AttackType.SpawnEnemies)
        {
            SetAnimState("SpawnEnemies");


        }


        if (_Attacktype[AttackNum].AType == AttackType.FollowBullet)
        {
            SetAnimState("FollowBullet");

        }

        if (_Attacktype[AttackNum].AType == AttackType.ShootLine)
        {
            SetAnimState("ShootLine");

        }

    }


    void DamagePlayer()
    {
        if (Lying) return;

        bool isLegsCollided = pl.Legscoll_obj.Contains(gameObject);
        bool isAttackCollided = (pl.Legscoll_obj.Contains(_Attacktype[AttackNum].AttackColl) &&
             AttackDuration > 0);

        if (isAttackCollided && _Attacktype[AttackNum].AttackColl.activeInHierarchy == false)
            return;

        if (isAttackCollided && _Attacktype[AttackNum].AttackColl.GetComponent<CapsuleCollider>() != null)
        {
            if(_Attacktype[AttackNum].AttackColl.GetComponent<CapsuleCollider>().enabled == false)
            return;
        }


        bool isMultiCollided = false;

        for (int i = 0; i < _Stats.Colliders.Count; i++)
        {

            if (_Stats.Colliders[i] != null)
            {
                if (_Stats.Colliders[i].activeInHierarchy)
                    if (pl.Legscoll_obj.Contains(_Stats.Colliders[i]))
                    {
                        if (AttackDuration <= 0)
                        {
                            pl.Legscoll_obj.Remove(_Stats.Colliders[i]);
                            return;
                        }

                        if (_Stats.Colliders[i].GetComponent<CapsuleCollider>() != null)
                        {
                            if(_Stats.Colliders[i].GetComponent<CapsuleCollider>().enabled)
                            isMultiCollided = true;
                        }
                        else if (_Stats.Colliders[i].GetComponent<BoxCollider>() != null)
                        {
                            if (_Stats.Colliders[i].GetComponent<BoxCollider>().enabled)
                                isMultiCollided = true;
                        }
                           
                    }
            }
        }
        if (isMultiCollided) print("isMultiCollided");

        if (isLegsCollided || isAttackCollided || isMultiCollided)
        {
            if(!pl.PlayerPause())
            pl.GetDamage(_Attacktype[AttackNum].AttackDamage);



           /* DelayBetweenAttacks = DelayBetweenAttacks_MAX;

            SetAnimState("PauseAfterAttack");
            StopWalking();
            ResetCurrentAttackNumber();
            _attackState = AttackState.PauseAfterAttack;*/
        }
        
    }




    bool CheckPositionToJump(Vector3 Pos)
    {
        bool result;
        Vector3 V = Pos / 1.2f;
        if (IsPointOutsideNavMesh(V))
        {

            result = true;

        }
        else result = false;

        return result;
    }

    void StandingAndWalking()
    {
      

        if (StartAttackingTimer > 0) return;
        
        if (!MoveBSpots.isWalking() && !MoveBSpots.Attacking)
        {
            Lying = false;
            SetAnimState("Standing");
            return;
        }

        if (_attackState == AttackState.Attack || _attackState == AttackState.Buildup)
        {
            Lying = false;
            return;
        }


        if (Lying && !MoveBSpots.isStunned())
        {
            StandupTimer = Time.fixedTime + 0.5f;
            SetAnimState("Standup");
            Lying = false;
        }

        if (StandupTimer > Time.fixedTime) return;
        if (WalkingDelay > 0) return;

        if (DelayBetweenAttacks > 0) return;






        if (_WalkingParts.Count == 0)
        {
            print(name + "walking");
            SetAnimState("Walking");
        }
        else
        if (WalkingDelay <= 0)
        {
            CurrentWalkingState = UnityEngine.Random.Range(0, _WalkingParts.Count);

            SetAnimState(_WalkingParts[CurrentWalkingState].AnimName);

            WalkingDelay = _WalkingParts[CurrentWalkingState].PartDuration;

        }




    }



    void Buildups()
    {
   
        if (Mathf.Abs(transform.position.x - pl.transform.position.x) > _Attacktype[AttackNum].AttackRange ||
            Mathf.Abs(transform.position.y - (pl.transform.position.y - 0.5f)) > _Attacktype[AttackNum].AttackRange ||
            Mathf.Abs(transform.position.z - pl.transform.position.z) > _Attacktype[AttackNum].AttackRange)
            return;


        if (!MoveBSpots.Attacking) return;
       
        if (_attackState == AttackState.Attack) return;
      

        MoveBSpots.RotateTowards(Target.position);
        //ResetCurrentAttackNumber();

        if (_Attacktype[AttackNum].AType == AttackType.ShootingStraight)
            _transform.LookAt(transform);

        StopWalking();
        if(NavAgent!=null)
        NavAgent.velocity = Vector3.zero;

        if (_Rigidbody != null)
            _Rigidbody.velocity = Vector3.zero;

        _attackState = AttackState.Buildup;
        AttackBuildup = buildupduration;

        if(EnemiesSpawner!=null)
        EnemiesSpawner.enabled = false;

    }


    bool isLookingAt(Transform target)
    {
        Vector3 directionToTarget = target.position - _transform.position;
        directionToTarget.y = 0; 

        Vector3 forward = _transform.forward;
        float dotProduct = Vector3.Dot(forward, directionToTarget.normalized);

        if (dotProduct > 0.99f) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    void ResetCurrentAttackNumber()
    {
        AttackNum = UnityEngine.Random.Range(0, _Attacktype.Count);
        NoAttackDelay = ResetAttackDelay;
    }

    public bool IsPointOutsideNavMesh(Vector3 point)
    {
        
        return false;
    }



    public void Death()
    {
        if (pl.EnemiesAttacking.Contains(gameObject))
            pl.EnemiesAttacking.Remove(gameObject);

        if (pl.BossesAttacking.Contains(gameObject))
            pl.BossesAttacking.Remove(gameObject);
    }


    bool RayCheck()
    {
        if(Boss) return true;

        rayhit = new List<GameObject>();

        Vector3 PosNormalize = (pl.transform.position - transform.position).normalized;

        float rayh = 0;
        if (NavAgent != null) rayh = NavAgent.height / 2;
        else if (GetComponent<CapsuleCollider>() != null)
            rayh = GetComponent<CapsuleCollider>().height / 2;
        else rayh = 1;

        rayh = pl.GetComponent<CapsuleCollider>().height / 2;
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + rayh, transform.position.z), new Vector3(PosNormalize.x * 100f, PosNormalize.y, PosNormalize.z * 100f));

       
       RaycastHit hit;

        if (Physics.Raycast(ray, out hit, ChasingRange * 100, AttackRayCheckMask))
            if (!rayhit.Contains(hit.collider.gameObject))
                rayhit.Add(hit.collider.gameObject);

        
        if (rayhit.Contains(pl.gameObject)) return true;
        else return false;
    }



}
