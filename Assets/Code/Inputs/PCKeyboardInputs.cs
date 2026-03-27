
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;




public class PCKeyboardInputs :  InputModeBase
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


        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
     
        horizontal_R = Input.GetAxis("Horizontal_R");
        vertical_R = Input.GetAxis("Vertical_R");

        horizontal_Folder = Input.GetAxis("Horizontal_R");
  

        _verticalPush = Input.GetButtonDown("Vertical");
        _horizontalPush = Input.GetButtonDown("Horizontal");
    
        fire = Input.GetMouseButtonDown(0);
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

        enter_b = Input.GetButtonDown("Enter");

        leftMouseButton = Input.GetMouseButtonDown(0);
        leftMouseButtonDown = Input.GetMouseButtonDown(0);


        rightMouseButton = Input.GetMouseButtonDown(1);


        exit_b = Input.GetButtonDown("Exit");
        menu_b = Input.GetButtonDown("Menu");

        //inventory_b = Input.GetKeyDown(KeyCode.I);
       // journal_b = Input.GetKeyDown(KeyCode.J);
      //  map = Input.GetKeyDown(KeyCode.M);

        run = Input.GetKey(KeyCode.LeftShift);

        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        jump = Input.GetKeyDown(KeyCode.Space);


        lightswitch = Input.GetKeyDown(KeyCode.L);
        heal_b = Input.GetKeyDown(KeyCode.E);
        ScanButton = Input.GetKeyDown(KeyCode.F);

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