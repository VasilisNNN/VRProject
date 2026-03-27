using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ISaveLoad
{
    [HideInInspector]
    public static bool SaveExists;



    public static List<int> VaultIDs { get; set; }
    public static List<int> VaultCount { get; set; }
    public static List<string> Tile_names = new List<string>();

    public static List<int> Tile_xpos = new List<int>();
    public static List<int> Tile_ypos = new List<int>();




    public static List<int> OB_IDs = new List<int>();
    public static List<string> OB_names = new List<string>();
    public static List<float> OB_xpos = new List<float>();
    public static List<float> OB_ypos = new List<float>();
    public static List<int> OB_horscale = new List<int>();
    public static List<string> OB_SpawnPoint = new List<string>();

    public static List<int> GenMap_GrowStates { get; set; }

    public static List<int> PreplacedObjects_GrowStates { get; set; }

    public static List<int> Dropped_IDs { get; set; }
    public static List<int> Dropped_Counts { get; set; }
    public static List<string> Dropped_names { get; set; }
    public static List<float> Dropped_xpos { get; set; }
    public static List<float> Dropped_ypos { get; set; }

    public static List<int> FloorStates = new List<int>();
    public static int RNDSTART_Y, RNDSTART_X, RNDStablePos, ObjectPlacement_seed_Start;

    public static Vector2Int RNDSHIFT;


    public static List<string> Trash_names = new List<string>();
    public static List<float> Trash_xpos = new List<float>();
    public static List<float> Trash_ypos = new List<float>();
    public static List<int> GrowStateList = new List<int>();

    [HideInInspector]
    public static List<int> Unlocked_IDs = new List<int>();

    public static int PreplacedObjects_Count, GenMap_GrowStates_Count, TOnBoard_Count, PitsOnBoard_Count, ConstructedStructures_Count, TRASHOnBoard_Count, ObjectsToDestroy_Count, DroppedItems_Count, Inventory_Count, UnlockedItems_Count;

    private static bool Resetpol = false;

    public static List<string> LocationsMenu = new List<string>();


    public static int SavingState, LoadingState;

    private static string[] TileNames;

  

    public static List<string> ACHNames = new List<string>();
    public static List<string> ObjectsToDestroy = new List<string>();


    public static string LastLocation { get; set; }


   
    public static string[] LocationsNames;
    public static int[] CreateLocationOnStart;

    public static List<int> BPConstructed = new List<int>();

    public static int DayNumber;
    public static float DayTime;
    public static int _TutorialPhase { get; set; }

    public static int CurrentCharacter { get; set; }
    
}
