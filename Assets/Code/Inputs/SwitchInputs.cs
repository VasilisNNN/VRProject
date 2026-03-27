
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_SWITCH
using System.Threading.Tasks;
using nn.hid;



public class SwitchInputs :MonoBehaviour, IInputs
{

    private NpadId npadId = NpadId.Invalid;
    private NpadStyle npadStyle = NpadStyle.Invalid;
    private NpadState npadState = new NpadState();

    public bool enter_b => GetInputDown(NpadButton.A);
    public bool enter_b_hold => GetInput(NpadButton.A);

    public bool inventory_b => GetInputDown(NpadButton.Y);


    public float _horizontal
    { 
        get{

           if (GetInput(NpadButton.StickLLeft)) return  -1;
            else if (GetInput(NpadButton.StickLRight)) return  1;
            else return  0;


        }
    }

    public float _vertical
    {
        get 
        {
            if (GetInput(NpadButton.StickLUp)) return 1;
            else if (GetInput(NpadButton.StickLDown)) return -1;
            else return 0;

        }
    }


    public float _horizontal_R
    {
        get
        {

            if (GetInput(NpadButton.StickRLeft)) return -1;
            else if (GetInput(NpadButton.StickRRight)) return 1;
            else return 0;


        }
    }

    public float _vertical_R
    {
        get
        {
            if (GetInput(NpadButton.StickRUp)) return 1;
            else if (GetInput(NpadButton.StickRDown)) return -1;
            else return 0;

        }
    }

     

    public bool exit_b => GetInputDown(NpadButton.B);
    public bool delete_b => GetInputDown(NpadButton.B);
    public bool menu_b => GetInputDown(NpadButton.Plus);
    public bool space_b => GetInputDown(NpadButton.R);


    public bool LeftMouseButton => Input.GetMouseButton(0);

    public bool LeftMouseButtonDown => Input.GetMouseButtonDown(0);
    public bool RightMouseButtonDown => Input.GetMouseButtonDown(1);
    public bool RightMouseButton => Input.GetMouseButton(1);
    public bool BKey => GetInputDown(NpadButton.X);

    public bool OKey => GetInputDown(NpadButton.ZR);

    public float DPADY
    {
        get
        {
            if (GetInput(NpadButton.Up)) return  1;
            else if (GetInput(NpadButton.Down)) return  -1;
            else return 0;

        }
    }


    public float DPADX
    {
        get
        {
            if (GetInput(NpadButton.Left)) return -1;
            else if (GetInput(NpadButton.Right)) return 1;
            else return 0;

        }

    }

    public float MouseScroll
    {
       get
       {
         if(GetInputDown(NpadButton.StickR))
        return 1;
            else if (GetInputDown(NpadButton.StickL))
                return -1;
            else return 0;
        }  
    }



    public bool _horizontal_DPAD_Push
    {
        get 
        {

            if (DPADX > 0.1f || DPADX < -0.1f)
            {
                if (GamepadHorTimer < Time.fixedTime)
                {
                    GamepadHorTimer = Time.fixedTime + 0.01f;
                    return true;
                
                }
                else return false;
            }
            else return false;
        }
    
    }
    public bool _vertical_DPAD_Push
    {
        get 
        {
            if (DPADY > 0.1f || DPADY < -0.1f)
            {
                if (GamepadVertTimer < Time.fixedTime)
                {
                
                    GamepadVertTimer = Time.fixedTime + 0.01f;
                    return true;
                }
                else return false;
            }
            else return false;

        }

    }

    public bool RightTrigger => GetInputDown(NpadButton.R);
    public bool LeftTrigger => GetInputDown(NpadButton.L);
    public bool ZLKey => GetInputDown(NpadButton.ZL);
    public bool R2 => GetInputDown(NpadButton.ZR);
    public bool L2 => GetInputDown(NpadButton.ZL);

    public bool FadeMode => GetInputDown(NpadButton.Y);
    public bool shift =>GetInputDown(NpadButton.RightStickPush);

    private Image JoyConImage, ProGamepadImage;
    private float GamepadHorTimer;
    private float GamepadVertTimer;

    private bool MidPadState;
    public bool PudState => MidPadState;
    public bool joystick => true;

    public bool MouseMode => false;
    public bool _horizontalPush
    {
        get 
        {
            if (_horizontal > 0.1f || _horizontal < -0.1f)
            {
                if (GamepadHorTimer < Time.fixedTime)
                {
                    GamepadHorTimer = Time.fixedTime + 0.01f;
                    return true;
            
                }
                else return false;
            }
            else return false;


        }

    }

    public bool _verticalPush
    {
        get
        {
            if (_vertical > 0.1f || _vertical < -0.1f)
            {
                if (GamepadVertTimer < Time.fixedTime)
                {
                    GamepadVertTimer = Time.fixedTime + 0.01f;
                    return true;

                }
                else return false;
            }
            else return false;


        }

    }


