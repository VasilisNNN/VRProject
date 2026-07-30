using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;



public class Player : MonoBehaviour
{
    public Sprite PlayerPortrait;
    public int CurrentPlayer = 0;
    public Transform HandTransform;

    public List<GameObject> Legscoll_obj = new List<GameObject>();
   
    public List<GameObject> Viewcoll_obj_Ray_left = new List<GameObject>();

    public List<GameObject> Viewcoll_obj_right = new List<GameObject>();
    public List<GameObject> Viewcoll_obj_Ray_right = new List<GameObject>();

    public List<GameObject> CarMeleecoll_obj = new List<GameObject>();
    public List<GameObject> Gun_Ray = new List<GameObject>();



    [HideInInspector]
    public List<GameObject> StraightRoads = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> EnemiesAttacking = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> BossesAttacking = new List<GameObject>();


    [HideInInspector]
    public Vector3 Viewcoll_HitPos , Gun_HitPos;
    [HideInInspector]
    public float HitDistance;


    [HideInInspector]
    public int HP;
    [HideInInspector]
    public int HPMax = 30;

    [HideInInspector]
    public int Hunger;
    [HideInInspector]
    public int HungerMax = 20;

    [HideInInspector]
    public int MaxStamina = 15;
    [HideInInspector]
    public int Stamina;

    public int LowHitDamage = 1;

    public int LowWaterDamage = 1;



  

    public float CarSpeed = 1;

    public int PistolDamage = 1;
    public int ShotgunDamage = 2;
    public int RifleDamage = 3;

    [HideInInspector]
    public int Protection;


    private GameObject DamageAnim, DamageAnimLight, HealAnim;



    private float DamageTimer;
    private float HPDecreaseTimer;
    private float HPDecreaseDelay = 20;
    public Transform Body;
    public Transform Sholders;

    float xRotation = 0f;
    float yRotation = 0f;
    float RotateHead = 0f;
    private Rigidbody controller;

    public float Speed = 6;
    public float gravity = -9.81f;

    [HideInInspector]
    public Vector3 desiredVelocity;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask wallMask;

    public Transform LeftCheck;
    public Transform RightCheck;


    private bool isGrounded;
 
    private GameObject Car;

    public Vector2 CameraYBorder = new Vector2(-90f, 90f);
    public Vector2 HeadRotationLimits = new Vector2(0, 0);
    public Vector2 PlayerRotationLimits = new Vector2(0, 0);

    public InputMode IM;

    public bool LockCamera { get; set; }
    public bool ZoomCamera { get; set; }

    private GameObject Wheel;
    private float CamShakeTimer;
    private float ShakeAmplitude, ShakeAmplitudeMax;
    public float LockCameraTimer;

    public float FOVShake;
    private Vector3 CamStartPos;
    private int CamSide;
    private int CamSideWalkY, CamSideWalkX;
    private float  CamYSpeed, CamXSpeed;

    public Vector2 difference { get; set; }
    private float rotationZ, rotationX, rotationY;
    public GameObject CutSceneObject;

    private float LERPX, LERPY, LERPZ;
    public Menu _Menu;
    public Inventory inv { get; set; }
    private bool Achunlocked;
    private float StartTimer, StaminaTimer;

    public Camera MainCamera;


    public OVRCameraRig VRCamera;


    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public float playerHeight;
    
    private float movespeed;

    public bool Chatting { get; set; }
    public float ShowAllDamageHeal { get; set; }


    public bool DEMO;
    public bool TEST;
    public bool MoveForwardAllTheTime;

    public SaveLoad SL { get; set; }
   
    public bool CutSceneMode { get; set; }
    public bool showjoycheck { get; set; }
    public bool OnStraightRoad { get; set; }

    [HideInInspector]
    public List<GameObject> Explosions = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> FleshDeath = new List<GameObject>();


    [HideInInspector]
    public GameObject UpDanger, DownDanger;
    public bool _gameover { get; private set; }
    public bool showUpgrades { get; set; }
    public bool showMap { get; set; }

    public float DontMoveTimer { get; set; }

