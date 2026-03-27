using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class SaveLoadBasic : ISaveLoad
{

    public int saveslotsnumber => 7;

    private float SecondsTimer;


    [HideInInspector]

    public BodySlots BodyUP;

    public string LastLocation { get; set; }

    public int Inventory_Count, Quests_Count;

    public string[] LocationsNames;
    public List<string> LocationsMenu = new List<string>();
    public int[] CreateLocationOnStart;


    public static Player pl;
    public static Inventory inv;

    public static Menu _Menu;

    public static SaveLoad SL;
    public static DayAndNight _DayAndNight;
    public StartStart StartStats;

    public List<string> DonotLoadStartPosLocation = new List<string>();
    public List<string> EndingsLocationsNames = new List<string>();
    public bool SetPlPos;
    public float SetPlPosTimer;

    [HideInInspector]
    public string PreviousLevel = "";

    [HideInInspector]
    public float PlayerXPos, PlayerZPos, PlayerYPos,
        PlayerXRotation, PlayerYRotation, PlayerZRotation,
        CameraXRotation, CameraYRotation, CameraZRotation;

    [HideInInspector]
    public List<string> ObjectsToDestroy = new List<string>();
    [HideInInspector]
    public List<string> ObjectsToDisable = new List<string>();
    [HideInInspector]
    public List<GameObject> CarList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> ObjectsToPick = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> TriggersActivated = new List<GameObject>();

    public int DayNumber { get; set; }
    public float DayTimer { get; set; }


    private List<int> UpgradesSlots = new List<int>();


    public int  ObjectsToDestroy_Count, DroppedItems_Count, UnlockedItems_Count;
   
    public int _TutorialPhase { get; set; }

    public int CurrentPlayer { get; set; }
    public int ContinueNumber { get; set; }
    public string LocationToLoadInBuffer { get; set; }


    public Upgrades _Upgrades;
  
    public ItemsSlotsUI  UpgradesUI;

    public float PlayerPosX;
    public float PlayerPosY;
    public abstract void Init();
   


    public abstract void MainSave(bool saveall, string locationname);

    public abstract void MainLoad(int slotnumber);



    public abstract void MenuLoad();








 




    public void UnLoadAll()
    {
        

        for (int i = 0; i < LocationsNames.Length; i++)
        {

            CreateLocationOnStart[i] = 0;

        }



        ObjectsToDestroy = new List<string>();
    }


    public void ApplyVariables(int slotnumber)
    {
        if (pl == null || _Menu.FirstStart == 0) return;

        if (_Menu.CurrentSlotLocations[slotnumber] == SceneManager.GetActiveScene().name)
        {
            if (!DonotLoadStartPosLocation.Contains(SceneManager.GetActiveScene().name))
            {
                Debug.Log("ApplyVariables 1");
                SetPlPos = true;
                SetPlPosTimer = Time.fixedTime + 0.5f;
            }
        }
        else if (GameObject.Find("StartPosition") != null)
        {
            Debug.Log("ApplyVariables 2");
            pl.transform.SetPositionAndRotation(GameObject.Find("StartPosition").transform.position, GameObject.Find("StartPosition").transform.rotation);

        }

        Debug.Log("ApplyVariables 3");

        if (_DayAndNight != null)
        {
            _DayAndNight.DayNumber = DayNumber;
            _DayAndNight.DayTimer = DayTimer;
        }
    }



}
