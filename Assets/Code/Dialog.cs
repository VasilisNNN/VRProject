using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Text;
using System;
using TMPro;



[RequireComponent(typeof(TextDatabase))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]

public class Dialog : MonoBehaviour
{

    //   [TextArea]
    private string DialogString;

    private GameObject Dialog_Obj;
    private float timer;

    private TextDatabase textdatabase;
    public bool PlayerTurn = true;
    private bool StartPlayerTurn;


    public int QuestID = -1;
    public int DialogID = 0;
    public int DialogID_AfterItem = -1;
    public int QuestItem = -1;
    public int QuestItemCount = 1;

    public int QuestIDDone = -1;
    public int[] Reward_ID;
    public int[] Reward_Count;

    public bool isTyping;
    public List<TextA> LinesEn { get; set; }

    private float typeSpeed = 0.05f;
    private string PrefabName;
    private int CurrentItem = -1;
    public int CurrentLine { get; private set; }

    private AudioClip Accept;
    private AudioClip[] TalkingClips;

    private AudioSource AS;
    private Player pl;
    public int CurrentDPart { get; private set; }
    private bool PrefabTag;



    private InputMode IM;
    private float linedelay;
    private int leter = 0;

    private AudioClip[] DialogClip;
    public bool Draw { get; private set; }
    public bool StartOnColl;
    public bool EndOnColl;
    public string EnterSceneOnEnd;

    private bool StartDraw;

    public bool PlayOnes;
    public Trigger TriggerAtTheEnd;

    private Menu _Menu;
    private Outline _Outline;

    public enum SoundType { Normal, Electro, Monster };
    public SoundType _SoundType;

    public bool StartToMoveOnEnd;

    public GameObject[] TurnOnAtTheEnd;
    private RectTransform MainCanvas;
    private UIAnimation DialogAnimation;
    private GameObject EButton;

    void Start()
    {
        MainCanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        _Outline = GetComponent<Outline>();
     
        if (DialogID == 0) print("OOOPS!!! WE HAVE WROND DIALOG NUMBER ON " + name);

        if(_SoundType == SoundType.Normal)
        DialogClip = new AudioClip[3] 
        { Resources.Load<AudioClip>("Sound/UI/Typing_0"),
        Resources.Load<AudioClip>("Sound/UI/Typing_1"),
        Resources.Load<AudioClip>("Sound/UI/Typing_2")};

        if (_SoundType == SoundType.Electro)
            DialogClip = new AudioClip[4]
            { Resources.Load<AudioClip>("Sound/UI/Typing_Radio_0"),
        Resources.Load<AudioClip>("Sound/UI/Typing_Radio_1"),
        Resources.Load<AudioClip>("Sound/UI/Typing_Radio_2"),
        Resources.Load<AudioClip>("Sound/UI/Typing_Radio_3")};

        if (_SoundType == SoundType.Monster)
            DialogClip = new AudioClip[3]
            { Resources.Load<AudioClip>("Sound/UI/Typing_Monster_0"),
        Resources.Load<AudioClip>("Sound/UI/Typing_Monster_1"),
        Resources.Load<AudioClip>("Sound/UI/Typing_Monster_2")};

        AS = GetComponent<AudioSource>();
        textdatabase = GetComponent<TextDatabase>();
        Dialog_Obj = GameObject.Find("Canvas").transform.Find("Dialog").gameObject;
        DialogAnimation = Dialog_Obj.GetComponent<UIAnimation>();
        EButton = Dialog_Obj.transform.Find("EButton").gameObject;

        if (GameObject.Find("Player") != null)
        {
            if(GameObject.Find("Player").GetComponent<Player>()!=null)
            pl = GameObject.Find("Player").GetComponent<Player>();

            _Menu = GameObject.Find("Player").GetComponent<Menu>();
            IM = GameObject.Find("Player").GetComponent<InputMode>();
        }
        else
        {
            gameObject.AddComponent<InputMode>();
            IM = GetComponent<InputMode>();
        }
     
        if (_Menu.Language == 0)
            LinesEn = textdatabase.textEN;

        if (_Menu.Language == 1)
            LinesEn = textdatabase.textUA;


        PlayerTurn = textdatabase.textEN[NumberInData(DialogID)].PlayerTurn;

        StartPlayerTurn = PlayerTurn;

        if (TurnOnAtTheEnd != null)
        {
            for (int i = 0; i < TurnOnAtTheEnd.Length; i++)
            {

                if (TurnOnAtTheEnd[i] != null)
                    TurnOnAtTheEnd[i].SetActive(false);

            }
        }

    }

   
    void Update()
    {

        if (_Outline != null && pl != null)
        {
            if (pl.PlayerMenusPause()) _Outline.OutlineColor = new Color(0, 0, 0, 0);
            else
            {
                if (pl.ViewColl(gameObject) || pl.ViewColl(gameObject))
                {
                    _Outline.OutlineColor = new Color(1, 1, 1, 1);
                }
                else _Outline.OutlineColor = new Color(0, 0, 0, 0);

            }
        }

        SetText();

    }


