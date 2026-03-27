using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
#if UNITY_SWITCH
using nn;

#endif


public class SwitchSaveLoad : SaveLoadBasic
{
    private Player pl;
    private Menu _menu;
    private Inventory inv;
    private Upgrades UP;
    private BodySlots BodyUP;

    public bool Saving;
    public bool SaveAll;
    public int slot;

    public string locationname;

    [HideInInspector]
    public List<string> ObjectToDestroy = new List<string>();
    [HideInInspector]
    public List<string> ObjectsToDisable = new List<string>();
    [HideInInspector]
    public List<GameObject> CarList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> ObjectsToPick = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> TriggersActivated = new List<GameObject>();

    [HideInInspector]
    public string[] LocationsNames;

    private DayAndNight _DayAndNight;
    private float DayTimer;
    private string PreviousLevel;
    private int DayNumber;

    [HideInInspector]
    public float PlayerXPos, PlayerZPos, PlayerYPos,
      PlayerXRotation, PlayerYRotation, PlayerZRotation,
      CameraXRotation, CameraYRotation, CameraZRotation;

    public StartStart StartStats;
    private int Inventory_Count, Quests_Count;
#if UNITY_SWITCH
    public nn.account.UserHandle userHandle;
    public nn.account.Uid userId;

    [HideInInspector]
    public string mountName = "DadLeftMeSave";
    private string fileName = "DadLeftMeSaveData";

    private string filePath;
    private nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

    private const int saveDataVersion = 1;
    private const int saveDataSize = 2048;
    private const int MenusaveDataSize = 512;

#endif
    public override void Init()
    {
#if UNITY_SWITCH
        if (GameObject.Find("Player") != null)
        {
            if (pl == null) pl = GameObject.Find("Player").GetComponent<Player>();
            if (_menu == null) _menu = GameObject.Find("Player").GetComponent<Menu>();
            inv = pl.GetComponent<Inventory>();
        }

        if (GameObject.Find("Upgrades") != null)
            UP = GameObject.Find("Upgrades").GetComponent<Upgrades>();

        if (GameObject.Find("BodySlots") != null)
            BodyUP = GameObject.Find("BodySlots").GetComponent<BodySlots>();

        _menu = GetComponent<Menu>();

        if (GameObject.Find("DayAndNight") != null)
            _DayAndNight = GameObject.Find("DayAndNight").GetComponent<DayAndNight>();

#endif

    }
    public override void MainSave(bool saveall, string locationname)
    {
#if UNITY_SWITCH
        byte[] data;
        byte[] MenuSavedata;

        // long thisDatasizeStart = saveDataSize * _menu.CurrentSlotNumber + MenusaveDataSize;
        long thisDatasizeStart = MenusaveDataSize;

        int currentpl = 0;
        if (pl != null)
        {
            _menu.CurrentSlotPlayers[_menu.CurrentSlotNumber] = pl.CurrentPlayer;
            currentpl = pl.CurrentPlayer;
        }

        int thisDatasize = saveDataSize * slot+ saveDataSize * currentpl * 7 + MenusaveDataSize;


        using (MemoryStream stream = new MemoryStream(MenusaveDataSize))
        {
        
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((double)_menu.MasterSliderValue);
            print("save slot " + slot);

            writer.Write((double)_menu.BGSliderValue);
            writer.Write((double)_menu.ObjectsSliderValue);

            
            writer.Write(_menu.ResolutionNumber);
                writer.Write(_menu.WindowNumber);
    
            writer.Write(_menu.Language);
            writer.Write(_menu.DrawTutorial);
            writer.Write(_menu.FirstStart);
            writer.Write(_menu.FirstLanguage);
        
            writer.Write((double)_menu.MouseSensitivity);

            _menu.CurrentSlotNumber = slot;

            writer.Write(slot);
            

            for (int i = 0; i < 7; i++)
            {
                print("save slot 2" + slot);
                writer.Write(_menu.CurrentSlotDates[i]);
                writer.Write(_menu.CurrentSlotLocations[i]);
                writer.Write(_menu.CurrentSlotTimes[i]);
                writer.Write(_menu.CurrentSlotPlayers[i]);
            }

            print("save slot 3" + slot);

            writer.Write(locationname);

      
             writer.Write(_menu.FirstEnding);
          

            stream.Close();
            MenuSavedata = stream.GetBuffer();
        }


        using (MemoryStream stream2 = new MemoryStream(saveDataSize))
        {
            BinaryWriter writer = new BinaryWriter(stream2);
            
 
            if (pl != null && SaveAll)
            {

            SWITCH_SAVE_Location(ref writer);
            SWITCH_SAVE_Playervariables(ref writer);
            SWITCH_SAVE_Inventory(ref writer);
            SWITCH_SAVE_Quests(ref writer);
            SWITCH_SAVE_Upgrades(ref writer);
            SWITCH_SAVE_BodySlots(ref writer);
          
           for (int i = 0; i < LocationsNames.Length; i++)
           {
                if (LocationsNames[i] == SceneManager.GetActiveScene().name)
                {
          
                  SWITCH_SAVE_ObjectToDestroy(ref writer);
                  SWITCH_SAVE_CarList(ref writer);
                  SWITCH_SAVE_ObjectsToPick(ref writer);
                   SWITCH_SAVE_TriggersActivated(ref writer);
                }
            }
                     
                Saving = false;
            }

            
            stream2.Close();
            data = stream2.GetBuffer();
            

        }


#if UNITY_SWITCH
        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

        nn.Result result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        //result.abortUnlessSuccess();

        if (!result.IsSuccess())
        {
            result = nn.fs.File.Create(filePath, thisDatasize);
            result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        }

        long CurrentSize = 0;
        nn.Result resultsize = nn.fs.File.GetSize(ref CurrentSize, fileHandle);

        if (CurrentSize < thisDatasize)
        {
            result = nn.fs.File.SetSize(fileHandle, thisDatasize);
        }
        if (CurrentSize > thisDatasize)
        {
            result = nn.fs.File.SetSize(fileHandle, CurrentSize);
        }

        result = nn.fs.File.Write(fileHandle, 0, MenuSavedata, MenuSavedata.LongLength, nn.fs.WriteOption.Flush);
        result.abortUnlessSuccess();

        if (pl != null && SaveAll)
        {
            result = nn.fs.File.Write(fileHandle, thisDatasizeStart, data, data.LongLength, nn.fs.WriteOption.Flush);
            result.abortUnlessSuccess();
        }

        nn.fs.File.Close(fileHandle);
        result = nn.fs.FileSystem.Commit(mountName);
        result.abortUnlessSuccess();

#if UNITY_SWITCH
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif



        Saving = false;

#endif
    }

