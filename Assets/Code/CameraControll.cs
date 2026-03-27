using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public Camera[] Cameras;
    public GameObject[] CamerasTriggers;
    public Vector3[] CamerasPos;


    private int CurrentCamera;
    

    private Player pl;
    private bool CheckOnStart = true;
    private void Awake()
    {
        pl = InitializeOnAwake.pl;
        CamerasPos = new Vector3[Cameras.Length];

        for (int i = 0; i < Cameras.Length; i++)
        {
            CamerasPos[i] = Cameras[i].transform.position;
        }


        for (int i = 0; i < Cameras.Length; i++)
        {
                Cameras[i].enabled = false;
        }

        Cameras[0].enabled = true;

    }


    void Update()
    {
       

        for (int i = 0; i < Cameras.Length; i++)
        {
           


            if (CamerasTriggers[i] != null)
            {
                if (pl.ViewColl(CamerasTriggers[i]) && Cameras[i].enabled)
                {


                    if (i < Cameras.Length - 1)
                    {
                        Cameras[i].enabled = false;
                        Cameras[i + 1].enabled = true;
                    }


                    break;

                }
            }

        }
    }
}