    public int MagCapacity = 0;



    public bool Showdamage { get; set; }


    public List<GameObject> Enemies = new List<GameObject>();
    private Scrollbar HPUI,  WaterUI, StaminaUI;
    private GameObject HPUIGameObject, StaminaUIGameObject;

    public float FOV_Main { get; set; }
    public float FOV_MainDefault { get; private set; }
    public DayAndNight DayNight { get; set; }
    public bool InDialog { get; set; }
    public GameObject CollidingDialog { get; set; }

    private bool Jumping;
    private float jumpforce;
    public Vector3 moveDirection { get; private set; }


    public float Game_SPEED { get; set; }

    public float UpdateRange = 200;

    public Gun _Gun { get; set; }

    public Transform _transform { get; private set; }

    public bool StartLoading { get; private set; }

    public GameObject MouseOB;
    private AudioSource EnemyAttackAudio;
    private AudioSource BossAttackAudio;



    private Light PointLight, SpotLight;
    private CollList ViewTriggerLeft, ViewTriggerRight;
    public GameObject PlayerView;
    private float runboost = 1;




    [SerializeField] Transform campos;

    private CinemachineVirtualCamera CarAimCamera;

    public GameObject HandObject { get; set; }
    public string HandObject_Anim { get; set; }
    private Image FadeIn;

    private Transform Steps_Transform;



    private Transform CanvasTransfrom;

    public PlayerAnimation Pl_Anim { get; set; }
    private float GameStartDelay;


    private Vector3 HeadBobPos;
    private float HeadBobSpeed_X, HeadBobSpeed_Y, StraifHeadRotation;
    private Vector3 PlayerViewStartPos, CameraStartPos;
    private AudioClip ScanClip;
    private void Awake()
    {
        MainCamera = VRCamera.centerEyeAnchor.GetComponent<Camera>();

#if UNITY_ANDROID || UNITY_XR_OPENXR
        MainCamera = VRCamera.centerEyeAnchor.GetComponent<Camera>();
       
       
       
       

#endif
    }
    void Start()
    {

        TEST = true;

        HeadBobSpeed_X = 1 * 0.3f;
        HeadBobSpeed_Y = -1 * 0.3f;

        Pl_Anim = GetComponent<PlayerAnimation>();
        CanvasTransfrom = GameObject.Find("Canvas").transform;
        ScanClip = Resources.Load<AudioClip>("Sound/Impacts/ImpactDrop");

           Steps_Transform = transform.Find("Steps");

        if (GameObject.Find("FadeIn") != null)
        {
            FadeIn = GameObject.Find("FadeIn").GetComponent<Image>();
            FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, 1);
        }
   

        if (GameObject.Find("CarAimCamera") != null)
            CarAimCamera = GameObject.Find("CarAimCamera").GetComponent<CinemachineVirtualCamera>();


        MouseOB = GameObject.Find("MouseUI");

        EnemyAttackAudio = transform.Find("EnemyAttackAudio").GetComponent<AudioSource>();
        BossAttackAudio = transform.Find("BossAttackAudio").GetComponent<AudioSource>();


        Game_SPEED = 1;
        ShakeAmplitude = 0f;
        ShakeAmplitudeMax = 0.02f;
        _transform = transform;
        yRotation = _transform.eulerAngles.y;
         HPUI = GameObject.Find("HP_Scrollbar").GetComponent<Scrollbar>();
        HPUIGameObject = GameObject.Find("HP_Scrollbar");


        StaminaUI = GameObject.Find("Stamina_Scrollbar").GetComponent<Scrollbar>();
        StaminaUIGameObject = GameObject.Find("Stamina_Scrollbar");

        WaterUI = GameObject.Find("Water_Scrollbar").GetComponent<Scrollbar>();



   
        _Gun = GetComponent<Gun>();
        SL = GetComponent<SaveLoad>();
     
