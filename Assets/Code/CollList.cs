using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollList : MonoBehaviour
{
  
    public List<GameObject> coll_obj = new List<GameObject>();
    public List<GameObject> rayhit = new List<GameObject>();
  
    public Vector3 HitPos { get; set; }
    public bool Ray;

    public float HitDistance { get; set; }

    public LayerMask mask;
    public float MaxDistance = 30;
    public Transform FinalObject;
    void FixedUpdate()
    {
        if (!Ray) return;
        

        rayhit = new List<GameObject>();
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward*100, new Color(1, 1, 1, 1),0.01f);

        if (Physics.Raycast( ray, out hit, MaxDistance, mask.value))
        {

            HitPos = hit.point;
            HitDistance = hit.distance;

            if (!rayhit.Contains(hit.collider.gameObject))
                rayhit.Add(hit.collider.gameObject);
        }

        if (FinalObject != null)
            FinalObject.position = HitPos - transform.forward*0.1f;


    }



    private void OnTriggerStay(Collider c)
    {

        if (!coll_obj.Contains(c.gameObject))
        {
            coll_obj.Add(c.gameObject);
        }

    }

    private void OnTriggerExit(Collider c)
    {

        if (coll_obj.Contains(c.gameObject))
        {
            coll_obj.Remove(c.gameObject);
        }

    }


    public List<GameObject> GetCollList()
    {
        return coll_obj;
    }
}

