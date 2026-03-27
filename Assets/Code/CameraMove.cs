using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour
{
        public Camera[] m_Cameras;
        private float Timer;
    private int CameraNum;
    private void Start()
    {
        Timer = Time.fixedTime + 20;
    }

    void Update()
        {

     
       

        if (Timer < Time.fixedTime)
        {
            CameraNum++;
            Timer = Time.fixedTime + 20;
        }

        if (CameraNum >= m_Cameras.Length) CameraNum = 0;

        }

    public void SetCamNum(int n)
    {
        CameraNum += n;
  
        if (CameraNum >= m_Cameras.Length) CameraNum = 0;

        m_Cameras[CameraNum].enabled = true;

        if (CameraNum > 0)
        m_Cameras[CameraNum - 1].enabled = false;
    

    }
}

