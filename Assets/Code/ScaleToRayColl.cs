using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToRayColl : MonoBehaviour
{
    private CollList Coll;
    void Start()
    {
        Coll = GetComponent<CollList>();
    }

    
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Coll.HitDistance * 0.006f);
    }
}
