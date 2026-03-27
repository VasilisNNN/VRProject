using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class LanguageControll : MonoBehaviour
{

    private string toolTipsText = "Start new game rewriting Continue data?";
    private string toolTipsTextYES = "Yes";
    private string toolTipsTextNO = "No";
    private string ObjectText = "Objects";
    private string BGText = "Background";
    private string MasterText = "Master";
    private string ScreenModeText = "Screen mode";
    private string ResolutionText = "Resolution";
    private string LanguageText = "Language";

    private string CameraSensitivityText = "CameraSensitivity";

    private string Options_String = "Options";
    private string OptionsApplyText = "Apply";
    private string LoadText = "Load";


    private string HPText = "HP";
    private string StaminaText = "Stamina";


    private Dropdown LanguageDropdown;
    private int Language;

    private Transform  OptionsAllTransform;

    public GameObject SaveSlotsUIOB { get; set; }

    public float MasterSliderValue, BGSliderValue, ObjectsSliderValue;
    public int ResolutionNumber, WindowNumber;
    public int DrawTutorial, FirstStart, FirstLanguage;



    private TextMeshProUGUI YesNoText, YesNo_Tes, YesNo_No, ScreenMode_Text, Resolution_Text, Language_Text, Options_Text, OptionsApply_Text;
    private TextMeshProUGUI BG_Text, Objects_Text, Master_Text, CameraSensitivity_Text;

    public UnityEngine.Audio.AudioMixer mg;

    private string[] LanguageNames_EN, LanguageNames_UA, LanguageNames_JP;
    private Menu _Menu;

    private string[] MenuNamesEN, MenuNamesUA;
    private GameObject ContinueOB;

     public void Start()
    {

        BG_Text = GameObject.Find("AudioOptions").transform.Find("BGText").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Objects_Text = GameObject.Find("ObjectsText").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Master_Text = GameObject.Find("MasterText").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        CameraSensitivity_Text = GameObject.Find("CameraSensitivity").transform.Find("Text").GetComponent<TextMeshProUGUI>();

        ContinueOB = GameObject.Find("Continue");
        MenuNamesEN = new string[11] { "Restart", "Load", "Save", "Options", "Back", "Exit", "New game", "To main menu", "Modes" + "\n" + "(Play the main game first)", "Modes", "Continue" };
        MenuNamesUA = new string[11] { "Почати спочатку", "Завантажити", "Зберегти", "Опції", "Назад", "Вийти з гри", "Нова гра", "У головне меню", "Моди" + "\n" + "(Спочатку пограйте у основну гру)", "Моди", "Продовжити" };


        _Menu = InitializeOnAwake._Menu;
        SaveSlotsUIOB = GameObject.Find("SaveSlotsUI");

        
        OptionsAllTransform = GameObject.Find("OptionsAll").transform;
        mg = Resources.Load<UnityEngine.Audio.AudioMixer>("Sound/NewAudioMixer");
      
        YesNoText = GameObject.Find("YesNo").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        YesNo_Tes = GameObject.Find("YesNo").transform.Find("YesButton").Find("Text").GetComponent<TextMeshProUGUI>();
        YesNo_No = GameObject.Find("YesNo").transform.Find("NoButton").Find("Text").GetComponent<TextMeshProUGUI>();

        ScreenMode_Text = GameObject.Find("ScreenModeText").GetComponent<TextMeshProUGUI>();
        Resolution_Text = GameObject.Find("Resolution").transform.Find("ResolutionText").GetComponent<TextMeshProUGUI>();
        if(GameObject.Find("LanguageBox")!=null)
        Language_Text = GameObject.Find("LanguageBox").transform.Find("LanguageText").GetComponent<TextMeshProUGUI>();

      //  Options_Text = GameObject.Find("OptionsText").transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
     //   OptionsApply_Text = GameObject.Find("OptionsApply").transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();



        LanguageNames_EN = new string[] { "English", "Ukrainian" };
        LanguageNames_UA = new string[] { "Англійська", "Українська" };
        LanguageNames_JP = new string[3] { "英語", "ウクライナ語", "日本語" };

        if (GameObject.Find("LanguageDropdown1") != null)
            LanguageDropdown = GameObject.Find("LanguageDropdown1").GetComponent<Dropdown>();
       
        
        LanguagesControll();

    }

  
    public void LanguagesControll()
    {
        Language = _Menu.Language;

        if (LanguageDropdown != null)
        {
            LanguageDropdown.options = new List<Dropdown.OptionData>();

            for (int i = 0; i < LanguageNames_EN.Length; i++)
            {
                if (Language == 0)
                {
                    LanguageDropdown.options.Add(new Dropdown.OptionData(LanguageNames_EN[i]));
                    LanguageDropdown.captionText.text = LanguageNames_EN[Language];
                }

                if (Language == 1)
                {
                    LanguageDropdown.options.Add(new Dropdown.OptionData(LanguageNames_UA[i]));
                    LanguageDropdown.captionText.text = LanguageNames_UA[Language];
                }

                if (Language == 2)
                {
                    LanguageDropdown.options.Add(new Dropdown.OptionData(LanguageNames_JP[i]));
                    LanguageDropdown.captionText.text = LanguageNames_JP[Language];
                }
            }
        }

        for (int i = 0; i < _Menu.Slots.Count; i++)
        {
            string locationname = _Menu.CurrentSlotLocations[i];
           /* if (Language == 1)
            {
                if (_Menu.CurrentSlotLocations[i] == "Main location") locationname = "Основна локація";
                if (_Menu.CurrentSlotLocations[i] == "Blood") locationname = "Кров";
                if (_Menu.CurrentSlotLocations[i] == "Boss rush") locationname = "Бос раш";
                if (_Menu.CurrentSlotLocations[i] == "Guns and Walls") locationname = "Зброя і стіни";
                if (_Menu.CurrentSlotLocations[i] == "Winter") locationname = "Зима";
            }

            if (Language == 2)
            {
                if (_Menu.CurrentSlotLocations[i] == "Main location") locationname = "主な所在地";
                if (_Menu.CurrentSlotLocations[i] == "Blood") locationname = "血";
                if (_Menu.CurrentSlotLocations[i] == "Boss rush") locationname = "ボスラッシュ";
                if (_Menu.CurrentSlotLocations[i] == "Guns and Walls") locationname = "銃と壁";
                if (_Menu.CurrentSlotLocations[i] == "Winter") locationname = "冬";
            }*/

            _Menu.Slots[i].transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
               locationname + "\n" + _Menu.CurrentSlotDates[i] + _Menu.CurrentSlotTimes[i];

            string SlotName = "Slot";

            if (Language == 1) SlotName = "Слот";
            if (Language == 2) SlotName = "スロット";

            _Menu.Slots[i].transform.Find("SlotName").GetComponent<TextMeshProUGUI>().text = SlotName + " " + i;
            print("SET Save Load Slots " + i +" / "+ _Menu.CurrentSlotLocations[i]);
        }

        if (Language == 0)
        {
            toolTipsText = "Start new game rewriting Continue data?";
            toolTipsTextYES = "Yes";
            toolTipsTextNO = "No";
            ObjectText = "Objects";
            BGText = "Background";
            MasterText = "Master";
            ScreenModeText = "Screen mode";
            ResolutionText = "Resolution";
            LanguageText = "Language";

            CameraSensitivityText = "Camera sensitivity";

            Options_String = "Options";
            OptionsApplyText = "Back";
            LoadText = "Loading";

            HPText = "Joy";


        }


        if (Language == 1)
        {
            LoadText = "Завантаження";
            OptionsApplyText = "Назад";
            Options_String = "Опції";
            MasterText = "Головна";
            BGText = "Фон";
            ObjectText = "Об'єкти";

            ScreenModeText = "Екран";
            ResolutionText = "Розділова здатність";
            LanguageText = "Мова";

            CameraSensitivityText = "Чутливість камери";

            toolTipsText = "Запустити нову гру переписавши поточне продовження?";
            toolTipsTextYES = "Так";
            toolTipsTextNO = "Ні";

            HPText = "Задоволення";


        }

        if (Language == 2)
        {
            LoadText = "ローディング";
            OptionsApplyText = "戻る";
            Options_String = "選択";
            MasterText = "ホーム";
            BGText = "背景";
            ObjectText = "モノ。";

            ScreenModeText = "画面";
            ResolutionText = "解像度";
            LanguageText = "言語";

            toolTipsText = "チュートリアルから始める価値はありますか？";
            toolTipsTextYES = "はい";
            toolTipsTextNO = "いいえ";
        }


        if (YesNoText != null)
        {
            YesNoText.text = toolTipsText;
            YesNo_Tes.text = toolTipsTextYES;
            YesNo_No.text = toolTipsTextNO;

        }

        if (GameObject.Find("HPText") != null)
        {
            GameObject.Find("HPText").GetComponent<TextMeshProUGUI>().text = HPText;
        }

        BG_Text.text = BGText;
        Objects_Text.text = ObjectText;
        Master_Text.text = MasterText;

        CameraSensitivity_Text.text = CameraSensitivityText;


#if UNITY_STANDALONE
        if (ScreenMode_Text != null)
            ScreenMode_Text.text = ScreenModeText;
        if (Resolution_Text != null)
            Resolution_Text.text = ResolutionText;
#endif

        if(Language_Text!=null)
        Language_Text.text = LanguageText;

      //  Options_Text.text = Options_String;
       // OptionsApply_Text.text = OptionsApplyText;


        string[] menuNames;

        if (Language == 0)
            menuNames = MenuNamesEN;
        else if (Language == 1)
            menuNames = MenuNamesUA;
        else
            return;

        if (ContinueOB != null)
            UpdateText("Continue", menuNames[10]);

        UpdateText("Start", menuNames[FirstStart == 0 ? 6 : 0]);
        UpdateText("Modes", menuNames[FirstStart == 0 ? 8 : 9]);
        UpdateText("ToMenu", menuNames[7]);
        UpdateText("Load", menuNames[1]);
        UpdateText("Save", menuNames[2]);
        UpdateText("Options", menuNames[3]);
        UpdateText("Exit", menuNames[4]);
        UpdateText("QuitGame", menuNames[5]);


    }


    void UpdateText(string gameObjectName, string text)
    {
        if (GameObject.Find(gameObjectName) == null) return;

        GameObject gameObject = GameObject.Find(gameObjectName);
        if (gameObject != null)
        {
            if (gameObject.transform.Find("Text") == null) return;

            TextMeshProUGUI textComponent = gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = text;
        }
    }

}
