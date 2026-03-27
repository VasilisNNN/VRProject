using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class StartStart
{

    public string PlayerName = "";

    public int MaxHP;
    public int MaxFuel;
    public int MaxWater;
    public int MaxHeat;

    public int[] StartItems;
    public int[] StartItemsCounts;

    public int[] BodyslotItems;

    public StartStart(int maxhp, int maxFuel, int maxWater, int maxHeat, int[] starti, int[] starticounts)
    {

        MaxHP = maxhp;
        MaxFuel = maxFuel;
        MaxWater = maxWater;
        MaxHeat = maxHeat;

        StartItems = starti;
        StartItemsCounts = starticounts;
    }

}

public class SaveLoad : MonoBehaviour
{

    public StartStart StartStats;

    private Player pl;
    private Inventory inv;
    private Menu _Menu;
    private DayAndNight _DayAndNight;


    

    private List<string> MenuLocations = new List<string>();




    public bool Saving { get; set; }
    public bool Loading { get; set; }

    public int SaveTimer { get; private set; }
    public int LoadTimer { get; private set; }

 
    
    public SaveLoadBasic SaveLoadCurrent;
    private SwitchSaveLoad SaveLoad_Switch = new SwitchSaveLoad();
    private PCSaveLoad SaveLoad_PC = new PCSaveLoad();

    public void Init()
    {
        SaveLoad_PC = new PCSaveLoad();
        SaveLoad_Switch = new SwitchSaveLoad();

        SaveLoad_Switch.StartStats = StartStats;
        SaveLoad_PC.StartStats = StartStats;

        SaveLoad_PC.Init();
        SaveLoadCurrent = SaveLoad_PC;


#if UNITY_SWITCH
      
        SaveLoad_Switch.Init();
        SaveLoadCurrent = SaveLoad_Switch;
     
#endif
        

#if UNITY_SWITCH

        mountName = "DadLeftMeSave";
        fileName = "DadLeftMeSaveData";

        filePath = string.Format("{0}:/{1}", mountName, fileName);
#endif



        if (GameObject.Find("Player") != null)
        {
            pl = InitializeOnAwake.pl;
            if(InitializeOnAwake.inv != null)
            inv = InitializeOnAwake.inv;

        }

     
        _Menu = InitializeOnAwake._Menu;

        if (GameObject.Find("DayAndNight") != null)
            _DayAndNight = GameObject.Find("DayAndNight").GetComponent<DayAndNight>();



        MenuLocations.Add("StartMenu");

        MenuLocations.Add("StartLoadScene");

        Time.timeScale = 1;
        _Menu.Init();
        SaveLoadCurrent.MenuLoad();

        if (!MenuLocations.Contains(SceneManager.GetActiveScene().name))
            MainLoad();

        _Menu.LoadMenu();

        SetObjectsToDestroy();
       
    }

    private void OnApplicationQuit()
    {
#if UNITY_SWITCH
        nn.fs.FileSystem.Unmount(mountName);
#endif
    }

    public void MainLoad()
    {

        SaveLoadCurrent.MainLoad(_Menu.CurrentSlotNumber);
 

    }

    private void Update()
    {

        SetPlayerPosition();
    }


    void SetPlayerPosition()
    {
        if (SaveLoadCurrent == null) return;

        if (!SaveLoadCurrent.SetPlPos) return;


        if (GameObject.Find("ForcedStartPosition") != null)
        {

            pl.transform.position = GameObject.Find("ForcedStartPosition").transform.position;
            pl.Legscoll_obj = new List<GameObject>();
            SaveLoadCurrent.SetPlPos = false;
            SaveLoadCurrent.SetPlPosTimer = -1;

            return;
             
        }


        if (SaveLoadCurrent.SetPlPosTimer > Time.fixedTime)
        {
           /* pl.transform.position = new Vector3(SaveLoadCurrent.PlayerXPos, SaveLoadCurrent.PlayerYPos, SaveLoadCurrent.PlayerZPos);
            print("load pl position ");
            pl.transform.rotation = Quaternion.Euler(SaveLoadCurrent.PlayerXRotation, SaveLoadCurrent.PlayerYRotation, SaveLoadCurrent.PlayerZRotation);
            pl.MainCamera.transform.eulerAngles =new Vector3(SaveLoadCurrent.CameraXRotation, SaveLoadCurrent.CameraYRotation, SaveLoadCurrent.CameraZRotation);
            pl.SetStartRotation(SaveLoadCurrent.CameraXRotation, SaveLoadCurrent.CameraYRotation);
            SaveLoadCurrent.SetPlPos = false;*/
            return;
        }

      
    }


    public void Save(bool SaveAll, int slot, string PrevLevel)
    {
        if (pl != null && SaveAll && !MenuLocations.Contains(SceneManager.GetActiveScene().name))
        {
            _Menu.FirstStart = 1;
            print("Change FS");
        }

        if (SaveLoadCurrent.EndingsLocationsNames.Contains(SceneManager.GetActiveScene().name))
        {
            _Menu.FirstEnding = 1;
        }

       if (SaveAll)
            _Menu.CurrentSlotLocations[slot] = SceneManager.GetActiveScene().name;


        _Menu.CurrentSlotNumber = slot;

        if(pl!=null)
            _Menu.CurrentSlotPlayers[slot] = pl.CurrentPlayer;

        SaveLoadCurrent.MainSave(SaveAll, PrevLevel);

    }





    public void SetObjectsToDestroy()
    {

        if (SaveLoadCurrent.ObjectsToDestroy_Count <= 0) return;

        for (int i = 0; i < SaveLoadCurrent.ObjectsToDestroy.Count; i++)
        {

            if (GameObject.Find(SaveLoadCurrent.ObjectsToDestroy[i]) != null)
            {
                Destroy(GameObject.Find(SaveLoadCurrent.ObjectsToDestroy[i]));


            }


        }

    }

    public void ResetVars()
    {
        PlayerPrefs.SetString("LastLevel", "");

    }



   




}