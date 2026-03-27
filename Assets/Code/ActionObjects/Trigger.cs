using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Trigger : MonoBehaviour
{
    private Player pl;
    private SaveLoad SL;
    private Inventory inv;

    public bool OnEnter;
    public bool CarTrigger;
    public int NeedItem = -1;
    public int NeedCount = 1;
    public bool PlayTargetSound = false;
    public GameObject[] CollisionTargets;
    public bool DestroyCollisionTargets;

    public GameObject[] ONObjects;
    public bool DoOnes;
    public bool UndoActionIfNoColl;
    public bool[] TF;
    public bool[] TFСhecks;
    public bool StartOFF = true;

    public bool LockCamera;
    public GameObject LockOnObject;
    public bool ZoomCamera;
    public float ShakeCamera = 0;
    public float LockCameraTimer = 0;
    public bool FlashLightFlickering; 
    public bool Finish { get; set; }

    public bool RestartFinish;

    public string AchivementName = "";

    public bool OnlyAnim;
    public string AnimationName = "";

    private float DestroyTimer;
    public float DestroyTimerDelay = 0;
    private bool ToDestroy;
    public bool OnlyOnBody;
    public float FogDistance = -1;
    public Color FogColor;
    public bool ChangeFogColor;

    public int StartQuestID = -1;
    public int EndQuestID = -1;

    public float RollBackDelay = 0;
    private float RollBackTimer = 0;

    private bool RolledBack;
   
    private string CurrentAnimation;
    private float AnimationFade = 0.1f;
    private GameObject Prise;
    public bool DontSave;
    private bool GetBack;
    private Outline _Outline;


    void Start()
    {
   
        pl = InitializeOnAwake.pl;
        inv = InitializeOnAwake.pl.GetComponent<Inventory>();
        SL = InitializeOnAwake.pl.GetComponent<SaveLoad>();
        TFСhecks = new bool[TF.Length];
       
        if (StartOFF)
        {
            for (int i = 0; i < TF.Length; i++)
            {
                OnTrigger(false, i);
            }
        }

        for (int i = 0; i < TF.Length; i++)
        TFСhecks[i] = false;

        RolledBack = true;

        _Outline = GetComponent<Outline>();
    }


    void Update()
    {
        PriseControll();

        OutlineManager();


        if (pl.StartLoading) return;
        if (pl.PlayerPause()) return;



        if (SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject) && !Finish  && !DontSave)
        {
            PlayTargetSound = false;

            if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().enabled = false;
            
            Allactions();

            Finish = true;
            return;
        }




        if (ToDestroy && DestroyTimer < Time.fixedTime)
        {
            for(int i =0;i< ONObjects.Length;i++)
            pl.DestroyObject(ONObjects[i]);

            Finish = true;

        }


        RollBackConroller();

        if (pl.Legscoll_obj.Contains(gameObject) 
            && !CheckCollistionWithTarget() && CollisionTargets.Length > 0
            && (pl.IM.enter_b || pl.IM.SpaceB || pl.IM.pick_item || pl.IM.LeftMouseButtonDown))
            pl._Menu.PlaySoundsPitched(pl._Menu.ErrorClip, 1);


        if (!CheckCollTargets())
        {

            if (!DoOnes)
            {
                if (!OnEnter && UndoActionIfNoColl && !GetBack)
                {
                    for (int i = 0; i < TF.Length; i++)
                        OnTrigger(!TF[i], i);

        

                    GetBack = true;
                }

                for (int i = 0; i < TF.Length; i++)
                    TFСhecks[i] = false;

            }
            CurrentAnimation = "";

                    return;
        }


        if (FogDistance > -1) RenderSettings.fogEndDistance = convertto(RenderSettings.fogEndDistance, FogDistance);

        if (ChangeFogColor)
        {
            RenderSettings.fogColor = new Color(convertto(RenderSettings.fogColor.r, FogColor.r), convertto(RenderSettings.fogColor.g, FogColor.g), convertto(RenderSettings.fogColor.b, FogColor.b), 1);
            pl.MainCamera.GetComponent<Camera>().backgroundColor = RenderSettings.fogColor;
        }
            

        if (!OnEnter)
        {
            if (DoOnes)
            {
                if (!Finish)
                {
                    Allactions();
                    
                    Finish = true;
                }
            }
            else Allactions();

        }
        else if (pl.IM.enter_b || pl.IM.SpaceB || pl.IM.pick_item || pl.IM.LeftMouseButton)
        {
  


            if (NeedItem == -1)
            {
                if (!DoOnes)
                {

                    Allactions();

                    for (int i = 0; i < TF.Length; i++)
                        TF[i] = !TF[i];

                }
                else
                {
                    if (!Finish)
                    {
                        Allactions();
                        Finish = true;
                    }

                }
            }
            else
            {
                if (pl.inv.CheckItem(NeedItem, NeedCount))
                {
                      
                        if (!Finish)
                        {
                            Allactions();
                            Finish = true;
                        }

                        
                }


            }

        }

                
            
        
       

        
    }




    float convertto(float start, float finish)
    {
        start = Mathf.Lerp(start, finish, (Time.deltaTime) / (Mathf.Clamp(Mathf.Abs(start - finish), 0, 1) * 3));

        return start;
    }


    void Allactions()
    {
        if (pl.HandObject == gameObject)
        return;

        if (DestroyTimerDelay > 0)
        {
            DestroyTimer = Time.fixedTime + DestroyTimerDelay;
            ToDestroy = true;
        }
        GetBack = false;

        for (int i = 0; i < TF.Length; i++)
        {

            if (!TFСhecks[i])
            {
                OnTrigger(TF[i], i);
               
                TFСhecks[i] = true;
            }

            if(!DontSave) SL.SaveLoadCurrent.TriggersActivated.Add(gameObject);

        }
        if (GetComponent<AudioSource>() != null)
        {
            print("PlayAudio " + name);
            GetComponent<AudioSource>().Play();
        }
        pl.LockCamera = LockCamera;
        pl.ZoomCamera = ZoomCamera;

        if(StartQuestID >- 1)
            pl.inv.AddQuest(StartQuestID);


        if (EndQuestID > -1)
        {
            if(!pl.inv.CheckQuestStart(EndQuestID))
            pl.inv.AddQuest(EndQuestID);


            pl.inv.DoneQuest(EndQuestID);
        }

        pl.CutSceneObject = LockOnObject;
        if (ShakeCamera > 0) pl.SetCamShakeTimer(ShakeCamera, 0.1f);

        if (LockCameraTimer > 0) pl.LockCameraTimer = Time.fixedTime + LockCameraTimer;

        if (GameObject.Find("Flashlight") != null)
        {
            if (FlashLightFlickering)
            {
                GameObject.Find("Flashlight").GetComponent<Animator>().
                SetBool("Flickering", !GameObject.Find("Flashlight").GetComponent<Animator>().GetBool("Flickering"));
                //  FlashLightFlickering = false;
                // print("FlashLightFlickering");
            }
        }

        if (NeedItem > -1) pl.inv.ReduceItemCount(NeedItem, NeedCount);

        
    }



    void GoThroughComponents(GameObject obj, bool TF)
    {

       // obj.SetActive(TF);

        if (obj.GetComponent<Outline>() != null)
            obj.GetComponent<Outline>().enabled = TF;


        if (obj.GetComponent<CapsuleCollider>() != null)
            obj.GetComponent<CapsuleCollider>().enabled = TF;

        if (obj.GetComponent<Dialog>() != null)
            obj.GetComponent<Dialog>().enabled = TF;

        if (obj.GetComponent<DamageHeal>() != null)
            obj.GetComponent<DamageHeal>().enabled = TF;


        if (obj.GetComponent<Image>() != null)
            obj.GetComponent<Image>().enabled = TF;

        if (obj.GetComponent<Enemy_Spawner>() != null)
        {
            if (!TF)
            {
                obj.GetComponent<Enemy_Spawner>().SendEnemiesBack();
                obj.GetComponent<Enemy_Spawner>().DisableOnEnemyBack = true;
            }
            else
            {

                obj.GetComponent<Enemy_Spawner>().enabled = true;
            }
        }

        

        if (obj.GetComponent<MoveBetweenSpots>() != null)
            obj.GetComponent<MoveBetweenSpots>().enabled = TF;


        if (obj.GetComponent<NavMeshAgent>() != null)
            obj.GetComponent<NavMeshAgent>().enabled = TF;



        if (obj.GetComponent<Light>() != null)
            obj.GetComponent<Light>().enabled = TF;

        if (TF)
        {
            if (obj.GetComponent<ParticleSystem>() != null)
                obj.GetComponent<ParticleSystem>().Play(true);
        }
        else
        {
            if (obj.GetComponent<ParticleSystem>() != null)
                obj.GetComponent<ParticleSystem>().Stop(true);
        }



        if (obj.GetComponent<Trigger>() != null)
            obj.GetComponent<Trigger>().Finish = false;

        if (obj.GetComponent<BoxCollider>() != null)
            obj.GetComponent<BoxCollider>().enabled = TF;

        if (obj.GetComponent<MeshCollider>() != null)
            obj.GetComponent<MeshCollider>().enabled = TF;
        
        if (obj.GetComponent<SpriteRenderer>() != null)
            obj.GetComponent<SpriteRenderer>().enabled = TF;

        if (obj.GetComponent<MeshRenderer>() != null)
            obj.GetComponent<MeshRenderer>().enabled = TF;

        if (obj.GetComponent<SkinnedMeshRenderer>() != null)
            obj.GetComponent<SkinnedMeshRenderer>().enabled = TF;


        if (obj.GetComponent<Attack>() != null)
            obj.GetComponent<Attack>().enabled = TF;


        for (int i = 0; i < obj.transform.childCount; i++)
        GoThroughComponents(obj.transform.GetChild(i).gameObject, TF);

    }

    void RollBackConroller()
    {
        if (RollBackDelay <= 0) return;
        
        if (RollBackTimer > 0)
        {
            RolledBack = false;
            RollBackTimer -= Time.deltaTime;
        }
        else
        {
            if (!RolledBack )
            {
                for (int i = 0; i < TF.Length; i++)
                {
                    if (TF[i])
                    {
                        OnTrigger(false, i);
                        TF[i] = false;
                    }
                }
                RolledBack = true;
            }
        }
        
    }


    void OutlineManager()
    {

            if (_Outline == null) return;
        Color color = new Color(1, 1, 1, 1);

        if (pl.Legscoll_obj.Contains(gameObject) || pl.ViewColl(gameObject))
        {
         
            if (NeedItem == -1)
            {
                
                    color = new Color(1, 1, 1, 1);
            }
            else
            {
                if (pl.inv.CheckItem(NeedItem, NeedCount))
                {
                    color = new Color(0.1f, 1, 0.1f, 1);
                }
                else color = new Color(1, 0.1f, 0.1f, 1);

            }
        }
        else color = new Color(0, 0, 0, 0);


        if (pl.CutSceneMode)
        {
            color = new Color(0, 0, 0, 0);

        }

        _Outline.OutlineColor = color;
    }



    public void OnTrigger(bool TF, int Obnum)
    {



        if (ONObjects.Length <= 0 || Obnum > ONObjects.Length - 1) return;

        if (ONObjects[Obnum] == null) return;

        if (AnimationName.Length <= 1)
            AnimationName = "Main";



        if (ONObjects[Obnum].GetComponent<Animator>() != null)
        {
            if (TF)
            {
                if (CurrentAnimation != AnimationName)
                {
                    if (RollBackDelay > 0)
                    {
                        RollBackTimer = RollBackDelay;
                    }
  
                    CurrentAnimation = AnimationName;
                }
            }
            else
             CurrentAnimation = "Back";
             

            ONObjects[Obnum].GetComponent<Animator>().CrossFade(CurrentAnimation, 0.1f);
        }

        if (PlayTargetSound)
        {
            if (ONObjects[Obnum].GetComponent<AudioSource>() != null && TF)
            {
                if (!ONObjects[Obnum].GetComponent<AudioSource>().isPlaying)
                {
                    ONObjects[Obnum].GetComponent<AudioSource>().Play();
                    print("PlayAudio " + ONObjects[Obnum].name);
                }
            }

            if (ONObjects[Obnum].GetComponent<AudioSource>() != null && !TF)
            {
                ONObjects[Obnum].GetComponent<AudioSource>().Stop();

            }
        }


    
        if (!OnlyAnim)
        {
          
            GoThroughComponents(ONObjects[Obnum], TF);

                
        }

      
    }

    bool CheckCollTargets()
    {
        bool result = false;

        if (OnlyOnBody)
        {
            if (pl.Legscoll_obj.Contains(gameObject))
                return true;
            else 
                return false;
        }



        if (CollisionTargets == null)
        {

            if (pl.Legscoll_obj.Contains(gameObject) || ((pl.ViewColl(gameObject) ) && !OnlyOnBody))
                return true;

            return false;
        }
        if (CollisionTargets.Length == 0)
        {

            if (pl.Legscoll_obj.Contains(gameObject) || ((pl.ViewColl(gameObject) ) && !OnlyOnBody))
                return true;

            return false;
        }

        for (int i = 0; i < CollisionTargets.Length; i++)
        {
            if (CollisionTargets[i] != null)
            if (GetComponent<CollList>().GetCollList().Contains(CollisionTargets[i]))
            {
                result = true;
                if (DestroyCollisionTargets)
                {
                    pl.DestroyObject(CollisionTargets[i]);
                    pl.MoveExplosion(CollisionTargets[i].transform.position);
                }
            }
        }

        return result;
    }


    bool CheckCollistionWithTarget()
    {
        for (int i = 0; i < CollisionTargets.Length; i++)
        {
            if (CollisionTargets[i] != null)
                if (GetComponent<CollList>().GetCollList().Contains(CollisionTargets[i]))
                {
                    return true;
                }
        }
        return false;
    }
    void PriseControll()
    {
        if (NeedItem <= -1) return;

        if (Prise == null)
        {
            Prise = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Prise"), GameObject.Find("Canvas").transform);
            Prise.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Items/" + inv.GetItemInDatabase(NeedItem).itemNames[0]);
            Prise.SetActive(false);
            Prise.transform.position = new Vector3(9999, 9999, 0);
            Prise.name = name + "Prise";

        }

        if (pl._Menu.MenuONOFF || pl._gameover || pl.inv.showinvent || pl.inv.showjournal || pl.showUpgrades)
        {
            Prise.SetActive(false);
            return;
        }

        Prise.transform.position = new Vector3(
            pl.MainCamera.WorldToScreenPoint(transform.position).x,
        pl.MainCamera.WorldToScreenPoint(transform.position).y, 0);

        Prise.transform.Find("PriseNumber").GetComponent<TextMeshProUGUI>().text = " x " + NeedCount;

        if (pl.Legscoll_obj.Contains(gameObject))
        {

            Prise.SetActive(true);

        }
        else
        {
            Prise.SetActive(false);
        }
    }



    private void OnDestroy()
    {
        if (Prise != null)
            Destroy(Prise);
    }


}
