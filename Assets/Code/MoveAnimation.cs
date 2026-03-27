using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    public Vector3 MAX = new Vector3(1, 1, 1);
    public Vector3 MIN = new Vector3(-1, -1, -1);
    private Vector3 BorderMAX,BorderMIN;
    private float SideZ = 1;
    private float SideY = 1;
    private float SideX = 1;
    public Vector3 Speed;
    public Vector3 Rotation;
    void Start()
    {
        BorderMAX = new Vector3(transform.position.x + MAX.x, transform.position.y + MAX.y, transform.position.z + MAX.z);
        BorderMIN = new Vector3(transform.position.x + MIN.x, transform.position.y + MIN.y, transform.position.z + MIN.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.z > BorderMAX.z && SideZ == 1) SideZ = -1;
        if (transform.position.z < BorderMIN.z && SideZ == -1) SideZ = 1;
        if (transform.position.y > BorderMAX.y && SideY == 1) SideY = -1;
        if (transform.position.y < BorderMIN.y && SideY == -1) SideY = 1;
        if (transform.position.x > BorderMAX.x && SideX == 1) SideX = -1;
        if (transform.position.x < BorderMIN.x && SideX == -1) SideX = 1;

        transform.position = new Vector3(transform.position.x + Speed.x * SideX, transform.position.y + Speed.y * SideY, transform.position.z + Speed.z * SideZ);

        transform.Rotate(Rotation);
    }
}
