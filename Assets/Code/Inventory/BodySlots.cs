using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

//using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
public class SlotRow
{
    public GameObject[] Slot;
    [HideInInspector]
    public List<Slot> SlotScrips = new List<Slot>();
    public List<Item> items = new List<Item>();

    public SlotRow(GameObject[] Slots, List<Item> itemss)
    {

        Slot = Slots;
        items = itemss;
    }
}


public class BodySlots : MonoBehaviour
{

    private float StartAnim;

    //private GameObject BG;

    
    private SlotRow[] Slots;

    //public List<Item> SlotsIDs = new List<Item>();
    private Player pl;

   
    private int CurrentSlot, CurrentRow;

    private float HorDelay, VertDelay, EnterDelay;
    private bool ShowUpgrade;


    private Text PlayerStats;

    [HideInInspector]
    public Gun PlayerGun;


    private Camera Cam;

    private AudioClip TakeItemClip, ClickClip;
    private GameObject AttackEffect;
    private GameObject MouseOB;
    private Inventory inv;

    private ItemsSlotsUI _ItemsSlotsUI;
    void Awake()
    {
        Slots = GetComponent<ItemsSlotsUI>().Slots;
        MouseOB = GameObject.Find("MouseOB");

        _ItemsSlotsUI = GetComponent<ItemsSlotsUI>();

        ClickClip = Resources.Load<AudioClip>("Sound/UI/Click_0");
        TakeItemClip = Resources.Load<AudioClip>("Sound/UI/Accept");

        Cam = Camera.main;

        PlayerStats = transform.Find("StartBG").Find("PlayerStats").GetComponent<Text>();
        

        //BG = transform.Find("BG").gameObject;

        StartAnim = Time.fixedTime + 1f;
        pl = InitializeOnAwake.pl;
        inv = InitializeOnAwake.pl.GetComponent<Inventory>();

        ShowUpgrade = false;
        inv.ONOFF(gameObject, false);

        
        PlayerGun = pl.GetComponent<Gun>();

 

        /*for (int r = 0; r < Slots.Length; r++)
        {
            if (Slots[r].items.Count < Slots[r].Slot.Length)
            {
                for (int i = 0; i < Slots[r].Slot.Length - Slots[r].items.Count; i++)
                {
                    Slots[r].items.Add(new Item());
                    Slots[r].items[i].itemID = -1;
                    

                }
            }
        }*/
    }

    void StatsControll()
    {
     //   pl.DamageAll =  pl.DamageAmount + pl.DamageBuff;

       // if (pl.DamageAll < 0 && PlayerGun.CurrentGunID > -1) pl.DamageAll = 0;



        if (pl._Menu.Language == 0)
        {
            PlayerStats.text = "PistolDamage: " + pl.PistolDamage + "\n"

             + "HP Max: " + pl.HPMax + "\n"
            + "HP: " + pl.HP + "\n";

          //  + "Speed: " + pl.Speed + "\n"
            // DashDurationStats.text = "Dash Distance: " + pl.DashDuration;
       
          //  + "Stamina Max: " + pl.MaxStamina + "\n"
          //  + "Stamina restore speed: " + pl.StaminaRestore + "\n";
          

        }

        if (pl._Menu.Language == 1)
        {
            PlayerStats.text = "Пошкодження: " + pl.PistolDamage + "\n"

            + "Максимальне HP: " + pl.HPMax + "\n"
            + "HP: " + pl.HP + "\n";

           //  + "Швидкість: " + pl.Speed + "\n";
            // DashDurationStats.text = "Деш: " + pl.DashDuration;
      
           // + "Максимальна стаміна: " + pl.MaxStamina + "\n"
           // + "Швидкість відновлення стаміни: " + pl.StaminaRestore + "\n";

         

        }

        if (pl._Menu.Language == 2)
        {
            PlayerStats.text = "ダメージ: " + pl.PistolDamage + "\n"


            + "最大ヘルス: " + pl.HPMax + "\n"
            + "健康: " + pl.HP + "\n";

           // + "速度: " + pl.Speed + "\n";
            // DashDurationStats.text = "Деш: " + pl.DashDuration;
       
           // + "最大スタミナ: " + pl.MaxStamina + "\n"
          //  + "スタミナ回復のスピード: " + pl.StaminaRestore + "\n";

            

        }


    }

   


