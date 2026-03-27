using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;


public class DayAndNight : MonoBehaviour
{

    private float DayLength = 600;
    public float DayTimer { get; set; }

    public Color DayLight;
    public Color DawnLight;
    public Color NightLight;

    public enum DayCycle { Morning, Day, Dawn, Night, AllTime};
    public DayCycle Day_Cycle;
    public Light DayGlobalLight;

    private GameObject DayNightCycleText;
    
    public AudioSource[] AudioSources;
    private string DayTime;

    public Vector2 _morningborder_Div = new Vector2(9999999999, 9999999999);
    public Vector2 _dayborder_Div = new Vector2(9999999999, 2);
    public Vector2 _dawnborder_Div = new Vector2(2, 1.5f);
    public Vector2 _nightborder_Div = new Vector2( 1.5f, 1);

    private Vector2 _morningborder, _dayborder, _dawnborder, _nightborder;


  
    public GameObject Arrow;
    private float ArrowRotation;

    private Player pl;
    private Menu _menu;

    public Material Skybox;
    private float skyrotation;
    [HideInInspector]
    public int DayNumber;


    private GameObject Dialog_Obj;
    private float DayLengthStart;

    private TextMeshProUGUI DayStartText;

    private bool DayChanged;
    public bool DayEnded { get; set; }

    void Start()
    {
        DayLength = 430;
        DayStartText = GameObject.Find("DayStartText").GetComponent<TextMeshProUGUI>();

        DayLengthStart = DayLength;
        Dialog_Obj = GameObject.Find("Dialog");
        _morningborder = new Vector2(DayLength / _morningborder_Div.x, DayLength / _morningborder_Div.y);
        _dayborder = new Vector2(DayLength / _dayborder_Div.x, DayLength / _dayborder_Div.y);
        _dawnborder = new Vector2(DayLength / _dawnborder_Div.x, DayLength / _dawnborder_Div.y);
        _nightborder = new Vector2(DayLength / _nightborder_Div.x, DayLength/ _nightborder_Div.y);

        //DayNightCycleText = GameObject.Find("DayNightCycleText");

      
        pl = InitializeOnAwake.pl;
        _menu = InitializeOnAwake.pl.GetComponent<Menu>();

        pl.DayNight = GetComponent<DayAndNight>();
      
        Day_Cycle = DayCycle.Morning;

        PlayDayStart();

    }


    void Update()
    {


        if ( pl._Menu.MenuONOFF) return;
        DayChanged = false;
       /* if (DayNumber > 17) DayLength = 250;
        else DayLength = DayLengthStart;*/

        SetDayText();


        if (pl.TEST)
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                PlayDayStart();
                DayChanged = true;
                DayNumber++;
                pl.SL.SaveLoadCurrent.DayNumber = DayNumber;
                pl._Menu.LoadScene_SA("Day" + DayNumber);

                PlayDayStart();
                ResetDialog();
                DayTimer = 0;
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {

                DayChanged = true;
                DayNumber--;
                pl.SL.SaveLoadCurrent.DayNumber = DayNumber;
                pl._Menu.LoadScene_SA("Day" + DayNumber);
                PlayDayStart();
                ResetDialog();
                DayTimer = 0;
            }
        }





        ArrowRotation = 360 / DayLength;
        Arrow.transform.rotation =  Quaternion.Euler(ArrowRotation * DayTimer, 0,0 );
        
        SkyBoxMaterial();


        DayTimer += Time.deltaTime;
        DayStartText.color = new Color(1, 1, 1, DayStartText.color.a - Time.deltaTime);

        if (DayTimer > DayLength)
        {
           
            DayEnded = true;
        }

        pl.SL.SaveLoadCurrent.DayNumber = DayNumber;
        pl.SL.SaveLoadCurrent.DayTimer = DayTimer;
        // DayNightCycleText.GetComponent<Text>().text = DayTime + (pl.SL.DayNumber +1);


