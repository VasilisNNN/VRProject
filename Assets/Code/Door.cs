using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;


public class Door : MonoBehaviour
{
    private Player pl;

    public string Location;
    public int DayPlus;
    public bool OpesSteamPage;

    public bool OnAction;

    private Outline _Outline;
    public bool LoadScneneImmediately;
    private Image FadeOut;

    private bool LoadThroughFade;
    // Start is called before the first frame update
    private float StartDelay;
    void Start()
    {
        if (GameObject.Find("FadeOut") != null)
        {
            FadeOut = GameObject.Find("FadeOut").GetComponent<Image>();
            FadeOut.color = new Color(FadeOut.color.r, FadeOut.color.g, FadeOut.color.b, 0);

        }

        if (LoadScneneImmediately) return;

            pl = InitializeOnAwake.pl;

        _Outline = GetComponent<Outline>();
        if(_Outline!=null)
        _Outline.OutlineColor = new Color(0, 0, 0, 0);
        StartDelay = Time.fixedTime +0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        OutlineConroller();
        if (StartDelay > Time.fixedTime)
        { 
            return;
        }
        if (LoadScneneImmediately)
        {
            if (FadeOut.color.a >= 1)
                pl._Menu.LoadScene_SA(Location);
            else
                FadeOut.color = new Color(FadeOut.color.r, FadeOut.color.g, FadeOut.color.b, FadeOut.color.a + Time.deltaTime*3);
                
            
            return;
        }

        if (LoadThroughFade)
        {
            if (FadeOut.color.a >= 1)
                pl._Menu.LoadScene_SA(Location); 
            else
                FadeOut.color = new Color(FadeOut.color.r, FadeOut.color.g, FadeOut.color.b, FadeOut.color.a + Time.deltaTime * 3);
            return;
        }



        if (!OnAction)
        {

            if (pl.Legscoll_obj.Contains(gameObject) ||
                (pl.ViewColl(gameObject) && (pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && _Outline!=null))
            {
                if(pl.DayNight!=null)
                pl.DayNight.DayNumber += DayPlus;
               
                FixParameters();
               
#if UNITY_STANDALONE
            if (OpesSteamPage) Application.OpenURL("https://store.steampowered.com/app/3687450/Dad_left_me_in_the_car/");
#endif
                if (FadeOut != null)
                {
                    LoadThroughFade = true;
                    return;
                }


                pl._Menu.LoadScene_SA(Location);

            }



                return;
        }



        if (_Outline != null)
        {

            if (pl.ViewColl(gameObject))
            {
                _Outline.OutlineColor = new Color(1, 1, 1, 1);
            }
            else _Outline.OutlineColor = new Color(0, 0, 0, 0);

        }



        if (pl.ViewColl(gameObject) && (pl.IM.enter_b || pl.IM.LeftMouseButtonDown))

        {
            if (pl.DayNight != null)
                pl.DayNight.DayNumber += DayPlus;
          
            FixParameters();
           
#if UNITY_STANDALONE
            if (OpesSteamPage) Application.OpenURL("https://store.steampowered.com/app/3687450/Dad_left_me_in_the_car/");
#endif
            if (FadeOut != null)
            {
                LoadThroughFade = true;
                return;
            }

            pl._Menu.LoadScene_SA(Location);

        }
    }


    void FixParameters()
    {
       

    }


    void OutlineConroller()
    {
        if (_Outline == null) return;
       

        if (pl.ViewColl(gameObject) )
        {

            _Outline.OutlineColor = new Color(1, 1, 1, 1);
          
        }
        else _Outline.OutlineColor = new Color(0, 0, 0, 0);

    }
}
