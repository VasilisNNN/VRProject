using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HandObject : MonoBehaviour
{
    private Player pl;
    private SaveLoad SL;
    public string Animation_PutInTheHand;
    private Rigidbody rigidbody;
    public bool FixRotation;
    public Vector3 RotationAngles;
    public Vector3 Scale = new Vector3(1,1,1);

    private AudioClip PickClip, DropClip;
    private Outline _Outline;

    void Start()
    {

        pl = InitializeOnAwake.pl;
        SL = InitializeOnAwake.pl.GetComponent<SaveLoad>();

        SL.SaveLoadCurrent.ObjectsToPick.Add(gameObject);
        rigidbody = GetComponent<Rigidbody>();

        PickClip = Resources.Load<AudioClip>("Sound/Objects/PickItem");
        DropClip = Resources.Load<AudioClip>("Sound/Objects/DropItem_General");
        _Outline = GetComponent<Outline>();
    }

   
    void Update()
    {
        if (pl.PlayerMenusPause()) return;
        Gravity();
        Pick();
        OutlineManager();

        if (pl.IM.exit_b)
        {
            DropHandObject();
        }
    }

    void Gravity()
    {
        if (rigidbody == null) return;

        if (pl.HandObject == gameObject)
            rigidbody.useGravity = false;
        else rigidbody.useGravity = true;
    }
    void Pick()
    {
        if (pl.HandObject == gameObject)
        {
            if (FixRotation)
            {
                transform.localEulerAngles = RotationAngles;
      
            }

            transform.localScale = Scale;
            return;

        }
     
        transform.localScale = Vector3.one;

        if (!pl.ViewColl(gameObject) || (!pl.IM.enter_b && !pl.IM.LeftMouseButtonDown)) return;

       

        if (pl.HandObject == null)
        {

            pl.HandObject_Anim = Animation_PutInTheHand;
          
            pl.HandObject = gameObject;
            pl._Menu.PlaySoundsPitched(PickClip,1);

        }


    }

    public void DropHandObject()
    {
        if (pl.HandObject != gameObject) return;

        float Ypos = transform.position.y;
        if (transform.position.y < pl._transform.position.y) Ypos = pl._transform.position.y;

        print("Distanse to wall " + Mathf.Abs((pl.Viewcoll_HitPos - pl._transform.position).magnitude));

        if (Mathf.Abs((pl.Viewcoll_HitPos - pl._transform.position).magnitude) < 0.5f)
            transform.position = pl._transform.position;
        else transform.position = new Vector3(transform.position.x, Ypos, transform.position.z) + pl.MainCamera.transform.forward/2;

        if (rigidbody != null)
            rigidbody.isKinematic = false;

        if (GetComponent<BoxCollider>() != null)
           GetComponent<BoxCollider>().isTrigger = false;

        gameObject.layer = 0;

        pl._Menu.PlaySoundsPitched(DropClip,1);

        gameObject.transform.parent = null;
        pl.HandObject = null;
    }

    void OutlineManager()
    {
        if (_Outline == null) return;

        if (pl.PlayerMenusPause())
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }


        if (pl.CutSceneMode)
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }

        if (pl.HandObject == gameObject)
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;

        }

        Color color = new Color(1, 1, 1, 1);

       
        if (pl.ViewColl(gameObject) || pl.ViewColl(gameObject))
        {
           color = new Color(0.1f, 1, 0, 1);
        }
        else color = new Color(0, 0, 0, 0);

        _Outline.OutlineColor = color;
    }
}