        for (int i = 0; i < 15; i++)
        {
            Explosions.Add(Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/Explosion")));
        }
        for (int i = 0; i < Explosions.Count; i++)
        {
            Explosions[i].transform.position = new Vector3(99999, 99999, 99999);
        }


        for (int i = 0; i < 15; i++)
        {
            FleshDeath.Add(Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/FleshDeath")));
        }


        for (int i = 0; i < FleshDeath.Count; i++)
        {
            FleshDeath[i].transform.position = new Vector3(99999, 99999, 99999);
        }
        
   
        if (MainCamera != null)
        MainCamera.backgroundColor = RenderSettings.fogColor;
      
      
        PlayerView.transform.localPosition = Vector3.zero;
        PlayerView.transform.localEulerAngles = Vector3.zero;
        SpotLight = PlayerView.transform.Find("Spot Light").GetComponent<Light>();
        SpotLight.enabled = false;

        PlayerViewStartPos = PlayerView.transform.localPosition;
        CameraStartPos = MainCamera.transform.localPosition;

        if (GetComponent<Menu>() == null) gameObject.AddComponent<Menu>();
        _Menu = GetComponent<Menu>();
        inv = GetComponent<Inventory>();

        Wheel = GameObject.Find("Steering_wheelPlayer");
        controller = GetComponent<Rigidbody>();
        
        IM = GetComponent<InputMode>();
        RotateHead = Body.eulerAngles.y;
        CamSide = 1;
       
        MaxStamina = 10;

        Stamina = MaxStamina;
        if (SL == null)
            print("SaveLoad is null");

        if (SL.SaveLoadCurrent == null)
            print("SaveLoadCurrent is null");

        if (GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel) != null)
        {
            transform.SetPositionAndRotation(GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel).transform.position, GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel).transform.rotation);
                   
                   
        }
       if (GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel) != null)
        {
            transform.SetPositionAndRotation(GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel).transform.position, GameObject.Find("StartPoint_" + SL.SaveLoadCurrent.PreviousLevel).transform.rotation);


        }

        if (MainCamera != null)
        CamStartPos = MainCamera.transform.localPosition;

        ViewTriggerLeft = GameObject.Find("ViewTriggerLeft").GetComponent<CollList>();
        ViewTriggerRight = GameObject.Find("ViewTriggerRight").GetComponent<CollList>();
        StartTimer = Time.fixedTime + 0.5f;
        FOV_MainDefault = 60;

        if (Screen.height > Screen.width)
        {
            FOV_Main = FOV_MainDefault = 120;
            MainCamera.nearClipPlane = 0.01f;
            _Menu.VerticalCamera = true;
        }
        else
        {
            MainCamera.nearClipPlane = 0.1f;
            _Menu.VerticalCamera = false;
        }
        if (FOV_Main <= 1) FOV_Main = FOV_MainDefault;

        if (HP <= 0) HP = 1;

        GameStartDelay = Time.fixedTime + 1;
    }


    void Update()
    {
      
        if (Screen.height > Screen.width )
        {
        
            FOV_Main = FOV_MainDefault = 100;
            MainCamera.nearClipPlane = 0.01f;
            _Menu.VerticalCamera = true;
        }
        else { 
            MainCamera.nearClipPlane = 0.02f;
            _Menu.VerticalCamera = false;
          }
    FOV_Main = FOV_MainDefault;
        

        if (FadeIn!=null)
        FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, FadeIn.color.a-Time.deltaTime);

   

        Pl_Anim.AnimatedPickedObjectManager();
      
        if (inv != null)
        {
            if (!PlayerMenusPause())
                Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
        }

        if (HP <= 0 && !_gameover)
        {
            StopMovement();
            IM.ActionDelay = Time.fixedTime + 0.5f;
            _gameover = true;
        }

        if (_gameover)
        {
            GameoverManager();
        }

        if (MainCamera == null) return;


        if (MainCamera.enabled)
        {
            Pl_Anim.AnimManager();
        }
        UIControll();
        if (PlayerPause())
        {
            StopMovement();
            return;
        }

      

       
        if (!MainCamera.enabled)
        {
            CutSceneMode = true;
            return;
        }

        DamageHealOutlineManager();
        ManageHandObject();
        ParameterManager();

        if (inv != null)
        {
            if (!PlayerMenusPause())
            {
                    CamShake();
                    if (!LockCamera) MoveControll();
                    

                CameraRotation();
                CameraFOVControll();
            }
               

        }


        Viewcoll_obj_Ray_left = ViewTriggerLeft.rayhit;
        Viewcoll_obj_Ray_right = ViewTriggerRight.rayhit;

        Gun_HitPos = GameObject.Find("GunTrigger").GetComponent<CollList>().HitPos;
        Viewcoll_HitPos = ViewTriggerLeft.HitPos;
          
        HitDistance = ViewTriggerLeft.HitDistance;
        LightManager();

        CarMeleecoll_obj = new List<GameObject>();
            
    
    }

    private void FixedUpdate()
    {
        if (_Menu == null) return;

        if (!_Menu.MenuONOFF)
        {
            CutsceneRotation();
        }
    }

    void ParameterManager()
    {
        HPDecreaseTimer -= Time.deltaTime;
        if (SceneManager.GetActiveScene().name.Contains("Appartment"))
        {
            ReduceParameter(ref HP, HPMax, 1, ref HPDecreaseTimer, HPDecreaseDelay);
            return;
        }
        if (!SceneManager.GetActiveScene().name.Contains("Day")) return;

    
        ReduceParameter(ref HP, HPMax, -1, ref HPDecreaseTimer, HPDecreaseDelay);
    }

    void LightManager()
    {
        SpotLight.intensity = Mathf.Lerp(SpotLight.intensity, 2 - (Mathf.Abs(5 - Mathf.Clamp(HitDistance, 0, 5)) * 1.1f), Time.deltaTime * 8);

        SpotLight.enabled = true;
    }


    void StopMovement()
    {
        if(CutSceneMode)
        MainCamera.transform.localPosition = CameraStartPos;

        if (controller==null) return;
        controller.velocity = Vector3.zero;
        moveDirection = Vector3.zero;
       
    }

    void UIControll()
    {
        if (HPUIGameObject == null) return;
        if (StaminaUIGameObject == null) return;



        HPUIGameObject.SetActive(!_Menu.HideUI);
        StaminaUIGameObject.SetActive(!_Menu.HideUI);


        HPUI.size = (float)HP / (float)HPMax;
       
        StaminaUI.size = (float)Stamina / (float)MaxStamina;
    }

    void MoveControll()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, groundMask);

        if (isGrounded && desiredVelocity.y < 0)
        {
            desiredVelocity.y = -2f;
        }

        if (_Menu.MouseSensitivity < 0.1f) _Menu.MouseSensitivity = 0.1f;

         float z = IM._vertical;
         float x = IM._horizontal;

        if (DontMoveTimer >= Time.fixedTime )
        {
            StopMovement();
            return;
        }

        if (IM.run && !IM.Aim)
        {
            if (movespeed > 0.01f)
                ReduceParameter(ref Stamina,MaxStamina,-1,ref StaminaTimer, 1);

            if (Stamina > 0) runboost = 1.6f;
            else runboost = 1;

        }
        else runboost = 1;


        if (!IM.run) ReduceParameter(ref Stamina, MaxStamina,1, ref StaminaTimer,1);

      
        if (Mathf.Abs( IM._horizontal) > 0.1f || Mathf.Abs(IM._vertical) > 0.1f)
            movespeed = Speed * runboost;
        else
        {
            controller.velocity = Vector3.zero;
            movespeed = 0;
            
        }

        moveDirection = MainCamera.transform.right * x + MainCamera.transform.forward * z;

        VelocityControl();

    }



    private void VelocityControl()
    {


        if (OnSlope() && !Jumping)
        {

            desiredVelocity = GetSlopeMoveDirection() * movespeed;
            controller.velocity = desiredVelocity;


     
            controller.useGravity = false;


            return;
        }




        float velocity_Y = jumpforce;
        float limitedVel_Y = 0;



        if (!Jumping)
        {
            if (isGrounded)
            {
              
                controller.useGravity = true;
                controller.mass = 1f;
                jumpforce = -2;
            }
            else
            {
               
                controller.mass = 10f;
                controller.useGravity = true;
        
            }

        }


        Vector3 inputMove = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
        desiredVelocity = inputMove * movespeed;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, desiredVelocity.normalized, out hit, 0.3f, wallMask))
        {
            Vector3 wallNormal = hit.normal;
            desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, wallNormal);
        }

        if(!isGrounded)
        desiredVelocity = new Vector3(desiredVelocity.x, gravity, desiredVelocity.z);
        else desiredVelocity = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);

      //  controller.velocity = desiredVelocity;

    }

    void CameraFOVControll()
    {
        if (FOVShake > 0) FOVShake -= Time.deltaTime*3;
        
        if(StaminaTimer>0)
        StaminaTimer -= Time.deltaTime;

        if (IM.Aim)
        {
            if (MainCamera.fieldOfView > 35f)
                MainCamera.fieldOfView -= 0.01f;
        }

        if (ZoomCamera)
        {
            if (MainCamera.fieldOfView > 35f)
                MainCamera.fieldOfView -= 0.01f;
            if (LockCameraTimer < Time.fixedTime) ZoomCamera = false;
        }
        else MainCamera.fieldOfView = FOV_Main + FOVShake;
        
    }


    
    


    void CamShake()
    {
        if (CamShakeTimer <= 0)
        {
            CamXSpeed = 0;
            CamYSpeed = 0;
            return;
        }



        if (CamSideWalkY == 0) CamSideWalkY = 3;
        if (CamSideWalkX == 0) CamSideWalkX = 1;

        if (CamYSpeed > 0.01f) CamSideWalkY = -3;
        if (CamYSpeed < -0.01f) CamSideWalkY = 3;

        CamYSpeed += CamSideWalkY * Time.deltaTime / 2;

        if (CamXSpeed > 0.01f) CamSideWalkX = -1;
        if (CamXSpeed < -0.01f) CamSideWalkX = 1;


        CamXSpeed += CamSideWalkX * Time.deltaTime / 2;

        CamShakeTimer -= Time.deltaTime;

        Transform CamTF = MainCamera.transform;

        if (CamShakeTimer - 0.1 > 0)
        {
            if (ShakeAmplitude > ShakeAmplitudeMax && CamSide == 1) CamSide = -1;
            if (ShakeAmplitude < -ShakeAmplitudeMax && CamSide == -1) CamSide = 1;

            ShakeAmplitude += Time.deltaTime * CamSide*2;
        }
        else ShakeAmplitude = 0;


        CamTF.localPosition = CamStartPos + new Vector3(0, ShakeAmplitude, 0);


    }


    public void ResetCamera()
    {
        xRotation =0;
       
        yRotation =0;
    }


    public void SetCameraRotation(float yrotation)
    {
        yRotation = yrotation;

    }

    void CameraRotation()
    {
      

        if (LockCamera) return;

        float MouseX = 0;
        float MouseY = 0;

        if (!IM.joystick)
        {
            MouseX = Input.GetAxis("Mouse X") * (_Menu.MouseSensitivity * 800) * Time.deltaTime;
            MouseY = Input.GetAxis("Mouse Y") * (_Menu.MouseSensitivity * 800) * Time.deltaTime;
        }
        else
        {
            MouseX = IM._horizontal_R * (_Menu.MouseSensitivity * 400) * Time.deltaTime;
            MouseY = IM._vertical_R * (_Menu.MouseSensitivity * 400) * Time.deltaTime;

        }


        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, CameraYBorder.x, CameraYBorder.y);

        yRotation += MouseX;

        if (PlayerRotationLimits != Vector2.zero)
            yRotation = Mathf.Clamp(yRotation, PlayerRotationLimits.x, PlayerRotationLimits.y);
     
        if(Steps_Transform != null)
        Steps_Transform.localEulerAngles = new Vector3(0, MainCamera.transform.eulerAngles.y, 0);

        StraifHeadRotation = Mathf.Lerp(StraifHeadRotation, IM._horizontal * -0.5f, Time.deltaTime*5);