    public override void MenuLoad()
    { 
    
    }


    public override void MainLoad(int slotnumber)
    {
        SWITCH_LOAD(_menu.CurrentSlotNumber);
    }




    void SWITCH_SAVE_Location(ref BinaryWriter writer)
    {


        PreviousLevel = SceneManager.GetActiveScene().name;

        writer.Write(PreviousLevel);
        writer.Write(DayNumber);

        if (_DayAndNight != null)
            DayTimer = _DayAndNight.DayTimer;

        writer.Write(DayTimer);

    }

    void SWITCH_SAVE_Playervariables(ref BinaryWriter writer)
    {
        writer.Write(pl.HPMax);
        writer.Write(pl.HP);

        writer.Write(pl.PistolDamage);
        writer.Write(pl.ShotgunDamage);
        writer.Write(pl.RifleDamage);

        writer.Write(pl.Hunger);

        writer.Write((double)pl.transform.position.x);
        writer.Write((double)pl.transform.position.z);
        writer.Write((double)pl.transform.position.y);

    }

    void SWITCH_SAVE_ObjectToDestroy(ref BinaryWriter writer)
    {
        writer.Write(ObjectToDestroy.Count);

        for (int i = 0; i < ObjectToDestroy.Count; i++)
            writer.Write(ObjectToDestroy[i]);

    }


    void SWITCH_SAVE_CarList(ref BinaryWriter writer)
    {
        writer.Write(CarList.Count);

        for (int i = 0; i < CarList.Count; i++)
        {
            writer.Write(CarList[i].name);
            writer.Write((double)CarList[i].transform.position.x);
            writer.Write((double)CarList[i].transform.position.y);
            writer.Write((double)CarList[i].transform.position.z);
        }

    }

    void SWITCH_SAVE_ObjectsToPick(ref BinaryWriter writer)
    {
        writer.Write(ObjectsToPick.Count);

        for (int i = 0; i < ObjectsToPick.Count; i++)
        {
            writer.Write(ObjectsToPick[i].name);
            writer.Write((double)ObjectsToPick[i].transform.position.x);
            writer.Write((double)ObjectsToPick[i].transform.position.y);
            writer.Write((double)ObjectsToPick[i].transform.position.z);

            string parrentname = "";
            if (ObjectsToPick[i].transform.parent != null)
                parrentname = ObjectsToPick[i].transform.parent.name;

            writer.Write(parrentname);
        }

    }
    void SWITCH_SAVE_TriggersActivated(ref BinaryWriter writer)
    {
        writer.Write(TriggersActivated.Count);

        for (int i = 0; i < TriggersActivated.Count; i++)
        {
            writer.Write(TriggersActivated[i].name);

        }

    }