    bool UpdatePadState()
    {

        NpadStyle handheldStyle = Npad.GetStyleSet(NpadId.Handheld);
        NpadState handheldState = npadState;
        if (handheldStyle != NpadStyle.None)
        {
            Npad.GetState(ref handheldState, NpadId.Handheld, handheldStyle);
            if (handheldState.buttons != NpadButton.None)
            {
                if (npadId != NpadId.Handheld)
                {
                    JoyConImage.color = new Color(1, 1, 1, 1);
                    ProGamepadImage.color = new Color(1, 1, 1, 0);
                }
                npadId = NpadId.Handheld;
                npadStyle = handheldStyle;
                npadState = handheldState;
                return true;
            }
        }

        NpadStyle no1Style = Npad.GetStyleSet(NpadId.No1);
        NpadState no1State = npadState;
        if (no1Style != NpadStyle.None)
        {
            Npad.GetState(ref no1State, NpadId.No1, no1Style);
            if (no1State.buttons != NpadButton.None)
            {
                if (npadId != NpadId.No1)
                {
                    ProGamepadImage.color = new Color(1, 1, 1, 1);
                    JoyConImage.color = new Color(1, 1, 1, 0);
                }

                npadId = NpadId.No1;
                npadStyle = no1Style;
                npadState = no1State;

                return true;
            }
        }

        if ((npadId == NpadId.Handheld) && (handheldStyle != NpadStyle.None))
        {

            npadId = NpadId.Handheld;
            npadStyle = handheldStyle;
            npadState = handheldState;
        }
        else if ((npadId == NpadId.No1) && (no1Style != NpadStyle.None))
        {

            npadId = NpadId.No1;
            npadStyle = no1Style;
            npadState = no1State;
        }
        else
        {
            npadId = NpadId.Invalid;
            npadStyle = NpadStyle.Invalid;
            npadState.Clear();
            return false;
        }
        return true;
    }


    public void Body()
    {

        DrawGamepad();
        MidPadState = UpdatePadState();



    }

    public void DrawGamepad()
    {
        if (JoyConImage.color.a > 0) JoyConImage.color = new Color(1, 1, 1, JoyConImage.color.a - 0.03f);
        if (ProGamepadImage.color.a > 0) ProGamepadImage.color = new Color(1, 1, 1, ProGamepadImage.color.a - 0.03f);

    }


    
  

   public  void Init()
    {

        if (GameObject.Find("GamepadChoise") == null)
        {
            GameObject GamepadChoise = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/GamepadChoise"), GameObject.Find("Canvas").transform);
            GamepadChoise.name = "GamepadChoise";

        }

        JoyConImage = GameObject.Find("GamepadChoise").transform.Find("Joycon").GetComponent<Image>();
        ProGamepadImage = GameObject.Find("GamepadChoise").transform.Find("ProGamepad").GetComponent<Image>();

        JoyConImage.color = new Color(1, 1, 1, 0);
        ProGamepadImage.color = new Color(1, 1, 1, 0);


        Npad.Initialize();
        Npad.SetSupportedIdType(new NpadId[] { NpadId.Handheld, NpadId.No1 });
        Npad.SetSupportedStyleSet(NpadStyle.FullKey | NpadStyle.Handheld | NpadStyle.JoyDual);

        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            NpadStyle handheldStyle = Npad.GetStyleSet(NpadId.Handheld);
            NpadState handheldState = npadState;

            if (handheldStyle != NpadStyle.None)
            {
                Npad.GetState(ref handheldState, NpadId.Handheld, handheldStyle);

                if (npadId != NpadId.Handheld)
                {
                    JoyConImage.color = new Color(1, 1, 1, 1);
                    ProGamepadImage.color = new Color(1, 1, 1, 0);
                }
                npadId = NpadId.Handheld;
                npadStyle = handheldStyle;
                npadState = handheldState;
            }

        }

    }

    bool GetInput(NpadButton button)
    {
        if (MidPadState)
        {
            return npadState.GetButton(button);
        }
        return false;
    }

    bool GetInputDown(NpadButton button)
    {
        if (MidPadState)
        {
            return npadState.GetButtonDown(button);
        }
        return false;
    }

    /*

    void SwitchGamepadControlls()
    {

        if (!UpdatePadState()) return;

        enter_b = npadState.GetButtonDown(NpadButton.A);
    
        Heal = npadState.GetButtonDown(NpadButton.X);


        delete_b = npadState.GetButton(NpadButton.B);
        menu_b = 
        Rightstickpush = npadState.GetButtonDown(NpadButton.StickR);
        HorizontalFlip = npadState.GetButtonDown(NpadButton.ZR);

       
        SideButton = npadState.GetButtonDown(NpadButton.X);

      
   


        if (npadState.GetButton(NpadButton.StickLLeft)) _horizontal = -1;
        else if (npadState.GetButton(NpadButton.StickLRight)) _horizontal = 1;
        else _horizontal = 0;


     




    

   




        if (_vertical > 0.1f || _vertical < -0.1f)
        {
            if (GamepadVertTimer < Time.fixedTime)
            {
                _verticalPush = true;
                GamepadVertTimer = Time.fixedTime + 0.01f;
            }
            else _verticalPush = false;
        }
        else _verticalPush = false;

      //  journal_b = npadState.GetButtonDown(NpadButton.L);
        QuestBook = npadState.GetButtonDown(NpadButton.L);
        space_b =
        //Dash = npadState.GetButtonDown(NpadButton.B);
        inventory_b = 

        BKey = 
        OKey = 


        


       
    }
     */




}
#endif