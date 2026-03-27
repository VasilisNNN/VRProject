using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPuzzle : MonoBehaviour
{

    public List<GameObject> ObjectsToRotate;
    private int currentobject;
    private float SwitchDelay;
    private Player pl;
    public Camera PuzzleCamera;
    private bool PuzzleStarted;
    Vector3 m_EulerAngleVelocity;
    private Rigidbody m_Rigidbody;

    public Rigidbody StartObject;
    public GameObject BottomObject;
    public GameObject FinishTriggger;
    private Vector3 StartObject_StartPos;


    private float StartPuzzleDelay;

    private AudioSource AS;
    private AudioClip RotationClip, StartClip, EndClip, ResultClip, SwitchClip, FailedClip;

    private GameObject RotationPuzzleUI;
    private bool Done;
    void Start()
    {
        pl = InitializeOnAwake.pl;
        StartObject_StartPos = StartObject.transform.position;

        AS = GetComponent<AudioSource>();

        RotationClip = Resources.Load<AudioClip>("Sound/Objects/RotatingGear");
        StartClip = Resources.Load<AudioClip>("Sound/UI/Typing_0");
        EndClip = Resources.Load<AudioClip>("Sound/UI/Click");
        ResultClip = Resources.Load<AudioClip>("Sound/UI/Ding_0");
        SwitchClip = Resources.Load<AudioClip>("Sound/UI/Typing_1");

       FailedClip = Resources.Load<AudioClip>("Sound/UI/Cancel");

        if (GameObject.Find("RotationPuzzleUI") == null)
        {
            RotationPuzzleUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/RotationPuzzleUI"), GameObject.Find("Canvas").transform);
            RotationPuzzleUI.name = "RotationPuzzleUI";
            RotationPuzzleUI.SetActive(false);
        }
        else RotationPuzzleUI = GameObject.Find("RotationPuzzleUI");
    }

    void Update()
    {
        OutlineConroller();

        if (pl.IM.exit_b || pl.IM.menu_b)
        {
            EndPuzzle();
           
        }

        if (Done)
        {

            for (int i = 0; i < ObjectsToRotate.Count; i++)
                if (currentobject != i)
                    ObjectsToRotate[i].GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 0);

            return;
        
        }

        if (pl.ViewColl(gameObject) && (pl.IM.enter_b || pl.IM.LeftMouseButtonDown))
        {
            StartPuzzle();
        }

        if (!PuzzleStarted)
        {
            for (int i = 0; i < ObjectsToRotate.Count; i++)
                ObjectsToRotate[i].GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 0);

            return;
        }

        if (StartObject.GetComponent<CollList>().coll_obj.Contains(BottomObject))
        {
            StartObject.useGravity = false;
            StartObject.transform.position = StartObject_StartPos;
            StartObject.velocity = Vector3.zero;
            StartObject.angularVelocity = Vector3.zero;
            PlaySoundForced(FailedClip);
        }

        if (StartObject.GetComponent<CollList>().coll_obj.Contains(FinishTriggger))
        {
            StartObject.velocity = Vector3.zero;
            PlaySoundForced(ResultClip);
            Done = true;
        }


        if (StartPuzzleDelay > Time.fixedTime) return;

        if (pl.IM.enter_b || pl.IM.LeftMouseButtonDown)
        {
            StartObject.useGravity = true;
            PlaySoundForced(SwitchClip);
        }


        for (int i = 0; i < ObjectsToRotate.Count; i++)
            if (currentobject != i)
                ObjectsToRotate[i].GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 0);


        ObjectsToRotate[currentobject].GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 1);


        if (pl.IM._vertical > 0 && currentobject > 0 && SwitchDelay < Time.fixedTime)
        {
            currentobject--;
            PlaySoundForced(SwitchClip);
            SwitchDelay = Time.fixedTime + 0.3f;
        }

        if (pl.IM._vertical < 0 && currentobject < ObjectsToRotate.Count-1 && SwitchDelay < Time.fixedTime)
        {
            currentobject++;
            PlaySoundForced(SwitchClip);
            SwitchDelay = Time.fixedTime + 0.3f;
        }

        if (pl.IM._horizontal == 0)
        StopSoundForced(RotationClip);

        m_EulerAngleVelocity = new Vector3(0, 10, 0);
        Quaternion deltaRotationright = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        Quaternion deltaRotationleft = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime*-1);

        if (pl.IM._horizontal < 0)
        {
            m_Rigidbody = ObjectsToRotate[currentobject].GetComponent<Rigidbody>();
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotationright);

            PlaySound(RotationClip);

        }
        if (pl.IM._horizontal > 0)
        {
            m_Rigidbody = ObjectsToRotate[currentobject].GetComponent<Rigidbody>();
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotationleft);

            PlaySound(RotationClip);
        }


    }

    public void StartPuzzle ()
    {
        if (PuzzleStarted) return;

        PlaySoundForced(StartClip);
        RotationPuzzleUI.SetActive(true);
        PuzzleCamera.enabled = true;
        pl.MainCamera.enabled = false;
        pl.CutSceneMode = true;
        PuzzleStarted = true;
        StartPuzzleDelay = Time.fixedTime + 0.1f;
    }

    public void EndPuzzle()
    {
        if (!PuzzleStarted) return;

        PlaySoundForced(EndClip);
        RotationPuzzleUI.SetActive(false);
        PuzzleCamera.enabled = false;
        pl.MainCamera.enabled = true;
        pl.CutSceneMode = false;
        PuzzleStarted = false;
        pl._Menu.ActionDelay = Time.fixedTime + 0.1f;
    }


    void OutlineConroller()
    {
        if (GetComponent<Outline>() == null) return;

        if (pl.ViewColl(gameObject))
        {
             GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 1);
           
        }
        else GetComponent<Outline>().OutlineColor = new Color(0, 0, 0, 0);

    }


    void PlaySound(AudioClip clip)
    {
        if (AS.isPlaying) return;

        AS.clip = clip;
        AS.Play();
    }

    void PlaySoundForced(AudioClip clip)
    {
        
        AS.clip = clip;
        AS.Play();
    }

    void StopSoundForced(AudioClip clip)
    {
        if (!AS.isPlaying || AS.clip!=clip) return;

        AS.Stop();
    }
}
