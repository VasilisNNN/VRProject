using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithOther : MonoBehaviour
{
    private Vector3 StartDifference;
    public Transform Target;

    public Vector3 SpecificStart = Vector3.zero;


    public bool StableConnect;
    void Start()
    {
        StartDifference = transform.position - Target.position;
    }
    

    void Update()
    {
        if (StableConnect)
        {
            transform.position = Target.position;
            return;
        }


        if (SpecificStart == Vector3.zero)
        transform.position = Target.position + StartDifference;
        else
            transform.position = Target.position + SpecificStart;


    }
}
