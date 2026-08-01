using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(Outline))]

    public class GetItem : MonoBehaviour
    {
        private Player pl;
        public int ItemNum;
        public int ItemCount = 1;
        private Inventory inv;



        public bool OnColl;
   
        private GameObject ViewTrigger;
        public AudioClip AClip;

        private Outline _Outline;
    public ItemsSlotsUI BodySlotsUI;


    private void Awake()
    {

        BodySlotsUI = GameObject.Find("BodySlots").GetComponent<ItemsSlotsUI>();
     inv = InitializeOnAwake.pl.GetComponent<Inventory>();

        name += SceneManager.GetActiveScene().name;

    }


    void Start()
        {

            pl = GameObject.Find("Player").GetComponent<Player>();
          
            _Outline = GetComponent<Outline>();
            _Outline.OutlineColor = new Color(0, 0, 0, 0);

            ViewTrigger = GameObject.Find("ViewTrigger");
          
        }

        void Update()
        {
        
            OutlineManager();

            if (pl.PlayerPause())
            {
                if (_Outline != null)
                    _Outline.OutlineColor = new Color(0, 0, 0, 0);
                return;
            }


            if (pl.IM.ActionDelay > Time.fixedTime) return;

            if (pl.SL.SaveLoadCurrent.ObjectsToDestroy.Contains(name))
            {
                print("Destroy " + name);
                Destroy(gameObject);
            }

    


            if (OnColl && (pl.Legscoll_obj.Contains(gameObject) || pl.CarMeleecoll_obj.Contains(gameObject)))
            {
                if (ItemNum > -1)
                {
                    if (inv.CheckBodyPart(ItemNum, Slot.bodypart.Hand))
                    {
                        for (int x = 0; x < BodySlotsUI.Slots.Length; x++)
                        for (int y = 0; y < BodySlotsUI.Slots[x].Slot.Length; y++)
                        if (BodySlotsUI.Slots[x].Slot[y].GetComponent<Slot>()._bodypart == Slot.bodypart.Hand)
                        {
                            if (BodySlotsUI.Slots[x].items[y].itemID == -1)
                            {
                                BodySlotsUI.AddSlotItem(ItemNum, x, y);
                             
                            }
                            else
                            {
                                        print("additem " + ItemNum);
                                        inv.AddItem(ItemNum, ItemCount, 99, 0);


                            }

                            break;
                        }

                    }
                    else
                    {
                        print("additem " + ItemNum);
                        inv.AddItem(ItemNum, ItemCount, 99, 0);

                    }

                    inv.AddItem(inv.GetItemInDatabase(ItemNum).AmmoID, inv.GetItemInDatabase(ItemNum).AmmoInGun, 99999, 0);

                }
                if (AClip != null)
                        pl.PlaySoundsPitched(AClip, 1);
                    Destroy(gameObject);

                pl.SL.SaveLoadCurrent.ObjectsToDestroy.Add(name);
            }


            if (pl.Legscoll_obj.Contains(gameObject)  || pl.CarMeleecoll_obj.Contains(gameObject) || pl.ViewColl(gameObject))
            {

                
                if (pl.IM.enter_b || pl.IM.SpaceB || pl.IM.pick_item || pl.IM.LeftMouseButton)
                {


                if (ItemNum > -1)
                {
                    if (inv.CheckBodyPart(ItemNum, Slot.bodypart.Hand))
                    {
                        for (int x = 0; x < BodySlotsUI.Slots.Length; x++)
                            for (int y = 0; y < BodySlotsUI.Slots[x].Slot.Length; y++)
                                if (BodySlotsUI.Slots[x].Slot[y].GetComponent<Slot>()._bodypart == Slot.bodypart.Hand)
                                {
                                    if (BodySlotsUI.Slots[x].items[y].itemID == -1)
                                    {
                                        BodySlotsUI.AddSlotItem(ItemNum, x, y);
                                      

                                    }
                                    else
                                    {
                                        inv.AddItem(ItemNum, ItemCount, 99, 0);


                                    }

                                    break;
                                }

                    }
                    else
                    {
                        inv.AddItem(ItemNum, ItemCount, 99, 0);

                    }

                    inv.AddItem(inv.GetItemInDatabase(ItemNum).AmmoID, inv.GetItemInDatabase(ItemNum).AmmoInGun, 99999, 0);

                }


                if (AClip != null)
                        pl.PlaySoundsPitched(AClip, 1);
                    Destroy(gameObject);

                    pl.SL.SaveLoadCurrent.ObjectsToDestroy.Add(name);
                }


            }

       
        }


        void OutlineManager()
        {

            if (_Outline == null) return;

        if (pl.PlayerPause())
        {
            if (_Outline != null)
                _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }

        if (!pl.Legscoll_obj.Contains(gameObject) && 
            (!pl.ViewColl(gameObject)) && pl.CarMeleecoll_obj.Contains(gameObject) )
        {
            _Outline.OutlineColor = new Color(0, 0, 0, 0);
            return;
        }

            if (pl.Legscoll_obj.Contains(gameObject) || pl.ViewColl(gameObject) || pl.CarMeleecoll_obj.Contains(gameObject) )
            _Outline.OutlineColor = new Color(1, 1, 1, 1);
            
            else _Outline.OutlineColor = new Color(0, 0, 0, 0);

        
        }


    }

