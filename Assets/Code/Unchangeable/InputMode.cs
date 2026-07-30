using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class InputMode : MonoBehaviour
{

    public InputModeBase CurrentInputs;
    private PCKeyboardInputs PCInputs;
    private VRInputs VR_Inputs;
    public float ActionDelay { get; set; }
    public float _horizontal { get; set; }
    public float _vertical { get; set; }

    public float DPADY { get; set; }
    public float DPADX { get; set; }


    public bool RightMouseButtonDown { get; set; }
   

    public float _horizontal2 { get; set; }
    public float _vertical2 { get; set; }
    public float _vertical_R { get; set; }
    public bool _vertical_R_Push;
    public float _horizontal_R { get; set; }
    public bool _horizontal_R_Push;

    public bool SpaceB { get; set; }
    public bool ScanButton { get; set; }
    public bool SpaceB2 { get; set; }
    public bool _vertical_button { get; set; }
    public bool Fire { get; set; }
    public bool Reload { get; set; }
    public bool Aim { get; set; }
    public bool run { get; set; }
    public float scrollWheel;
    public bool _horizontalPush { get; set; }

    public bool _verticalPush { get; set; }

    public float HorizontalArrows { get; set; }
    public float HorizontalArrows2 { get; set; }

    public float VerticalArrows { get; set; }
    private int _horizontalScroll_Timer;
    public float horizontal_Folder { get; set; }
    
    public float Enterdeley { get; set; }


    public bool exit_b { get; set; }
    public bool pick_item { get; set; }

    public bool LeftMouseButton { get; set; }
    public bool LeftMouseButtonDown { get; set; }


    public bool RightMouseButton { get; set; }
    public bool HandsRotation { get;set;}

    public bool menu_b { get; set; }
    public bool inventory_b { get; set; }
    public bool journal_b { get; set; }
    public bool Achbutton { get; set; }

    public bool enter_b { get; set; }

    public bool enter_b_hold { get; set; }
    public bool UpgradeButton { get; set; }


    private float horizontal_b, scrollV;


    public bool joystick = false;

    public float GamepadVertTimer { get; set; }
    public float GamepadHorTimer { get; set; }


    public bool _vertical_DPAD_Push { get; set; }
    public bool _horizontal_DPAD_Push { get; set; }


    private float GamepadRHorTimer;

    public bool MouseMode { get; set; }
    private Vector3 PrevMousePos;
    private float MousePosTimer;

    public bool LeftTrigger { get; private set; }
    public bool RightTrigger { get; private set; }
    public int CraftedItems { get; set; }

    public bool shift { get; private set; }
    public bool map { get; private set; }

    public bool ZLKey { get; private set; }
    public bool R2 { get; private set; }
    public bool L2 { get; private set; }


    private void Awake()
    {
        PCInputs = new PCKeyboardInputs();
        PCInputs.Init();

        VR_Inputs = new VRInputs();
        VR_Inputs.Init();
#if UNITY_STANDALONE

        CurrentInputs = PCInputs;


#endif

#if UNITY_ANDROID || UNITY_XR_OPENXR

#if UNITY_EDITOR
        CurrentInputs = PCInputs;
#else
        CurrentInputs = VR_Inputs;
#endif

#endif

#if UNITY_SWITCH


        CurrentInputs = new SwitchInputs();

#endif

#if UNITY_PS5 || UNITY_PS4
        PS_SaveMain = GameObject.Find("Constructor").GetComponent<SonySaveDataMain>();
#endif
        CurrentInputs.Init();

    }

    void Update()
    {



        CurrentInputs.MainUpdate();

        if (!CurrentInputs.PudState) return;

        enter_b = CurrentInputs.enter_b;
        enter_b_hold = CurrentInputs.enter_b_hold;
        inventory_b = CurrentInputs.inventory_b;

        _horizontal = CurrentInputs.horizontal;
        _vertical = CurrentInputs.vertical;
        _horizontal_R = CurrentInputs.horizontal_R;
        _vertical_R = CurrentInputs.vertical_R;
        horizontal_Folder = CurrentInputs.horizontal_Folder;

        exit_b = CurrentInputs.exit_b;
     

        menu_b = CurrentInputs.menu_b;
        LeftMouseButton = CurrentInputs.LeftMouseButton;
        LeftMouseButtonDown = CurrentInputs.LeftMouseButtonDown;
        RightMouseButtonDown = CurrentInputs.RightMouseButtonDown;
        SpaceB = CurrentInputs.space_b;
        Fire = CurrentInputs.fire;
        Aim = CurrentInputs.RightMouseButton;

        // MouseScroll = CurrentInputs.MouseScroll;

        DPADX = CurrentInputs.DPADX;
        DPADY = CurrentInputs.DPADY;

        _horizontal_DPAD_Push = CurrentInputs._horizontal_DPAD_Push;
        _vertical_DPAD_Push = CurrentInputs._vertical_DPAD_Push;
        RightTrigger = CurrentInputs.RightTrigger;
        LeftTrigger = CurrentInputs.LeftTrigger;

        ZLKey = CurrentInputs.ZLKey;
        R2 = CurrentInputs.R2;
        L2 = CurrentInputs.L2;

        _horizontalPush = CurrentInputs._horizontalPush;
        _verticalPush = CurrentInputs._verticalPush;

        MouseMode = CurrentInputs.MouseMode;
        joystick = CurrentInputs.joystick;

        shift = CurrentInputs.shift;

        run = CurrentInputs.shift;

        ScanButton = CurrentInputs.ScanButton;

    }




}
