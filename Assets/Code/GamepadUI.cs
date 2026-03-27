using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamepadUI : MonoBehaviour
{

    private InputMode IM;
    public bool DrawOnlyOnGamepad;

    public Sprite MouseSPRT;
    public Sprite KeyBoardSPRT;

    public Sprite GamepadSPRT;

    public Sprite SwitchGamepad;
    private Sprite StartSPRT;
    private Image IMG;
    private SpriteRenderer SPRT;

    private void Awake()
    {
        IMG = GetComponent<Image>();
        SPRT = GetComponent<SpriteRenderer>();

#if UNITY_SWITCH

        if (IMG != null)
            IMG.sprite = SwitchGamepad;

        if (SPRT != null)
            SPRT.sprite = SwitchGamepad;
#endif



    }

    void Start()
    {
        if (GameObject.Find("Player") != null)
            IM = InitializeOnAwake.IM;
        else
        {
            gameObject.AddComponent<InputMode>();
            IM = GetComponent<InputMode>();
        }



        if(IMG!=null)
        StartSPRT = IMG.sprite;
    }
    
    void Update()
    {
        if (!IM.joystick)
        {
            if (IMG != null)
            {
                if (DrawOnlyOnGamepad) IMG.enabled = false;
                if (GamepadSPRT != null)
                {
                    if (IM.MouseMode) IMG.sprite = MouseSPRT;
                    else IMG.sprite = KeyBoardSPRT;

                }
            }

            if (SPRT != null)
            {
                if (DrawOnlyOnGamepad) SPRT.enabled = false;

                if (GamepadSPRT != null)
                {
                    if (IM.MouseMode) SPRT.sprite = MouseSPRT;
                    else SPRT.sprite = KeyBoardSPRT;

                }
            }
            return;
        }

        if (IMG != null)
        {
            if (DrawOnlyOnGamepad) IMG.enabled = true;

#if UNITY_STANDALONE
            if (GamepadSPRT != null)
            IMG.sprite = GamepadSPRT;
            else IMG.enabled = false;
#endif

#if UNITY_SWITCH
            if (SwitchGamepad != null)
                IMG.sprite = SwitchGamepad;
            else IMG.enabled = false;
#endif
        }

        if (SPRT != null)
        {

            if (DrawOnlyOnGamepad) SPRT.enabled = true;

#if UNITY_STANDALONE
            if (GamepadSPRT != null) 
            SPRT.sprite = GamepadSPRT;
            else SPRT.enabled = false;
#endif

#if UNITY_SWITCH
            if (SwitchGamepad != null)
                SPRT.sprite = SwitchGamepad;
            else SPRT.enabled = false;
#endif
        }
        
    }
}
