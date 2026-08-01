using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class ItemsSlotsUI : MonoBehaviour
{
    private float StartAnim;
    
    public SlotRow[] Slots;
    private Player pl;


    public int CurrentSlot { get; set; }
    public int CurrentRow { get; set; }

    private float HorDelay, VertDelay, EnterDelay;
    private bool ShowSlots;

    
    private Camera Cam;

    private AudioClip TakeItemClip, ClickClip;
    private GameObject AttackEffect;
    private GameObject MouseOB, ItemOnChoose;


    private BodySlots UP;

    public bool showthis { get; private set; }

    public bool CanPickMultipleItems;
    public bool CanPickTopItems = true;
    private Item CurrentItem;

    private List<GameObject> FolderButtons = new List<GameObject>();
    private int CurrentFolder;
    private List<Item> CraftingFolder = new List<Item>();
    private GameObject LeftFolder, RightFolder;


    public bool CraftUI;
    public Inventory inv;
    private int GunInBodyX, GunInBodyY, GunCarInBodyX, GunCarInBodyY;

    private void Awake()
    {
         
      


    pl = InitializeOnAwake.pl;
        inv = InitializeOnAwake.pl.GetComponent<Inventory>();

        for (int r = 0; r < Slots.Length; r++)
        {
            for (int i = 0; i < Slots[r].Slot.Length; i++)
            {

                if (Slots[r].Slot[i].GetComponent<Slot>() != null)
                    Slots[r].SlotScrips.Add(Slots[r].Slot[i].GetComponent<Slot>());

                Slots[r].items.Add(new Item());
                Slots[r].items[i].itemID = -1;



                if (Slots[r].Slot[i].transform.Find("ItemUpgrade" + r + i) == null)
                {
                    GameObject ItemOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Item"), Slots[r].Slot[i].transform);
                    ItemOB.GetComponent<RectTransform>().position = Slots[r].Slot[i].GetComponent<RectTransform>().position;
                    ItemOB.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    ItemOB.name = "ItemUpgrade" + r + i;

                    inv.ONOFF(ItemOB, false);

                    if (Slots[r].items[i].itemID > -1)
                    {
                        ItemOB.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Textures/Items/" + Slots[r].items[i].itemNames[0]);
                    }

                }

            }
        }
    }
    void Start()
    {
       /* if (CraftUI)
        {
            LeftFolder = transform.Find("LeftFolder").gameObject;
            RightFolder = transform.Find("RightFolder").gameObject;

            FolderButtons.Add(transform.Find("BuildingsFolderButton").gameObject);
            FolderButtons.Add(transform.Find("GrassFolderButton").gameObject);
            FolderButtons.Add(transform.Find("StoneFolderButton").gameObject);
        }*/

        UP = GetComponent<BodySlots>();
        MouseOB = GameObject.Find("MouseUI");
        ItemOnChoose = GameObject.Find("ChooseUI").transform.Find("ItemOnChoose").gameObject;

        
        ClickClip = Resources.Load<AudioClip>("Sound/UI/Click_0");
        TakeItemClip = Resources.Load<AudioClip>("Sound/UI/Accept");

        Cam = Camera.main;
    
        
        StartAnim = Time.fixedTime + 1f;
       
        ShowSlots = false;
        pl.inv.ONOFF(gameObject, false);



        

    }

  
    void Update()
    {

        if (pl.IM.menu_b || !pl.inv.showinvent)
        {
            CloseUI();
        }

        


        if (showthis)
        {
         
            ShowBufferItem();

            ChoiseSlotsWithMouse();

            MoveAndChoose();

          
            if (pl.IM.menu_b && showthis)
            {
                ExitCrafting();
            }
        }


        
    }


    void MoveAndChoose()
    {
 
        if (!pl.inv.showinvent)
        {
            pl.inv.UnSetBufferItem();
        }



        ShowTopSlots();

        Action();

  

    }



    void ShowTopSlots()
    {
        if (!pl.inv.showinvent) return;
    
        int ii = 0;

        for (int r = 0; r < Slots.Length; r++)
        for (int i = 0; i < Slots[r].Slot.Length; i++)
        {

                  CreateItemOnSlot(r, i);
                
                ii++;
        }
        
    }

    void CreateItemOnSlot(int r, int i)
    {



      

        if (Slots[r].items[i] == null)
        {
            if (Slots[r].Slot[i].transform.Find("ItemUpgrade" + r + i)!=null)
                Slots[r].Slot[i].transform.Find("ItemUpgrade" + r + i).Find("Text").GetComponent<Text>().text = "";

            return;
        }



        GameObject ItemUpgrade = Slots[r].
            Slot[i].transform.Find("ItemUpgrade" + r + i).gameObject;
      


        if (Slots[r].items[i].itemID <= -1)
        {
            if (ItemUpgrade != null)
            {
                pl.inv.ONOFF(ItemUpgrade, false);
                ItemUpgrade.transform.Find("Status").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                ItemUpgrade.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            return;
        }

        if (ItemUpgrade == null)
        {
            GameObject ItemOB = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Item"), Slots[r].Slot[i].transform);
            ItemOB.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            ItemOB.name = "ItemUpgrade" + r + i;
            ItemUpgrade = ItemOB;
        }

       

            pl.inv.ONOFF(ItemUpgrade, true);
            ItemUpgrade.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Items/" + pl.inv.GetItemInDatabase(Slots[r].items[i].itemID).itemNames[0]);

       
        ItemUpgrade.name = "ItemUpgrade" + r + i;
            ItemUpgrade.GetComponent<Image>().color = new Color(1, 1, 1, 1);

           if(Slots[r].items[i].Count>1)
            ItemUpgrade.transform.Find("Text").GetComponent<Text>().text = "x" + Slots[r].items[i].Count;
          


            Image IMG = ItemUpgrade.transform.Find("Status").GetComponent<Image>();
         
            IMG.color = new Color(1, 1, 1, 1);


        pl.inv.SetStatus(pl.inv.GetItemInDatabase(Slots[r].items[i].itemID), ref IMG);


     
        
    }

    

    void Add_Item_ToUpperSegment()
    {
        if (pl.IM.ActionDelay >= Time.fixedTime ||
            EnterDelay >= Time.fixedTime ||
            Slots[CurrentRow].items[CurrentSlot].itemID <= -1 ||
            pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(pl.inv.EscapeInventory))
            return;

        if (UP == null)
        {
            if (pl.inv.BufferItem.itemID != -1) return;
            
            if (pl.IM.exit_b || pl.IM.RightMouseButton )
               DropItemFromSlots();

          

            if (pl.IM.enter_b || (pl.IM.LeftMouseButtonDown && pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[CurrentRow].Slot[CurrentSlot])))
            {
                SetBufferItemFromBody();
            }

            if (pl.IM.enter_b_hold || (pl.IM.LeftMouseButton && pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[CurrentRow].Slot[CurrentSlot])))
            {
                
                ProgressiveActionDelay();
            }
        
            return;
        }
        
        SwapItems();

    }


    void SwapItems()
    {

        if ((pl.inv.BufferItem._itemtype == Item.itemtype.gun && Slots[CurrentRow].items[CurrentSlot]._itemtype == Item.itemtype.gun || pl.inv.BufferItem._bodypart != null) && pl.inv.BufferItem.itemID > -1)
        {

           

            if (pl.IM.enter_b || (pl.IM.LeftMouseButtonDown && pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[CurrentRow].Slot[CurrentSlot])))
            {

                if (CompairBodyparts(pl.inv.BufferItem._bodypart, Slots[CurrentRow].items[CurrentSlot]._bodypart))
                {

                    int BodyID = Slots[CurrentRow].items[CurrentSlot].itemID;
                    int BodyDurability = Slots[CurrentRow].items[CurrentSlot].Durability;
                    int BodyAmmo = Slots[CurrentRow].items[CurrentSlot].AmmoInGun;


                    UP.AddUpgradeItem(pl.inv.BufferItem.itemID, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun, CurrentRow, CurrentSlot);

                    SetBufferItemFromBodyExchange(BodyID, BodyDurability, BodyAmmo);
                }
            }
        }
        else
        {
            if (pl.inv.BufferItem.itemID == -1)
            {
                if (pl.IM.exit_b || pl.IM.RightMouseButton)
                    DropItemFromSlots();

              

                if (pl.IM.enter_b || (pl.IM.LeftMouseButtonDown && pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[CurrentRow].Slot[CurrentSlot])))
                {
                    SetBufferItemFromBody();


                }
            }
        }

    }

    void ON_ChooseTopSegmentSlots()
    {

        if ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && pl.inv.MouseCollideWithSlots() && pl.inv.BufferItem.itemID>-1)
        {
            print("ADD ITEM FROM UP");
            
            pl.inv.UnSetBufferItem();
            pl.inv.SetCurrentFolder();
            ItemOnChoose.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            pl.IM.ActionDelay = Time.fixedTime + 0.3f;
        }

        if (!pl.inv.ChooseTopSegmentSlot) return;

        ColorOfBufferOnTheBody();
        MoveChoiseUI();

        SetTooltips_AndChoosePos();
     

        if (!MouseCollideWithSlots() && pl.IM.MouseMode && pl.inv.PauseInventory)
        {
            pl.inv.ToolTip.transform.Find("Text").GetComponent<Text>().text = "";
            
            // pl.inv.ToolTip.transform.position = new Vector3(99999, 99999, 0);
            pl.inv.Choose.transform.position = pl.MouseOB.transform.position;
        }
        
        Add_Item_ToUpperSegment();

        if ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && !pl.inv.MouseCollideWithSlots() && !pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(pl.inv.EscapeInventory) && pl.IM.ActionDelay < Time.fixedTime && !pl.inv.crafting && pl.inv.BufferItem.itemID > -1 && EnterDelay < Time.fixedTime && Slots[CurrentRow].items[CurrentSlot].itemID == -1)
        AddItemToTheSlot();


        if (pl.IM.exit_b || (pl.IM._vertical < 0 || pl.IM.DPADY < 0) && CurrentRow == Slots.Length - 1 && VertDelay < Time.fixedTime)
        {
            pl.inv.UnSetBufferItem();
            pl.inv.SetCurrentFolder();
        }

     

    }

    void Action()
    {
     
        if (!pl.inv.showinvent || !showthis || pl.inv.showDiscardMenu)
            return;

        ON_ChooseTopSegmentSlots();

        SelectFolder();

        List<GameObject> MouseOBCollList = pl.MouseOB.GetComponent<CollList2D>().GetCollList();
       
        if (pl.inv.GetCurrentItem() != null && !pl.inv.ChooseTopSegmentSlot && pl.inv.GetCurrentItem().itemID > -1 && !MouseOBCollList.Contains(pl.inv.EscapeInventory)
                && !MouseOBCollList.Contains(pl.inv.LeftArrow) && !MouseOBCollList.Contains(pl.inv.RightArrow) &&
                !MouseOBCollList.Contains(pl.inv.CraftingCross) && pl.IM.ActionDelay<Time.fixedTime)
        {

     
                SetBufferItemFromInventory();

            if (pl.inv.GetCurrentItem().CanBeDropped)
            {
                if (!pl.inv.crafting && !pl.inv.PauseInventory)
                    DropItemFromInventory();
                
            }
            else if ((pl.IM.exit_b || pl.IM.RightMouseButton || pl.IM.LeftMouseButtonDown) && !pl.inv.PauseInventory)
                pl._Menu.PlaySoundsPitched(pl._Menu.ErrorClip,1);

            
        }


        if ((pl.IM._vertical > 0 || pl.IM.DPADY > 0) && !pl.inv.ChooseTopSegmentSlot && VertDelay < Time.fixedTime)
        {

            //STARTING TO CHOOSE BODY SLOTS
            print("STARTING TO CHOOSE BODY SLOTS");
            pl.inv.ONOFF(gameObject, true);
            pl.inv.BufferItem = new Item();
            
            pl.inv.PauseInventory = true;
            pl.inv.ChooseTopSegmentSlot = true;
            CurrentRow = Slots.Length - 1;

            VertDelay = Time.fixedTime + 0.1f;
        }



  


    }


   

    void SelectFolder()
    {




        /*
        for (int i = 0; i < FolderButtons.Count; i++)
        {
            if (pl.MouseOB.GetComponent<CollList>().coll_obj.Contains(FolderButtons[i]) && (pl.IM.LeftMouseButtonDown || pl.IM.enter_b))
            {
                pl.PlaySoundsPitched(ClickClip, 1);
               // FolderButtons[i].transform.Find("NewItemTag").gameObject.SetActive(false);
                CurrentFolder = i;
                pl.inv.PauseInventory = true;
                pl.inv.ChooseTopSegmentSlot = true;
                UpdateCraftingFolder();
            }
        }

        if (!pl.inv.PauseInventory) return;

        if (CraftUI)
        {
            if ((pl.IM.LeftTrigger || ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && pl.MouseOB.GetComponent<CollList>().coll_obj.Contains(LeftFolder))) && pl.IM.ActionDelay < Time.fixedTime && CurrentFolder > 0)
            {
                pl.PlaySoundsPitched(ClickClip, 0.8f + CurrentFolder * 0.05f);
                CurrentFolder--;
               // FolderButtons[CurrentFolder].transform.Find("NewItemTag").gameObject.SetActive(false);

                UpdateCraftingFolder();
                pl.IM.ActionDelay = 0.1f;
            }

            if ((pl.IM.RightTrigger || ((pl.IM.enter_b || pl.IM.LeftMouseButtonDown) && pl.MouseOB.GetComponent<CollList>().coll_obj.Contains(RightFolder))) && pl.IM.ActionDelay < Time.fixedTime && CurrentFolder < FolderButtons.Count - 1)
            {
                pl.PlaySoundsPitched(ClickClip, 0.8f + CurrentFolder * 0.05f);
                CurrentFolder++;
               // FolderButtons[CurrentFolder].transform.Find("NewItemTag").gameObject.SetActive(false);

                UpdateCraftingFolder();
                pl.IM.ActionDelay = 0.1f;
            }
        }

        if (CraftingFolder.Count <= 0)
        {
            for (int r = 0; r < Slots.Length; r++)
            {
                for (int i = 0; i < Slots[r].items.Count; i++)
                {
                    if (Slots[r].items[i].Structure && Slots[r].items[i]._StructureType == Item.StructureType.Building)
                    {
                        CraftingFolder.Add(pl.inv.DeepCopyItem(Slots[r].items[i].itemID, Slots[r].items[i].Count, 999));
                        
                    }

                }
            }
          
        }

        */


    }



  
  
  

    void DropItemFromInventory()
    {

      if ((pl.IM.exit_b && !pl.IM.joystick) || pl.IM.RightMouseButton )
        pl.inv.OpenDiscardMenu();
        //  pl.inv.DropItemInSameSpot(pl._transform.position, pl.inv.GetCurrentItem().Count, new int[1] { pl.inv.GetCurrentItem().itemID }, pl.inv.GetCurrentItem().Durability);
        // pl.inv.RemoveCurrentSlot(pl.inv.GetCurrentItem().Count);

    }


  void DropItemFromSlots()
  {
      if (!pl.IM.exit_b && !pl.IM.RightMouseButton) return;
      if (pl.inv.VaultUI == null) return;
      if (!pl.inv.VaultUI.showthis) return;
      if (pl.inv.crafting) return;

      pl.inv.DropItemInSameSpot(pl._transform.position, Slots[CurrentRow].items[CurrentSlot].Count,  new int[1] { Slots[CurrentRow].items[CurrentSlot].itemID }, pl.inv.GetCurrentItem().Durability, pl.inv.GetCurrentItem().AmmoInGun);

      Slots[CurrentRow].items[CurrentSlot] = new Item();

  }

  void SetBufferItemFromInventory()
  {

      if (!pl.IM.enter_b && !pl.IM.LeftMouseButtonDown) return;

     
  

      if ((pl.inv.GetCurrentItem().HP >0 || pl.inv.GetCurrentItem().Satiety > 0 || pl.inv.GetCurrentItem().DamageBuff > 0) && !pl.inv.VaultUI.showthis)
      {

          //  pl.DamageBoost(pl.inv.GetCurrentItem().DamageBuff, pl.inv.GetCurrentItem().DamageBuffTime);
            //pl.Heal(pl.inv.GetCurrentItem().HP, pl.inv.GetCurrentItem().MagicEffectToCast);
           // pl.Eating(pl.inv.GetCurrentItem().Satiety, pl.inv.GetCurrentItem().MagicEffectToCast);

            pl.inv.RemoveCurrentSlot(1);
            

            return;

      }



      if (pl.inv.crafting) return;

      //------------------------------Manually move items



      if (!CanPickMultipleItems)
          SetBufferItem(1);
      else
      {
          if (pl.IM.shift)
              SetBufferItem(10);
          else SetBufferItem(1);
      }

      pl.IM.ActionDelay = Time.fixedTime + 0.2f;

  }



  public void AddSlotItem(int id, int row, int slot)
  {
      if (id <= -1)
      {
          return;
      }

      if (pl.inv == null || pl == null || pl.inv.GetItemInDatabase(id) == null)
          return;

      if (UP != null)
      {
          if (pl.inv.GetItemInDatabase(id).itemID > -1)
          {

                /* if (pl.inv.GetItemInDatabase(id)._itemtype == Item.itemtype.gun)
                 {
                     UP.PlayerGun.SetGunID(id, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun);

                     //PlayerGun.GunTip.transform.position = PlayerGun.Hand.transform.position + new Vector3(0, pl.inv.GetItemInDatabase(id).GunLength, 0);

                 }
                 */
             
            }



            UP.AddUpgradeItem(pl.inv.BufferItem.itemID, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun, CurrentRow, CurrentSlot);
            UP.AddSubtractStats(id, 1);


            Slots[row].items[slot] = pl.inv.DeepCopyItem(id, 1, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun);
            Slots[row].items[slot].Count = pl.inv.BufferItem.Count;

        }


        Slots[row].items[slot] = pl.inv.DeepCopyItem(id, 1, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun);
      Slots[row].items[slot].Count = pl.inv.BufferItem.Count;
      print("AddSlotItem");

      pl.inv.BufferItem = new Item();
      EnterDelay = Time.fixedTime + 0.1f;

  }

    public void AddSlotItem(int id, int durability, int ammo, int row, int slot)
    {
        if (id <= -1)
        {
            return;
        }

        if (pl.inv == null || pl == null || pl.inv.GetItemInDatabase(id) == null)
            return;

        if (UP != null)
        {
            if (pl.inv.GetItemInDatabase(id).itemID > -1)
            {

                /* if (pl.inv.GetItemInDatabase(id)._itemtype == Item.itemtype.gun)
                 {
                     UP.PlayerGun.SetGunID(id, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun);

                     //PlayerGun.GunTip.transform.position = PlayerGun.Hand.transform.position + new Vector3(0, pl.inv.GetItemInDatabase(id).GunLength, 0);

                 }
                 */

            }



            UP.AddUpgradeItem(pl.inv.BufferItem.itemID, durability, ammo, CurrentRow, CurrentSlot);
            UP.AddSubtractStats(id, 1);

        }


        Slots[row].items[slot] = pl.inv.DeepCopyItem(id, 1, durability, ammo);
        Slots[row].items[slot].Count = pl.inv.BufferItem.Count;
        print("AddSlotItem");

        pl.inv.BufferItem = new Item();
        EnterDelay = Time.fixedTime + 0.1f;

    }

    void SetBufferItem(int count)
  {
      if (UP == null)
      {
          if (pl.inv.BufferItem.itemID == -1 && pl.inv.GetCurrentItem() != null)
          {

              CurrentRow = Slots.Length - 1;

              pl.inv.ONOFF(gameObject, true);
              ItemOnChoose.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Items/" + pl.inv.GetCurrentItem().itemNames[0]);
              ItemOnChoose.GetComponent<Image>().enabled = true;
              pl.inv.PauseInventory = true;

              pl.inv.BufferItem = pl.inv.DeepCopyItem(pl.inv.GetCurrentItem().itemID, count, pl.inv.GetCurrentItem().Durability, pl.inv.BufferItem.AmmoInGun);
              pl.inv.BufferItem.itemID = pl.inv.GetCurrentItem().itemID;

              if (pl.inv.GetCurrentItem().Count >= count)
              {
                  pl.inv.BufferItem.Count = count;
                  pl.inv.RemoveCurrentSlot(count);
              }
              else
              {
                  pl.inv.BufferItem.Count = pl.inv.GetCurrentItem().Count;
                  pl.inv.RemoveCurrentSlot(pl.inv.GetCurrentItem().Count);
              }

              pl.inv.ChooseTopSegmentSlot = true;



          }
          return;
      }


      if (pl.inv.GetCurrentItem().HP <= 0 && pl.inv.GetCurrentItem().Satiety <= 0 && pl.inv.BufferItem.itemID == -1 && pl.inv.GetCurrentItem() != null)
      {
          CurrentRow = Slots.Length - 1;

          pl.inv.ONOFF(gameObject, true);
          ItemOnChoose.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Items/" + pl.inv.GetCurrentItem().itemNames[0]);
          ItemOnChoose.GetComponent<Image>().enabled = true;
          pl.inv.PauseInventory = true;

          if (pl.inv.BufferItem.itemID > -1) pl.inv.AddItem(pl.inv.BufferItem.itemID, 1, pl.inv.BufferItem.Durability, pl.inv.BufferItem.AmmoInGun);

          pl.inv.BufferItem = pl.inv.DeepCopyItem(pl.inv.GetCurrentItem().itemID, count, pl.inv.GetCurrentItem().Durability, pl.inv.BufferItem.AmmoInGun);
          pl.inv.BufferItem.itemID = pl.inv.GetCurrentItem().itemID;
          pl.inv.BufferItem.Count = count;
          pl.inv.ChooseTopSegmentSlot = true;

          pl.inv.RemoveCurrentSlot(count);
      }



  }

    public void RemoveSlotItem(int id, Slot.bodypart bodypart )
    {

        if (inv == null || pl == null)
            return;

        if (inv.CheckBodyPart(id, bodypart))
        {
            for (int x = 0; x < Slots.Length; x++)
                for (int y = 0; y < Slots[x].Slot.Length; y++)
                    if (Slots[x].Slot[y].GetComponent<Slot>()._bodypart == Slot.bodypart.Hand)
                    {
                        Slots[x].items[y] = new Item();
                        break;
                    }

        }



    
  
        EnterDelay = Time.fixedTime + 0.1f;

    }





    void MoveChoiseUI()
  {
      if (!showthis) return;
      if ((pl.IM._vertical < -0.5f || pl.IM.DPADY < 0) && CurrentRow < Slots.Length - 1 && VertDelay < Time.fixedTime)
      {
          if (UP != null)
          {
              if (CurrentRow == 0 && CurrentSlot < Slots[CurrentRow].Slot.Length - 1)
              {
                  CurrentSlot++;
              }

              if (CurrentRow == 1 && CurrentSlot > 0)
              {
                  CurrentSlot--;
              }
          }

          pl.inv.StopShake();

          CurrentRow++;

          pl.inv.PlaySoundsPitched(ClickClip, 1);
          if (CurrentSlot > Slots[CurrentRow].items.Count - 1) CurrentSlot = Slots[CurrentRow].items.Count - 1;
          VertDelay = Time.fixedTime + 0.1f;
      }

      if ((pl.IM._vertical > 0.5f || pl.IM.DPADY > 0) && CurrentRow > 0 && VertDelay < Time.fixedTime)
      {
          if (UP != null)
          {
              if (CurrentRow == 1 && CurrentSlot > 0)
              {
                  CurrentSlot--;
              }

              if (CurrentRow == 2 && CurrentSlot < Slots[CurrentRow].Slot.Length - 1)
              {
                  CurrentSlot++;
              }
          }

          pl.inv.StopShake();
          CurrentRow--;
          pl.inv.PlaySoundsPitched(ClickClip, 1);
          if (CurrentSlot > Slots[CurrentRow].items.Count - 1) CurrentSlot = Slots[CurrentRow].items.Count - 1;
          VertDelay = Time.fixedTime + 0.1f;
      }

      if ((pl.IM._horizontal > 0.5f || pl.IM.DPADX > 0) && HorDelay < Time.fixedTime)
      {
          if (CurrentSlot < Slots[CurrentRow].Slot.Length - 1)
          {
              pl.inv.StopShake();
              CurrentSlot++;
              pl.inv.PlaySoundsPitched(ClickClip, 1);
              HorDelay = Time.fixedTime + 0.1f;
          }
          else pl.inv.Shake(new Vector3(4, 0, 0));

      }

      if ((pl.IM._horizontal < -0.5f || pl.IM.DPADX < 0)  && HorDelay < Time.fixedTime)
      {
          if (CurrentSlot > 0)
          {
              pl.inv.StopShake();
              CurrentSlot--;
              pl.inv.PlaySoundsPitched(ClickClip, 1);
              HorDelay = Time.fixedTime + 0.1f;
          }
          else pl.inv.Shake(new Vector3(4,0,0));

      }


  }

  void SetBufferItemFromBodyExchange(int ID, int Durability, int Ammo)
  {
      if (pl.inv.crafting)
      {

          return;
      }



      if (!CanPickTopItems) return;

      ItemOnChoose.GetComponent<Image>().sprite =
      Resources.Load<Sprite>("Textures/Items/" + pl.inv.GetItemInDatabase(Slots[CurrentRow].items[CurrentSlot].itemID).itemNames[0]);
      ItemOnChoose.GetComponent<Image>().enabled = true;

      if (UP != null)
      {
          UP.AddSubtractStats(ID, -1);

      }

      pl.inv.BufferItem = pl.inv.DeepCopyItem(ID, 1, Durability, Ammo);


      pl.inv.PlaySoundsPitched(TakeItemClip, 1);


      EnterDelay = Time.fixedTime + 0.1f;
  }

  void SetBufferItemFromBody()
  {

      if (pl.inv.crafting)
      {


          return;
      }

        if (UP == null) return;

      ItemOnChoose.GetComponent<Image>().sprite =
      Resources.Load<Sprite>("Textures/Items/" + pl.inv.GetItemInDatabase(Slots[CurrentRow].items[CurrentSlot].itemID).itemNames[0]);
      ItemOnChoose.GetComponent<Image>().enabled = true;

     
      

        UP.AddSubtractStats(Slots[CurrentRow].items[CurrentSlot].itemID, -1);


        pl.inv.BufferItem = pl.inv.DeepCopyItem(Slots[CurrentRow].items[CurrentSlot].itemID, Slots[CurrentRow].items[CurrentSlot].Count, Slots[CurrentRow].items[CurrentSlot].Durability, Slots[CurrentRow].items[CurrentSlot].AmmoInGun);

      pl.inv.PlaySoundsPitched(TakeItemClip, 1);
      print("set buffer item " + pl.inv.BufferItem.itemID);


      Slots[CurrentRow].items[CurrentSlot] = new Item();
      Slots[CurrentRow].items[CurrentSlot].itemID = -1;


      EnterDelay = Time.fixedTime + 0.1f;
  }




  public bool MouseCollideWithSlots()
  {
      int result = 0;

      for (int r = 0; r < Slots.Length; r++)
      {
          for (int i = 0; i < Slots[r].Slot.Length; i++)
          {

              if (pl.MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[r].Slot[i]))
              {
                  result++;
              }

          }
      }
      if (result > 0)
          return true;
      else return false;
  }

  void AddItemToTheSlot()
  {
      if (pl.inv.BufferItem.itemID <= -1)
          return;

      if (UP != null)
      {
          if (pl.inv.BufferItem._bodypart == null)
              return;


          if (pl.inv.BufferItem._bodypart.Length <= 0)
              return;
      }


      if (UP != null)
      {
          for (int b = 0; b < pl.inv.BufferItem._bodypart.Length; b++)
          {
              if (Slots[CurrentRow].Slot[CurrentSlot].GetComponent<Slot>()._bodypart == pl.inv.BufferItem._bodypart[b])
              {

                  AddSlotItem(pl.inv.BufferItem.itemID, CurrentRow, CurrentSlot);
                  break;
              }
          }
      }
      else
      {

          AddSlotItem(pl.inv.BufferItem.itemID, CurrentRow, CurrentSlot);

      }


  }


  void ShowBufferItem()
  {

      ItemOnChoose = pl.inv.Choose.transform.Find("ItemOnChoose").gameObject;

      if (pl.inv.BufferItem.itemID > -1)
      {
          ItemOnChoose.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Items/" + pl.inv.BufferItem.itemNames[0]);
          ItemOnChoose.GetComponent<Image>().enabled = true;
      }
      else ItemOnChoose.GetComponent<Image>().enabled = false;


      if (pl.inv.Choose.activeInHierarchy && ItemOnChoose.activeInHierarchy)
      {

          if (pl.inv.BufferItem.itemID == -1)
          {

              pl.inv.Choose.transform.Find("ItemOnChooseNum").GetComponent<Text>().enabled = false;


          }
          else
          {
              if (pl.inv.BufferItem.itemNames != null)
              {

                  pl.inv.Choose.transform.Find("ItemOnChooseNum").GetComponent<Text>().enabled = true;


                  if (pl.inv.BufferItem.CanStack)
                      pl.inv.Choose.transform.Find("ItemOnChooseNum").GetComponent<Text>().text = "x " + pl.inv.BufferItem.Count;
                  else pl.inv.Choose.transform.Find("ItemOnChooseNum").GetComponent<Text>().text = "";

              }
          }


      }
  }

  void ChoiseSlotsWithMouse()
  {
      if (!pl.IM.MouseMode) return;

      for (int x = 0; x < Slots.Length; x++)
      {
          for (int y = 0; y < Slots[x].Slot.Length; y++)
          {
              if (MouseOB.GetComponent<CollList2D>().GetCollList().Contains(Slots[x].Slot[y]))
              {
                  if (CurrentRow != x || CurrentSlot != y)
                  {
                      pl.PlaySoundsPitched(ClickClip, 1);
                  }


                  CurrentRow = x;
                  CurrentSlot = y;
                  pl.inv.ChooseTopSegmentSlot = true;
                  pl.inv.PauseInventory = true;
              }
          }
      }
  }



  void ColorOfBufferOnTheBody()
  {
      if (pl.inv.BufferItem.itemID <= -1)
          return;

      int i = 0;

      if (UP != null)
      {
          if (pl.inv.BufferItem._bodypart == null)
          {
              ItemOnChoose.GetComponent<Image>().color = new Color(1, 0.1f, 0.1f, 1);
              return;
          }

          if (pl.inv.BufferItem._bodypart.Length <= 0)
              return;




          for (int b = 0; b < pl.inv.BufferItem._bodypart.Length; b++)
          {
              if (Slots[CurrentRow].Slot[CurrentSlot].GetComponent<Slot>()._bodypart == pl.inv.BufferItem._bodypart[b])
              {
                  i++;

              }

          }
      }
      else i = 1;

      if (i > 0)
      {
          ItemOnChoose.GetComponent<Image>().color = new Color(0.8f, 1, 0.8f, 1);
      }
      else ItemOnChoose.GetComponent<Image>().color = new Color(1, 0.1f, 0.1f, 1);

  }

  public void StartUI()
  {
      showthis = true;
        pl.inv.ONOFF(gameObject, true);

        ItemOnChoose.GetComponent<Image>().sprite = null;
      ItemOnChoose.GetComponent<Image>().enabled = false;

 
      print(name);
    

        //  ShowSlots = false;
    }

  public void CloseUI()
  {

      if (showthis)
      {
          pl.inv.UnSetBufferItem();

          CurrentRow = 0;
          CurrentSlot = 0;

          pl._Menu.ONOFFUI(pl.inv.EscapeInventory.transform, false);

          ItemOnChoose.GetComponent<Image>().sprite = null;
          ItemOnChoose.GetComponent<Image>().enabled = false;
          pl.IM.ActionDelay = Time.fixedTime + 0.1f;
          pl._Menu.ActionDelay = Time.fixedTime + 0.1f;
            
            pl.inv.CloseDiscardMenu();
            showthis = false;
          pl.inv.ONOFF(gameObject, false);
            print("CloseUI");
      }


      // ShowSlots = true;
  }




  void SetTooltips_AndChoosePos()
  {
      if ((MouseCollideWithSlots() && pl.IM.MouseMode) || !pl.IM.MouseMode)
      {

          if (Slots[CurrentRow].items[CurrentSlot] != null && Slots[CurrentRow].items[CurrentSlot].itemID > -1)
          {
             // pl.inv.ToolTip.SetActive(true);
              pl.inv.ToolTip.transform.Find("Text").GetComponent<Text>().text = pl.inv.ToolpitString(Slots[CurrentRow].items[CurrentSlot]);



             /* if (pl.inv.Choose.transform.position.y < 700)
                  pl.inv.ToolTip.transform.position = pl.inv.Choose.transform.position + new Vector3(0.1f,0,0);
              else
                  pl.inv.ToolTip.transform.position = new Vector3(pl.inv.Choose.transform.position.x, 700, pl.inv.ToolTip.transform.position.z) + new Vector3(40, 0, 0);

             */

        pl.inv.ToolTip.transform.SetAsLastSibling();
            }
            else
            {
                if (!pl.IM.MouseMode)
                    pl.inv.Choose.transform.position = new Vector3(99999, 99999, 999);
                else
                    pl.inv.Choose.transform.position = pl.MouseOB.transform.position;


                // pl.inv.ToolTip.transform.position = new Vector3(99999, 99999, 0);
                pl.inv.ToolTip.transform.Find("Text").GetComponent<Text>().text = "";
               
                //  pl.inv.ToolTip.SetActive(false);
            }

            

          

            if(CurrentRow< Slots.Length )
                if(CurrentSlot < Slots[CurrentRow].Slot.Length )
            pl.inv.Choose.transform.position = Slots[CurrentRow].Slot[CurrentSlot].transform.position + pl.inv.ShakeVector();
        }
    }



    




  
    void ExitCrafting()
    {
        pl.inv.crafting = false;

        pl.inv.PauseInventory = false;
       

        pl._Menu.ONOFFUI(pl.inv.EscapeInventory.transform, true);

        CloseUI();

        pl._Menu.ActionDelay = Time.fixedTime + 0.1f;

        pl.IM.ActionDelay = Time.fixedTime + 0.2f;

    }
    void ProgressiveActionDelay()
    {

        if (pl.IM.CraftedItems > 4)
            pl.IM.ActionDelay = Time.fixedTime + 0.2f;
        else if (pl.IM.CraftedItems > 3)
            pl.IM.ActionDelay = Time.fixedTime + 0.35f;
        else if (pl.IM.CraftedItems >= 2)
            pl.IM.ActionDelay = Time.fixedTime + 0.5f;
        else
            pl.IM.ActionDelay = Time.fixedTime + 0.7f;

        pl.IM.CraftedItems++;
    }

    bool CompairBodyparts(Slot.bodypart[] PartOne, Slot.bodypart[] PartTwo)
    {
        bool res = false;

        Array.ForEach(PartOne, part =>
        {
            Array.ForEach(PartTwo, part2 =>
            {
                res = (part == part2) ? true : res = false;
            });
        });

        return res;
    }

   
    

    public void SetCurrentFolder()
    {

        if (CurrentFolder > FolderButtons.Count - 1) CurrentFolder = FolderButtons.Count - 1;

        FolderButtons[CurrentFolder].transform.Find("NewItemTag").gameObject.SetActive(false);

        CurrentItem = new Item();
     
    }


    public void SetFolder(int c)
    {

        if (c > FolderButtons.Count - 1) c = FolderButtons.Count - 1;

        FolderButtons[c].transform.Find("NewItemTag").gameObject.SetActive(false);
        CurrentFolder = c;
        CurrentItem = new Item();
 

    }

    public void SetGunOnBodyPosition(int x, int y)
    {
        GunInBodyX = x;
        GunInBodyY = y;

    }

    public void SetCarGunOnBodyPosition(int x, int y)
    {
        GunCarInBodyX = x;
        GunCarInBodyY = y;

    }

    public Item GetGunOnBody()
    {
        return Slots[GunInBodyX].items[GunInBodyY];
    }

    public Item GetCarGunOnBody()
    {
        return Slots[GunCarInBodyX].items[GunCarInBodyY];
    }



}
