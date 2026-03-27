using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayDraw : MonoBehaviour
{
    public int TargetDay;
    public int ActiveAfterDay = -1;
    public int DeactiveAfterDay = -1;
    private Player pl;
    private int localday;

    private Animator Anim;

    public float DayStartDelay = 0;

    private float AnimSpeedStart;
    private bool JumpToTime;
    void Start()
    {
        Anim = GetComponent<Animator>();
        if(Anim!=null)
        AnimSpeedStart = Anim.speed;
        pl = InitializeOnAwake.pl;
        localday = -1;
        
        if (Anim != null) Anim.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

       
    }


    void Update()
    {
     
        if (!pl.StartLoading)
        {
            if (!JumpToTime)
            {
                if (Anim != null)
                {
                    if(Anim.GetCurrentAnimatorClipInfo(0).Length>0 && pl.DayNight!=null)
                    Anim.PlayInFixedTime(Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name, 1, pl.SL.SaveLoadCurrent.DayTimer);
                }
                JumpToTime = true;
            }
        }


        if (pl._Menu.MenuONOFF)
        {
            if (Anim != null) Anim.speed = 0;
            return;
        }
        else if (Anim != null) Anim.speed = AnimSpeedStart;


        if (TargetDay > -1)
        {
       
            if (pl.SL.SaveLoadCurrent.DayNumber == TargetDay)
            {
              
                if (localday != pl.SL.SaveLoadCurrent.DayNumber)
                {
                    print("pl.DayNight.DayNumber " + pl.SL.SaveLoadCurrent.DayNumber);
                    if (Anim != null) Anim.enabled = true;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(true);
                    localday = pl.SL.SaveLoadCurrent.DayNumber;
                }
            }
            else
            {
                if (localday != pl.SL.SaveLoadCurrent.DayNumber)
                {
                    if (Anim != null) Anim.enabled = false;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(false);
                    localday = pl.SL.SaveLoadCurrent.DayNumber;
                }
            }
        }

        if (pl.DayNight == null)
        {
          
            return;
        }


        if (pl.DayNight.DayTimer < DayStartDelay) return;

        if (pl.DayNight.IsDayChangedThisFrame())
        {
            if (Anim != null) Anim.enabled = false;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

        }

     

        if (ActiveAfterDay > -1)
        {
            if (pl.DayNight.DayNumber > ActiveAfterDay)
            {
                if (localday != pl.DayNight.DayNumber)
                {
                    if (Anim != null) Anim.enabled = true;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(true);
                    localday = pl.DayNight.DayNumber;
                }
            }
            else
            {
                if (localday != pl.DayNight.DayNumber)
                {
                    if (Anim != null) Anim.enabled = false;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(false);
                    localday = pl.DayNight.DayNumber;
                }
            }

        }


        if (DeactiveAfterDay > -1)
        {
            if (pl.DayNight.DayNumber > DeactiveAfterDay)
            {
                if (localday != pl.DayNight.DayNumber)
                {
                    if (Anim != null) Anim.enabled = false;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(false);
                    localday = pl.DayNight.DayNumber;
                }
            }
            else
            {
                if (localday != pl.DayNight.DayNumber)
                {
                    if (Anim != null) Anim.enabled = true;
                    for (int i = 0; i < transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(true);
                    localday = pl.DayNight.DayNumber;
                }
            }

        }
    }
}
