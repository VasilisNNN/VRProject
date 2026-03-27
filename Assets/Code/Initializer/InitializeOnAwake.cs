using UnityEngine;
using Meta.XR.MRUtilityKit;
using UnityEngine.XR;
using UnityEngine.Events;
using System.Linq;
public class InitializeOnAwake : MonoBehaviour
{

    public static Player pl;
    public static SaveLoad SL;
    public static Camera MainCamera;
    public static Inventory inv;
    public static InputMode IM;
    public static Menu _Menu;
    public bool cutscene;
    public bool test;
    public bool createRoomMesh;

    private LanguageControll Language_Controll;
    private float RoomMeshDelay;
    private bool RoomMeshCreated;
    void Awake()
    {
        GameObject plob;

        if (test)
        {
            plob = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/PlayerFPSNoHands"));
        }
        else
        {
            if (!cutscene)
                plob = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/PlayerFPS"));
            else plob = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/PlayerMenu"));
        }
   
        plob.name = "Player";
        IM = plob.GetComponent<InputMode>();
        if (!cutscene)
        {
            pl = plob.GetComponent<Player>();
            pl.transform.position = GameObject.Find("StartPosition").transform.position;
           
            pl.GetComponent<Rigidbody>().MovePosition(GameObject.Find("StartPosition").transform.position);
            inv = plob.GetComponent<Inventory>();
        }

        _Menu = plob.GetComponent<Menu>();
        SL = plob.GetComponent<SaveLoad>();
        plob.AddComponent<LanguageControll>();
        Language_Controll = plob.GetComponent<LanguageControll>();

       
        _Menu.LC = Language_Controll;
        SL.Init();
        RoomMeshDelay = Time.fixedTime + 4;
      // MainCamera = plob.transform.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (createRoomMesh && !RoomMeshCreated)
        {
            EffectMesh roomeffectmesh;
            if (RoomMeshDelay < Time.fixedTime)
            {
               GameObject roommesh = Instantiate(Resources.Load<GameObject>("Prefabs/VR/RoomMesh"));
                roomeffectmesh = roommesh.GetComponent<EffectMesh>();

                var values = roomeffectmesh.EffectMeshObjects.Values.ToList();

               /* for (int i = 0; i < values.Count; i++)
                    values[i].mesh;*/
             
                        
                        
                RoomMeshCreated = true;
            }
        }
    }
}
