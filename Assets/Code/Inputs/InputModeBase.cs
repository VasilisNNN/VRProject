using UnityEngine;


public abstract class InputModeBase 
{
    public float horizontal;
    public float vertical;
    public float horizontal_R;
    public float vertical_R;
    public float horizontal_Folder;

    public bool run ;
    public bool aim;
    public bool jump;


    public bool _verticalPush;
    public bool _horizontalPush;

    public bool fire;
    public bool fireHold;

    public bool enter_b;

    public bool leftMouseButton;
    public bool leftMouseButtonDown;

    public bool rightMouseButton;

    public bool exit_b;
    public bool menu_b ;

    public bool inventory_b ;
    public bool journal_b;
    public bool map;

    public float scrollWheel;

    public bool lightswitch;
    public bool heal_b;
    public bool PudState;
    public bool enter_b_hold ;

    

    public bool delete_b ;
    public bool space_b;
    public bool ScanButton;

    public bool LeftMouseButtonDown => Input.GetMouseButtonDown(0);
    public bool RightMouseButtonDown => Input.GetMouseButtonDown(1);

    public float MouseScroll => Input.GetAxis("Mouse ScrollWheel");


    public bool LeftMouseButton => Input.GetMouseButton(0);


    public bool RightMouseButton => Input.GetMouseButton(1);

    public bool SideButton => false;

    public bool LeftTrigger => false;
    public bool RightTrigger => false;

    public bool R2 => Input.GetKeyDown(KeyCode.T);
    public bool L2 => Input.GetKeyDown(KeyCode.R);

    public bool ZLKey => Input.GetKeyDown(KeyCode.M);

    public bool FadeMode => Input.GetKeyDown(KeyCode.N);

    public bool QuestBook => Input.GetKeyDown(KeyCode.Q);



    public bool _vertical_R_Push => Input.GetButtonDown("Vertical_R");
    public bool _horizontal_R_Push => Input.GetButtonDown("Horizontal_R");


    public float VerticalArrows => Input.GetAxis("VerticalArrows");

    public bool shift => Input.GetKey(KeyCode.LeftShift);




    public bool HorizontalFlip => Input.GetButtonDown("HorizontalFlip");
    public bool Rightstickpush => Input.GetButtonDown("RightStickPush");

    public bool BKey => Input.GetButtonDown("BKey");
    public bool OKey => Input.GetButtonDown("OKey");

    public float DPADY => 0;

    public float DPADX => 0;

    public bool _horizontal_DPAD_Push => false;

    public bool _vertical_DPAD_Push => false;
    public bool joystick => false;


    public float GamepadVertTimer;

    public bool MouseMode;
    public float SensitivityMulitpier;
 
    public Menu _Menu;

    public abstract void Init();
    public abstract void MainUpdate();

    public abstract bool MouseCollideWithButton(GameObject Button);
    
}
