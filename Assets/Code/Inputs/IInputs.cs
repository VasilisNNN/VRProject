using System.Numerics;

internal interface IInputs
{

    public bool PudState { get; }
    public bool enter_b { get; }
    public bool enter_b_hold { get; }
    public bool inventory_b { get; }

    public float _horizontal { get; }
    public float _vertical { get; }

    public float _horizontal_R { get; }
    public float _vertical_R { get; }

    public bool exit_b { get; }
    public bool delete_b { get; }

    public bool menu_b { get; }
    public bool space_b { get; }

 
    public bool LeftMouseButton { get; }
    public bool LeftMouseButtonDown { get; }
    public bool RightMouseButtonDown { get; }
    public bool RightMouseButton { get; }
    public float DPADY { get; }
    public float DPADX { get; }

    public bool BKey { get; }
    public bool OKey { get; }

    public float MouseScroll { get; }

    public bool _horizontal_DPAD_Push { get; }

    public bool _vertical_DPAD_Push { get; }

    public bool RightTrigger { get; }
    public bool LeftTrigger { get; }
    public bool ZLKey { get; }
    public bool R2 { get; }
    public bool L2 { get; }


    public bool _horizontalPush { get; }
    public bool _verticalPush { get; }

    public bool joystick { get; }

    public bool MouseMode { get; }
    public bool shift { get; }
    public void Init()
    { 
    
    
    }

    public void Body()
    {


    }

}