#if !UNITY_XR && !UNITY_ANDROID
        if (PlayerRotationLimits != Vector2.zero)
            MainCamera.transform.localRotation = Quaternion.Euler(xRotation, Mathf.Clamp(yRotation, PlayerRotationLimits.x, PlayerRotationLimits.y), 0f);
        else
            MainCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, HeadBob().y + StraifHeadRotation);

    if (CamShakeTimer<=0 )
            MainCamera.transform.localPosition = CameraStartPos + HeadBob();
   
#endif

#if UNITY_ANDROID && UNITY_EDITOR
        /*  if (PlayerRotationLimits != Vector2.zero)
              MainCamera.transform.localRotation = Quaternion.Euler(xRotation, Mathf.Clamp(yRotation, PlayerRotationLimits.x, PlayerRotationLimits.y), 0f);
          else
              MainCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, HeadBob().y + StraifHeadRotation);



      if (CamShakeTimer<=0 )
              MainCamera.transform.localPosition = CameraStartPos + HeadBob();
         */
#endif




        _transform.localRotation = Quaternion.Euler(0, 0, 0f);
        PlayerView.transform.localPosition = PlayerViewStartPos;

    
            if (HeadRotationLimits.x != 0)
        {
            RotateHead += MouseX;
            RotateHead = Mathf.Clamp(RotateHead, HeadRotationLimits.x, HeadRotationLimits.y);
        }
        

    }

    void DamageHealOutlineManager()
    {
        if (CutSceneMode) ShowAllDamageHeal = -1;
        if (PlayerPause()) return;

        if (ShowAllDamageHeal > 0)
        {
            ShowAllDamageHeal -= Time.deltaTime;
        }

        if (IM.ScanButton)
        {
            if (Stamina == MaxStamina)
            {
                ShowAllDamageHeal = 4;
                PlaySoundsPitched(ScanClip, 1);
                Stamina = 0;
            }
            else
            {
                _Menu.PlayErrorSound();
            
            }

        }


    }



    public void SetCamShakeTimer(float timer, float _ShakeAplitude)
    {
        if (CamShakeTimer > 0) return;
        FOVShake = 1;
        CamShakeTimer = timer;
        ShakeAmplitudeMax = _ShakeAplitude;
    }



    void CutsceneRotation()
    {
        if (LockCameraTimer < Time.fixedTime && LockCamera)
        {
          //  transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);

            LockCamera = false;
        }

        if (LockCamera)
        {
            if (CutSceneObject != null)
            {
                Vector3 target = CutSceneObject.transform.position;
                difference = target - transform.position;

                rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                rotationX = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                rotationY = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                LERPX = Mathf.Lerp(LERPX, CutSceneObject.transform.position.x, Time.deltaTime / 8);
                LERPY = Mathf.Lerp(LERPY, CutSceneObject.transform.position.y, Time.deltaTime / 8);
                LERPZ = Mathf.Lerp(LERPZ, CutSceneObject.transform.position.z, Time.deltaTime / 8);

                // print(LERPY);

               // MainCamera.transform.LookAt(new Vector3(LERPX, LERPY, LERPZ));

             }
        }
        else
        {
            LERPX = transform.position.x;
            LERPY = transform.position.y;
            LERPZ = transform.position.z;
        }
    }


    private bool OnSlope()
    {
        if(!isGrounded) return false;

        if (Physics.Raycast(GroundCheck.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle > 3;
        }

        return false;
    }




    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void PlaySoundsPitched(AudioClip AC, float Pitch)
    {
        GetComponent<AudioSource>().clip = AC;
        GetComponent<AudioSource>().pitch = Pitch;
        GetComponent<AudioSource>().Play();
    }
    private void OnTriggerStay(Collider c)
    {

        if (!Legscoll_obj.Contains(c.gameObject))
        {
            Legscoll_obj.Add(c.gameObject);
        }

    }

    private void OnTriggerExit(Collider c)
    {

        if (Legscoll_obj.Contains(c.gameObject))
        {
            Legscoll_obj.Remove(c.gameObject);
        }

    }


 

    public void Heal(int Heal)
    {
        if (Heal == 0) return;
        if (_gameover) return;


        if (HP < HPMax)
            HP += Heal;

        if (HP > HPMax) HP = HPMax;

        JoyCheck.RemoveJoyObject(gameObject);

        if (HealAnim == null)
        {
            HealAnim = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/HealAnim"));
            HealAnim.transform.localPosition = GameObject.Find("Canvas").transform.position;


            HealAnim.name = "HealAnim";
            HealAnim.transform.parent = GameObject.Find("Canvas").transform;
        }
        if (HealAnim != null)
        {
            if (!HealAnim.GetComponent<AudioSource>().isPlaying)
                HealAnim.GetComponent<AudioSource>().Play();

            HealAnim.GetComponent<Animator>().Play("Start");
        }

    }
  


   

    public void GetDamage(int Damage)
    {
        if (_gameover) return;
        if (Damage == 0) return;
        if (DamageTimer > Time.fixedTime) return;

        HP -= Damage;

        if (DamageAnim == null)
        {
            DamageAnim = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DamageAnim"), CanvasTransfrom);
            DamageAnim.transform.localPosition = Vector3.zero;

            DamageAnim.name = "DamageAnim";
            DamageAnim.GetComponent<Animator>().Play("Start");
        }
        if (DamageAnim != null)
        {
            if (!DamageAnim.GetComponent<AudioSource>().isPlaying)
                DamageAnim.GetComponent<AudioSource>().Play();
            print("DamageAnim START");



            DamageAnim.GetComponent<Animator>().Play("Start", 0, 0f);

        }
        DamageTimer = Time.fixedTime + 0.1f;
        HPDecreaseTimer = Time.fixedTime + HPDecreaseDelay;
    }

    public void GetDamageLight(int Damage)
    {
        if (_gameover) return;
        if (Damage == 0) return;
        if (DamageTimer > Time.fixedTime) return;

        HP -= Damage;

        if (DamageAnimLight == null)
        {
            DamageAnimLight = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DamageAnimLight"), CanvasTransfrom);
            DamageAnimLight.transform.localPosition = Vector3.zero;

            DamageAnimLight.name = "DamageAnimLight";
            DamageAnimLight.GetComponent<Animator>().Play("Start");
        }
        if (DamageAnimLight != null)
        {
            if (!DamageAnimLight.GetComponent<AudioSource>().isPlaying)
                DamageAnimLight.GetComponent<AudioSource>().Play();
          


            DamageAnimLight.GetComponent<Animator>().Play("Start", 0, 0f);

        }

        DamageTimer = Time.fixedTime + 0.1f;
        HPDecreaseTimer = Time.fixedTime + HPDecreaseDelay;
    }
    void ManageHandObject()
    {

        if (PlayerPause()) return;
        if (HandObject == null) return;

        HandObject.transform.parent = HandTransform;
        HandObject.transform.localPosition = Vector3.zero;
        HandObject.transform.localEulerAngles = Vector3.zero;
      //  HandObject.transform.localScale = Vector3.one;
        if (HandObject.GetComponent<Rigidbody>() != null)
            HandObject.GetComponent<Rigidbody>().isKinematic = true;

        if (HandObject.GetComponent<BoxCollider>() != null)
            HandObject.GetComponent<BoxCollider>().isTrigger = true;
        HandObject.layer = 10;


        
        
    }

    public void ReduceParameter(ref int param, int maxparam, int grow,ref float timer, float delay)
    {
        if (timer > 0) return;



        param += grow;
        if (param > maxparam) param = maxparam;
        if (param < 0) param = 0;


        timer = delay;

    }
   
   

    void GameoverManager()
    {
        if (IM.ActionDelay > Time.fixedTime) return;

        if (IM.enter_b || IM.SpaceB || IM.LeftMouseButtonDown)
        {

            HP = HPMax;
      

            SL.Save(false, 6, SceneManager.GetActiveScene().name);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        }

        if (GameObject.Find("GameoverOB") != null) return;
        GameObject GameoverOB;

        if (HP <= 0)
        {
          
            
                if (DamageAnim != null)
                    Destroy(DamageAnim.gameObject);

                if (HealAnim != null)
                    Destroy(HealAnim.gameObject);

            GameoverOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Gameover"), GameObject.Find("Canvas").transform);
            GameoverOB.name = "GameoverOB";
            return;
        }

        if (DayNight == null) return;

        if (!DayNight.DayEnded) return;

        _gameover = true;

        if (DamageAnim != null)
            Destroy(DamageAnim.gameObject);

        if (HealAnim != null)
            Destroy(HealAnim.gameObject);

        GameoverOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/GameoverDayNight"), GameObject.Find("Canvas").transform);
        GameoverOB.name = "GameoverOB";
        

    }




    private Vector3 HeadBob()
    {

        if (PlayerMenusPause()) return new Vector3(0, 0, 0);

        if (movespeed == 0)
        {
            HeadBobPos = new Vector3(
               Mathf.Lerp(HeadBobPos.x, HeadBobSpeed_X * 0.3f, Time.deltaTime / 20),
                 Mathf.Lerp(HeadBobPos.y, HeadBobSpeed_Y * 0.3f, Time.deltaTime / 20), 0);

        }
        else
        {
            HeadBobPos = new Vector3(
     Mathf.Lerp(HeadBobPos.x, HeadBobSpeed_X * 0.6f, Time.deltaTime / 6),
       Mathf.Lerp(HeadBobPos.y, HeadBobSpeed_Y * 0.6f, Time.deltaTime / 6), 0);
        }


        if (HeadBobPos.x > 0.2f)
        {
            HeadBobSpeed_X = Random.Range(-1, -1.2f);
            HeadBobSpeed_Y = Random.Range(1, 1.2f);
        }


        if (HeadBobPos.x < -0.2f)
        {
            HeadBobSpeed_X = Random.Range(1, 1.2f);
            HeadBobSpeed_Y = Random.Range(1, 1.2f);
        }

        if (HeadBobPos.y > 0.02f) HeadBobSpeed_Y = -1;


        return MainCamera.transform.right * HeadBobPos.x + MainCamera.transform.up * HeadBobPos.y;

    }
    public bool PlayerPause()
    {
        if (GameStartDelay > Time.fixedTime) return true;
        if (Pl_Anim.AnimationPause > 0) return true;
        if (_gameover || showUpgrades || _Menu.MenuONOFF || CutSceneMode || inv.showinvent || inv.showjournal || showjoycheck) return true;
        return false;
    }
    public bool PlayerMenusPause()
    {

        if (_Menu.MenuONOFF || inv.showinvent || inv.showjournal || showUpgrades || showjoycheck) return true;
        return false;
    }

    public void SetStartRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

   

    public void MoveExplosion(Vector3 pos)
    {
       
        for (int i = 0; i < Explosions.Count; i++)
        {
            if (Explosions[i].transform.position == new Vector3(99999, 99999, 99999))
            {
                Explosions[i].transform.position = pos;
                Explosions[i].GetComponent<Animator>().Play("Start");

                if (!Explosions[i].GetComponent<AudioSource>().isPlaying)
                    Explosions[i].GetComponent<AudioSource>().Play();

                break;
            }
        }

    }

    public bool ViewColl(GameObject tocoll)
    {
        if(ViewTriggerLeft.rayhit.Contains(tocoll) ||
            ViewTriggerRight.rayhit.Contains(tocoll))
            return true;

        return false;
    }
    public void DestroyObject(GameObject GO)
    {
        if (GO == null) return;

        if(Viewcoll_obj_Ray_left.Contains(GO))
            Viewcoll_obj_Ray_left.Remove(GO);

        if (Viewcoll_obj_Ray_right.Contains(GO))
            Viewcoll_obj_Ray_right.Remove(GO);


        SL.SaveLoadCurrent.ObjectsToDestroy.Add(GO.name);
        Destroy(GO);
    }
}
