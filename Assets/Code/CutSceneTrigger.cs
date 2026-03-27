using UnityEngine;




public class CutSceneTrigger : MonoBehaviour
{
    private Player pl;
    private Outline _Outline;
    public Transform Body;


    private bool Play, StartPlacing;

    public Animator Anim;
    private string CurrentAnimState;
    public float CutsceneDuration = 10f;
    private float CutsceneTimer;


    public int Joy;
    public int Trauma;
    public bool RideOnce;
    public bool ResetAnim = true;
    public bool OnButton = true;

    void Start()
    {
      
        _Outline = GetComponent<Outline>();
        pl = InitializeOnAwake.pl;
        if(Anim==null)
        Anim = GetComponent<Animator>();

        if (!pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject) && Joy>0)
            JoyCheck.AddJoyObject(gameObject);
    }

   
    void Update()
    {
        OutlineManager();

        if (pl._gameover)
        {
         
            pl.SL.SaveLoadCurrent.TriggersActivated.Remove(gameObject);
            return;
        }


        if (pl.PlayerMenusPause())
        {
            return;
        }

     

        if (pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject) && !Play && RideOnce)
        {
    
            if (pl.ViewColl(gameObject) && ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && OnButton))
            {
     
                pl._Menu.PlaySoundsPitched(pl._Menu.ErrorClip,1);
            }

            SetAnimState("End");
            return;
        }


    

        if (Play)
        {
        
            SetAnimState("Main");
            if (CutsceneTimer > 0) CutsceneTimer -= Time.deltaTime;
            pl.MainCamera.transform.localEulerAngles = Vector3.zero;

            pl.GetComponent<CapsuleCollider>().isTrigger = true;
            pl.GetComponent<Rigidbody>().useGravity = false;
            pl.GetComponent<Rigidbody>().isKinematic = true;
            pl._transform.localEulerAngles = Vector3.zero;

            pl.transform.localPosition = Vector3.zero;

            if (CutsceneTimer <= 0)
            {
                StopScene();
            }


        }

        if (!Play && StartPlacing)
        {
            pl.GetComponent<CapsuleCollider>().isTrigger = true;
            pl.GetComponent<Rigidbody>().useGravity = false;
            pl.GetComponent<Rigidbody>().isKinematic = true;


            pl.transform.localPosition = new Vector3(
                Mathf.Lerp(pl.transform.localPosition.x, 0, Time.deltaTime*15 ),
                Mathf.Lerp(pl.transform.localPosition.y, 0, Time.deltaTime*15),
                Mathf.Lerp(pl.transform.localPosition.z, 0, Time.deltaTime*15));

   

            pl.MainCamera.transform.localRotation = Quaternion.Lerp(
                       pl.MainCamera.transform.localRotation, Quaternion.Euler(0,0,0),Time.deltaTime * 15 );

            pl.transform.localRotation = Quaternion.Lerp(
                      pl.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 15);

            pl.ResetCamera();

            if (Mathf.Abs( pl.transform.localPosition.x) < 0.01f &&
                Mathf.Abs(pl.transform.localPosition.y) < 0.01f &&
               Mathf.Abs(pl.transform.localPosition.z) < 0.01f)
            {
             
                Play = true;
                CutsceneTimer = CutsceneDuration;
            }
        }


        if (CollCheck() && !Play && !StartPlacing)
        {
            
            StartPlacing = true;
            if (Body != null)
            {
                pl.transform.parent = Body;
            }
            pl.CutSceneMode = true;
        
        }



    }

    void SetAnimState(string _name)
    {
        if (CurrentAnimState == _name) return;

       
               Anim.Play(_name, 0);
                CurrentAnimState = _name;

         

    }


    void StopScene()
    {
     
        pl.GetDamage(Trauma);
        pl.Heal(Joy);

        if (!pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject))
        {

            JoyCheck.RemoveJoyObject(gameObject);
            pl.SL.SaveLoadCurrent.TriggersActivated.Add(gameObject);
        }


        pl.CutSceneMode = false;
        StartPlacing = false;
        Play = false;
        CurrentAnimState = "";

        pl.transform.parent = null;

        if (Body.parent!=null)
            pl.MainCamera.transform.eulerAngles = Body.eulerAngles + Body.parent.eulerAngles;

        pl.SetCameraRotation(Body.eulerAngles.y );

       // pl.transform.rotation = Body.rotation;

        pl.GetComponent<CapsuleCollider>().isTrigger = false;
        pl.GetComponent<Rigidbody>().useGravity = true;
        pl.GetComponent<Rigidbody>().isKinematic = false;

        if (pl.Legscoll_obj.Contains(gameObject)) pl.Legscoll_obj.Remove(gameObject);

        if(ResetAnim)
        SetAnimState("Start");

      
    }


    bool CollCheck()
    {
        if ((pl.ViewColl(gameObject) && (pl.IM.enter_b || pl.IM.LeftMouseButtonDown)) && OnButton)
            return true;
        if (pl.Legscoll_obj.Contains(gameObject) && ! OnButton)
            return true;

        return false;
    }
    private void OnDisable()
    {
        JoyCheck.RemoveJoyObject(gameObject);

        if (!pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject))
        {
            pl.SL.SaveLoadCurrent.TriggersActivated.Add(gameObject);
            pl.SL.SaveLoadCurrent.TriggersActivated.Add(gameObject);
        }
    }


    void OutlineManager()
    {
        if (_Outline == null) return;
        
        if (pl.PlayerMenusPause())
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }
        if(pl.CutSceneMode)
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }


        if (pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject) && !Play && RideOnce)
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }



            if (pl != null && pl.ShowAllDamageHeal <= 0)
        {
            if (pl.Legscoll_obj.Contains(gameObject) || pl.ViewColl(gameObject))
            {
                if (!pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject))
                    _Outline.OutlineColor = new Color(1, 1, 1, 1);
                else _Outline.OutlineColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            }
            else _Outline.OutlineColor = new Color(0, 0, 0, 0);

        }


        if (pl.ShowAllDamageHeal > 0 && !pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject))
        {
           _Outline.OutlineColor = new Color(1, 1, 1, 1);
        }


    }



}