        if (DayTimer > _morningborder.x && DayTimer < _morningborder.y)
        {

            for (int i = 0; i < AudioSources.Length; i++)
            {
                if (i != 0)
                {
                    if (AudioSources[i].volume > 0)
                        AudioSources[i].volume -= Time.deltaTime;
                }
                else if (AudioSources[i].volume < 1) AudioSources[i].volume += Time.deltaTime;

            }



            DayGlobalLight.color = new Color(Mathf.Lerp(DayGlobalLight.color.r, DayLight.r, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.g, DayLight.g, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.b, DayLight.b, Time.deltaTime), 1);

            if (Day_Cycle != DayCycle.Morning)
            {
              
                Day_Cycle = DayCycle.Morning;
            }


            DayTime = "Morning";
        }


        if (DayTimer > _dayborder.x && DayTimer < _dayborder.y)
        {

            for (int i = 0; i < AudioSources.Length; i++)
            {
                if (i != 0)
                {
                    if (AudioSources[i].volume > 0)
                        AudioSources[i].volume -= Time.deltaTime;
                }
                else if (AudioSources[i].volume < 1) AudioSources[i].volume += Time.deltaTime;
            
            }



            DayGlobalLight.color = new Color(Mathf.Lerp(DayGlobalLight.color.r, DayLight.r, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.g, DayLight.g, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.b, DayLight.b, Time.deltaTime), 1);
            if (Day_Cycle != DayCycle.Day)
            {
                
                Day_Cycle = DayCycle.Day;
            }
            DayTime = "Day";
        }

        if (DayTimer >= _dawnborder.x && DayTimer < _dawnborder.y)
        {
          
            for (int i = 0; i < AudioSources.Length; i++)
            {
                if (i != 1)
                {
                    if (AudioSources[i].volume > 0)
                        AudioSources[i].volume -= Time.deltaTime;
                }
                else if (AudioSources[i].volume < 1) AudioSources[i].volume += Time.deltaTime;

            }

            DayGlobalLight.color = new Color(Mathf.Lerp(DayGlobalLight.color.r, DawnLight.r, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.g, DawnLight.g, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.b, DawnLight.b, Time.deltaTime), 1);
            Day_Cycle = DayCycle.Dawn;
            DayTime = "Dusk";
        }

        if (DayTimer >= _nightborder.x && DayTimer < _nightborder.y)
        {
            for (int i = 0; i < AudioSources.Length; i++)
            {
                if (i != 2)
                {
                    if (AudioSources[i].volume > 0)
                        AudioSources[i].volume -= Time.deltaTime;
                }
                else if (AudioSources[i].volume < 1) AudioSources[i].volume += Time.deltaTime;
            }

            if (Day_Cycle != DayCycle.Night)
            {
               
                Day_Cycle = DayCycle.Night;
            }

           
            DayGlobalLight.color = new Color(Mathf.Lerp(DayGlobalLight.color.r, NightLight.r, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.g, NightLight.g, Time.deltaTime), Mathf.Lerp(DayGlobalLight.color.b, NightLight.b, Time.deltaTime), 1);
         
            DayTime = "Night";
        }



    }


    void SkyBoxMaterial()
    {
        Skybox.SetColor("_Tint", DayGlobalLight.color);
        skyrotation += Time.deltaTime;
        if (skyrotation > 360) skyrotation = 0;
        Skybox.SetFloat("_Rotation", skyrotation);
    }


    public void ResetDialog()
    {
      
        if (pl != null)
        {
            pl.InDialog = false;
        }

    

       // Dialog_Obj.GetComponent<Animator>().SetBool("Play", false);

        if (Dialog_Obj.transform.Find("TextPlayer").GetComponent<Text>() != null)
            Dialog_Obj.transform.Find("TextPlayer").GetComponent<Text>().text = "";

        if (Dialog_Obj.transform.Find("TextCharacter").GetComponent<Text>() != null)
            Dialog_Obj.transform.Find("TextCharacter").GetComponent<Text>().text = "";

        




    }


    void PlayDayStart()
    {
        DayStartText.color = new Color(1, 1, 1, 1);
  
       

        DayStartText.GetComponent<AudioSource>().Play();
    }


    void SetDayText()
    {
        int daynum = DayNumber + 1;

        if (_menu.Language == 0)
            DayStartText.text = "Day " + daynum;

        if (_menu.Language == 1)
            DayStartText.text = "День " + daynum;

        if (_menu.Language == 2)
            DayStartText.text = "День " + daynum;

    }


    public bool IsDayChangedThisFrame()
    {
        return DayChanged;
    }
}
