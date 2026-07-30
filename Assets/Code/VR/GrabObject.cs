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

        if (CollissionCheck()) 
        { 
            _Outline.enabled = true;
            return;
        }


         _Outline.enabled = false;
    }


    bool CollissionCheck()
    {
        for (int i = 0; i < Colliders.Count; i++)
        {
            if (pl.ViewColl(Colliders[i].gameObject))
                return true;
            
        }
        return false;

    }

    void Update()
    {

        OutlineManager();



        if (CollissionCheck() && IM.enter_b)
        {
            for (int i = 0; i < Colliders.Count; i++)
            {
                Colliders[i].isTrigger = true;
            }
                transform.parent = pl.VRCamera.rightHandAnchor;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            _rigidbody.isKinematic = true;
            inHand = true;
        }

        if (IM.exit_b)
        {

            for (int i = 0; i < Colliders.Count; i++)
            {
                Colliders[i].isTrigger = false;
            }


            inHand = false;
            transform.parent = null;
            _rigidbody.isKinematic = false;

        }


    }
}
