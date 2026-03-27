using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class StatsControll : MonoBehaviour
{
    public int HP = 3;


    #if UNITY_STANALONE
    [Separator]
     #endif

    public int DatabaseID = -1;
    public bool AddItemAutomaticly;
    public bool DropFixedAmount;

    public int[] ItemIDs;
    public int ItemCount = 1;


    public bool RNDItemDrop;



    public enum SoundType { Wood, Metal, Flesh, Plant, Silent, NoSoundType };


    public SoundType _SoundType;

    public AudioClip DeathClip;
    
    public bool Stunned { get; set; }


    public int MAXHP { get; set; }
    public float InvisTimer { get; set; }

    public Material StartMaterial { get; set; }

    private List<AudioClip> DamageClips = new List<AudioClip>();
    public List<AudioClip> CollisionClips = new List<AudioClip>();
    public bool AudioPlayed;
    
    public bool ReduceAlphaOnColl;
    public Material NewMaterialOnColl;

    public float CollMaterialTimer { get; set; }
    public bool StartColl { get; set; }
    
    
    public GameObject HPUI { get; set; }

    
    private Player pl;
    private Inventory inv;

    private Gun gun;


    public bool Friend;

    private Material StunMaterial;

    
    private AudioClip WorkerRebirthClip;

    
    [HideInInspector]
    public bool DrawUI;
    [HideInInspector]
    private bool DrawHP;

    
    private CollList Coll;


    private AudioSource AU;
    

    [HideInInspector]
    public bool InList = false;
    
    private float StunnedDelay;

    private bool InCamera;


    public List<GameObject> Colliders;
    [HideInInspector]
    public string SpawnPointName = "";
    private void Awake()
    {
        
        ChangeTheName();
        
        pl = InitializeOnAwake.pl;
        inv = InitializeOnAwake.pl.GetComponent<Inventory>();


    


    }


    private void Start()
    {
     
        AU = GetComponent<AudioSource>();
        
        Coll = GetComponent<CollList>();
        
        StunMaterial = Resources.Load<Material>("Materials/DoodleHorizontal");
        WorkerRebirthClip = Resources.Load<AudioClip>("Sound/Objects/WorkerRebirth_1");
        

        gun = InitializeOnAwake.pl.GetComponent<Gun>();
        
       /* HPUI = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/HPUI"), GameObject.Find("Canvas").transform);
        HPUI.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.6f * 1.2f, transform.position.z));
        HPUI.transform.SetAsFirstSibling();

        ONOFF(HPUI, false);*/

        

        MAXHP = HP;
        
        if(DeathClip==null)
        SetDeathClip();

        //&& !constr.OBOnBoard.Contains(new ObjectOnBoard(DatabaseID, transform.position, inv.GetItemInDatabase(DatabaseID).itemNames[0], gameObject))

        // HungerTimer = pl.DayNight.DayLength / 1.5f;


        //if (GameObject.Find(name) != null && GameObject.Find(name) != gameObject) Destroy(gameObject);

        
     
    }

    private void Update()
    {
        if (pl.SL.SaveLoadCurrent.ObjectsToDestroy.Contains(gameObject.name) && SpawnPointName.Length<=0)
            Destroy(gameObject);

        // UIControll();

        if (HP <= 0) ObjectsDeath();

        CheckInCamera();
    }


    void CheckInCamera()
    {
        float maxAngle = 45;

        Vector3 directionToObject = (transform.position - pl.MainCamera.transform.position).normalized;
        
        Vector3 cameraForward = pl.MainCamera.transform.forward;
        
        float angle = Vector3.Angle(cameraForward, directionToObject);
        
        if (angle <= maxAngle)
        {
            InCamera = true;
        }
        else
        {
            InCamera = false;
     
        }
    }

    void SetDeathClip()
    {
        
        if (_SoundType == SoundType.Wood)
        {
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Wood/Club/Club_On_Wood_Club_1_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Wood/Club/Club_On_Wood_Club_2_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Wood/Club/Club_On_Wood_Club_3_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Wood/Club/Club_On_Wood_Club_4_Short"));

            DeathClip = Resources.Load<AudioClip>("Sound/Sound Library - Battle/Sword/Sword_On_Wood/Wood/Sword_On_Wood_Wood_2");
        }

        if (_SoundType == SoundType.Metal)
        {
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Metal/Metal/Club_On_Metal_Metal_1_Short"));

            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Metal/Metal/Club_On_Metal_Metal_2_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Metal/Metal/Club_On_Metal_Metal_3_Short"));

            DeathClip = Resources.Load<AudioClip>("Sound/Sound Library - Battle/Sword/Sword_On_Metal/Metal/Sword_On_Metal_Metal_1");

        }

        if (_SoundType == SoundType.Flesh)
        {
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Flesh/Club_On_Flesh_Flesh_1_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Flesh/Club_On_Flesh_Flesh_2_Short"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Flesh/Club_On_Flesh_Flesh_3_Short"));

            DeathClip = Resources.Load<AudioClip>("Sound/Sound Library - Battle/Sword/Sword_On_Flesh/Flesh/Sword_On_Flesh_Flesh_1_Short");

        }

        if (_SoundType == SoundType.Plant)
        {
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Club/Club_On_Flesh_Club_1"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Club/Club_On_Flesh_Club_2"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Sound Library - Battle/Club/Club On Flesh/Club/Club_On_Flesh_Club_0"));


            DeathClip = Resources.Load<AudioClip>("Sound/Sound Library - Battle/Sword/Sword_On_Metal/Impact/Sword_On_Metal_Impact_3");

        }

        if (_SoundType == SoundType.Silent)
        {
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Objects/Slurp"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Objects/Slurp"));
            DamageClips.Add(Resources.Load<AudioClip>("Sound/Objects/Slurp"));

            DeathClip = Resources.Load<AudioClip>("Sound/Objects/Slurp");

        }

        if (_SoundType == SoundType.NoSoundType)
        {
            DeathClip = null;

        }
    }

    public void ChangeTheName()
    {
       while(GameObject.Find(name) != null && GameObject.Find(name)!=gameObject)
            name += "1";

    }

 

    public void GetDamage(int damage)
    {
        if (damage == 0 || InvisTimer > Time.fixedTime) return;

        if (DamageClips.Count > 0)
        {
            int rnd = UnityEngine.Random.Range(0, DamageClips.Count);
            if (rnd >= DamageClips.Count) rnd = 0;
          
            PlaySoundsPitched(DamageClips[rnd], 1);
        }

        /* 
        if(pl.Showdamage)
        inv.ADDPickedName(damage.ToString(), 1, 1, transform.position);
        */

       // ONOFF(HPUI, true);

        HP -= damage;

      
        InvisTimer = Time.fixedTime + 0.05f;

    }

    public void CollisionAudio()
    {
        if (CollisionClips.Count > 0 && AU != null)
        {
            AU.clip = CollisionClips[UnityEngine.Random.Range(0, CollisionClips.Count)];
            AU.Play();
        }

    }




    


    
    public void PlaySoundsPitched(AudioClip AC, float pitch)
    {
        //print("P");
        pl.GetComponent<AudioSource>().clip = AC;
        pl.GetComponent<AudioSource>().pitch = pitch;
        pl.GetComponent<AudioSource>().Play();
    }

    void UIControll()
    {
        if ((transform.position - pl._transform.position).magnitude > 40)
        {
            if (DrawHP)
            {
                ONOFF(HPUI, false);
                DrawHP = false;
            }

            return;
        }

        if (HP < MAXHP)
        {
            if (!DrawHP)
            {
                ONOFF(HPUI, true);
                DrawHP = true;
            }
        }
        

        Vector2 SliderSize = new Vector2(3.5f / pl.MainCamera.orthographicSize, 3.5f / pl.MainCamera.orthographicSize) / 1.7f;

        if (HPUI != null )
        {
            if (InCamera)
            {
                HPUI.transform.position = pl.MainCamera.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.6f * 1.2f, transform.position.z));
            }else
                HPUI.transform.position = new Vector3(9999,9999,0);

            HPUI.transform.localScale = SliderSize;

            float hpx = (float)HP / (float)MAXHP;
            HPUI.transform.Find("Slider").transform.localScale = new Vector3(hpx, 1, 1);
        }
        

    }


 

    void ObjectsDeath()
    {
       
        if (pl.Enemies.Contains(gameObject))
            pl.Enemies.Remove(gameObject);


        // Destroy(HPUI);
        pl.SL.SaveLoadCurrent.ObjectsToDestroy.Add(gameObject.name);
        BlowObject();

        if(GetComponent<Attack>()!=null)
            GetComponent<Attack>().Death();


        if (ItemIDs.Length > 0)
        {
            if (RNDItemDrop)
            {
               int r = UnityEngine.Random.Range(0, ItemIDs.Length);

                if (ItemIDs[r] > -1)
                {
                    inv.AddItem(ItemIDs[r],
                          ItemCount, 99,
                          inv.GetItemInDatabase(ItemIDs[r]).AmmoID);

                    print("ADD NEW ITEM");
                }
            }
            else
            {
                for(int i=0; i< ItemIDs.Length; i++)
                    inv.AddItem(ItemIDs[i], ItemCount, 99, inv.GetItemInDatabase(ItemIDs[i]).AmmoID);
                
            }
        }



    }

   

    public void BlowObject()
    {
        if (_SoundType == StatsControll.SoundType.Flesh)
            MoveFleshDeathEffect();
        else MoveExplosion();


        Destroy(gameObject);
    
    }


    void MoveExplosion()
    {
   
        for (int i = 0; i < pl.Explosions.Count; i++)
        {
            if (pl.Explosions[i].transform.position == new Vector3(99999, 99999, 99999))
            {
                pl.Explosions[i].transform.position = transform.position;
                pl.Explosions[i].GetComponent<Animator>().Play("Start");

                if (!pl.Explosions[i].GetComponent<AudioSource>().isPlaying)
                    pl.Explosions[i].GetComponent<AudioSource>().Play();
                
                break;
            }
        }

    }



    void MoveFleshDeathEffect()
    {

        for (int i = 0; i < pl.FleshDeath.Count; i++)
        {
            if (pl.FleshDeath[i].transform.position == new Vector3(99999, 99999, 99999))
            {
                pl.FleshDeath[i].transform.position = transform.position;
                pl.FleshDeath[i].GetComponent<Animator>().Play("Start");

                if (!pl.FleshDeath[i].GetComponent<AudioSource>().isPlaying)
                    pl.FleshDeath[i].GetComponent<AudioSource>().Play();

                break;
            }
        }

    }




    public void ONOFF(GameObject g, bool TF)
    {
        if (g.GetComponent<Image>() != null)
            g.GetComponent<Image>().enabled = TF;

        if (g.GetComponent<Text>() != null)
            g.GetComponent<Text>().enabled = TF;

        if (g.GetComponent<SpriteRenderer>() != null)
            g.GetComponent<SpriteRenderer>().enabled = TF;


        if (g.GetComponent<BoxCollider2D>() != null)
            g.GetComponent<BoxCollider2D>().enabled = TF;


        if (g.GetComponent<AudioSource>() != null)
            g.GetComponent<AudioSource>().Play();

        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (g.transform.GetChild(i).GetComponent<Image>() != null)
                g.transform.GetChild(i).GetComponent<Image>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                g.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<BoxCollider2D>() != null)
                g.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<Text>() != null)
                g.transform.GetChild(i).GetComponent<Text>().enabled = TF;


            for (int ii = 0; ii < g.transform.GetChild(i).childCount; ii++)
            {
                if (g.transform.GetChild(i).GetChild(ii).GetComponent<Image>() != null)
                    g.transform.GetChild(i).GetChild(ii).GetComponent<Image>().enabled = TF;

                if (g.transform.GetChild(i).GetChild(ii).GetComponent<Text>() != null)
                    g.transform.GetChild(i).GetChild(ii).GetComponent<Text>().enabled = TF;

            }

        }

    }
}
