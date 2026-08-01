using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionWithObject : MonoBehaviour
{
    public int ObjectID = -1;
    private CollList Coll;

  
    void Start()
    {
        Coll = GetComponent<CollList>();
    }


    void Update()
    {
        for (int i = 0; i < Coll.coll_obj.Count; i++)
        {
            if (Coll.coll_obj[i].GetComponent<GrabObject>().ID == ObjectID)
            {
                Coll.coll_obj[i].transform.parent = transform.parent;
                Coll.coll_obj[i].transform.localPosition = Vector3.zero;
            }
        }

    }
}
