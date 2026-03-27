using UnityEngine;


public class DamageHeal : MonoBehaviour
{
    public int Trauma = 1;
    public bool DoNoDestroy;
    public int Joy = 0;
    public string PlayerAnimation;

    public int AddItem = -1;
    public int AddItemCount = 1;
    public int TimeAnxiety = 0;

    public float ReEnableDelay = 0.8f;

    private Player pl;
    private float Timer, StartTimer;

    public bool Explode = true;


    public Vector2 DamageDelay2 = new Vector2(0, 0);
    private float DamageTimer = 0;

    public bool OnColl; 
    public bool OnBody;
    private bool isColliding;


    private AudioSource AS;
    public Animator Anim;
    private Outline _Outline;
    private Color StartColor;

    public AudioClip AudioOnAnimStop;
    public AudioClip AudioOnPickUp;
    private float AnimSpeedOnPlay;
    private MeshRenderer Mesh;
    private BoxCollider Box;
    private MeshCollider MechColl;

    private bool isPicked;
    private CollList Coll;
    public bool IgnoreTotalJoy;
    void Start()
    {
        if (!IgnoreTotalJoy)
        {
            if(Joy>0)
            JoyCheck.AddJoyObject(gameObject);
        }

        Mesh = GetComponent<MeshRenderer>();
        Box = GetComponent<BoxCollider>();
        MechColl = GetComponent<MeshCollider>();
        Coll = GetComponent<CollList>();

        pl = InitializeOnAwake.pl;
     
        StartTimer = Time.fixedTime + 1;
       
        AS = GetComponent<AudioSource>();

        if (Anim == null)
        {
            Anim = GetComponent<Animator>();
            
        }

        DamageTimer = Random.Range(DamageDelay2.x, DamageDelay2.y);
        if (Anim != null)
        {
            Anim.Play("Start");
            Anim.speed = 60 / DamageTimer;
        }


        Timer = -1;

        _Outline = GetComponent<Outline>();

        if (_Outline != null)
        {
            StartColor = GetComponent<Outline>().OutlineColor;
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
        }

        while (GameObject.Find(name) != null && GameObject.Find(name) != gameObject)
            name += "N";

    }


