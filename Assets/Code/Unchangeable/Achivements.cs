using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_STANDALONE
//using Steamworks;
#endif

public class Achivements : MonoBehaviour
{
    private bool Achunlocked;
    private Player pl;
    private Inventory inv;
    private Menu menu;
    private GameObject AchMenu;
    private InputMode IM;
    private SaveLoad SL;

    private bool Draw;
    public bool ShowAch { get; set; }


    private List<string> ACHNames = new List<string>();

    private AchivementsDatabase AD;


    private GameObject SM;
    // Start is called before the first frame update

    private StatsControll JurbaST, WheellerST, BlockheadST, Noface_0ST, NofaceST;
    void Start()
    {
        //SM = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SteamManager"));
        // SM.name = "SteamManager";
  

            AchMenu = GameObject.Find("Achivements");
        IM = GetComponent<InputMode>();
        pl = GetComponent<Player>();
        inv = GetComponent<Inventory>();
        menu = GetComponent<Menu>();

        SL = GetComponent<SaveLoad>();
        AD = GetComponent<AchivementsDatabase>();


        if (SL != null)
        {

        }


        ONOFF(AchMenu, ShowAch);

        for (int i = 0; i < AchMenu.transform.childCount; i++)
        {
            if (AchMenu.transform.GetChild(i).GetComponent<Image>() != null)
                AchMenu.transform.GetChild(i).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);


            for (int b = 0; b < AchMenu.transform.GetChild(i).transform.childCount; b++)
            {
                if (AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Image>() != null)
                    AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

                /*if (AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Text>() != null)
                {
                    for (int a = 0; a < AD.textEN.Count; a++)
                    {
                        if(AD.textEN[a].IconName == AchMenu.transform.GetChild(i).name)
                        AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Text>().text = AD.textEN[a].line[0].line[0];
                    }

                }*/

                for (int bb = 0; bb < AchMenu.transform.GetChild(i).GetChild(b).transform.childCount; bb++)
                {
                    if (AchMenu.transform.GetChild(i).GetChild(b).GetChild(bb).GetComponent<Image>() != null)
                        AchMenu.transform.GetChild(i).GetChild(b).GetChild(bb).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

                }
            }
        }

        if (GameObject.Find("JurbaBoss") != null) {
            JurbaST = GameObject.Find("JurbaBoss").GetComponent<StatsControll>();
            }

        if (GameObject.Find("WheellerBoss") != null)
        {
            WheellerST = GameObject.Find("WheellerBoss").GetComponent<StatsControll>();
        }

        if (GameObject.Find("BlockheadBoss") != null)
        {
            BlockheadST = GameObject.Find("BlockheadBoss").GetComponent<StatsControll>();
        }

        if (GameObject.Find("NofaceBoss") != null)
        {
            Noface_0ST = GameObject.Find("NofaceBoss").GetComponent<StatsControll>();
        }

        if (GameObject.Find("NofaceDadFinalBoss") != null)
        {
            NofaceST = GameObject.Find("NofaceDadFinalBoss").GetComponent<StatsControll>();
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (pl == null) return;

#if UNITY_STANDALONE
        if (pl.ViewColl(GameObject.Find("NucTrigger")))
        {
            SetAch("The start and the end");
        }

      

        if (pl.SL.SaveLoadCurrent.DayNumber>=2)
        {
            SetAch("Three hours");
        }

        if (SceneManager.GetActiveScene().name == "WaterPark")
        {
            //SetAch("Defeat Jurba");
            SetAch("Waterpark");
        }


        if (SceneManager.GetActiveScene().name == "LockerRoom")
        {
            SetAch("Locker room");
        }

        if (SceneManager.GetActiveScene().name == "Lakes")
        {
            SetAch("Lakes");
        }

        if (SceneManager.GetActiveScene().name == "LostCity")
        {
            SetAch("Going home");
        }

        if (SceneManager.GetActiveScene().name == "Ending Good 0")
        {
            SetAch("Good Ending");
        }
        if (SceneManager.GetActiveScene().name == "Ending Bad 0")
        {
            SetAch("Bad Ending");
        }
        if (SceneManager.GetActiveScene().name == "Ending Good")
        {
            SetAch("You made it");
        }
        if (SceneManager.GetActiveScene().name == "Ending Good 1")
        {
            SetAch("We all made it");
        }
        if (SceneManager.GetActiveScene().name == "Ending Bad")
        {
            SetAch("You are alone");
        }
        if (SceneManager.GetActiveScene().name == "Ending Bad 1")
        {
            SetAch("United in death");
        }


        if (SceneManager.GetActiveScene().name == "JurbaBossfight")
        {
            if(JurbaST==null)
                SetAch("Defeat Jurba");
            else 
            if (JurbaST.HP == 0)
                SetAch("Defeat Jurba");
        }
    
        if (SceneManager.GetActiveScene().name == "WheellerBossfight")
        {
            if (WheellerST == null)
                SetAch("Defeat Wheeller");
            else
            if (WheellerST.HP == 0)
                SetAch("Defeat Wheeller");
        }


        if (SceneManager.GetActiveScene().name == "BlockheadBossfight")
        {
            if (BlockheadST == null)
                SetAch("Defeat Blockhead");
            else
            if (BlockheadST.HP == 0)
                SetAch("Defeat Blockhead");
        }


        if (SceneManager.GetActiveScene().name == "NofaceBossfight_0")
        {
            if (Noface_0ST == null)
                SetAch("Defeat Blockhead");
            else
            if (Noface_0ST.HP == 0)
                SetAch("Defeat Blockhead");
        }



        if (SceneManager.GetActiveScene().name == "NofaceBossfight_Final")
        {
            if (NofaceST == null)
            {
                SetAch("The final fight");
                print("Boss is null");
            
            }
            else
            if (NofaceST.HP == 0)
                SetAch("The final fight");
        }

#endif

    }