    public void AddSubtractStats(int ItemID, int direction)
    {
        if (ItemID > -1)
        {
           
            if (inv.GetItemInDatabase(ItemID).AddSlots * direction>0) inv.AddInvSlot(inv.GetItemInDatabase(ItemID).AddSlots);
            if (inv.GetItemInDatabase(ItemID).AddSlots * direction < 0) inv.RemoveInvSlot(inv.GetItemInDatabase(ItemID).AddSlots);

            inv.slotX += inv.GetItemInDatabase(ItemID).AddSlots * direction;

            

   
            pl.HPMax += inv.GetItemInDatabase(ItemID).MaxHP * direction;

            pl.Protection += inv.GetItemInDatabase(ItemID).Protection * direction;



    
            

            if (pl.HPMax > 50) pl.HPMax = 50;

            if (inv.GetItemInDatabase(ItemID).MaxHP != 0)
            {
                if (pl.HP < pl.HPMax)
                {
                    if (pl.HPMax - pl.HP < inv.GetItemInDatabase(ItemID).MaxHP)
                        pl.HP += inv.GetItemInDatabase(ItemID).MaxHP * direction;
                    else pl.HP = pl.HPMax;



                }

                if (pl.HP > pl.HPMax) pl.HP = pl.HPMax;
            }

           
        

            pl.Speed += inv.GetItemInDatabase(ItemID).Speed * direction;

        }







    }

    void Update()
    {
      

        if (!inv.crafting )
        {
          
            

            StatsControll();
            
        }

        MoveAndChoose();

        
    }


    void MoveAndChoose()
    {
        DestroyGunOnLowDurability();

     
        /* if (pl.IM.Heal && EnterDelay < Time.fixedTime)
        {
            HealPlayer();
        }*/


      //  Slots = inv.UpgradesUI.Slots;
        
    }



  
 
