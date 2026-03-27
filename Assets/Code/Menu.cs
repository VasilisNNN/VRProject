using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;





public class Menu : MonoBehaviour
{
    // private List<GameObject> objects = new List<GameObject>();



    private bool _options, _exit, quit, _modes, DrawSaveSlots, LoadSlotsOn, SaveSlotsOn;
    private int SaveSlotNumber;
    private bool _gameplaymenu, _audiomenu, _screenmenu;

    public bool MenuONOFF { get; set; }
    public UnityEngine.Audio.AudioMixer mg;

    public bool FullScreen;

    private bool YesNo;

    private GameObject MenuChoose;

    private List<GameObject> MenuButtons = new List<GameObject>();
    private List<GameObject> OptionsButtons = new List<GameObject>();
    private List<GameObject> YesNoButtons = new List<GameObject>();


    private Player pl;
    private int MenuButtonNum;

    private GameObject GameplayOptionsButton, AudioOptionsButton, ScreenOptionsButton;

    private InputMode IM;
    private float ScrollDelay;
    public List<GameObject> Slots = new List<GameObject>();
    private int SlotXPOS, SlotYPOS;
    private GameObject MenuAllObject;
    private Transform MenuAllTransform, OptionsAllTransform, GameplayMenuTransform, AudioMenuTransform, ScreenMenuTransform;

    public float MasterSliderValue, BGSliderValue, ObjectsSliderValue;
    public int ResolutionNumber, WindowNumber;
    public int Language, DrawTutorial, FirstStart, FirstLanguage;

    public string[] CurrentSlotLocations, CurrentSlotDates, CurrentSlotTimes;
    public int[] CurrentSlotPlayers;
    public int CurrentSlotNumber;

    private GameObject ResDropDownOB, YesNoOB, ContinueOB, AfterEndingOB, ToMenuOB, YesButtonOB, ToolTipsYesButtonOB, ToolTipsNoButtonOB, NoButtonOB, StartOB, SaveOB, LoadOB, OptionsOB, ModesOB, ExitOB, OptionsApplyOB, WindowDropdownOB;
    private Slider MasterSlider, BGSlider, ObjectsSlider, MouseSensitivitySlider;
    private TMP_Dropdown WindowDropdown, ResDropDown, LanguageDropdown;
    public GameObject SaveSlotsUIOB { get; set; }

    public GameObject MouseOB { get; set; }


    private SaveLoad SL;
 

    public float ActionDelay { get; set; }

    [HideInInspector]
    public AudioClip ClickClip,ErrorClip;

    public float MouseSensitivity = 0.5f;

    public string StartScene = "";
    private float indialog = 0;

    public bool HideUI;
    private Toggle HideUIToggle, TransparencyUIToggle, DistanceFadeUIToggle;


    private string[] LanguageNames_EN, LanguageNames_UA, LanguageNames_JP;

    [HideInInspector]
    public int DefaultSaveSlot = 6;
    [HideInInspector]
    public int FirstEnding = 0;
    [HideInInspector]
    public LanguageControll LC;
    private int MenuFolderNum;
    private List<GameObject> GameplayOptionsButtons = new List<GameObject>();
    private List<GameObject> AudioOptionsButtons = new List<GameObject>();
    private List<GameObject> ScreenOptionsButtons = new List<GameObject>();

    private bool StartMenu;
    public bool VerticalCamera;
    private GameObject CanvasObject;
    private MenuAnimation Menu_Anim;
    private GameObject CurrentButton;
    public void Init()
    {
        CanvasObject = GameObject.Find("Canvas");
        
        if(GameObject.Find("MenuAnimation")!=null)
        Menu_Anim = GameObject.Find("MenuAnimation").GetComponent<MenuAnimation>();

        DefaultSaveSlot = 6;
        LanguageNames_EN = new string[] { "English", "Ukrainian" };
        LanguageNames_UA = new string[] { "Англійська", "Українська" };
        LanguageNames_JP = new string[3] { "英語", "ウクライナ語", "日本語" };



        SL = InitializeOnAwake.SL;

        ClickClip = Resources.Load<AudioClip>("Sound/UI/Click_1");
        ErrorClip = Resources.Load<AudioClip>("Sound/UI/Cancel");


        ToMenuOB = GameObject.Find("ToMenu");
        ContinueOB = GameObject.Find("Continue");
        AfterEndingOB = GameObject.Find("AfterEnding");
        YesButtonOB = GameObject.Find("YesButton");
        ToolTipsYesButtonOB = GameObject.Find("ToolTipsYesButton");
        ToolTipsNoButtonOB = GameObject.Find("ToolTipsNoButton");
        NoButtonOB = GameObject.Find("NoButton");
        StartOB = GameObject.Find("Start");
        SaveOB = GameObject.Find("Save");
        LoadOB = GameObject.Find("Load");
        OptionsOB = GameObject.Find("Options");
        ModesOB = GameObject.Find("Modes");
        ExitOB = GameObject.Find("Exit");
        YesNoOB = GameObject.Find("YesNo");

        OptionsApplyOB = GameObject.Find("OptionsApply");
        MasterSlider = GameObject.Find("MasterSlider").GetComponent<Slider>();
        BGSlider = GameObject.Find("BGSlider").GetComponent<Slider>();
        ObjectsSlider = GameObject.Find("ObjectsSlider").GetComponent<Slider>();
        MouseSensitivitySlider = GameObject.Find("MouseSlider").GetComponent<Slider>();

        if (GameObject.Find("LanguageDropdown1") != null)
            LanguageDropdown = GameObject.Find("LanguageDropdown1").GetComponent<TMP_Dropdown>();

        ResDropDownOB = GameObject.Find("ResDropdown");
        ResDropDown = ResDropDownOB.GetComponent<TMP_Dropdown>();

        WindowDropdownOB = GameObject.Find("WindowDropdown");
        WindowDropdown = WindowDropdownOB.GetComponent<TMP_Dropdown>();

        GameplayMenuTransform = GameObject.Find("GameplayOptions").transform;
        AudioMenuTransform = GameObject.Find("AudioOptions").transform;
        ScreenMenuTransform = GameObject.Find("ScreenOptions").transform;


        SaveSlotsUIOB = GameObject.Find("SaveSlotsUI");

        MenuChoose = GameObject.Find("MenuChoose");
        MenuAllObject = GameObject.Find("MenuAll");

        MenuAllTransform = GameObject.Find("MenuAll").transform;
        OptionsAllTransform = GameObject.Find("OptionsAll").transform;

        mg = Resources.Load<UnityEngine.Audio.AudioMixer>("Sound/NewAudioMixer");
        IM = GetComponent<InputMode>();
        pl = InitializeOnAwake.pl;


        GameplayOptionsButton = GameObject.Find("GameplayOptionsButton");
        AudioOptionsButton = GameObject.Find("AudioOptionsButton");
        ScreenOptionsButton = GameObject.Find("ScreenOptionsButton");

        HideUIToggle = GameObject.Find("HideUI").GetComponent<Toggle>();
        HideUIToggle.isOn = HideUI;


        if (Slots.Count < 6)
        {
            for (int i = 0; i < 6; i++)
            {
                Slots.Add(GameObject.Find("SaveSlotsUI").transform.Find("Slot (" + i + ")").gameObject);
            }
        }


    }



