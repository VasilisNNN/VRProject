using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeParameters
{
    public GameObject Slot;
    public int MaxHP;
    public int MaxFuel;
    public float CarSpeed;

    public int PistolDamage;
    public int ShotgunDamage;
    public int RifleDamage;

    public int PistolAmmo;
    public int ShotgunAmmo;
    public int RifleAmmo;

    public int CurrentLevel;

    public int ResourceID = -1;
    public int ResourceCount = 0;
}

[System.Serializable]
public class SlotRowUpgrades
{
    public UpgradeParameters[] Line;
  
}


public class Upgrades : MonoBehaviour
{
    public GameObject Choose { get; private set; }
    private InputMode IM;
    private int CurrentItem;

    private Player pl;
    private Menu _menu;
    public SlotRowUpgrades[] Slots;
    [HideInInspector]
    public bool showthis;

    private int CurrentRow, CurrentSlot;

    private float HorDelay, VertDelay;

    private AudioClip ClickClip;

    private Text StatsText;

    void Start()
    {

        Choose = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ChooseUI"), transform);
        Choose.name = "UpgradeChoose";

        IM = InitializeOnAwake.pl.GetComponent<InputMode>();

        pl = InitializeOnAwake.pl;
        _menu = InitializeOnAwake.pl.GetComponent<Menu>();

        _menu.ONOFFUI(transform, false);

        StatsText = transform.Find("StatsText").GetComponent<Text>();
    }
    

    void Update()
    {
        ScrollThroughUpgrades();

        SetTooltips_AndChoosePos();

        SetSlotText();
    }
    
    void ScrollThroughUpgrades()
    {
        if (IM.UpgradeButton && pl._Menu.ActionDelay<Time.fixedTime)
        {
            showthis = !showthis;

            if(!showthis)
                _menu.ONOFFUI(transform, false);
            else
                _menu.ONOFFUI(transform, true);

          
            pl.showUpgrades = showthis;
            pl._Menu.ActionDelay = Time.fixedTime + 0.05f;
        }

        if (!showthis) return;

        if (IM.exit_b && pl._Menu.ActionDelay < Time.fixedTime)
        {
            showthis = false;

            _menu.ONOFFUI(transform, false);
          
            pl.showUpgrades = showthis;
            pl._Menu.ActionDelay = Time.fixedTime + 0.05f;
        }





        if (IM.enter_b || IM.SpaceB) AddUpgrade();


        if ((IM._vertical < -0.5f || IM.DPADY < 0) && CurrentRow < Slots.Length - 1 && VertDelay < Time.fixedTime)
        {
          
            pl.inv.StopShake();

            CurrentRow++;

            pl.inv.PlaySoundsPitched(ClickClip, 1);
            //if (CurrentSlot > Slots[CurrentRow].items.Count - 1) CurrentSlot = Slots[CurrentRow].items.Count - 1;
            VertDelay = Time.fixedTime + 0.1f;
        }

        if ((IM._vertical > 0.5f || IM.DPADY > 0) && CurrentRow > 0 && VertDelay < Time.fixedTime)
        {
         

            pl.inv.StopShake();
            CurrentRow--;
            pl.inv.PlaySoundsPitched(ClickClip, 1);
            //if (CurrentSlot > Slots[CurrentRow].items.Count - 1) CurrentSlot = Slots[CurrentRow].items.Count - 1;
            VertDelay = Time.fixedTime + 0.1f;
        }

        if ((IM._horizontal > 0.5f || IM.DPADX > 0) && HorDelay < Time.fixedTime)
        {
            if (CurrentSlot < Slots[CurrentRow].Line.Length - 1)
            {
                pl.inv.StopShake();
                CurrentSlot++;
                pl.inv.PlaySoundsPitched(ClickClip, 1);
                HorDelay = Time.fixedTime + 0.1f;
            }
            else pl.inv.Shake(new Vector3(4, 0, 0));

        }

        if ((IM._horizontal < -0.5f || IM.DPADX < 0) && HorDelay < Time.fixedTime)
        {
            if (CurrentSlot > 0)
            {
                pl.inv.StopShake();
                CurrentSlot--;
                pl.inv.PlaySoundsPitched(ClickClip, 1);
                HorDelay = Time.fixedTime + 0.1f;
            }
            else pl.inv.Shake(new Vector3(4, 0, 0));

        }

    }

