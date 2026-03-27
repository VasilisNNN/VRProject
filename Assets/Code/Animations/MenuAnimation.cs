using UnityEngine;
using System.Collections.Generic;

public class MenuAnimState
{
    public Color GlobalLightColor;
    public Color SkyColor;
    public string ButtonObject;
    public string AnimationName;
    public AudioSource AudioOn;
    public AudioSource AudioOff;
    public MenuAnimState(string button,string anim, Color globalLColor, Color skyColor,
        AudioSource audioOn, AudioSource audioOff)
    {

        AudioOn = audioOn;
        AudioOff = audioOff;
        ButtonObject = button;
        GlobalLightColor = globalLColor;
        SkyColor = skyColor;
        AnimationName = anim;
    }

}


public class MenuAnimation : MonoBehaviour
{
    public List<MenuAnimState> Menu_Anim_States = new List<MenuAnimState>();
    private string CurrentButton;
    public Light GlobalLight;
    public Color CurrentSkyColor;
    private Animator Anim;
    private string CurrentAnimation;
    private AudioSource SpookyEMB, NormalEMB;
    void Start()
    {
        Anim = GetComponent<Animator>();
        SpookyEMB = GameObject.Find("SpookyEMB").GetComponent<AudioSource>();
        NormalEMB = GameObject.Find("NormalEMB").GetComponent<AudioSource>();

        Menu_Anim_States.Add(new MenuAnimState("", "Normal",
            CurrentSkyColor, new Color(1, 1, 1), NormalEMB, SpookyEMB));

        Menu_Anim_States.Add(
            new MenuAnimState("Continue", "Normal",
            CurrentSkyColor, new Color(1, 1, 1), NormalEMB, SpookyEMB));

        Menu_Anim_States.Add(
            new MenuAnimState("Start", "Normal",
            CurrentSkyColor, new Color(1, 1, 1), NormalEMB, SpookyEMB));
        
        Menu_Anim_States.Add(
            new MenuAnimState("Options", "Normal",
            CurrentSkyColor, new Color(1, 1, 1), NormalEMB, SpookyEMB));


        Menu_Anim_States.Add(
            new MenuAnimState("QuitGame", "ExitAnimation", 
            new Color(1, 0, 0), new Color(0, 0, 0), SpookyEMB, NormalEMB));

      

    }

  
    void Update()
    {
        for (int i = 0; i < Menu_Anim_States.Count; i++)
        {
            if (Menu_Anim_States[i].ButtonObject == CurrentButton)
            {
              
                
                    SetColor(ref CurrentSkyColor, Menu_Anim_States[i].GlobalLightColor);
                    RenderSettings.skybox.SetColor("_Tint", CurrentSkyColor);
                    RenderSettings.fogColor = CurrentSkyColor;
                    GlobalLight.color = CurrentSkyColor;

                Menu_Anim_States[i].AudioOn.volume += Time.deltaTime * 5;
                Menu_Anim_States[i].AudioOff.volume -= Time.deltaTime * 5;


                if (CurrentAnimation != Menu_Anim_States[i].AnimationName)
                {
                    Anim.CrossFade(Menu_Anim_States[i].AnimationName, Time.deltaTime * 5);
                    CurrentAnimation = Menu_Anim_States[i].AnimationName;
                }
            }
        }

        if (CurrentButton == null)
        {
            SetColor(ref CurrentSkyColor, Menu_Anim_States[0].GlobalLightColor);
            RenderSettings.skybox.SetColor("_Tint", CurrentSkyColor);
            RenderSettings.fogColor = CurrentSkyColor;

            GlobalLight.color = CurrentSkyColor;

            Menu_Anim_States[0].AudioOn.volume += Time.deltaTime * 5;
            Menu_Anim_States[0].AudioOff.volume -= Time.deltaTime * 5;


            if (CurrentAnimation != Menu_Anim_States[0].AnimationName)
            {
                Anim.CrossFade(Menu_Anim_States[0].AnimationName, Time.deltaTime * 5);
                CurrentAnimation = Menu_Anim_States[0].AnimationName;
            }
        }
    }

    void SetColor( Color Target, Color Source)
    {
        Target = new Color(

                   Mathf.Lerp(Target.r,
                  Source.r, Time.deltaTime * 5),

                   Mathf.Lerp(Target.g,
                   Source.g, Time.deltaTime * 5),

                   Mathf.Lerp(Target.b,
                   Source.b, Time.deltaTime * 5));
    }

    void SetColor(ref Color Target, Color Source)
    {
        Target = new Color(

                   Mathf.Lerp(Target.r,
                  Source.r, Time.deltaTime * 5),

                   Mathf.Lerp(Target.g,
                   Source.g, Time.deltaTime * 5),

                   Mathf.Lerp(Target.b,
                   Source.b, Time.deltaTime * 5));
    }

    public void SetCurrentButton(GameObject Button)
    {
        CurrentButton = Button.name;
    }
}
