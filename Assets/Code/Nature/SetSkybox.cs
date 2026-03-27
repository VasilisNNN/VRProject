using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkybox : MonoBehaviour
{
   // private Camera MainCamera;
    public Material skybox;
    private void Update()
    {
        RenderSettings.skybox = skybox;
    }
}
