using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class JoyCheck : MonoBehaviour
{
    private static List<GameObject> TotalJoyObjects = new List<GameObject>(); 
 
    private AudioSource AS;
    private bool Played;
    private GameObject YesNoOB, YesButton, NoButton;
    private Player pl;
    private Menu _menu;
    private GameObject MenuChoose;
    private Outline _Outline;


    private void Start()
    {

        pl = InitializeOnAwake.pl;
        _menu = InitializeOnAwake._Menu;
        YesNoOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/YesNoJoy"), GameObject.Find("Canvas").transform);
        YesButton = YesNoOB.transform.Find("YesButton").gameObject;
        NoButton = YesNoOB.transform.Find("NoButton").gameObject;

        AS = GetComponent<AudioSource>();
        Played = false;
        MenuChoose = GameObject.Find("MenuChoose");
        YesNoOB.SetActive(false);
        _Outline = GetComponent<Outline>();
    }

    void Update()
    {
        Manager();
    }
    void Manager()
    {


        if (pl.ViewColl(gameObject)) _Outline.enabled = true;
        else _Outline.enabled = false;

        if (TotalJoyObjects.Count <= 0)
        {
            if (!Played)
            {
                AS.Play();
                Played = true;
            }
        }


        if ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && pl.ViewColl(gameObject))
        {
            if (TotalJoyObjects.Count > 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                YesNoOB.SetActive(true);
                pl.showjoycheck = true;
            } else
            {


                pl._Menu.LoadScene_SA("FeedMum");
            }

        }

        if (_menu.ClickButton(YesButton) )
        {
            pl._Menu.LoadScene_SA("FeedMum");
        }

        if (_menu.ClickButton(NoButton))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            YesNoOB.SetActive(false);
            _menu.ONOFFUI(MenuChoose.transform, false);
            pl.showjoycheck = false;
            
        }

        
    }


    public static void AddJoyObject(GameObject obj)
    {
        if (TotalJoyObjects.Contains(obj)) return;
        TotalJoyObjects.Add(obj);
    }

    public static void RemoveJoyObject(GameObject obj)
    {
        if (!TotalJoyObjects.Contains(obj)) return;
        TotalJoyObjects.Remove(obj);
    }
}