    public void AddUpgradeItem( int id,int durability, int ammo, int row, int slot)
    {
        if (id <= -1)
        {
            return;
        }

        if(inv == null || pl == null || inv.GetItemInDatabase(id) == null)
            return;

        if (inv.GetItemInDatabase(id).itemID > -1)
            {
      
           
            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.gun )
                {

                PlayerGun.SetGunID(id, durability, ammo);


                //PlayerGun.GunTip.transform.position = PlayerGun.Hand.transform.position + new Vector3(0, inv.GetItemInDatabase(id).GunLength, 0);

               }




            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.cargun && inv.GetItemInDatabase(id)._bodypart[0] == Slot.bodypart.Front)
            {

                PlayerGun.SetCarFrontGunID(id, durability , ammo);
                
            }

            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.cargun && inv.GetItemInDatabase(id)._bodypart[0] == Slot.bodypart.Back)
            {


                PlayerGun.SetCarBackGunID(id, durability, ammo);

            }

            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.carmelee)
            {
                print("SetCarMeleeID 0");
                PlayerGun.SetCarMeleeID(id, durability, ammo);

            }
        }

        
            AddSubtractStats(id, 1);
        Item item = inv.DeepCopyItem(id, 1, durability, ammo);

        _ItemsSlotsUI.Slots[row].items[slot] = item;
        
        print("AddUpgradeItem");

        /* if (item.Satiety!=0)
         {

             pl.Eating(item.Satiety, item.MagicEffectToCast);
             print("FOOOOOOD");
         }*/


        inv.BufferItem = new Item();
            
            EnterDelay = Time.fixedTime + 0.1f;
        
    }

    public void AddUpgradeItemToClosestEmptySlot(int id, int durability, int ammo)
    {
        
        if (id <= -1)
        {
            return;
        }

        if (inv == null || pl == null || inv.GetItemInDatabase(id) == null)
            return;


        if (inv.GetItemInDatabase(id)._bodypart == null || inv.GetItemInDatabase(id)._bodypart.Length <= 0) return;



        AddSubtractStats(id, 1);
        Item item = inv.DeepCopyItem(id, 1, durability, ammo);
        
        for (int x = 0; x < Slots.Length; x++)
            for (int y = 0; y < Slots[x].items.Count; y++)
                for (int b = 0; b < inv.GetItemInDatabase(id)._bodypart.Length; b++)
                {
                    
                    if (Slots[x].Slot[y].
                        GetComponent<Slot>()._bodypart == 
                        inv.GetItemInDatabase(id)._bodypart[b] && Slots[x].items[y].itemID <= -1)
                    {

                        _ItemsSlotsUI.Slots[x].items[y] = item;
                        
                        if (inv.GetItemInDatabase(id).itemID > -1)
                        {
                            
                            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.gun)
                                PlayerGun.SetGunID(item.itemID, durability, ammo);


                            print("AddUpgradeItemToClosestEmptySlot 1 "+x + y +" ** "+ item.itemID);

                            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.cargun && inv.GetItemInDatabase(id)._bodypart[b] == Slot.bodypart.Front)
                            {


                                PlayerGun.SetCarFrontGunID(id, durability, ammo);

                            }

                            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.cargun && inv.GetItemInDatabase(id)._bodypart[b] == Slot.bodypart.Back)
                            {


                                PlayerGun.SetCarBackGunID(id, durability, ammo);

                            }

                            if (inv.GetItemInDatabase(id)._itemtype == Item.itemtype.carmelee)
                            {
                                print("SetCarMeleeID 1");
                                PlayerGun.SetCarMeleeID(id, durability, ammo);

                            }

                        }
                        return;

                    }
                }



        inv.BufferItem = new Item();
 
        EnterDelay = Time.fixedTime + 0.1f;

    }

    public bool CheckItem(int id)
    {
        bool result = false;
        for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].items.Count; i++)
            {
                if (Slots[r].items[i] != null)
                {
                    if (Slots[r].items[i].itemID == id)
                    {
                        result = true;
                        break;
                    }
                    else result = false;
                }
            }
        }


        return result;
    }



    public Item GetItem(int id)
    {
        Item result = null;
        for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].items.Count; i++)
            {
                if (Slots[r].items[i] != null)
                {
                    result = Slots[r].items[i];
                    break;
                }
            }


        }
        return result;
    }



    public bool CheckEveryItem(int id)
    {
        bool result = false;

        int rzlt = 0;
        for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].items.Count; i++)
            {
                if (Slots[r].items[i] != null)
                {
                    if (Slots[r].items[i].itemID == id)
                    {
                        rzlt += 1;
                        break;
                    }
                    else rzlt += 0;
                }
            }
        }




        if (rzlt > 0) result = true;
        return result;
    }

    public void AddItemOnBodyAutomaticly(int ItemID, int durability, int ammo)
    {
     
        int emptyslotsX = -1;
        int emptyslotsY = -1;

        if (inv.GetItemInDatabase(ItemID) == null) return;
        if (inv.GetItemInDatabase(ItemID)._bodypart == null) return;

        for (int x = 0; x < Slots.Length; x++)
        {
            for (int y = 0; y < Slots[x].Slot.Length; y++)
            {
                for (int b = 0; b < inv.GetItemInDatabase(ItemID)._bodypart.Length; b++)
                {
                    if (Slots[x].Slot[y].GetComponent<Slot>()._bodypart == inv.GetItemInDatabase(ItemID)._bodypart[b] && Slots[x].items[y].itemID == -1)
                    {

                        emptyslotsX = x;
                        emptyslotsY = y;

                        break;

                    }
                }

            }

            if (emptyslotsX > -1 || emptyslotsY > -1)
            {
                break;
            }


        }


        if (emptyslotsX == -1 && emptyslotsY == -1)
        {
            for (int b = 0; b < inv.GetItemInDatabase(ItemID)._bodypart.Length; b++)
            {
                for (int x = 0; x < Slots.Length; x++)
                {
                    for (int y = 0; y < Slots[x].Slot.Length; y++)
                    {


                        if (Slots[x].Slot[y].GetComponent<Slot>()._bodypart == inv.GetItemInDatabase(ItemID)._bodypart[b])
                        {
                            emptyslotsX = x;
                            emptyslotsY = y;
                            break;
                        }
                    }

                }
            }
        }




        if (emptyslotsX > -1 && emptyslotsY > -1)
        {
            if (_ItemsSlotsUI.Slots[emptyslotsX].items[emptyslotsY].itemID > -1)
            {
                inv.AddItem(_ItemsSlotsUI.Slots[emptyslotsX].items[emptyslotsY].itemID, 1, inv.GetItemInDatabase(Slots[emptyslotsX].items[emptyslotsY].itemID).Durability, inv.GetItemInDatabase(Slots[emptyslotsX].items[emptyslotsY].itemID).AmmoInGun);
                AddSubtractStats(_ItemsSlotsUI.Slots[emptyslotsX].items[emptyslotsY].itemID, -1);
                _ItemsSlotsUI.Slots[emptyslotsX].items[emptyslotsY] = new Item();


            }

            
            AddUpgradeItem(ItemID, durability, ammo, emptyslotsX, emptyslotsY);

            inv.PlaySoundsPitched(TakeItemClip, 1);
           
        }

        print("AddItemOnBodyAutomaticly");
       
        if (inv.GetItemInDatabase(ItemID).HP != 0)
        {
            print("HEAL");
            pl.Heal(inv.GetItemInDatabase(ItemID).HP);
       
        }

       /* if (inv.GetItemInDatabase(ItemID).Satiety != 0)
        {
            pl.Eating(inv.GetItemInDatabase(ItemID).Satiety, inv.GetItemInDatabase(ItemID).MagicEffectToCast);
           
        }*/

        if(inv.BufferItem.itemID>-1)
        print("BUFFER ITEM: " + inv.BufferItem.itemNames[0]);

     //   inv.RemoveCurrentSlot();


    }



 
    
    void DestroyGunOnLowDurability()
    {
        if ( PlayerGun.GunIDInHand <= -1)
            return;

        for (int x = 0; x < Slots.Length; x++)
        {
            for (int y = 0; y < Slots[x].Slot.Length; y++)
            {
                if (Slots[x].Slot[y] != null && inv.GetItemInDatabase(PlayerGun.GunIDInHand) != null && inv.GetItemInDatabase(PlayerGun.GunIDInHand)._bodypart != null)
                {
                    for (int b = 0; b < inv.GetItemInDatabase(PlayerGun.GunIDInHand)._bodypart.Length; b++)
                    {
                        if (Slots[x].Slot[y].GetComponent<Slot>()._bodypart == inv.GetItemInDatabase(PlayerGun.GunIDInHand)._bodypart[b] && inv.GetItemInDatabase(PlayerGun.GunIDInHand).Durability<=0)
                        {
                            /*
                            AttackEffect = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/Explosion_Wood_Effect"));

                            AttackEffect.transform.position = PlayerGun.GunObject.transform.position;
                            */

                            AddSubtractStats(Slots[x].items[y].itemID, -1);

                            _ItemsSlotsUI.Slots[x].items[y] = new Item();
                            _ItemsSlotsUI.Slots[x].items[y].itemID = -1;

                            print("DestroyGunOnLowDurability");

                            PlayerGun.SetGunID(-1, -1, 0);
                            return;

                        }
                    }
                }
            }
        }




    }


    public void SetupItemsIntoSlots()
    {
        /*for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].Slot.Length; i++)
            {
            

                if (Slots[r].Slot[i].transform.Find("ItemUpgrade" + r + i) == null)
                {
                    GameObject ItemOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Item"), Slots[r].Slot[i].transform);
                    ItemOB.GetComponent<RectTransform>().position = Slots[r].Slot[i].GetComponent<RectTransform>().position;


                    ItemOB.name = "ItemUpgrade" + r + i;


                    if (Slots[r].items[i].itemID > -1)
                    {
                        ItemOB.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Sprites/Items/" + Slots[r].items[i].itemNames[0]);
                    }
                    ItemOB.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }

            }
        }*/
    }

    public SlotRow[] GetSlots()
    {
        return _ItemsSlotsUI.Slots;
    }

    void HealPlayer()
    {
        for (int i = 0; i < inv.inventory.Count; i++)
        {
            if (inv.inventory[i].HP > 0)
            {
                if (pl.HP + inv.inventory[i].HP <= pl.HPMax)
                {
                    pl.Heal(inv.inventory[i].HP);


                }
                else
                {
                    pl.Heal(pl.HPMax);
                }


                inv.inventory[i] = new Item();
                break;

            }
        }


        EnterDelay = Time.fixedTime + 1f;

    }
   



   
    
}
