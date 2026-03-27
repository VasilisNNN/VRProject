using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMeter : MonoBehaviour
{
    public Vector3 Direction;
    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3( 
            Target.rotation.eulerAngles.x * Direction.x,
            Target.rotation.eulerAngles.y * Direction.y, 
            Target.rotation.eulerAngles.x * Direction.z);
    }
}
