using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabObject : MonoBehaviour
{
    private Player pl;
    private InputMode IM;
    private Rigidbody _rigidbody;
    private Outline _Outline;
    private List<BoxCollider> Colliders = new List<BoxCollider>();

    public bool inHand { get; set; }
    public bool inRightHand, inLeftHand;

    public int ID;

    void Start()
    {
        pl = InitializeOnAwake.pl;
        IM = InitializeOnAwake.IM;
        _rigidbody = GetComponent<Rigidbody>();
        _Outline = GetComponent<Outline>();

        for (int i = 0; i < transform.Find("Colliders").transform.childCount; i++)
            Colliders.Add(transform.Find("Colliders").transform.GetChild(i).GetComponent<BoxCollider>());
        _Outline.OutlineColor = new Color(1, 1, 1, 1);


    }

    void OutlineManager()
    {
        if (inHand) 
        {
            _Outline.enabled = false; 
            return; 
        }

        if (RightCollissionCheck() || LeftCollissionCheck()) 
        { 
            _Outline.enabled = true;
            return;
        }


         _Outline.enabled = false;
    }


    bool RightCollissionCheck()
    {
        if(pl.RightHandObject !=null) return false;
        for (int i = 0; i < Colliders.Count; i++)
        {
            if (pl.Viewcoll_obj_Ray_right.Contains(Colliders[i].gameObject))
                return true;
            
        }
        return false;

    }


    bool LeftCollissionCheck()
    {
        if (pl.LeftHandObject != null) return false;

        for (int i = 0; i < Colliders.Count; i++)
        {
            if (pl.Viewcoll_obj_Ray_left.Contains(Colliders[i].gameObject))
                return true;

        }
        return false;

    }

    void Update()
    {

        OutlineManager();



        if ((RightCollissionCheck() || LeftCollissionCheck()) && IM.enter_b)
        {
            for (int i = 0; i < Colliders.Count; i++)
            {
                Colliders[i].isTrigger = true;
            }


            if (RightCollissionCheck())
            {
                pl.Right_VRHand.isHolding = true;
                inRightHand = true;
                transform.parent = pl.RightHandAnchor;
                _rigidbody.position = pl.RightHandAnchor.position;
                pl.RightHandObject = gameObject;

            }




            else if (LeftCollissionCheck())
            {
                pl.Left_VRHand.isHolding = true;
                inLeftHand = true;
                transform.parent = pl.LeftHandAnchor;
                _rigidbody.position = pl.LeftHandAnchor.position;
                pl.LeftHandObject = gameObject;
            }


        
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            _rigidbody.isKinematic = true;
            inHand = true;

           
        }


        if (IM.exit_b)
        {
            if (pl.LeftHandObject = gameObject) pl.LeftHandObject = null;
            if (pl.RightHandObject = gameObject) pl.RightHandObject = null;

            for (int i = 0; i < Colliders.Count; i++)
            {
                Colliders[i].isTrigger = false;
            }

            if (inLeftHand) pl.Left_VRHand.isHolding = false;
            if (inRightHand) pl.Right_VRHand.isHolding = false;



            inHand = false;
            transform.parent = null;
            _rigidbody.isKinematic = false;

        }


    }
}
