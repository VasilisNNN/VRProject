
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;




public class VRInputs :  InputModeBase
{
    private float MousePosTimer;
    private Vector3 PrevMousePos;

    private GameObject MouseOB;
    private GameObject MenuChoose;
    private Player pl;


    public override void Init()
    {
        pl = InitializeOnAwake.pl;
        _Menu = InitializeOnAwake._Menu;
        MouseOB = GameObject.Find("MouseUI");
        MenuChoose = GameObject.Find("MenuChoose");
    }
    public override void MainUpdate()
    {
        if (GameObject.Find("ScreenJoystick") != null)
        {
            MonoBehaviour.Destroy(GameObject.Find("ScreenJoystick"));
            MonoBehaviour.Destroy(GameObject.Find("StickBG"));
        }

        PudState = true;
        /* if (pl != null)
         {
             if (pl.dontMove())
             {
                 Cursor.visible = true;
                 Cursor.lockState = CursorLockMode.None;
             }
             else
             {
                 Cursor.visible = false;
                 Cursor.lockState = CursorLockMode.Locked;
             }

         }
         else
         {
             Cursor.visible = true;
             Cursor.lockState = CursorLockMode.None;
         }*/


        SensitivityMulitpier = 800;
        space_b = Input.GetButtonDown("Space");


        horizontal = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        vertical = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;


        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
           OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            enter_b = true;
        else
            enter_b = false;


        horizontal_R = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        vertical_R = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

        horizontal_Folder = Input.GetAxis("Horizontal_R");
  

        _verticalPush = Input.GetButtonDown("Vertical");
        _horizontalPush = Input.GetButtonDown("Horizontal");
    
        fire = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        fireHold = Input.GetMouseButton(0);
        aim = Input.GetMouseButton(1);

        if (vertical > 0.1f || vertical < -0.1f)
        {
            if (GamepadVertTimer < Time.fixedTime)
            {
                _verticalPush = true;
                GamepadVertTimer = Time.fixedTime + 0.01f;
            }
            else _verticalPush = false;
        }
        else _verticalPush = false;



        leftMouseButton = OVRInput.GetDown(OVRInput.Button.One);
        leftMouseButtonDown = OVRInput.GetDown(OVRInput.Button.One);


        rightMouseButton = Input.GetMouseButtonDown(1);


        exit_b = OVRInput.GetDown(OVRInput.Button.Two);
        menu_b = OVRInput.GetDown(OVRInput.Button.Start);

        //inventory_b = Input.GetKeyDown(KeyCode.I);
        // journal_b = Input.GetKeyDown(KeyCode.J);
        //  map = Input.GetKeyDown(KeyCode.M);

        run = Input.GetKey(KeyCode.LeftShift);

        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        jump = Input.GetKeyDown(KeyCode.Space);


        lightswitch = Input.GetKeyDown(KeyCode.L);
        heal_b = Input.GetKeyDown(KeyCode.E);
        ScanButton = OVRInput.GetDown(OVRInput.Button.Three);

        if (leftMouseButton || leftMouseButtonDown || rightMouseButton || Mathf.Abs(horizontal_R) > 0.1f || Mathf.Abs(vertical_R) > 0.1f) MouseMode = true;

        if (enter_b || exit_b || menu_b || run || aim || jump ||
            Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f) MouseMode = false;


    }


    public override bool MouseCollideWithButton(GameObject Button)
    {
        if (Button == null)
            return false;

        if ((MouseOB.GetComponent<CollList2D>().coll_obj.Contains(Button) && MouseMode) ||
            (MenuChoose.GetComponent<CollList2D>().coll_obj.Contains(Button) && !MouseMode))

        {
            return true;
        }

        return false;

    }


}