    public void SetAch(string n)
    {
        #if UNITY_STANDALONE
        /*if (pl != null)
        {
            if (!pl.DEMO)
            {
                if (!ACHNames.Contains(n))
                {
                    ACHNames.Add(n);
                }
                if (SL != null)
                {
                    if (!SL.ACHNames.Contains(n))
                        SL.ACHNames.Add(n);
                }

                SteamUserStats.GetAchievement(n, out Achunlocked);

                if (!Achunlocked)
                {
                    SteamUserStats.SetAchievement(n);
                    SteamUserStats.StoreStats();

                }
            }
        }
        else
        {

            if (!ACHNames.Contains(n))
            {
                ACHNames.Add(n);
            }

            if (SL != null)
            {
                if (!SL.ACHNames.Contains(n))
                    SL.ACHNames.Add(n);
            }

            SteamUserStats.GetAchievement(n, out Achunlocked);

            if (!Achunlocked)
            {
                SteamUserStats.SetAchievement(n);
                SteamUserStats.StoreStats();

            }
        }*/
#endif
    }

    void AlphaControll()
    {
        for (int i = 0; i < AchMenu.transform.childCount; i++)
        {
            for (int j = 0; j < ACHNames.Count; j++)
            {

                if (AchMenu.transform.GetChild(i).name == ACHNames[j])
                {
                    if (AchMenu.transform.GetChild(i).GetComponent<Image>() != null)
                        AchMenu.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);

                    for (int b = 0; b < AchMenu.transform.GetChild(i).transform.childCount; b++)
                    {
                        if (AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Image>() != null)
                            AchMenu.transform.GetChild(i).GetChild(b).GetComponent<Image>().color = new Color(1, 1, 1, 1);

                    }
                }
                
            }


           
        }

    }

        void ONOFF(GameObject g, bool TF)
    {
        if (g.GetComponent<Image>() != null)
            g.GetComponent<Image>().enabled = TF;

        if (g.GetComponent<Text>() != null)
            g.GetComponent<Text>().enabled = TF;

        if (g.GetComponent<SpriteRenderer>() != null)
            g.GetComponent<SpriteRenderer>().enabled = TF;

        if (g.GetComponent<BoxCollider2D>() != null)
            g.GetComponent<BoxCollider2D>().enabled = TF;

      
        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (g.transform.GetChild(i).GetComponent<Image>() != null)
                g.transform.GetChild(i).GetComponent<Image>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                g.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<BoxCollider2D>() != null)
                g.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = TF;

            if (g.transform.GetChild(i).GetComponent<Text>() != null)
                g.transform.GetChild(i).GetComponent<Text>().enabled = TF;


            for (int ii = 0; ii < g.transform.GetChild(i).childCount; ii++)
            {
                if (g.transform.GetChild(i).GetChild(ii).GetComponent<Image>() != null)
                    g.transform.GetChild(i).GetChild(ii).GetComponent<Image>().enabled = TF;

                if (g.transform.GetChild(i).GetChild(ii).GetComponent<Text>() != null)
                    g.transform.GetChild(i).GetChild(ii).GetComponent<Text>().enabled = TF;

                for (int iii = 0; iii < g.transform.GetChild(i).GetChild(ii).childCount; iii++)
                {
                    if (g.transform.GetChild(i).GetChild(ii).GetChild(iii).GetComponent<Image>() != null)
                        g.transform.GetChild(i).GetChild(ii).GetChild(iii).GetComponent<Image>().enabled = TF;

                    if (g.transform.GetChild(i).GetChild(ii).GetChild(iii).GetComponent<Text>() != null)
                        g.transform.GetChild(i).GetChild(ii).GetChild(iii).GetComponent<Text>().enabled = TF;

                }

            }

        }

    }
}