    void SWITCH_SAVE_Upgrades(ref BinaryWriter writer)
    {



        for (int x = 0; x < UP.Slots.Length; x++)
            for (int y = 0; y < UP.Slots[x].Line.Length; y++)
            {
                writer.Write(UP.Slots[x].Line[y].CurrentLevel);

            }


    }

    void SWITCH_SAVE_BodySlots(ref BinaryWriter writer)
    {

        for (int r = 0; r < BodyUP.GetSlots().Length; r++)
        {
            for (int i = 0; i < BodyUP.GetSlots()[r].items.Count; i++)
            {

                writer.Write(BodyUP.GetSlots()[r].items[i].itemID);
                writer.Write(BodyUP.GetSlots()[r].items[i].Durability);
                writer.Write(BodyUP.GetSlots()[r].items[i].AmmoInGun);

            }


        }

    }


    void SWITCH_SAVE_Inventory(ref BinaryWriter writer)
    {


        writer.Write(inv.inventory.Count);


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

            writer.Write(ii);
            writer.Write(iicount);
            writer.Write(iidurability);
            writer.Write(iiammo);

        }


    }


    void SWITCH_SAVE_Quests(ref BinaryWriter writer)
    {

        writer.Write(inv.Quests.Count);


        for (int i = 0; i < inv.Quests.Count; i++)
        {
            int ii = -1;
            int iidone = 1;

            if (inv.Quests[i] != null && inv.Quests[i].ID > -1)
                ii = inv.Quests[i].ID;

            if (inv.Quests[i].Done)
                iidone = 1;
            else iidone = 0;



            writer.Write(ii);
            writer.Write(iidone);
        }

    }

    void SWITCH_LOAD(int slotnumber)
    {
#if UNITY_SWITCH

        if (pl == null) return;
        long thisDatasizeStart = MenusaveDataSize;

        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);

        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result)) { return; }
        result.abortUnlessSuccess();

        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        result.abortUnlessSuccess();

        if (!result.IsSuccess())
        {
            result = nn.fs.File.Create(filePath, saveDataSize);
            result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        }

        long fileSize = 0;
        result = nn.fs.File.GetSize(ref fileSize, fileHandle);
        result.abortUnlessSuccess();

        if (fileSize < saveDataSize * slotnumber + MenusaveDataSize || !result.IsSuccess())
        {
            long s = saveDataSize * slotnumber + saveDataSize * _menu.CurrentSlotPlayers[slotnumber]*7 + MenusaveDataSize;
            result = nn.fs.File.SetSize(fileHandle, s);
            result.abortUnlessSuccess();
        }

        long truefsize = fileSize - thisDatasizeStart;

        byte[] data = new byte[truefsize];

        // Debug.Log("LOAD thisDatasizeStart: " + thisDatasizeStart);

        result = nn.fs.File.Read(fileHandle, thisDatasizeStart, data, data.LongLength);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);

        //  Debug.Log("_menu.CreateLocationOnStart: " + _menu.CreateLocationOnStart);


        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);
            
            SWITCH_LOAD_Location(ref reader);
            SWITCH_LOAD_Playervariables(ref reader);
            SWITCH_LOAD_Inventory(ref reader);
            SWITCH_LOAD_Quests(ref reader);
            SWITCH_LOAD_Upgrades(ref reader);
            SWITCH_LOAD_BodySlots(ref reader);
    

          for (int i = 0; i < LocationsNames.Length; i++)
            {
                if (LocationsNames[i] == SceneManager.GetActiveScene().name)
                {
                    SWITCH_LOAD_ObjectToDestroy(ref reader);
                    SWITCH_LOAD_CarList(ref reader);
                    SWITCH_LOAD_ObjectsToPick(ref reader);
                    SWITCH_LOAD_TriggersActivated(ref reader);
                }
            }
           
            if (_DayAndNight != null)
            {
                 
                _DayAndNight.DayNumber = DayNumber;
                _DayAndNight.Day = DayTimer;
            }

        }

        if (PreviousLevel == SceneManager.GetActiveScene().name )
        {
            if (pl != null)
            {
               // pl.transform.position = new Vector3(PlayerXPos, PlayerYPos, PlayerZPos);

                print(PreviousLevel + " pl.transform.position " + new Vector3(PlayerXPos, PlayerYPos, PlayerZPos));
            }
        }

