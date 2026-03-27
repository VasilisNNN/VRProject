using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransferJoy : MonoBehaviour
{
    private int TargetJoy, SourceJoy;
    private Slider TargetSlider;
    private TextMeshProUGUI SourceJoyText;
    private TextMeshProUGUI TargetJoyText;
    private SaveLoad SL;
    private Player pl;
    private float TransferTimer;

    private AudioSource AS;
    private AudioClip Click;

    void Start()
    {
        Click = Resources.Load<AudioClip>("Sound/UI/Click_1");
        AS = GetComponent<AudioSource>();
        SL = InitializeOnAwake.SL;
        pl = InitializeOnAwake.pl;
        TargetSlider = GameObject.Find("MumSlider").GetComponent<Slider>();
        SourceJoyText = GameObject.Find("SourceJoyText").GetComponent<TextMeshProUGUI>();
        TargetJoyText = GameObject.Find("TargetJoyText").GetComponent<TextMeshProUGUI>();
        SourceJoy = pl.HP;
        TransferTimer = Time.fixedTime + 3;
    }


    void Update()
    {
        TargetJoyText.text = " * " + TargetJoy;
        SourceJoyText.text = " * " + pl.HP;
        pl.CutSceneMode = true;
        pl.InDialog = true;
        if (TransferTimer < Time.fixedTime )
        {
            if (pl.HP > 5 && TargetJoy<10)
            {
                TargetJoy += 1;
                pl.HP -= 1;
                PlaySound(Click);
            }
            else
                pl._Menu.LoadScene_SA("Appartment");

            TransferTimer = Time.fixedTime + 1;

        }

        TargetSlider.value = (float)TargetJoy / 10f;


    }


    void PlaySound(AudioClip clip)
    {

        if (AS.isPlaying) return;
        AS.clip = clip;
        AS.Play();
    }


}
