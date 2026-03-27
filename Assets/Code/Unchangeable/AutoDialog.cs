using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(TextDatabase))]
[RequireComponent(typeof(InputMode))]

public class AutoDialog : MonoBehaviour
{
    private TextDatabase textdatabase;
    private GameObject Dialog_Obj;

    public int ID;
    private int linenum;
    private int CurrentPart, num;

    public float delay = 1;
    private float lettersdelay, replicadelay;

    private float timer;
    public GameObject GO;

    public string LocationName = "";
    private bool PlayerTurn;
    public bool Loop;

    private string FinalLine;
    private InputMode IM;
    public bool GoToSteam;

    private bool typing;
    private List<TextA> MainList;

    private Player pl;
    private UIAnimation DialogAnimation;

    private AudioSource AS;
    private AudioClip[] DialogClip;

    private GameObject EButton;


    void Start()
    {
        if (GetComponent<TextDatabase>() == null) gameObject.AddComponent<TextDatabase>();
        textdatabase = GetComponent<TextDatabase>();

        Dialog_Obj = GameObject.Find("Canvas").transform.Find("Dialog").gameObject;
        DialogAnimation = Dialog_Obj.GetComponent<UIAnimation>();

        EButton = Dialog_Obj.transform.Find("EButton").gameObject;
        DialogAnimation.Play = true;

        MainList = textdatabase.textEN;

        timer = Time.fixedTime + 1 + MainList[NumberInData(ID)].line[0].line[0].Length * 0.04f;

        IM = GetComponent<InputMode>();
        PlayerTurn = true;
        ONOFFUI(Dialog_Obj.transform, true);
        typing = true;
        pl = InitializeOnAwake.pl;

        pl.InDialog = true;

        AS = Dialog_Obj.GetComponent<AudioSource>();
        DialogClip = new AudioClip[3]
      { Resources.Load<AudioClip>("Sound/UI/Typing_0"),
        Resources.Load<AudioClip>("Sound/UI/Typing_1"),
        Resources.Load<AudioClip>("Sound/UI/Typing_2")};

        replicadelay = Time.fixedTime + 5;
    }
    
    void Update()
    {
        EButton.SetActive(false);
        //  DialogOB.GetComponent<Dialog>().enabled = false;

        if (pl._Menu.Language ==0)
        MainList = textdatabase.textEN;

        if (pl._Menu.Language == 1)
            MainList = textdatabase.textUA;

        if (CurrentPart > MainList[NumberInData(ID)].line.Length - 1) CurrentPart = MainList[NumberInData(ID)].line.Length - 1;
        if (num > MainList[NumberInData(ID)].line[CurrentPart].line.Length - 1) num = MainList[NumberInData(ID)].line[CurrentPart].line.Length - 1;
        if (!pl.InDialog) return;

        if (Dialog_Obj != null)
        {
            // DialogOB.transform.position = Camera.main.WorldToScreenPoint(transform.position);

            if (linenum < MainList[NumberInData(ID)].line[CurrentPart].line[num].Length)
            {
                if (lettersdelay < Time.fixedTime)
                {
                    FinalLine += MainList[NumberInData(ID)].line[CurrentPart].line[num][linenum];
                    linenum++;
                    PlaySoundsPitched(DialogClip[UnityEngine.Random.Range(0, DialogClip.Length)], 1);

                    replicadelay = Time.fixedTime + linenum * 0.1f;
                    lettersdelay = Time.fixedTime + 0.01f;
                }
            }
            else
            {
         

                linenum = MainList[NumberInData(ID)].line[CurrentPart].line[num].Length;

                if (replicadelay < Time.fixedTime )
                {
                    FinalLine = "";
                    linenum = 0;
                    if (num < MainList[NumberInData(ID)].line[CurrentPart].line.Length - 1)
                        num++;
                    else if (CurrentPart < MainList[NumberInData(ID)].line.Length - 1)
                    {
                        CurrentPart++;
                    }
                    else
                    {
                        if (DialogAnimation.Play)
                        {
                            
                            ResetDialog();
                        }
                    }

                   
                }

            }


           
            Dialog_Obj.transform.Find("TextPlayer").GetComponent<TextMeshProUGUI>().text = FinalLine;

            Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().sprite =
            Resources.Load<Sprite>("Textures/CharactersIcons/" + MainList[NumberInData(ID)].IconName);



            if (pl.PlayerPortrait == null)
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().sprite =
             Resources.Load<Sprite>("Textures/CharactersIcons/Player");
            }
            else
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().sprite = pl.PlayerPortrait;

            }

        }
       



        if (typing)
        {
            if (PlayerTurn)
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().enabled = true;
                Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().enabled = false;
            }
            else
            {
                Dialog_Obj.transform.Find("FaceLeft").GetComponent<Image>().enabled = false;
                Dialog_Obj.transform.Find("FaceRight").GetComponent<Image>().enabled = true;
            }

        }

        
    }


    private int NumberInData(int ID)
    {
        int r = 0;
        for (int i = 0; i < MainList.Count; i++)
        {
            if (textdatabase.textEN[i].ID == ID) r = i;
        }
        return r;
    }

    private void OnDisable()
    {
        ResetDialog();
    }

    public void ResetDialog()
    {
    

        pl.CollidingDialog = null;
        pl.InDialog = false;


        PlayerTurn = true;

        typing = false;

        linenum = 0;
        FinalLine = "";
        num = 0;
        timer = 0;



        DialogAnimation.Play = false;


        pl._Menu.ActionDelay = Time.fixedTime + 0.1f;

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


    public void ONOFFUI(Transform tr, bool TF)
    {
        if (tr == null) return;
        tr.gameObject.SetActive(TF);

    }

}
