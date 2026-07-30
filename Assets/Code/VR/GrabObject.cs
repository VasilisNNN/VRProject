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
    void Start()
    {
        pl = InitializeOnAwake.pl;
        IM = InitializeOnAwake.IM;
        _rigidbody = GetComponent<Rigidbody>();
        _Outline = GetComponent<Outline>();

        for (int i = 0; i < transform.Find("Colliders").transform.childCount; i++)
            Colliders.Add(transform.Find("Colliders").transform.GetChild(i).GetComponent<BoxCollider>());

    }

    void Update()
    {
        if (pl.Viewcoll_obj_Ray_right.Contains(gameObject)) _Outline.enabled = true;
        else _Outline.enabled = false;

        if (pl.ViewColl(gameObject) && IM.enter_b)
        {

            transform.parent = pl.VRCamera.rightHandAnchor;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            _rigidbody.isKinematic = true;

        }

        if (IM.exit_b)
        {

            transform.parent = null;
            _rigidbody.isKinematic = false;

        }


    }
}