    void SetSlotText()
    {
        if (!showthis) return;

    


        string slottext = "";

        string red = "#FF5D5D";
        string green = "#9DFF99";
        string yellow = "#FFF224";
        
        string plus = "";

        string maxHP = "";
        string maxFuel = "";
        string carSpeed = "";
        string pistolDamage = "";
        string shotgunDamage = "";
        string rifleDamage = "";


        if (_menu.Language == 0) maxHP = "MaxHP: ";
        if (_menu.Language == 1) maxHP = "Максимальне здоров'я: ";

        if (_menu.Language == 0) maxFuel = "Max fuel: ";
        if (_menu.Language == 1) maxFuel = "Максимальне паливо: ";


        if (_menu.Language == 0) carSpeed = "Speed in vehicles: ";
        if (_menu.Language == 1) carSpeed = "Швидкість у транспорті: ";

        if (_menu.Language == 0) pistolDamage = "Pistol Damage: ";
        if (_menu.Language == 1) pistolDamage = "Ушкодження від пістолета: ";

        if (_menu.Language == 0) shotgunDamage = "Shotgun Damage: ";
        if (_menu.Language == 1) shotgunDamage = "Ушкодження від дробовика: ";

        if (_menu.Language == 0) rifleDamage = "Rifle Damage: ";
        if (_menu.Language == 1) rifleDamage = "Ушкодження від рушниці: ";


        StatsText.text =
            maxHP + pl.HPMax + "\n" +
            carSpeed + pl.CarSpeed + "\n" +
            pistolDamage + pl.PistolDamage + "\n" +
            shotgunDamage + pl.ShotgunDamage + "\n" +
            rifleDamage + pl.RifleDamage + "\n";



        for (int x = 0; x < Slots.Length; x++)
            for (int y = 0; y < Slots[x].Line.Length; y++)
            {
                slottext = "";
                if (Slots[x].Line[y].MaxHP != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";
                    if (Slots[x].Line[y].MaxHP > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].MaxHP < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }
                    slottext += maxHP + stargcolortag + plus + Slots[x].Line[y].MaxHP + endgcolortag;
                    slottext += "\n";
                }


                if (Slots[x].Line[y].MaxFuel != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";

                    if (Slots[x].Line[y].MaxFuel > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].MaxFuel < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }

                    slottext += maxFuel + stargcolortag + plus + Slots[x].Line[y].MaxFuel + endgcolortag;
                    slottext += "\n";
                }

                if (Slots[x].Line[y].CarSpeed != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";

                    if (Slots[x].Line[y].CarSpeed > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].CarSpeed < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }

                    slottext += carSpeed + stargcolortag + plus + (Slots[x].Line[y].CarSpeed - 1) * 100 + " % " + endgcolortag;
                    slottext += "\n";
                }


                if (Slots[x].Line[y].PistolDamage != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";

                    if (Slots[x].Line[y].PistolDamage > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].PistolDamage < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }

                    slottext += pistolDamage + stargcolortag + plus + Slots[x].Line[y].PistolDamage + endgcolortag;
                    slottext += "\n";
                }

                if (Slots[x].Line[y].ShotgunDamage != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";

                    if (Slots[x].Line[y].ShotgunDamage > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].ShotgunDamage < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }

                    slottext += shotgunDamage + stargcolortag + plus + Slots[x].Line[y].ShotgunDamage + endgcolortag;
                    slottext += "\n";
                }

                if (Slots[x].Line[y].RifleDamage != 0)
                {
                    string stargcolortag = "";
                    string endgcolortag = "";

                    if (Slots[x].Line[y].RifleDamage > 0)
                    {
                        stargcolortag = "<color=" + green + ">";
                        endgcolortag = "</color>";
                        plus = "+ ";
                    }
                    else if (Slots[x].Line[y].RifleDamage < 0)
                    {
                        stargcolortag = "<color=" + red + ">";
                        endgcolortag = "</color>";
                    }

                    slottext += rifleDamage + stargcolortag + plus + Slots[x].Line[y].RifleDamage + endgcolortag;
                    slottext += "\n";
                }




                Slots[x].Line[y].Slot.transform.Find("Text").GetComponent<Text>().text = slottext;
            }
    }


    void AddUpgrade()
    {
   
        if (Slots[CurrentRow].Line[CurrentSlot].ResourceID > -1)
        {
            if(!pl.inv.CheckItem(Slots[CurrentRow].Line[CurrentSlot].ResourceID, Slots[CurrentRow].Line[CurrentSlot].ResourceCount) )
            return;
        }


        if (Slots[CurrentRow].Line[CurrentSlot].CurrentLevel > 4)
        {
            _menu.PlaySoundsPitched(_menu.ErrorClip,1);
            return;
        }



        pl.HPMax += Slots[CurrentRow].Line[CurrentSlot].MaxHP;
        pl.CarSpeed += Slots[CurrentRow].Line[CurrentSlot].CarSpeed;

        pl.PistolDamage += Slots[CurrentRow].Line[CurrentSlot].PistolDamage;
        pl.ShotgunDamage += Slots[CurrentRow].Line[CurrentSlot].ShotgunDamage;
        pl.RifleDamage += Slots[CurrentRow].Line[CurrentSlot].RifleDamage;


        _menu.ActionDelay = Time.fixedTime + 0.1f;

    }


    void SetTooltips_AndChoosePos()
    {
        if ((MouseCollideWithSlots() && IM.MouseMode) || !IM.MouseMode)
        {

            if (CurrentRow < Slots.Length)
                if (CurrentSlot < Slots[CurrentRow].Line.Length)
                {
                    Choose.transform.position = Slots[CurrentRow].Line[CurrentSlot].Slot.transform.position /*+ pl.inv.ShakeVector()*/;
                    Choose.GetComponent<RectTransform>().sizeDelta = Slots[CurrentRow].Line[CurrentSlot].Slot.GetComponent<RectTransform>().sizeDelta;
                }
        }
    }

    public bool MouseCollideWithSlots()
    {
        int result = 0;

        for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].Line.Length; i++)
            {

                if (pl._Menu.MouseOB.GetComponent<CollList2D>().coll_obj.Contains(Slots[r].Line[i].Slot))
                {
                    result++;
                }

            }
        }
        if (result > 0)
            return true;
        else return false;
    }
}
