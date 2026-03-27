using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PCSaveLoad : SaveLoadBasic
{
    override public void Init()
    {

        _Menu = InitializeOnAwake._Menu;
        SL = InitializeOnAwake.SL;

        if (GameObject.Find("BodySlots") != null)
            BodyUP = GameObject.Find("BodySlots").GetComponent<BodySlots>();

        if (GameObject.Find("Player") != null)
        {
            
            pl = InitializeOnAwake.pl;
            inv = InitializeOnAwake.inv;
        }


        if (GameObject.Find("DayAndNight") != null)
            _DayAndNight = GameObject.Find("DayAndNight").GetComponent<DayAndNight>();


        LocationsMenu.Add("StartMenu");
        LocationsMenu.Add("ChoosePlayer_Main");
        LocationsMenu.Add("ChoosePlayer_Tutorial");
        LocationsMenu.Add("Outro");

        LocationsNames = new string[] { "Intro", "Car", 
            "Day0",
            "Day1",
            "Day2",
            "Day3",
            "Day4",
            "Day5",
            "Day6",
            "Day7",
            "Day8",
            "Day9",
            "Day10",
            "Day11",
            "Day12",
            "Day13",
            "Day14",
            "Day15",
            "Day16",

            "FeedMum",
            "Appartment"
             };

        EndingsLocationsNames.Add("Ending Bad");
        EndingsLocationsNames.Add("Ending Bad 0");
        EndingsLocationsNames.Add("Ending Bad 1");
        EndingsLocationsNames.Add("Ending Good");
        EndingsLocationsNames.Add("Ending Good 0");
        EndingsLocationsNames.Add("Ending Good 1");


        DonotLoadStartPosLocation.Add("Intro");

        CreateLocationOnStart = new int[LocationsNames.Length];

    }
    override public void MainSave(bool SaveAll, string locationname)
    {
        string SlotName = "Slot" + _Menu.CurrentSlotNumber + _Menu.CurrentSlotPlayers[_Menu.CurrentSlotNumber];
        
        MenuSave(locationname);


        UNITY_SAVE_Location(SaveAll, SlotName);
        UNITY_SAVE_Playervariables(SaveAll, SlotName);
        UNITY_SAVE_Inventory(SaveAll, SlotName);
        UNITY_SAVE_Quests(SaveAll, SlotName);
        UNITY_SAVE_BodySlots(SaveAll, SlotName);


        for (int i = 0; i < LocationsNames.Length; i++)
        {
            if (LocationsNames[i] == SceneManager.GetActiveScene().name)
            {
                UNITY_SAVE_ObjectToDestroy(SaveAll, SlotName);
                UNITY_SAVE_CarList(SaveAll, SlotName);
                UNITY_SAVE_ObjectsToPick(SaveAll, SlotName);
                UNITY_SAVE_TriggersActivated(SaveAll, SlotName);
            }
        }
    }

    override public void MainLoad(int slotnumber)
    {
        if (_Menu.FirstStart == 0)
        {
            if (GameObject.Find("StartPoint_0") != null)
                pl._transform.SetPositionAndRotation(GameObject.Find("StartPoint_0").transform.position, GameObject.Find("StartPoint_0").transform.rotation);
         
        }

    

        Time.timeScale = 1;

        string SlotName = "Slot" + slotnumber + _Menu.CurrentSlotPlayers[slotnumber];


        UNITY_LOAD_Location(SlotName);
        UNITY_LOAD_PlayerVariables(SlotName);
        UNITY_LOAD_Inventory(SlotName);
        UNITY_LOAD_Quests(SlotName);

        UNITY_LOAD_BodySlots(SlotName);


        for (int i = 0; i < LocationsNames.Length; i++)
        {
            if (LocationsNames[i] == SceneManager.GetActiveScene().name)
            {
                UNITY_LOAD_ObjectToDestroy(SlotName);
                UNITY_LOAD_CarList(SlotName);
                UNITY_LOAD_ObjectsToPick(SlotName);
                UNITY_LOAD_TriggersActivated(SlotName);
            }
        }

        ApplyVariables(slotnumber);
    }




    void UNITY_LOAD_Location(string SlotName)
    {
        if (pl == null) return;
        if (_Menu.FirstStart == 0) return;
        PreviousLevel = PlayerPrefs.GetString("PreviousLevel UNITY_LOAD_Location" + SlotName);
        DayNumber = PlayerPrefs.GetInt("DayNumber" + SlotName);
        DayTimer = PlayerPrefs.GetFloat("DayTimer" + SlotName);

        if (_DayAndNight != null)
        {
            _DayAndNight.DayNumber = DayNumber;
            _DayAndNight.DayTimer = DayTimer;
        }

    }

    void UNITY_LOAD_PlayerVariables(string SlotName)
    {
        if (pl == null) return;

       /* if (BodyUP != null)
        {
            for (int r = 0; r < BodyUP.GetSlots().Length; r++)
            {
                if (BodyUP.GetSlots()[r].items.Count < BodyUP.GetSlots()[r].Slot.Length)
                {
                    for (int i = 0; i < BodyUP.GetSlots()[r].Slot.Length - BodyUP.GetSlots()[r].items.Count; i++)
                    {
                        BodyUP.GetSlots()[r].items.Add(new Item());
                        BodyUP.GetSlots()[r].items[i].itemID = -1;


                    }
                }
            }
        }*/

        if (_Menu.FirstStart == 1)
        {

            pl.HPMax = PlayerPrefs.GetInt("HPMax" + SlotName);
            pl.HP = PlayerPrefs.GetInt("PlayerHP" + SlotName);
       
        
            PlayerXPos = PlayerPrefs.GetFloat("PlayerXPos" + SlotName);
            PlayerZPos = PlayerPrefs.GetFloat("PlayerZPos" + SlotName);
            PlayerYPos = PlayerPrefs.GetFloat("PlayerYPos" + SlotName);


            PlayerXRotation = PlayerPrefs.GetFloat("PlayerXRotation" + SlotName);
            PlayerYRotation = PlayerPrefs.GetFloat("PlayerYRotation" + SlotName);
            PlayerZRotation = PlayerPrefs.GetFloat("PlayerZRotation" + SlotName);


            CameraXRotation = PlayerPrefs.GetFloat("CameraXRotation" + SlotName);
            CameraYRotation = PlayerPrefs.GetFloat("CameraYRotation" + SlotName);
            CameraZRotation = PlayerPrefs.GetFloat("CameraZRotation" + SlotName);




            if (pl.HPMax <= 0) pl.HPMax = StartStats.MaxHP;
            if (pl.HP > pl.HPMax) pl.HP = pl.HPMax;

        
            if (pl.HP <= 1) pl.HP = StartStats.MaxHP;
        }
        else
        {
     

            pl.HP = StartStats.MaxHP;
            pl.HPMax = StartStats.MaxHP;



            if (pl.HP <= 0) pl.HP = StartStats.MaxHP;

            if (inv != null)
            {

                for (int i = 0; i < StartStats.StartItems.Length; i++)
                    inv.AddItemNOAUDIO_NOPickedNames(StartStats.StartItems[i], StartStats.StartItemsCounts[i], inv.GetItemInDatabase(StartStats.StartItems[i]).Durability, inv.GetItemInDatabase(StartStats.StartItems[i]).AmmoInGun, new Vector2(99999, 99999));

                for (int i = 0; i < StartStats.BodyslotItems.Length; i++)
                {
                    BodyUP.AddUpgradeItemToClosestEmptySlot(StartStats.BodyslotItems[i], inv.GetItemInDatabase(StartStats.BodyslotItems[i]).Durability, inv.GetItemInDatabase(StartStats.BodyslotItems[i]).AmmoInGun);
                
                }
            }

        }



    }


    void UNITY_LOAD_ObjectToDestroy(string SlotName)
    {
        if (pl == null) return;


        int ObjectToDestroy_Count = PlayerPrefs.GetInt("ObjectToDestroy_Count" + SlotName);

        for (int i = 0; i < ObjectToDestroy_Count; i++)
        {
            string destname = PlayerPrefs.GetString("ObjectToDestroy " + i + SlotName);

            if (_Menu.FirstStart > 0)
            {
                ObjectsToDestroy.Add(destname);
             
            }

        }
    }

    void UNITY_LOAD_CarList(string SlotName)
    {
        if (pl == null) return;
        if (_Menu.FirstStart == 0) return;

        int CarList_Count = PlayerPrefs.GetInt("CarList_Count" + SlotName);

        string name = "";
        float posx = 0;
        float posy = 0;
        float posz = 0;

        for (int i = 0; i < CarList_Count; i++)
        {

            name = PlayerPrefs.GetString("CarList " + i + SlotName);
            posx = PlayerPrefs.GetFloat("CarList XPos" + i + SlotName);
            posy = PlayerPrefs.GetFloat("CarList YPos" + i + SlotName);
            posz = PlayerPrefs.GetFloat("CarList ZPos" + i + SlotName);



            if (GameObject.Find(name) != null)
            {
              

                GameObject.Find(name).GetComponent<Rigidbody>().position = new Vector3(posx, posy, posz);

            }
        }



    }




    void UNITY_LOAD_ObjectsToPick(string SlotName)
    {
        if (pl == null) return;
        if (_Menu.FirstStart == 0) return;

        int ObjectsToPick_Count = PlayerPrefs.GetInt("ObjectsToPick_Count" + SlotName);

        string name = "";
        float posx = 0;
        float posy = 0;
        float posz = 0;
        string parentname = "";

        for (int i = 0; i < ObjectsToPick_Count; i++)
        {

            name = PlayerPrefs.GetString("ObjectsToPick " + i + SlotName);
            posx = PlayerPrefs.GetFloat("ObjectsToPick XPos" + i + SlotName);
            posy = PlayerPrefs.GetFloat("ObjectsToPick YPos" + i + SlotName);
            posz = PlayerPrefs.GetFloat("ObjectsToPick ZPos" + i + SlotName);
            parentname = PlayerPrefs.GetString("ObjectsToPick Parent" + i + SlotName);

            if (_Menu.FirstStart > 0)
            {
                if (GameObject.Find(name) != null)
                {
                    if (parentname.Length <= 1)
                        GameObject.Find(name).transform.parent = null;

                    GameObject.Find(name).transform.position = new Vector3(posx, posy, posz);

                }
            }
        }



    }

    void UNITY_LOAD_TriggersActivated(string SlotName)
    {
        if (pl == null) return;
        if (_Menu.FirstStart == 0) return;

        int TriggersActivated_Count = PlayerPrefs.GetInt("TriggersActivated_Count" + SlotName);

        string name = "";

        for (int i = 0; i < TriggersActivated_Count; i++)
        {
            name = PlayerPrefs.GetString("TriggersActivated " + i + SlotName);


            if (GameObject.Find(name) != null)
            {
                TriggersActivated.Add(GameObject.Find(name));
            }



        }

        if (_Menu.FirstStart <= 0) return;

        for (int i = 0; i < TriggersActivated.Count; i++)
        {
            if (TriggersActivated[i] != null)
            {
                if (TriggersActivated[i].GetComponent<Trigger>() != null)
                {
                    for (int j = 0; j < TriggersActivated[i].GetComponent<Trigger>().TF.Length; j++)
                        TriggersActivated[i].GetComponent<Trigger>().OnTrigger(TriggersActivated[i].GetComponent<Trigger>().TF[j], j);
                }

                if (TriggersActivated[i].GetComponent<Dialog>() != null)
                    TriggersActivated[i].GetComponent<Dialog>().TurnOnAllAfterEnd();
            }
        }

    }
    void UNITY_LOAD_BodySlots(string SlotName)
    {
        if (BodyUP == null) return;


        /*
        for (int r = 0; r < BodyUP.GetSlots().Length; r++)
        {
            for (int i = 0; i < BodyUP.GetSlots()[r].items.Count; i++)
            {
                int ii = PlayerPrefs.GetInt("SlotsIDs" + r + i + SlotName);
                int iidurab = PlayerPrefs.GetInt("SlotsDurability" + r + i + SlotName);
                int iiammo = PlayerPrefs.GetInt("SlotsAmmo" + r + i + SlotName);


                print("UNITY_LOAD_BodySlots " + ii + SlotName);

                if (ii > -1 && _menu.FirstStart > 0)
                {

                    BodyUP.AddUpgradeItem(ii, iidurab, iiammo, r, i);
                    //  break;
                }

            }


        }
        */


    }


    void UNITY_LOAD_Inventory(string SlotName)
    {
        Inventory_Count = PlayerPrefs.GetInt("Inventory_Count" + SlotName);

        if (inv != null)
        {
            if (Inventory_Count > inv.slotX)
            {
                inv.slotX = Inventory_Count;

            }
        }

        if (_Menu.FirstStart == 0) return;

      
        for (int i = 0; i < Inventory_Count; i++)
        {
            int ii = PlayerPrefs.GetInt("Item" + i + SlotName);
            int iicount = PlayerPrefs.GetInt("ItemCount" + i + SlotName);
            int iidurability = PlayerPrefs.GetInt("ItemDurability" + i + SlotName);
            int iiammo = PlayerPrefs.GetInt("ItemAmmo" + i + SlotName);

            if (inv != null)
            {
                // if (ii > -1) _constr.inv.AddItem(_constr.inv.GetItemInDatabase(ii).itemID,1);
                if (ii > -1)
                {
                    if (iicount > 0)
                        inv.AddItemNOAUDIO(ii, iicount, iidurability, iiammo, new Vector2(99999, 99999));
                    else inv.AddItemNOAUDIO(ii, 1, iidurability, iiammo, new Vector2(99999, 99999));

                }
            }

        }



    }

    void UNITY_LOAD_Quests(string SlotName)
    {
        Quests_Count = PlayerPrefs.GetInt("Quests_Count" + SlotName);




        if (_Menu.FirstStart == 0) return;

        for (int i = 0; i < Quests_Count; i++)
        {
            int ii = PlayerPrefs.GetInt("Quest" + i + SlotName);
            int iidone = PlayerPrefs.GetInt("QuestDone" + i + SlotName);


            if (inv != null)
            {
                inv.AddQuest(ii);

                if (ii > -1)
                {
                    if (iidone > 0)
                        inv.DoneQuest(ii);


                }
            }

        }



    }





    void UNITY_SAVE_Location(bool SaveAll, string SlotName)
    {


        if (pl != null && SaveAll)
        {

            PreviousLevel = SceneManager.GetActiveScene().name;

            PlayerPrefs.SetString("PreviousLevel UNITY_LOAD_Location" + SlotName, PreviousLevel);


            PlayerPrefs.SetInt("DayNumber" + SlotName, DayNumber);

            if (_DayAndNight != null)
                DayTimer = _DayAndNight.DayTimer;

            PlayerPrefs.SetFloat("DayTimer" + SlotName, DayTimer);

        }
    }


    void UNITY_SAVE_Playervariables(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;

        PlayerPrefs.SetInt("HPMax" + SlotName, pl.HPMax);
        PlayerPrefs.SetInt("PlayerHP" + SlotName, pl.HP);


        PlayerPrefs.SetFloat("PlayerXPos" + SlotName, pl.transform.position.x);
        PlayerPrefs.SetFloat("PlayerZPos" + SlotName, pl.transform.position.z);
        PlayerPrefs.SetFloat("PlayerYPos" + SlotName, pl.transform.position.y);

        PlayerPrefs.SetFloat("PlayerXRotation" + SlotName, pl.transform.eulerAngles.x);
        PlayerPrefs.SetFloat("PlayerYRotation" + SlotName, pl.transform.eulerAngles.y);
        PlayerPrefs.SetFloat("PlayerZRotation" + SlotName, pl.transform.eulerAngles.z);

        PlayerPrefs.SetFloat("CameraXRotation" + SlotName, pl.MainCamera.transform.eulerAngles.x);
        PlayerPrefs.SetFloat("CameraYRotation" + SlotName, pl.MainCamera.transform.eulerAngles.y);
        PlayerPrefs.SetFloat("CameraZRotation" + SlotName, pl.MainCamera.transform.eulerAngles.z);


    }

    void UNITY_SAVE_ObjectToDestroy(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;


        PlayerPrefs.SetInt("ObjectToDestroy_Count" + SlotName, ObjectsToDestroy.Count);

        for (int i = 0; i < ObjectsToDestroy.Count; i++)
        {
            PlayerPrefs.SetString("ObjectToDestroy " + i + SlotName, ObjectsToDestroy[i]);
           
        }
    }

    void UNITY_SAVE_CarList(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;


        PlayerPrefs.SetInt("CarList_Count" + SlotName, CarList.Count);

        for (int i = 0; i < CarList.Count; i++)
        {
            PlayerPrefs.SetString("CarList " + i + SlotName, CarList[i].name);
            PlayerPrefs.SetFloat("CarList XPos" + i + SlotName, CarList[i].transform.position.x);
            PlayerPrefs.SetFloat("CarList YPos" + i + SlotName, CarList[i].transform.position.y);
            PlayerPrefs.SetFloat("CarList ZPos" + i + SlotName, CarList[i].transform.position.z);
        }
    }


    void UNITY_SAVE_ObjectsToPick(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;


        PlayerPrefs.SetInt("ObjectsToPick_Count" + SlotName, ObjectsToPick.Count);

        for (int i = 0; i < ObjectsToPick.Count; i++)
        {
            if (ObjectsToPick[i] != null)
            {
                PlayerPrefs.SetString("ObjectsToPick " + i + SlotName, ObjectsToPick[i].name);
                PlayerPrefs.SetFloat("ObjectsToPick XPos" + i + SlotName, ObjectsToPick[i].transform.position.x);
                PlayerPrefs.SetFloat("ObjectsToPick YPos" + i + SlotName, ObjectsToPick[i].transform.position.y);
                PlayerPrefs.SetFloat("ObjectsToPick ZPos" + i + SlotName, ObjectsToPick[i].transform.position.z);

                string parrentname = "";
                if (ObjectsToPick[i].transform.parent != null)
                    parrentname = ObjectsToPick[i].transform.parent.name;

                PlayerPrefs.SetString("ObjectsToPick Parent" + i + SlotName, parrentname);
            }
        }
    }

    void UNITY_SAVE_TriggersActivated(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;


        PlayerPrefs.SetInt("TriggersActivated_Count" + SlotName, TriggersActivated.Count);

        for (int i = 0; i < TriggersActivated.Count; i++)
        {
            if (TriggersActivated[i] != null)
            {
                PlayerPrefs.SetString("TriggersActivated " + i + SlotName, TriggersActivated[i].name);
            }

        }
    }


    void UNITY_SAVE_BodySlots(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll) return;




        for (int r = 0; r < BodyUP.GetSlots().Length; r++)
        {
            for (int i = 0; i < BodyUP.GetSlots()[r].items.Count; i++)
            {

                PlayerPrefs.SetInt("SlotsIDs" + r + i + SlotName, BodyUP.GetSlots()[r].items[i].itemID);
                PlayerPrefs.SetInt("SlotsDurability" + r + i + SlotName, BodyUP.GetSlots()[r].items[i].Durability);
                PlayerPrefs.SetInt("SlotsAmmo" + r + i + SlotName, BodyUP.GetSlots()[r].items[i].AmmoInGun);



            }


        }
    }


    void UNITY_SAVE_Inventory(bool SaveAll, string SlotName)
    {
        if (pl == null || !SaveAll || inv ==null) return;


        PlayerPrefs.SetInt("Inventory_Count" + SlotName, inv.inventory.Count);


        for (int i = 0; i < inv.inventory.Count; i++)
        {
            int ii = -1;
            int iicount = 1;
            int iidurability = 1;
            int iiammo = 0;

            if (inv.inventory[i] != null && inv.inventory[i].itemID > -1)
                ii = inv.inventory[i].itemID;
            iicount = inv.inventory[i].Count;
            iidurability = inv.inventory[i].Durability;
            iiammo = inv.inventory[i].AmmoInGun;

            PlayerPrefs.SetInt("Item" + i + SlotName, ii);
            PlayerPrefs.SetInt("ItemCount" + i + SlotName, iicount);
            PlayerPrefs.SetInt("ItemDurability" + i + SlotName, iidurability);
            PlayerPrefs.SetInt("ItemAmmo" + i + SlotName, iiammo);
        }
    }

    void UNITY_SAVE_Quests(bool SaveAll, string SlotName)
    {

        if (pl == null || !SaveAll) return;


        PlayerPrefs.SetInt("Quests_Count" + SlotName, inv.Quests.Count);


        for (int i = 0; i < inv.Quests.Count; i++)
        {
            int ii = -1;
            int iidone = 1;

            if (inv.Quests[i] != null && inv.Quests[i].ID > -1)
                ii = inv.Quests[i].ID;

            if (inv.Quests[i].Done)
                iidone = 1;
            else iidone = 0;



            PlayerPrefs.SetInt("Quest" + i + SlotName, ii);
            PlayerPrefs.SetInt("QuestDone" + i + SlotName, iidone);
        }
    }


    public void MenuSave(string PrevLevel)
    {

#if UNITY_STANDALONE
        UNITY_SAVE_MENU(PrevLevel);
#endif



    }

    void UNITY_SAVE_MENU(string PrevLevel)
    {
        Debug.Log("SAVE MouseSensitivity " + _Menu.MouseSensitivity);
        PlayerPrefs.SetFloat("MasterSlider", _Menu.MasterSliderValue);
        PlayerPrefs.SetFloat("BGSlider", _Menu.BGSliderValue);
        PlayerPrefs.SetFloat("ObjectsSlider", _Menu.ObjectsSliderValue);

        PlayerPrefs.SetInt("Resolution", _Menu.ResolutionNumber);
        PlayerPrefs.SetInt("WindowNumber", _Menu.WindowNumber);

        PlayerPrefs.SetInt("Language", _Menu.Language);
        PlayerPrefs.SetInt("DrawTutorial", _Menu.DrawTutorial);
        PlayerPrefs.SetInt("FirstStart", _Menu.FirstStart);
        PlayerPrefs.SetInt("FirstLanguage", _Menu.FirstLanguage);


        PlayerPrefs.SetFloat("MouseSensitivity", _Menu.MouseSensitivity);

        if (_Menu.HideUI)
        PlayerPrefs.SetInt("HideUI",1);
        else PlayerPrefs.SetInt("HideUI", 0);

        PlayerPrefs.SetInt("CurrentSlotNumber", _Menu.CurrentSlotNumber);


        for (int i = 0; i < _Menu.CurrentSlotLocations.Length; i++)
        {
            PlayerPrefs.SetString("CurrentSlotNumber" + i + "Location", _Menu.CurrentSlotLocations[i]);
            Debug.Log("SAVE Save slot " + i + " / " + _Menu.CurrentSlotLocations[i]);

            PlayerPrefs.SetString("CurrentSlotNumber" + i + "Time", _Menu.CurrentSlotTimes[i]);
            PlayerPrefs.SetString("CurrentSlotNumber" + i + "Date", _Menu.CurrentSlotDates[i]);
            PlayerPrefs.SetInt("CurrentSlotPlayers" + i, _Menu.CurrentSlotPlayers[i]);
        }


        PlayerPrefs.SetString("PrevLevel", PrevLevel);
        PlayerPrefs.SetInt("FirstEnding", _Menu.FirstEnding);


    }

    public override void MenuLoad()
    {
        if (_Menu == null) _Menu = InitializeOnAwake._Menu;

        if (InitializeOnAwake._Menu == null) Debug.Log("Menu is null");

        _Menu.Slots = new List<GameObject>();
        _Menu.SaveSlotsUIOB = GameObject.Find("SaveSlotsUI");
        _Menu.CurrentSlotLocations = new string[] { " ", " ", " ", " ", " ", " ", " " };
        _Menu.CurrentSlotDates = new string[] { " ", " ", " ", " ", " ", " ", " " };
        _Menu.CurrentSlotTimes = new string[] { " ", " ", " ", " ", " ", " ", " " };
        _Menu.CurrentSlotPlayers = new int[] { 0, 0, 0, 0, 0, 0, 0 };

         UNITY_LOAD_MENU();

    }



    void UNITY_LOAD_MENU()
    {

        if (_Menu.Slots.Count < 6)
        {
            for (int i = 0; i < 6; i++)
            {
                _Menu.Slots.Add(_Menu.SaveSlotsUIOB.transform.Find("Slot (" + i + ")").gameObject);
            }
        }

        if (GameObject.Find("Player") != null)
        {
            if (pl == null) pl = GameObject.Find("Player").GetComponent<Player>();
            if (_Menu == null) _Menu = GameObject.Find("Player").GetComponent<Menu>();
        }

        _Menu.MasterSliderValue = PlayerPrefs.GetFloat("MasterSlider");
        _Menu.BGSliderValue = PlayerPrefs.GetFloat("BGSlider");
        _Menu.ObjectsSliderValue = PlayerPrefs.GetFloat("ObjectsSlider");

        _Menu.ResolutionNumber = PlayerPrefs.GetInt("Resolution");
        _Menu.WindowNumber = PlayerPrefs.GetInt("WindowNumber");

        _Menu.Language = PlayerPrefs.GetInt("Language");
        _Menu.DrawTutorial = PlayerPrefs.GetInt("DrawTutorial");
        _Menu.FirstStart = PlayerPrefs.GetInt("FirstStart");
        _Menu.FirstLanguage = PlayerPrefs.GetInt("FirstLanguage");
        _Menu.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        Debug.Log("LOAD MouseSensitivity " + _Menu.MouseSensitivity);

        
        _Menu.HideUI = PlayerPrefs.GetInt("HideUI") == 1 ? true : false;
    


        _Menu.CurrentSlotNumber = PlayerPrefs.GetInt("CurrentSlotNumber");

        for (int i = 0; i < _Menu.CurrentSlotLocations.Length; i++)
        {

            _Menu.CurrentSlotLocations[i] = PlayerPrefs.GetString("CurrentSlotNumber" + i + "Location");
            _Menu.CurrentSlotTimes[i] = PlayerPrefs.GetString("CurrentSlotNumber" + i + "Time");
            _Menu.CurrentSlotDates[i] = PlayerPrefs.GetString("CurrentSlotNumber" + i + "Date");
            _Menu.CurrentSlotPlayers[i] = PlayerPrefs.GetInt("CurrentSlotPlayers" + i);

             Debug.Log("LOAD Save slot " + i + " / " + _Menu.CurrentSlotLocations[i]);

        }

        if (pl != null)
        {
            _Menu.CurrentSlotPlayers[_Menu.CurrentSlotNumber] = pl.CurrentPlayer;

        }
        PreviousLevel = PlayerPrefs.GetString("PrevLevel");
        _Menu.FirstEnding = PlayerPrefs.GetInt("FirstEnding");



    }


}