    public void Start()
    {
        Init();

        MenuONOFF = false;
        Cursor.visible = false;


        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            StartMenu = true;
             MenuONOFF = true;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            ONOFFUI(MenuAllTransform, true);
            Cursor.visible = true;

        }
        else StartMenu = false;

      
        if (FirstStart != 0)
        {
            if (StartMenu)
                MenuButtons.Add(MenuAllObject.transform.Find("Continue").gameObject);
        }


        if (MenuAllObject.transform.Find("Start") != null)
            MenuButtons.Add(MenuAllObject.transform.Find("Start").gameObject);


       /* if (FirstEnding != 0)
        {
            if (AfterEndingOB != null)
            {
                if (StartMenu)
                    MenuButtons.Add(AfterEndingOB);
            }
        }*/

        if (MenuAllObject.transform.Find("Load") != null)
            MenuButtons.Add(MenuAllObject.transform.Find("Load").gameObject);

        if (!StartMenu)
            MenuButtons.Add(MenuAllObject.transform.Find("Save").gameObject);


        MenuButtons.Add(MenuAllObject.transform.Find("Options").gameObject);

        //if (!StartMenu)
            MenuButtons.Add(MenuAllObject.transform.Find("QuitGame").gameObject);

        if (!StartMenu)
            MenuButtons.Add(MenuAllObject.transform.Find("ToMenu").gameObject);


#if UNITY_SWITCH
        Destroy(GameObject.Find("OptionsAll").transform.Find("LanguageDropdown1").gameObject);
         Destroy(GameObject.Find("OptionsAll").transform.Find("ResDropdown").gameObject);
         Destroy(GameObject.Find("OptionsAll").transform.Find("WindowDropdown").gameObject);

     
 
        Destroy(MenuAllObject.transform.Find("QuitGame").gameObject);

        Destroy(GameObject.Find("OptionsAll").transform.Find("WindowDropdown").gameObject);
        Destroy(GameObject.Find("OptionsAll").transform.Find("ResDropdown").gameObject);
        

#endif


        ONOFFUI(OptionsAllTransform, false);
        YesNoButtons.Add(YesNoOB.transform.Find("YesButton").gameObject);
        YesNoButtons.Add(YesNoOB.transform.Find("NoButton").gameObject);

        if (MenuAllObject == null)
        {
            GameObject MenuObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/MenuAll"), CanvasObject.transform);
            MenuObject.name = "MenuAll";
        }

        if (OptionsAllTransform == null)
        {
            GameObject OptionsObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/OptionsAll"), CanvasObject.transform);
            OptionsObject.name = "OptionsAll";
        }

        if (YesNoOB == null)
        {
            GameObject OptionsObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/YesNo"), CanvasObject.transform);
            OptionsObject.name = "YesNo";
            YesNoOB = OptionsObject;
        }

        MouseOB = GameObject.Find("MouseUI");
        MouseOB.transform.SetAsLastSibling();

        if (GameObject.Find("Achivements") != null)
            ONOFFUI(GameObject.Find("Achivements").transform, false);


        if (MenuAllObject.transform.Find("Continue") != null)
        {
            if (FirstStart == 0) ONOFFUI(MenuAllObject.transform.Find("Continue"), false);
            else ONOFFUI(MenuAllObject.transform.Find("Continue"), true);
        }


        /*if (AfterEndingOB != null)
        {
            if (FirstEnding == 0) ButtonEnableDisable(AfterEndingOB.transform, false);
            else ButtonEnableDisable(AfterEndingOB.transform, true);
        }*/


        //PlayerPrefs.DeleteAll();

        OptionsButtons.Add(GameplayOptionsButton);
        OptionsButtons.Add(AudioOptionsButton);
        OptionsButtons.Add(ScreenOptionsButton);
    

        GameplayOptionsButtons.Add(MouseSensitivitySlider.gameObject);
        GameplayOptionsButtons.Add(OptionsApplyOB);


        AudioOptionsButtons.Add(MasterSlider.gameObject);
        AudioOptionsButtons.Add(BGSlider.gameObject);
        AudioOptionsButtons.Add(ObjectsSlider.gameObject);
        AudioOptionsButtons.Add(OptionsApplyOB);

#if UNITY_STANDALONE
        ScreenOptionsButtons.Add(WindowDropdownOB);
        ScreenOptionsButtons.Add(ResDropDownOB);
        ScreenOptionsButtons.Add(OptionsApplyOB);
#endif

        ONOFFUI(GameplayMenuTransform, false);
        ONOFFUI(AudioMenuTransform, false);
        ONOFFUI(ScreenMenuTransform, false);

        ONOFFUI(OptionsAllTransform, false);
        ONOFFUI(SaveSlotsUIOB.transform, false);
        ONOFFUI(MenuAllTransform, MenuONOFF);
        ONOFFUI(YesNoOB.transform, false);

        ONOFFUI(MenuChoose.transform, true);


#if UNITY_STANDALONE
        if (Screen.currentResolution.width<=10) Screen.SetResolution(1920, 1080, FullScreen);
      
#endif


       


    }







    void Menu_Visuals()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
         
            HideUIToggle.isOn = !HideUIToggle.isOn;
        }
        if(HideUIToggle!=null)
        HideUI = HideUIToggle.isOn;


        if (VerticalCamera) CanvasObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.7f;
        else CanvasObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;


#if UNITY_STANDALONE
        if (Mathf.Abs( Input.mousePosition.x)<Mathf.Infinity)
        MouseOB.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
#endif

#if UNITY_SWITCH
        MouseOB.transform.position = new Vector3(99999, 999999, 1);
