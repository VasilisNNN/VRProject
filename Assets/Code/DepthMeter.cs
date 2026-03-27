using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthMeter : MonoBehaviour
{
    private GameObject Arrow, WaterSurface;
    private Player pl;
    void Start()
    {
        Arrow = transform.Find("Arrow").gameObject;
        WaterSurface = GameObject.Find("WaterSurface").gameObject;

        pl = InitializeOnAwake.pl;
    }
    
    void Update()
    {
        float ypos = Mathf.Clamp((pl.transform.position.y - (WaterSurface.transform.position.y - 120)) / 330,-0.45f, 0.45f);
        Arrow.transform.localPosition =new Vector3(0, ypos, 0);
    }
}
