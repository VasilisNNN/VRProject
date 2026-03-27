using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteAudio : MonoBehaviour
{
    public AudioSource[] Audio;
    void OnEnable()
    {
        for (int i = 0; i < Audio.Length; i++)
            if (Audio[i] != null) Audio[i].Pause();
    }


    void OnDisable()
    {
        for (int i = 0; i < Audio.Length; i++)
            if(Audio[i]!=null) Audio[i].UnPause();
    }
}