    void SetText()
    {
        EButton.SetActive(false);

        if (_Menu.Language == 0)
            LinesEn = textdatabase.textEN;

        if (_Menu.Language == 1)
            LinesEn = textdatabase.textUA;

        if (pl.inv.showinvent || pl.inv.showjournal || pl._Menu.MenuONOFF) return;
        if (pl.IM.ActionDelay > Time.fixedTime) return;

        if (IM.exit_b || IM.menu_b)
        {
            ResetDialog();
           
        }

        if (!_Menu.MenuONOFF)
        {

            if (Draw)
            {
                if (IM.enter_b || IM.SpaceB || IM.pick_item || IM.LeftMouseButtonDown)
                {
                    NextLine();
                }
            }

            if (((pl.Legscoll_obj.Contains(gameObject) || pl.ViewColl(gameObject)) || (StartOnColl && pl.Legscoll_obj.Contains(gameObject))) && !Draw)
            {
         

                pl.CollidingDialog = gameObject;
                if (((IM.enter_b || IM.SpaceB || IM.pick_item || IM.LeftMouseButtonDown) && !PlayOnes) || (StartOnColl && !StartDraw))
                {
                   
                    StartDialog();
                }
            }

            if (!pl.Legscoll_obj.Contains(gameObject) && !pl.ViewColl(gameObject))
            {
                if (pl.CollidingDialog == gameObject)
                pl.CollidingDialog = null;

            }


            if ((!pl.Legscoll_obj.Contains(gameObject) && !pl.ViewColl(gameObject)) && Draw && !StartOnColl)
               ResetDialog();


            if(StartOnColl && !pl.Legscoll_obj.Contains(gameObject) && Draw) ResetDialog();

        }
      
        
        if (EndOnColl && (!pl.Legscoll_obj.Contains(gameObject) && !pl.ViewColl(gameObject)))
        {
            StartDraw = false;

           
            if (Draw)
            {
                DialogAnimation.Play = false;
             
                isTyping = false;
                ResetDialog();
                Draw = false;
            }


    
        }
        


        if (Draw)
        {
            Dialog_Obj.transform.Find("DialogBG").GetComponent<Image>().enabled = true;

            if (PlayerTurn)
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().enabled = true;
                Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>().enabled = true;

                Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().enabled = false;
                Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().enabled = false;
                Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>().enabled = false;

                Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().enabled = true;
                Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>().enabled = true;
            }

            TextScroll(LinesEn[NumberInData(DialogID)].
              line[CurrentDPart].
              line[CurrentLine]);



            if (CurrentDPart == LinesEn[NumberInData(DialogID)].line.Length - 1 && CurrentLine == LinesEn[NumberInData(DialogID)].line[CurrentDPart].line.Length - 1)
            {
                if (CurrentDPart < LinesEn[NumberInData(DialogID)].line.Length && CurrentLine < LinesEn[NumberInData(DialogID)].line[CurrentDPart].line.Length)
                {
                    if (LinesEn[NumberInData(DialogID)].line[CurrentDPart].line[CurrentLine].Contains("link=Dialog"))
                        Dialog_Obj.transform.Find("EButton").gameObject.SetActive(false);
                    else Dialog_Obj.transform.Find("EButton").gameObject.SetActive(true);
                }
                else Dialog_Obj.transform.Find("EButton").gameObject.SetActive(true);


                Dialog_Obj.transform.Find("EButton").Find("Text").GetComponent<TextMeshProUGUI>().text = "Finish!";
            }
            else
            {
                Dialog_Obj.transform.Find("EButton").gameObject.SetActive(true);
                Dialog_Obj.transform.Find("EButton").Find("Text").GetComponent<TextMeshProUGUI>().text = "Next...";
            }




            // Dialog_Obj.transform.Find("EButton").Find("Text").GetComponent<Text>().enabled =
            //   Dialog_Obj.transform.Find("EButton").GetComponent<Image>().enabled;

            if (!isTyping)
                Dialog_Obj.transform.Find("EButton").gameObject.SetActive(true);
            else
                Dialog_Obj.transform.Find("EButton").gameObject.SetActive(false);
            //  }

            textdatabase = GetComponent<TextDatabase>();
        }

      
   

    }





    void NextLine()
    {


        if (isTyping) return;

        if (timer >= Time.fixedTime) return;
            
        isTyping = true;
             
        PrefabName = "";
        CurrentItem = -1;
        DialogString = "";
        leter = 0;

        if (CurrentLine < LinesEn[NumberInData(DialogID)].line[CurrentDPart].line.Length - 1)
        {
            CurrentLine++;


        }
        else
        {

            if (CurrentDPart < LinesEn[NumberInData(DialogID)].line.Length - 1)
            {
                CurrentDPart++;

                if (LinesEn[NumberInData(DialogID)].line.Length > 1)
                {
                    PlayerTurn = !PlayerTurn;
                    print("SWITCH ROLES");
                }
            }
            else
            {

                if (QuestID > -1)
                {
                    print("ActOnDialogEnd " + name + QuestID);
                    if (!pl.inv.CheckQuestDone(QuestID))
                        pl.inv.AddQuest(QuestID);

                }
                       

                ResetDialog();

                if (StartToMoveOnEnd)
                {
                    GetComponent<NavMeshAgent>().enabled = true;
                    GetComponent<MoveToTarget>().enabled = true;
                }


                TurnOnAllAfterEnd();

                DialogString = "";
                 
                CurrentDPart = 0;
                CurrentLine = 0;
            }


            CurrentLine = 0;

        }

        timer = Time.fixedTime + 0.1f;
            

    }



    private void TextScroll(string LineOfTextNOTAGS)
    {


        //DialogString = "";



       /* if ((pl.Inp.enter_b|| pl.Inp.SpaceB) && timer < Time.fixedTime && isTyping)
        {
            isTyping = false;
            DialogString = LineOfTextNOTAGS;
        }*/

        if (isTyping)
        {



            if (leter < LineOfTextNOTAGS.Length)
            {
                if (linedelay < Time.fixedTime)
                {
                    DialogString += LineOfTextNOTAGS[leter];
                    leter++;
                    PlaySoundsPitched(DialogClip[UnityEngine.Random.Range(0, DialogClip.Length)], 1);
                    linedelay = Time.fixedTime + 0.01f;
                }
            }
            else
            {
                isTyping = false;
                leter = LineOfTextNOTAGS.Length;
            }


            if (!AS.isPlaying)
            {
                /* AS.clip = TalkingClips[UnityEngine.Random.Range(0, TalkingClips.Length)];
                 AS.pitch = UnityEngine.Random.Range(1, 0.8f);
                 AS.Play();*/
            }
        
            // yield return new WaitForSeconds(typeSpeed);
        }
        
        if (Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>() != null)
            Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>().text = DialogString;

        if (Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>() != null)
            Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>().text = DialogString;




        //isTyping = false;

    }




    public int NumberInData(int ID)
    {
        int r = 0;
        for (int i = 0; i < LinesEn.Count; i++)
        {
            if (LinesEn[i].ID == ID)
            {
                // print("textdatabase.textEN[i].ID" + textdatabase.textEN[i].ID);
                r = i;
            }
            //   else print("ID NOT FOUND!");
        }
        return r;
    }


 
    public void ResetDialog()
    {
        if (!Draw) return;

        pl.CollidingDialog = null;
        pl.InDialog = false;

        leter = 0;
        DialogString = "";
        PlayerTurn = StartPlayerTurn;
   
        CurrentLine = 0;
        CurrentDPart = 0;
        isTyping = false;
        Draw = false;

        DialogAnimation.Play = false;
        
        if (Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>() != null)
            Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>().text = DialogString;

        if (Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>() != null)
            Dialog_Obj.transform.Find("TextCharacter").GetComponent<TextMeshProUGUI>().text = DialogString;

        ActOnDialogEnd();

        _Menu.ActionDelay = Time.fixedTime + 0.1f;

    }

    void ActOnDialogEnd()
    {
        if (EnterSceneOnEnd.Length > 1) SceneManager.LoadScene(EnterSceneOnEnd);

        if (TriggerAtTheEnd != null) TriggerAtTheEnd.enabled = true;

  
       

    }
    void FinishQuest()
    {
        if(QuestIDDone>-1) pl.inv.DoneQuest(QuestIDDone);

        if (QuestItem <= -1) return;
        if (QuestID <= -1) return;
        if (!pl.inv.CheckItem(QuestItem, QuestItemCount)) return;
        if (pl.inv.CheckQuestDone(QuestID)) return;
        
        pl.inv.ReduceItemCount(QuestItem, 1);
        pl.inv.DoneQuest(QuestID);

        if (Reward_ID.Length > 0)
        {
            for (int i = 0; i < Reward_ID.Length; i++)
            {
                pl.inv.AddItem(Reward_ID[i], Reward_Count[i],99,0);
            }
        }


        if (DialogID_AfterItem > -1) DialogID = DialogID_AfterItem;
        
    }

    void StartDialog()
    {

        FinishQuest();
        

        Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().sprite =
        Resources.Load<Sprite>("Textures/CharactersIcons/" + LinesEn[NumberInData(DialogID)].IconName);

        if (pl.PlayerPortrait == null)
        {
            Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().sprite =
         Resources.Load<Sprite>("Textures/CharactersIcons/Player");
        }
        else
        {
            Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().sprite = pl.PlayerPortrait;

        }


        if (timer < Time.fixedTime)
        {
            DialogAnimation.Play = true;
           
            isTyping = true;
            Draw = true;
            pl.InDialog = true;
            StartDraw = true;
        }


        print(name + " Dialog 1");

    }


     void PlaySoundsPitched(AudioClip AC, float pitch)
    {
        if (!AS.isPlaying)
        {
            AS.clip = AC;
            AS.pitch = pitch;
            AS.Play();
        }
    }
    public void TurnOnAllAfterEnd()
    {
        if (TurnOnAtTheEnd == null) return;
        

        for (int i = 0; i < TurnOnAtTheEnd.Length; i++)
        {
            if (!pl.SL.SaveLoadCurrent.TriggersActivated.Contains(gameObject))
                pl.SL.SaveLoadCurrent.TriggersActivated.Add(gameObject);

            if (TurnOnAtTheEnd[i] != null)
                TurnOnAtTheEnd[i].SetActive(true);

        }
        
    }

    private void OnDisable()
    {
        if(_Outline!=null)
        _Outline.OutlineColor = new Color(0, 0, 0, 0);
    }

  
}