#endif
    }
    void SWITCH_LOAD_Location(ref BinaryReader reader)
    {
        if (_menu.FirstStart == 0) return;
        PreviousLevel = reader.ReadString();
        DayNumber = reader.ReadInt32();
        DayTimer = reader.ReadInt32();

        if (_DayAndNight != null)
        {
            _DayAndNight.DayNumber = DayNumber;
            _DayAndNight.DayTimer = DayTimer;
        }
    }



    void SWITCH_LOAD_Playervariables(ref BinaryReader reader)
    {

        if (BodyUP != null)
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
        }

        int hpmax = reader.ReadInt32();
        int hp = reader.ReadInt32();

        int hunger = reader.ReadInt32();

        int heat = reader.ReadInt32();
        int water = reader.ReadInt32();

        int pistoldamage = reader.ReadInt32();
        int shotgundamage = reader.ReadInt32();
        int rifledamage = reader.ReadInt32();

        double playerxPos = reader.ReadDouble();
        double playerzPos = reader.ReadDouble();
        double playeryPos = reader.ReadDouble();
        double playerYRotation = reader.ReadDouble();


        if (_menu.FirstStart == 1)
        {

            pl.HPMax = hpmax;
            pl.HP = hp;
         


            PlayerXPos = (float)playerxPos;
            PlayerZPos = (float)playerzPos;
            PlayerYPos = (float)playeryPos;
            PlayerYRotation = (float)playerYRotation;


            if (pl.HP <= 0) pl.HP = StartStats.MaxHP;

            if (pl.HP > pl.HPMax) pl.HP = StartStats.MaxHP;

            if (pl.HPMax <= 0) pl.HPMax = 30;






        }
        else
        {
            
            pl.HP = StartStats.MaxHP;
            pl.HPMax = StartStats.MaxHP;


            if (pl.HP <= 0) pl.HP = StartStats.MaxHP;

            pl.Hunger = 0;

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
    void SWITCH_LOAD_ObjectToDestroy(ref BinaryReader reader)
    {

        int ObjectToDestroy_Count = reader.ReadInt32();

        for (int i = 0; i < ObjectToDestroy_Count; i++)
        {
            string destname = reader.ReadString();

            if (_menu.FirstStart > 0)
            {
                ObjectToDestroy.Add(destname);
                MonoBehaviour.Destroy(GameObject.Find(destname));
            }

        }


    }



    void SWITCH_LOAD_CarList(ref BinaryReader reader)
    {



        int CarList_Count = reader.ReadInt32();

        string name = "";
        float posx = 0;
        float posy = 0;
        float posz = 0;

        for (int i = 0; i < CarList_Count; i++)
        {

            name = reader.ReadString();
            posx = (float)reader.ReadDouble();
            posy = (float)reader.ReadDouble();
            posz = (float)reader.ReadDouble();

            if (GameObject.Find(name) != null && _menu.FirstStart != 0)
            {
                GameObject.Find(name).transform.position = new Vector3(posx, posy, posz);

            }
        }
    }


    void SWITCH_LOAD_ObjectsToPick(ref BinaryReader reader)
    {



        int ObjectsToPick_Count = reader.ReadInt32();

        string name = "";
        float posx = 0;
        float posy = 0;
        float posz = 0;

        for (int i = 0; i < ObjectsToPick_Count; i++)
        {

            name = reader.ReadString();
            posx = (float)reader.ReadDouble();
            posy = (float)reader.ReadDouble();
            posz = (float)reader.ReadDouble();
            if (_menu.FirstStart > 0)
            {
                if (GameObject.Find(name) != null && _menu.FirstStart != 0)
                {
                    GameObject.Find(name).transform.position = new Vector3(posx, posy, posz);

                }
            }
        }
    }
    void SWITCH_LOAD_TriggersActivated(ref BinaryReader reader)
    {



        int TriggersActivated_Count = reader.ReadInt32();

        string name = "";


        for (int i = 0; i < TriggersActivated_Count; i++)
        {

            name = reader.ReadString();
            if (_menu.FirstStart > 0)
            {
                if (GameObject.Find(name) != null && _menu.FirstStart != 0)
                {
                    TriggersActivated.Add(GameObject.Find(name));
                    for (int j = 0; j < TriggersActivated[i].GetComponent<Trigger>().TF.Length; j++)
                        TriggersActivated[i].GetComponent<Trigger>().OnTrigger(TriggersActivated[i].GetComponent<Trigger>().TF[j], j);

                }
            }
        }
    }
    void SWITCH_LOAD_Upgrades(ref BinaryReader reader)
    {
        if (pl == null) return;

        if (UP == null) return;

        for (int x = 0; x < UP.Slots.Length; x++)
            for (int y = 0; y < UP.Slots[x].Line.Length; y++)
            {
                int upgradesSlots = reader.ReadInt32();

                if (_menu.FirstStart > 0)
                    UP.Slots[x].Line[y].CurrentLevel = upgradesSlots;
            }

    }



    void SWITCH_LOAD_BodySlots(ref BinaryReader reader)
    {
        if (BodyUP == null) return;



        for (int r = 0; r < BodyUP.GetSlots().Length; r++)
        {
            for (int i = 0; i < BodyUP.GetSlots()[r].items.Count; i++)
            {
                int ii = reader.ReadInt32();
                int iidurab = reader.ReadInt32();
                int iiammo = reader.ReadInt32();


               
                if (ii > -1 && _menu.FirstStart > 0)
                {

                    BodyUP.AddUpgradeItem(ii, iidurab, iiammo, r, i);
                    //  break;
                }

            }


        }
    }

    void SWITCH_LOAD_Inventory(ref BinaryReader reader)
    {

        Inventory_Count = reader.ReadInt32();

        if (inv != null)
        {
            if (Inventory_Count > inv.slotX)
            {
                inv.slotX = Inventory_Count;

            }
        }


        if (_menu.FirstStart == 0) return;



        for (int i = 0; i < Inventory_Count; i++)
        {
            int ii = reader.ReadInt32();
            int iicount = reader.ReadInt32();
            int iidurability = reader.ReadInt32();
            int iiammo = reader.ReadInt32();

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




    void SWITCH_LOAD_Quests(ref BinaryReader reader)
    {
        Quests_Count = reader.ReadInt32();


        if (_menu.FirstStart <= 0) return;



        for (int i = 0; i < Quests_Count; i++)
        {
            int ii = reader.ReadInt32();
            int iidone = reader.ReadInt32();


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


    void SWITCH_LOAD_MENU()
    {
#if UNITY_SWITCH
        

        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result)) { return; }
        result.abortUnlessSuccess();
      
        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        result.abortUnlessSuccess();
       
        if (!result.IsSuccess())
        {
            result = nn.fs.File.Create(filePath, MenusaveDataSize);
            result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        }

        long fileSize = 0;
        result = nn.fs.File.GetSize(ref fileSize, fileHandle);
        result.abortUnlessSuccess();

        if (fileSize < MenusaveDataSize || !result.IsSuccess())
        {
            long s = MenusaveDataSize;
            result = nn.fs.File.SetSize(fileHandle, s);
            result.abortUnlessSuccess();
        }

        long truefsize = MenusaveDataSize;

        byte[] data = new byte[truefsize];
      
        result = nn.fs.File.Read(fileHandle, 0, data, data.LongLength);
        result.abortUnlessSuccess();
        
        nn.fs.File.Close(fileHandle);

        

        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);


            _menu.MasterSliderValue = (float)reader.ReadDouble();
         
            _menu.BGSliderValue = (float)reader.ReadDouble();
            _menu.ObjectsSliderValue = (float)reader.ReadDouble();

            _menu.ResolutionNumber = reader.ReadInt32();
             _menu.WindowNumber = reader.ReadInt32();
            _menu.Language = reader.ReadInt32();
            _menu.DrawTutorial = reader.ReadInt32();
            _menu.FirstStart = reader.ReadInt32();
            _menu.FirstLanguage = reader.ReadInt32();
        
            _menu.MouseSensitivity = (float)reader.ReadDouble();

            _menu.CurrentSlotNumber = reader.ReadInt32();



            for (int i = 0; i < 7; i++)
            {
                _menu.CurrentSlotDates[i] = reader.ReadString();
                _menu.CurrentSlotLocations[i] = reader.ReadString();
                _menu.CurrentSlotTimes[i] = reader.ReadString();
                _menu.CurrentSlotPlayers[i] = reader.ReadInt32();
            }


            PreviousLevel = reader.ReadString();

              if(pl!=null)
             _menu.CurrentSlotPlayers[_menu.CurrentSlotNumber] = pl.CurrentPlayer;
        

            print("READ PreviousLevel " + PreviousLevel);

            SaveTimer = reader.ReadInt32();
            LoadTimer = reader.ReadInt32();

           _menu.FirstEnding = reader.ReadInt32();
        }
#endif
    }
}