#endif


        if (StartMenu) return;

        if (IM.LeftMouseButton || IM.enter_b || IM.exit_b || IM.pick_item)
        {
            if (MenuAllObject != null)
            {
                if (MenuAllObject.GetComponent<Animator>() != null)
                    MenuAllObject.GetComponent<Animator>().SetBool("Finish", true);
            }

            if (GameObject.Find("MenuStart") != null)
                GameObject.Find("MenuStart").GetComponent<Animator>().SetBool("Finish", true);

        }

    }

   

    void Update()
    {
       /* if (AfterEndingOB != null)
        {
            if (!_options && !SaveSlotsOn && !LoadSlotsOn)
            {
                if (FirstEnding == 0) ButtonEnableDisable(AfterEndingOB.transform, false);
                else ButtonEnableDisable(AfterEndingOB.transform, true);
            }else ButtonEnableDisable(AfterEndingOB.transform, false);
        }*/


        if (quit) Quit();

    

        Menu_Visuals();


        bool gameover = false;
        bool invshow = false;
        bool cutscenemode = false;
        if (pl != null && pl.InDialog) indialog = Time.fixedTime + 0.1f;

        if (pl != null)
        {
            gameover = pl._gameover;
            if(pl.inv != null)
            invshow = pl.inv.showinvent;

            cutscenemode = pl.CutSceneMode;
        }
        
        if (cutscenemode && MenuONOFF)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ONOFFUI(OptionsAllTransform, false);
            ONOFFUI(MenuAllTransform, false);
            ONOFFUI(MenuChoose.transform, false);
            MenuONOFF = false;
            return;
        }

        if (!StartMenu)
        {
           

            if (!cutscenemode && !gameover && !invshow && IM.menu_b && ActionDelay < Time.fixedTime && indialog < Time.fixedTime  && !DrawSaveSlots && !YesNo && !_options)
            {
                MenuONOFF = !MenuONOFF;

                if (MenuONOFF)
                {
                    Time.timeScale = 1;


                    Cursor.lockState = CursorLockMode.None;

                    ONOFFUI(OptionsAllTransform, false);
                    ONOFFUI(MenuAllTransform, true);
                    ONOFFUI(MenuChoose.transform, true);

                    Cursor.visible = true;

                }
                else
                {

                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    ONOFFUI(OptionsAllTransform, false);
                    ONOFFUI(MenuAllTransform, false);
                    print("OFF MenuAllTransform");
                    ONOFFUI(MenuChoose.transform, false);
                 
                }

                LC.LanguagesControll();
            }

        }
        MenuButtonsMove();
        ApplyAllOptions();


        if (MenuONOFF)
        {
           
          

            if (!_modes && !YesNo)
            {
                GeneralMenu();
            }

            if (!_modes) YESNOMenu();

        }
        else
        {

            //  ONOFFUI(OptionsAllTransform, false);
            // ONOFFUI(MenuAllTransform, false);
            MenuButtonNum = 0;
            _options = false;
            _modes = false;
        }


        if (SaveSlotsOn) SaveSlots();
        if (LoadSlotsOn) LoadSlots();

        SaveSlotsControlls();



    }

    void ApplyAllOptions()
    {
        if (_options && (IM.exit_b || IM.menu_b))
        {
            MenuButtonNum = 0;

            ONOFFUI(OptionsAllTransform, false);

            ONOFFUI(MenuAllTransform, true);
            ONOFFUI(MenuChoose.transform, true);

            ONOFFUI(GameplayMenuTransform, false);
            ONOFFUI(AudioMenuTransform, false);
            ONOFFUI(ScreenMenuTransform, false);
            _options = false;
            _modes = false;

            ActionDelay = Time.fixedTime + 0.3f;

        }



        if (!ClickButton(OptionsApplyOB)) return;

        ONOFFUI(GameplayMenuTransform, false);
        ONOFFUI(AudioMenuTransform, false);
        ONOFFUI(ScreenMenuTransform, false);
        _gameplaymenu = false;
        _audiomenu = false;
        _screenmenu = false;

        SetMenuChoosePos(MenuButtons[0].transform.position, 
            MenuButtons[0].GetComponent<RectTransform>().sizeDelta, 
            MenuButtons[0]);


#if UNITY_STANDALONE
        if (WindowDropdown != null)
            {
                if (WindowDropdown.captionText.text == "Fullscreen")
                {
                    print("Full screen on");
                    
                    WindowNumber = 0;
                    FullScreen = true;
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }

                if (WindowDropdown.captionText.text == "Windowed")
                {
                 
                    WindowNumber = 1;
                    FullScreen = false;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
            }



        if (ResDropDown != null)
        {
            if (ResDropDown.captionText.text == "1920 * 1080")
            {
                Screen.SetResolution(1920, 1080, FullScreen);

                ResolutionNumber = 0;
            }
            
            else if (ResDropDown.captionText.text == "3840 * 2160")
            {
                Screen.SetResolution(3840, 2160, FullScreen);

                ResolutionNumber = 1;
            }
            
            else if (ResDropDown.captionText.text == "1280 * 720")
            {

                Screen.SetResolution(1280, 720, FullScreen);

                ResolutionNumber = 2;

            }

            else if (ResDropDown.captionText.text == "1080 * 1920")
            {
                Screen.SetResolution(1080, 1920, FullScreen);

                ResolutionNumber = 3;
            }

            else if (ResDropDown.captionText.text == "900 * 1600")
            {
                Screen.SetResolution(900, 1600, FullScreen);

                ResolutionNumber = 4;
            }



            else
            {
                Screen.SetResolution(1920, 1080, FullScreen);
                ResolutionNumber = 1;
            }



        }
        #endif



            if (LanguageDropdown != null)
            {
                for (int i = 0; i < LanguageNames_EN.Length; i++)
                {
                    if (Language == 0)
                    {
                        if (LanguageDropdown.captionText.text == LanguageNames_EN[i])
                            Language = i;
                    }

                    if (Language == 1)
                    {
                        if (LanguageDropdown.captionText.text == LanguageNames_UA[i])
                            Language = i;
                    }

                    if (Language == 2)
                    {
                        if (LanguageDropdown.captionText.text == LanguageNames_JP[i])
                            Language = i;
                    }




                }

                LanguageDropdown.options = new List<TMP_Dropdown.OptionData>();

                for (int i = 0; i < LanguageNames_EN.Length; i++)
                {
                    if (Language == 0)
                    {
                        LanguageDropdown.options.Add(new TMP_Dropdown.OptionData(LanguageNames_EN[i]));
                        LanguageDropdown.captionText.text = LanguageNames_EN[Language];
                    }

                    if (Language == 1)
                    {
                        LanguageDropdown.options.Add(new TMP_Dropdown.OptionData(LanguageNames_UA[i]));
                        LanguageDropdown.captionText.text = LanguageNames_UA[Language];
                    }

                    if (Language == 2)
                    {
                        LanguageDropdown.options.Add(new TMP_Dropdown.OptionData(LanguageNames_JP[i]));
                        LanguageDropdown.captionText.text = LanguageNames_JP[Language];
                    }
                }




            }

            MenuButtonNum = 0;

            ONOFFUI(OptionsAllTransform, false);

            ONOFFUI(MenuAllTransform, true);
            ONOFFUI(MenuChoose.transform, true);

            _options = false;
            _modes = false;

            SL.Save(false, DefaultSaveSlot, SceneManager.GetActiveScene().name);
            LC.LanguagesControll();
            ActionDelay = Time.fixedTime + 0.3f;

        
    }



    void GeneralMenu()
    {
        if (ClickButton(SaveSlotsUIOB.transform.Find("Close").gameObject))
        {
            LoadSlotsOn = false;
            DrawSaveSlots = false;
            ONOFFUI(SaveSlotsUIOB.transform, false);
            ONOFFUI(MenuAllTransform, true);
            ActionDelay = Time.fixedTime + 0.3f;
            SetMenuChoosePos(
                MenuButtons[0].transform.position, 
                MenuButtons[0].GetComponent<RectTransform>().sizeDelta, 
                MenuButtons[0]);

        }

        if (DrawSaveSlots) return;


       
        if (_options)
        {

            if (YesNo) return;

            if (!_gameplaymenu && !_audiomenu && !_screenmenu)
            {

                ONOFFUI(OptionsAllTransform, true);
                ONOFFUI(GameplayMenuTransform, true);
                ONOFFUI(MenuAllTransform, false);
                ONOFFUI(MenuChoose.transform, true);
                MenuButtonNum = 0;
                SetMenuChoosePos(
                    OptionsButtons[0].transform.position, 
                    OptionsButtons[0].GetComponent<RectTransform>().sizeDelta, 
                    OptionsButtons[0]);
                _gameplaymenu = true;
              
            }
            OptionsManager();

            if ((UIColl(GameplayOptionsButton) && !IM.MouseMode) || (ClickButton(GameplayOptionsButton) && IM.MouseMode))
            {
                ONOFFUI(GameplayMenuTransform, true);
                ONOFFUI(AudioMenuTransform, false);
                ONOFFUI(ScreenMenuTransform, false);
                _gameplaymenu = true;
                _audiomenu = false;
                _screenmenu = false;
                MenuButtonNum = 0;
            }

            if ((UIColl(AudioOptionsButton) && !IM.MouseMode ) || (ClickButton(AudioOptionsButton) && IM.MouseMode))
            {

                ONOFFUI(GameplayMenuTransform, false);
                ONOFFUI(AudioMenuTransform, true);
                ONOFFUI(ScreenMenuTransform, false);
                _gameplaymenu = false;
                _audiomenu = true;
                _screenmenu = false;
                MenuButtonNum = 0;
            }

            if ((UIColl(ScreenOptionsButton) && !IM.MouseMode) || (ClickButton(ScreenOptionsButton) && IM.MouseMode))
            {
                ONOFFUI(GameplayMenuTransform, false);
                ONOFFUI(AudioMenuTransform, false);
                ONOFFUI(ScreenMenuTransform, true);
                _gameplaymenu = false;
                _audiomenu = false;
                _screenmenu = true;
                MenuButtonNum = 0;
            }


            return;
        }

        if (ClickButton(ToMenuOB))
        {
            System.DateTime moment = System.DateTime.Now;

            CurrentSlotLocations[DefaultSaveSlot] = SceneManager.GetActiveScene().name;
            CurrentSlotTimes[DefaultSaveSlot] = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            CurrentSlotDates[DefaultSaveSlot] = moment.Month + "/" + moment.Day + "/" + moment.Year + "   ";
            CurrentSlotNumber = DefaultSaveSlot;

            SL.Save(true, DefaultSaveSlot , SceneManager.GetActiveScene().name);

            SceneManager.LoadScene("StartMenu");
            ActionDelay = Time.fixedTime + 0.3f;
        }

        if (ClickButton(ContinueOB) )
        {

            ContinueGame();

            ActionDelay = Time.fixedTime + 0.3f;
        }

        /*if (ClickButton(AfterEndingOB))
        {

            AfterEndingGame();

            ActionDelay = Time.fixedTime + 0.3f;
        }*/


        if (ClickButton(StartOB))
        {

            if (FirstStart == 0)
            {
                DrawTutorial = 0;
                StartGame();


            }
            else
            {
                if (ActionDelay < Time.fixedTime)
                {

                    YesNo = true;
                    ONOFFUI(YesNoOB.transform, true);
                    ONOFFUI(OptionsAllTransform, false);
                    MenuButtonNum = 0;

                    ActionDelay = Time.fixedTime + 0.3f;
                }
            }
        }


        if (ClickButton(SaveOB) && !DrawSaveSlots)
        {
            SaveSlotsOn = true;
            LoadSlotsOn = false;
            DrawSaveSlots = true;
            ONOFFUI(SaveSlotsUIOB.transform, true);
            ONOFFUI(MenuAllTransform, false);
            ONOFFUI(MenuChoose.transform, true);
            for (int i = 0; i < 6; i++)
            {
                SaveSlotsUIOB.transform.Find("Slot (" + i + ")").Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
                        CurrentSlotLocations[i] + "\n" + CurrentSlotDates[i] + CurrentSlotTimes[i];

            }
            SetMenuChoosePos(SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").position, 
                SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").GetComponent<RectTransform>().sizeDelta,
                SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").gameObject);

            LC.LanguagesControll();
            ActionDelay = Time.fixedTime + 0.1f;

        }

        if (ClickButton(LoadOB) && !DrawSaveSlots)
        {
            LoadSlotsOn = true;
            SaveSlotsOn = false;
            DrawSaveSlots = true;
            ONOFFUI(SaveSlotsUIOB.transform, true);
            ONOFFUI(MenuAllTransform, false);
            ONOFFUI(MenuChoose.transform, true);

            for (int i = 0; i < 6; i++)
            {
                Slots[i].transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
                    CurrentSlotLocations[i] + "\n" + CurrentSlotDates[i] + CurrentSlotTimes[i];

            }

            SetMenuChoosePos(SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").position, 
                SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").GetComponent<RectTransform>().sizeDelta,
                SaveSlotsUIOB.transform.Find("Slot (" + 0 + ")").gameObject);

            LC.LanguagesControll();
            ActionDelay = Time.fixedTime + 0.1f;
        }



        if(ContinueOB!=null)
        if (ClickButton(ContinueOB)) ContinueGame();

        if (ClickButton(OptionsOB) )
        {
            MenuButtonNum = 0;
          
            ONOFFUI(OptionsAllTransform, true);

            ONOFFUI(MenuAllTransform, false);
            ONOFFUI(MenuChoose.transform, true);

            _options = true;
        }

        if (GameObject.Find("Exit") != null && !StartMenu)
        {

            if (ClickButton(ExitOB))
            {
                // ONOFFUI(GameObject.Find("MenuUp").transform, false);

                if (MenuONOFF)
                {
                    IM.ActionDelay = Time.fixedTime + 0.2f;
                    Cursor.lockState = CursorLockMode.Locked;

                    ONOFFUI(OptionsAllTransform, false);
                    ONOFFUI(MenuAllTransform, false);
                    ONOFFUI(MenuChoose.transform, false);
                    MenuONOFF = false;
                }
            }
        }

        if (ClickButton(GameObject.Find("QuitGame")) ) quit = true;

    }


    void YESNOMenu()
    {
        if (ClickButton(YesButtonOB))
        {
            if (ActionDelay < Time.fixedTime)
            {
                MenuButtonNum = 0;
                DrawTutorial = 0;

                StartGame();

                ActionDelay = Time.fixedTime + 0.3f;
            }
            //  StartGame();
        }

        if (ClickButton(ToolTipsYesButtonOB))
        {
            if (ActionDelay < Time.fixedTime)
            {

            }
        }

        if (ClickButton(ToolTipsNoButtonOB) )
        {
            if (ActionDelay < Time.fixedTime)
            {
                DrawTutorial = 1;
                StartGame();
            }
        }

        if (ClickButton(NoButtonOB) )
        {
            ONOFFUI(MenuAllTransform, true);
            ONOFFUI(YesNoOB.transform, false);

            YesNo = false;
            MenuButtonNum = 0;

            ActionDelay = Time.fixedTime + 0.3f;
        }

    }

    void SaveSlotsControlls()
    {
        if (!DrawSaveSlots) return;


        if (((IM.DPADX > 0) || IM._horizontal > 0 && IM._horizontalPush) && ActionDelay < Time.fixedTime && SlotXPOS < 2)
        {
            SlotXPOS++;
            ActionDelay = Time.fixedTime + 0.3f;
        }
        if (((IM.DPADX < 0 ) || IM._horizontal < 0 && IM._horizontalPush) && ActionDelay < Time.fixedTime && SlotXPOS > 0)
        {
            SlotXPOS--;
            ActionDelay = Time.fixedTime + 0.3f;
        }

        if (((IM.DPADY < 0 ) || IM._vertical < 0 && IM._verticalPush) && ActionDelay < Time.fixedTime && SlotYPOS < 1)
        {
            SlotYPOS++;
            ActionDelay = Time.fixedTime + 0.3f;
        }

        if (((IM.DPADY > 0 ) || IM._vertical > 0 && IM._verticalPush) && ActionDelay < Time.fixedTime && SlotYPOS > 0)
        {
            SlotYPOS--;
           ActionDelay = Time.fixedTime + 0.3f;
        }

        if (!IM.MouseMode)
            SetMenuChoosePos(Slots[SlotXPOS + 3 * SlotYPOS].transform.position, Slots[SlotXPOS + 3 * SlotYPOS].GetComponent<RectTransform>().sizeDelta * Slots[SlotXPOS + 3 * SlotYPOS].GetComponent<RectTransform>().localScale, Slots[SlotXPOS + 3 * SlotYPOS]);

    }





    void MenuButtonsMove()
    {
        if (!MenuONOFF) return;
        if (_modes) return;
        if (DrawSaveSlots) return;

        if (_options)
        {
            ScrollThroughFolders(OptionsButtons, ref MenuFolderNum);
            if(_gameplaymenu)
                ScrollThroughMenu(GameplayOptionsButtons, ref MenuButtonNum);
            if (_audiomenu)
                ScrollThroughMenu(AudioOptionsButtons, ref MenuButtonNum);
            if (_screenmenu)
                ScrollThroughMenu(ScreenOptionsButtons, ref MenuButtonNum);


            return;
        }

        if (YesNo)
        {
            ScrollThroughFolders(YesNoButtons, ref MenuButtonNum);
            return;
        }
        
           
        ScrollThroughMenu(MenuButtons, ref MenuButtonNum);
      
    }






    void StartGame()
    {
  
        FirstStart = 0;
        SL.SaveLoadCurrent.DayNumber = 0;
        print("StartGame DayNumber " + SL.SaveLoadCurrent.DayNumber);
        SL.Save(false, DefaultSaveSlot , "StartMenu");
        SceneManager.LoadScene(StartScene);
    }

    void ContinueGame()
    {
        FirstStart = 1;
        FirstLanguage = 1;
        if (SL.SaveLoadCurrent.PreviousLevel.Length > 1)
        {
            if (SL.SaveLoadCurrent.PreviousLevel != "StartMenu")
            {
                SL.Save(false, DefaultSaveSlot , SL.SaveLoadCurrent.PreviousLevel);
                SceneManager.LoadScene(SL.SaveLoadCurrent.PreviousLevel);
            }
            else
            {
                SL.Save(false, DefaultSaveSlot , "StartMenu");
                SceneManager.LoadScene(StartScene);

            }
        }
        else
        {
            SL.Save(false, DefaultSaveSlot , "StartMenu");
            SceneManager.LoadScene(StartScene);
        }
    }

    void AfterEndingGame()
    {
        FirstStart = 1;
        FirstLanguage = 1;
        
        SL.Save(false, DefaultSaveSlot, "StartMenu");
        SceneManager.LoadScene("WaterPark");
        
    }



    void OptionsManager()
    {

        if (MouseSensitivitySlider.value > 0)
            MouseSensitivity = MouseSensitivitySlider.value;
        else MouseSensitivity = 0.1f;

        ManageVolumeSliders("Master", MasterSlider);
        ManageVolumeSliders("BG", BGSlider);
        ManageVolumeSliders("Objects", ObjectsSlider);


        if (_audiomenu)
        {
            ChangingSlider(MasterSlider, AudioOptionsButtons, ref MasterSliderValue);
            ChangingSlider(BGSlider, AudioOptionsButtons, ref BGSliderValue);
            ChangingSlider(ObjectsSlider, AudioOptionsButtons, ref ObjectsSliderValue);
        }

        if (_gameplaymenu)
        {
            ChangingSlider(MouseSensitivitySlider, GameplayOptionsButtons, ref MouseSensitivity);
          

            //ChangingDropdown(LanguageDropdown, GameplayOptionsButtons, ref Language);

        }

        if (!IM.joystick)
        {
            MasterSliderValue = MasterSlider.value;
            BGSliderValue = BGSlider.value;
            ObjectsSliderValue = ObjectsSlider.value;

            MouseSensitivity = MouseSensitivitySlider.value;
         
        }


#if UNITY_STANDALONE

        if (_screenmenu)
        {
            ChangingDropdown(ResDropDown, ScreenOptionsButtons, ref ResolutionNumber);
            ChangingDropdown(WindowDropdown, ScreenOptionsButtons, ref WindowNumber);
        }

        if (WindowNumber == 0)
            FullScreen = true;
        else
            FullScreen = false;
#endif

    }

    void SaveSlots()
    {
        System.DateTime moment = System.DateTime.Now;


        for (int i = 0; i < Slots.Count; i++)
        {

            if (ClickButton(Slots[i]))
            {
                CurrentSlotLocations[i] = SceneManager.GetActiveScene().name;
                CurrentSlotTimes[i] = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                CurrentSlotDates[i] = moment.Month + "/" + moment.Day + "/" + moment.Year + "   ";


                SL.Save(true, i, SceneManager.GetActiveScene().name);
                CurrentSlotNumber = i;
                SetMenuChoosePos(MenuButtons[0].transform.position, MenuButtons[0].GetComponent<RectTransform>().sizeDelta, MenuButtons[0]);


                ONOFFUI(SaveSlotsUIOB.transform, false);
                ONOFFUI(MenuAllTransform, true);
                LC.LanguagesControll();

                DrawSaveSlots = false;
                SaveSlotsOn = false;
                ActionDelay = Time.fixedTime + 0.3f;

            }
        }


        if (IM.exit_b || IM.menu_b)
        {
            ONOFFUI(SaveSlotsUIOB.transform, false);
            ONOFFUI(MenuAllTransform, true);
            SetMenuChoosePos(MenuButtons[0].transform.position, MenuButtons[0].GetComponent<RectTransform>().sizeDelta, MenuButtons[0]);

            DrawSaveSlots = false;
            SaveSlotsOn = false;
            ActionDelay = Time.fixedTime + 0.2f;
        }

    }


    void LoadSlots()
    {
        for (int i = 0; i < Slots.Count; i++)
        {

            if (ClickButton(Slots[i]))
            {
                if (CurrentSlotLocations[i].Length > 0)
                {
                 
                    SceneManager.LoadScene(CurrentSlotLocations[i]);
                    SL.SaveLoadCurrent.PreviousLevel = CurrentSlotLocations[i];
                    SL.Save(false, i, CurrentSlotLocations[i]);
                   
                    DrawSaveSlots = false;
                    LoadSlotsOn = false;
                    ONOFFUI(SaveSlotsUIOB.transform, false);
                    ONOFFUI(MenuAllTransform, true);
                    SetMenuChoosePos(MenuButtons[0].transform.position, MenuButtons[0].GetComponent<RectTransform>().sizeDelta, MenuButtons[0]);

                    ActionDelay = Time.fixedTime + 0.3f;
                }
            }
        }

        if (IM.exit_b || IM.menu_b)
        {
            ONOFFUI(SaveSlotsUIOB.transform, false);
            ONOFFUI(MenuAllTransform, true);
            DrawSaveSlots = false;
            LoadSlotsOn = false;
            SetMenuChoosePos(MenuButtons[0].transform.position, MenuButtons[0].GetComponent<RectTransform>().sizeDelta, MenuButtons[0]);

            ActionDelay = Time.fixedTime + 0.2f;
        }

    }


    void Quit()
    {
        Application.Quit();
    }

    public bool UIColl(GameObject Button)
    {

        if (Button == null) return false;
        if (Button.GetComponent<BoxCollider2D>() == null) return false;
        if (!Button.GetComponent<BoxCollider2D>().enabled) return false;

        if (IM.joystick)
        {

            if (IM.CurrentInputs.MouseCollideWithButton(Button))
            {
                //  if (Button.GetComponent<Slider>() == null && Button.GetComponent<TMP_Dropdown>() == null)
                //   Button.transform.localScale = new Vector3(Mathf.Lerp(Button.transform.localScale.x, 1.2f, Time.deltaTime * 3), Mathf.Lerp(Button.transform.localScale.y, 1.2f, Time.deltaTime * 3), 1);
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
            else
            {
                if (Button != null)
                    Button.transform.localScale = new Vector3(1f, 1f, 1);
                return false;
            }


        }


        if (!IM.CurrentInputs.MouseCollideWithButton(Button))
        {

            if (Button != null)
                Button.transform.localScale = new Vector3(1f, 1f, 1);

            return false;

        }

        // if (Button.GetComponent<Slider>() == null && Button.GetComponent<TMP_Dropdown>() == null)
        //     Button.transform.localScale = new Vector3(Mathf.Lerp(Button.transform.localScale.x, 1.2f, Time.deltaTime * 3), Mathf.Lerp(Button.transform.localScale.y, 1.2f, Time.deltaTime * 3), 1);

        if (!IM.MouseMode)
        {
            if(Menu_Anim!=null)
            Menu_Anim.SetCurrentButton(Button);
            return true;
        }

        for (int i = 0; i < MenuButtons.Count; i++)
        {
            if (Button == MenuButtons[i] && MenuButtonNum != i && !YesNo)
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);

                return true;
            }
        }


        for (int i = 0; i < YesNoButtons.Count; i++)
        {
            if (Button == YesNoButtons[i] && MenuButtonNum != i)
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }


        for (int i = 0; i < OptionsButtons.Count; i++)
        {
            if (Button == OptionsButtons[i] && MenuFolderNum != i)
            {
                MenuFolderNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }

        for (int i = 0; i < GameplayOptionsButtons.Count; i++)
        {
            if (Button == GameplayOptionsButtons[i] && MenuButtonNum != i)
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }

        for (int i = 0; i < AudioOptionsButtons.Count; i++)
        {
            if (Button == AudioOptionsButtons[i] && MenuButtonNum != i)
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }

        for (int i = 0; i < ScreenOptionsButtons.Count; i++)
        {
            if (Button == ScreenOptionsButtons[i] && MenuButtonNum != i)
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }


        for (int i = 0; i < Slots.Count; i++)
        {
            if (Button == Slots[i])
            {
                MenuButtonNum = i;
                if (Menu_Anim != null)
                    Menu_Anim.SetCurrentButton(Button);
                return true;
            }
        }


        SetMenuChoosePos(Button.transform.position, Button.GetComponent<RectTransform>().sizeDelta, Button);
        if (Menu_Anim != null)
            Menu_Anim.SetCurrentButton(Button);
        return true;


    }

    void CreateButton(string name, int num)
    {
        if (GameObject.Find(name) == null)
        {
            GameObject uib = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Menu/" + name), GameObject.Find("Canvas").transform);
            uib.name = name;
            uib.transform.position = new Vector3(uib.transform.position.x, uib.transform.position.y + num * 3, uib.transform.position.z);
            //objects.Add(uib);
        }
    }

    public void LoadMenu()
    {
        if (FirstStart == 0 && StartMenu)
        {
         
            ObjectsSliderValue = 0.8f;
            BGSliderValue = 0.8f;

            MasterSliderValue = 0.8f;
            MouseSensitivity = 0.5f;

            HideUI = false;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

#if UNITY_SWITCH
            print(nn.oe.Language.GetDesired());
#endif

        if (FirstLanguage == 0)
        {
#if UNITY_STANDALONE

            /* if (SteamManager.Initialized)
             {
                 if (SteamApps.GetCurrentGameLanguage() == "English") Language = 0;
                 if (SteamApps.GetCurrentGameLanguage() == "Ukrainian" || SteamApps.GetCurrentGameLanguage() == "Russian") Language = 1;
                 if (SteamApps.GetCurrentGameLanguage().Contains("Japanese") ) Language = 2;
                 if (SteamApps.GetCurrentGameLanguage().Contains("Chinese")) Language = 3;

             }
             else
             {
                 Debug.LogWarning("Steamworks is not initialized.");
             }*/
#endif

#if UNITY_SWITCH
            print(nn.oe.Language.GetDesired());

            if (nn.oe.Language.GetDesired().Contains("English")) Language = 0;
            if (nn.oe.Language.GetDesired().Contains("Ukrainian") || nn.oe.Language.GetDesired().Contains("Russian")) Language = 1;
            if (nn.oe.Language.GetDesired().Contains("Japanese") ) Language = 2;
            //if (nn.oe.Language.GetDesired().Contains("Chinese")) Language = 3;

#endif
         
        }

     
        if (FirstStart > 0)
        {
            if (ObjectsSlider != null)
                ObjectsSlider.value = ObjectsSliderValue;

            if (BGSlider != null)
                BGSlider.value = BGSliderValue;

            if (MasterSlider != null)
                MasterSlider.value = MasterSliderValue;

  

                MouseSensitivitySlider.value = MouseSensitivity;

            if (HideUIToggle != null)
                HideUIToggle.isOn = HideUI;

        }

        if (ObjectsSlider != null)
        {
            ObjectsSlider.value = ObjectsSliderValue;


        }

        SetVolumeSlider(ObjectsSlider, "Objects");

        if (BGSlider != null)
        {
            BGSlider.value = BGSliderValue;


        }


        SetVolumeSlider(BGSlider, "BG");

        if (MasterSlider != null)
            MasterSlider.value = MasterSliderValue;


        SetVolumeSlider(MasterSlider, "Master");



#if UNITY_STANDALONE


        if (WindowDropdown != null)
         WindowDropdown.SetValueWithoutNotify(WindowNumber);

        if (ResDropDown != null)
            ResDropDown.SetValueWithoutNotify(ResolutionNumber);

      
        for (int i = 0; i < 6; i++)
        {
            Slots[i].transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
               CurrentSlotLocations[i] + "\n" + CurrentSlotDates[i] + CurrentSlotTimes[i];

        }
#endif

        if (LanguageDropdown != null)
            LanguageDropdown.SetValueWithoutNotify(Language);

  

    }




    
   
    public void LoadScene_SA(string scenename)
    {
        CurrentSlotNumber = DefaultSaveSlot;
        pl.SL.Save(true, DefaultSaveSlot, scenename);
        SceneManager.LoadScene(scenename);
    }

   public bool ClickButton(GameObject button)
    {
        if (UIColl(button) && IM.MouseMode) 
        {
            SetMenuChoosePos(button.transform.position, button.GetComponent<RectTransform>().sizeDelta, button);
          
        }
        if (UIColl(button) && (IM.enter_b || IM.LeftMouseButton || IM.pick_item) && ActionDelay < Time.fixedTime) return true;
        else return false;
    }
    void SetMenuChoosePos(Vector2 pos, Vector2 size, GameObject button)
    {
        if (MenuChoose.transform.position ==  new Vector3 (pos.x,pos.y, MenuChoose.transform.position.z)) return;

        MenuChoose.transform.position = pos;
        MenuChoose.GetComponent<RectTransform>().sizeDelta = new Vector3(size.x * 1,size.y * 1, 0);
        MenuChoose.GetComponent<BoxCollider2D>().size = new Vector3(size.x * 1, size.y * 1, 0);
        CurrentButton = button;
        PlaySoundsPitched(ClickClip, 1);
    }


    public void PlaySoundsPitched(AudioClip AC, float Pitch)
    {
        GetComponent<AudioSource>().clip = AC;
        GetComponent<AudioSource>().pitch = Pitch;
        GetComponent<AudioSource>().Play();
    }

    public void PlayErrorSound()
    {
        GetComponent<AudioSource>().clip = ErrorClip;
        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().Play();
    }


    public void ONOFFUI(Transform tr, bool TF)
    {
        if (tr == null) return;
        SetComponentEnabled(tr, TF);
        SetChildComponentEnabled(tr, TF);
    }

    public void ButtonEnableDisable(Transform tr, bool TF)
    {
        if (tr == null) return;
        ButtonOnOff(tr, TF);
        SetButtonComponentEnabled(tr, TF);
    }

    private void ButtonOnOff(Transform tr, bool TF)
    {
        Image imageComponent = tr.GetComponent<Image>();
        if (imageComponent != null)
        {
            if(!TF)
            imageComponent.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        else
                imageComponent.color = new Color(1f, 1f, 1f, 1f);
        }

        Text textComponent = tr.GetComponent<Text>();

        if (textComponent != null)
        {
            if (!TF)
                textComponent.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            else
                textComponent.color = new Color(1f, 1f, 1f, 1f);
        }

        TextMeshProUGUI textmeshComponent = tr.GetComponent<TextMeshProUGUI>();


        if (textmeshComponent != null)
        {
            if (!TF)
                textmeshComponent.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            else
                textmeshComponent.color = new Color(1f, 1f, 1f, 1f);
        }

        Slider sliderComponent = tr.GetComponent<Slider>();
        if (sliderComponent != null)
            sliderComponent.enabled = TF;

        BoxCollider2D colliderComponent = tr.GetComponent<BoxCollider2D>();
        if (colliderComponent != null)
            colliderComponent.enabled = TF;
    }

    private void SetComponentEnabled(Transform tr, bool TF)
    {
        Image imageComponent = tr.GetComponent<Image>();
        if (imageComponent != null)
            imageComponent.enabled = TF;

        Text textComponent = tr.GetComponent<Text>();
        if (textComponent != null)
            textComponent.enabled = TF;

        TextMeshProUGUI textmeshComponent = tr.GetComponent<TextMeshProUGUI>();
        if (textmeshComponent != null)
            textmeshComponent.enabled = TF;

        Slider sliderComponent = tr.GetComponent<Slider>();
        if (sliderComponent != null)
            sliderComponent.enabled = TF;

        BoxCollider2D colliderComponent = tr.GetComponent<BoxCollider2D>();
        if (colliderComponent != null)
            colliderComponent.enabled = TF;
    }
    private void SetButtonComponentEnabled(Transform tr, bool TF)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform child = tr.GetChild(i);
            ButtonOnOff(child, TF);
            SetButtonComponentEnabled(child, TF);
        }
    }

    private void SetChildComponentEnabled(Transform tr, bool TF)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform child = tr.GetChild(i);
            SetComponentEnabled(child, TF);
            SetChildComponentEnabled(child, TF);
        }
    }


    void SetVolumeSlider(Slider slider, string volumeVar)
    {
        if (slider == null) return;

        float sl = 3 * (slider.value * 10) - 30;
        if (sl > 10) sl = 10;

        if (slider.value <= 0)
            mg.SetFloat(volumeVar, -200);
        else
            mg.SetFloat(volumeVar, sl);



    }

   



    void ChangingSlider(Slider slider, List<GameObject> list, ref float SliderValue)
    {
        if (slider == null) return;
        if (MenuButtonNum >= list.Count) return;
        if (slider.gameObject != list[MenuButtonNum]) return;
        if (Mathf.Abs(IM._horizontal) == 0 ||
           (Mathf.Abs(IM.DPADX) != 1 && IM.joystick)) return;
        if (ScrollDelay >= Time.fixedTime) return;
        if (ActionDelay >= Time.fixedTime) return;


        if (IM._horizontal > 0)
            slider.value+=0.1f;

        if (IM._horizontal < 0)
            slider.value-= 0.1f;


        SliderValue = slider.value;

        ScrollDelay = Time.fixedTime + 0.2f;
        ActionDelay = Time.fixedTime + 0.2f;

    }


    void ChangingDropdown(TMP_Dropdown dropdown, List<GameObject> list, ref int SliderValue)
    {
        if (dropdown == null) return;
        if (list.Count <= 0) return;

        if (MenuButtonNum > list.Count - 1) MenuButtonNum = list.Count - 1;
      
        if (dropdown.gameObject != list[MenuButtonNum]) return;

        if (Mathf.Abs(IM._horizontal) == 0 ||
           (Mathf.Abs(IM.DPADX) != 1 && IM.joystick)) return;


        if (ScrollDelay >= Time.fixedTime) return;


        if (ActionDelay >= Time.fixedTime) return;


        if (IM._horizontal > 0)
            dropdown.value++;

        if (IM._horizontal < 0)
            dropdown.value--;


        SliderValue = dropdown.value;

        PlaySoundsPitched(ClickClip, 1);
        ScrollDelay = Time.fixedTime + 0.2f;
        ActionDelay = Time.fixedTime + 0.2f;

    }


    void ScrollThroughMenu(List<GameObject> buttonslist, ref int num)
    {
        for (int i = 0; i < buttonslist.Count; i++)
            UIColl(buttonslist[i]);

        if ((IM._vertical < 0  || IM.DPADY < 0)  && ScrollDelay < Time.fixedTime)
        {
            if (num < buttonslist.Count - 1)
            {
                num++;
             
                ScrollDelay = Time.fixedTime + 0.5f;
            }
            
            SetMenuChoosePos(buttonslist[num].transform.position, buttonslist[num].GetComponent<RectTransform>().sizeDelta, buttonslist[num]);
       
        }
        if ((IM._vertical > 0 || IM.DPADY > 0) && ScrollDelay < Time.fixedTime)
        {
            if (num > 0)
            {
            num--;
         
            ScrollDelay = Time.fixedTime + 0.5f;
            }
            

            SetMenuChoosePos(buttonslist[num].transform.position, buttonslist[num].GetComponent<RectTransform>().sizeDelta, buttonslist[num]);
           
        }

    }



    void ScrollThroughFolders(List<GameObject> buttonslist, ref int num)
    {
        for (int i = 0; i < buttonslist.Count; i++)
            UIColl(buttonslist[i]);

        if (IM.horizontal_Folder > 0 && num < buttonslist.Count - 1 && ScrollDelay < Time.fixedTime)
        {
          
            num++;
            PlaySoundsPitched(ClickClip, 1 - 0.05f * num);

            SetMenuChoosePos(buttonslist[num].transform.position, buttonslist[num].GetComponent<RectTransform>().sizeDelta, buttonslist[num]);
            ScrollDelay = Time.fixedTime + 0.1f;
        }

        if (IM.horizontal_Folder < 0 && num > 0 && ScrollDelay < Time.fixedTime)
        {
            num--;
            PlaySoundsPitched(ClickClip, 1 - 0.05f * num);
 
            SetMenuChoosePos(buttonslist[num].transform.position, buttonslist[num].GetComponent<RectTransform>().sizeDelta, buttonslist[num]);
            ScrollDelay = Time.fixedTime + 0.1f;
        }

 

    }

    void ManageVolumeSliders(string FloatName, Slider slider)
    {
        float vol = 3 * (slider.value * 10) - 30;
        if (vol > 10) vol = 10;
        mg.SetFloat(FloatName, vol);

    }
}