    void Update()
    {
        if (pl.Pl_Anim != null)
        {
            if (pl.Pl_Anim.AnimationPause > 0)
            {
                if (_Outline != null)
                {
                    _Outline.OutlineColor = new Color(0, 0, 0, 0);
                    _Outline.enabled = false;
                }

                return;

            }
        }


        if (pl.SL.SaveLoadCurrent.ObjectsToDestroy.Contains(gameObject.name) && !DoNoDestroy)
        {
       
            Destroy(gameObject);
            return;
        }
        OutlineManager();

        if (Coll != null)
        {
            if (DamageTimer > 0)
            {
                DamageTimer -= Time.deltaTime;
             

            }
            if (Coll.rayhit.Contains(pl.gameObject) && DamageTimer <= 0)
            {
                UseItem();
                DamageTimer = 1;
            }
            return;
        }
       
        if (pl.PlayerPause())
        {
            if (Anim != null) Anim.speed = 0;
            return;

        }
        else if (Anim != null)
        {
            if(Anim.speed == 0)
            Anim.speed = AnimSpeedOnPlay;
        }

        if (Anim != null)
            AnimSpeedOnPlay = Anim.speed;





        if (StartTimer > Time.fixedTime && (DamageDelay2.x == 0 && DamageDelay2.y == 0)) return;

        if (DamageDelay2.x > 0 && DamageDelay2.y > 0)
        {
            DamageDelay();

            return;
        }

   


        if (((pl.ViewColl(gameObject) && !OnBody) || (pl.Legscoll_obj.Contains(gameObject) && OnBody))
            && (((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && !OnColl) || (OnColl && !isColliding)) && Timer < Time.fixedTime)
        {
            PickItem();
            isColliding = true;
        }

        if ((!pl.ViewColl(gameObject) && !OnBody) || 
            (!pl.Legscoll_obj.Contains(gameObject) && OnBody))
            isColliding = false;


        if (Timer - 0.1f < Time.fixedTime && Timer > Time.fixedTime && Explode)
        {
           /* if (explosionint > -1)
                pl.Explosions[explosionint].transform.position = new Vector3(99999, 99999, 99999);

            if (GetComponent<SphereCollider>() != null)
                GetComponent<SphereCollider>().enabled = true;

            if (Box!= null)
                Box.enabled = true;

            if (MechColl != null)
                MechColl.enabled = true;
            if(Mesh!=null)
            Mesh.enabled = true;
       
            */
        }

    }
    void DamageDelay()
    {

        if (DamageTimer > 0)
        {
            DamageTimer -= Time.deltaTime;
            if (Anim != null)
            {
                Anim.Play("Main");
            }

        }



        if (pl.ViewColl(gameObject) && (((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && !OnColl) || (OnColl && !isColliding)) && Timer < Time.fixedTime)
        {
            if (AS != null) AS.Play();

            if (AudioOnAnimStop != null)
                pl.PlaySoundsPitched(AudioOnAnimStop, 1);

            float Delay = Random.Range(DamageDelay2.x, DamageDelay2.y);
            if (Anim != null)
            {
                Anim.Play("Start");
                Anim.speed = 60 / Delay;
            }

            DamageTimer = Delay;

        }

        if (DamageTimer <= 0)
        {
            float Delay = Random.Range(DamageDelay2.x, DamageDelay2.y);
            print(name + "DamageTimer < 0");

            PickItem();

            if (AS != null)
                AS.Play();




            if (Anim != null)
            {
                Anim.Play("Start");
                Anim.speed = 60 / Delay;
            }


            DamageTimer = Delay;
        }

    }
    public void ResetTimer()
    {
        Timer = -1;
    }



    void PickItem()
    {
        if (PlayerAnimation.Length > 0)
        {
            if (pl.Pl_Anim != null)
                pl.Pl_Anim.SetPickedAnimatedObject(PlayerAnimation, gameObject);
           
            
            pl.PlaySoundsPitched(AudioOnPickUp, 1);

            if (_Outline != null)
            {
                _Outline.OutlineColor = new Color(0, 0, 0, 0);
                _Outline.enabled = false;
            }

            return;

        }


        UseItem();
    }


    public void UseItem()
    {
      

        if(AddItem>-1)
        pl.inv.AddItem(AddItem, AddItemCount, 99, 0);


        isPicked = true;
        pl.Heal(Joy);

        
        if (Trauma > 0 && !pl.CutSceneMode)
        {
            pl.GetDamage(Trauma);
            
            if (Explode)
            pl.MoveExplosion(transform.position);
        }

        if (!DoNoDestroy)
        {
            pl.SL.SaveLoadCurrent.ObjectsToDestroy.Add(gameObject.name);
            pl.DestroyObject(gameObject);
        }
        Timer = Time.fixedTime + ReEnableDelay;

    }

    private void OnDisable()
    {
        JoyCheck.RemoveJoyObject(gameObject);
    }

    private void OnDestroy()
    {
        
            JoyCheck.RemoveJoyObject(gameObject);
    }

    void OutlineManager()
    {
        if (_Outline == null) return;


     


        if (pl.CutSceneMode)
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }

        if (pl.PlayerMenusPause())
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }

        if (pl.ShowAllDamageHeal > 0)
        {
        _Outline.enabled = true;
        _Outline.OutlineColor = new Color(StartColor.r, StartColor.g, StartColor.b, 1);
        }
        else
        {
            if (pl.ViewColl(gameObject))
            {
            _Outline.OutlineColor = new Color(StartColor.r, StartColor.g, StartColor.b, 1);
            _Outline.enabled = true;
            }
            else
            {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            _Outline.enabled = false;
            }
        }
        
    }

}