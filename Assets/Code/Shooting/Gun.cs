using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gun : MonoBehaviour

{
    public int DamageAmount = 0;
    public float TargetDistance;
    public float AllowedRange = 30.0f;

    private Player pl;
    private Menu _Menu;
    public GameObject CrossObject;
    public GameObject MechanicsObject;
    

    public float ReloadTimer {get;set; }
    private float GunQuickSwitchTimer;
    private float  AimDurationTimer;
    private AudioSource GunSound;
    [HideInInspector]
    public float ShotDurationTimer;

    [HideInInspector]
    public int CurrentAmmoID = -1;
    public int CurrentGunID = -1;

    [HideInInspector]
    public Item GunInHandItem;
    public Item CarGunInHandItem;

    [HideInInspector]
    public int GunIDInHand = -1;
    public int GunInHandAmmo = -1;

    public int CarFrontGunID = -1;
    public int CarBackGunID = -1;
    public int CarMeleeID = -1;


    private int GunDurability;
    public GameObject GunObject;

    private GameObject GunTip;
    private Text AmmoText;
    private bool Aiming;



    private GameObject ShootEffect;
    private List<GameObject> BloodEffects = new List<GameObject>();
    private List<GameObject> WallEffects = new List<GameObject>();
    private List<GameObject> WallCannonEffects = new List<GameObject>();
   
    private Transform GunTrigger;

    public bool TurretMode { get; set; }

    private Inventory inv;
    private Mesh GunInHandMesh;
    private Material GunInHandMaterial;
    private MeshFilter GunObjectMeshFilter;

    private RaycastHit gunrayhit;
    public LayerMask GunMask;
    private List<Vector3> GunHipPositions = new List<Vector3>();
    private List<RectTransform> AimPointParts = new List<RectTransform>();
    private GameObject CentralPointParts;
    private GameObject AimPoint;
    private Transform Canvas_TR;
    private float FinalGunSpread;


    public ItemsSlotsUI BodySlotsUI;
    private CollList MeleeColl;
    private float AttackDuration;
    private string AmmoT;
    private void Start()
    {
        BodySlotsUI = GameObject.Find("BodySlots").GetComponent<ItemsSlotsUI>();
        MeleeColl = GunObject.transform.Find("MeleeCollider").GetComponent<CollList>();
        GunInHandItem = new Item();
        GunObject.GetComponent<MeshFilter>().mesh = null;

        AmmoText = GameObject.Find("AmmoText").GetComponent<Text>();

        pl = InitializeOnAwake.pl;
        inv = InitializeOnAwake.inv;
        _Menu = InitializeOnAwake._Menu;

        if (GetComponent<Menu>() == null) print("MENU IS NULLL");


        GunTip = GunObject.transform.Find("GunTip").gameObject;
        GunSound = GunObject.GetComponent<AudioSource>();

        GunObjectMeshFilter = GunObject.GetComponent<MeshFilter>();

        AimPoint = Instantiate(Resources.Load<GameObject>("Prefabs/UI/AimPoint"), GameObject.Find("Canvas").transform);
       
            AimPoint.transform.SetAsFirstSibling();

        AimPointParts.Add(AimPoint.transform.Find("UP").GetComponent<RectTransform>());
        AimPointParts.Add(AimPoint.transform.Find("RIGHT").GetComponent<RectTransform>());
        AimPointParts.Add(AimPoint.transform.Find("DOWN").GetComponent<RectTransform>());
        AimPointParts.Add(AimPoint.transform.Find("LEFT").GetComponent<RectTransform>());
        CentralPointParts = AimPoint.transform.Find("Center").gameObject;
        Canvas_TR = GameObject.Find("Canvas").transform;



        for (int i = 0; i < 3; i++)
        {
             BloodEffects.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Blood")));
        }

        for (int i = 0; i < 10; i++)
        {
            WallEffects.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BulletHitsWall")));
        }

        for (int i = 0; i < 10; i++)
        {
            WallCannonEffects.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Effects/CannonHitsWall")));
        }

        GunTrigger = GameObject.Find("GunTrigger").transform;

    }


    void Update ()
    {

        AimPointControll();

        if (_Menu.MenuONOFF || 
            inv.showinvent || inv.showjournal || 
            pl.InDialog || pl.showUpgrades) return;




        QuiсkWeaponSwap();
  
        GunUI();
        UpdateCurrentIDs();
        // CurrentIDs();
        Aim();
     
        Shoot();
        

        if (pl.IM.Reload )
        Reload();



        Animations();
        Timers();
        DropGun();
    }

    void Animations()
    {
        
            if (GunIDInHand > -1)
            {
                GunObjectMeshFilter.mesh = GunInHandMesh;
                GunObject.GetComponent<MeshRenderer>().material = GunInHandMaterial;


            }
            else
            {
                GunObjectMeshFilter.mesh = null;

            }
            if (GunInHandItem.itemID > -1)
                if (GunInHandItem.PrefabObject.transform.Find("GunTip") != null)
                    SetGunTip(GunTip, GunInHandItem.PrefabObject.transform.Find("GunTip").transform.localPosition);

        



        if (ShootEffect == null) return;

        ShootEffect.transform.position = GunTip.transform.position;


    }

    void UpdateCurrentIDs()
    {
       
            CurrentGunID = GunIDInHand;
            CurrentAmmoID = GunInHandAmmo ;
        



        if (GunIDInHand == -1) CurrentGunID = -1;
      
        if (CurrentGunID <= -1) CurrentAmmoID = -1;
        else CurrentAmmoID = GunInHandAmmo;
    }

   
    void Aim()
    {
       
       

        if (ReloadTimer > 0) return;
        if (ShotDurationTimer > 0) return;
       
        GunTrigger.localPosition = new Vector3(0, 0, GunTrigger.localPosition.z);

        

        if (pl.IM.Aim)
        {
            if (!Aiming)
            {
               
               
                AimDurationTimer = 0.1f;
                Aiming = true;

            }

            //if (Aiming && AimDurationTimer < 0) pl.PlayAnim("AimEnd");

           
            pl.FOV_Main = Mathf.Lerp(pl.FOV_Main, pl.FOV_MainDefault - 10, Time.deltaTime * 30);

          

        }
        else
        {
            Aiming = false;

  


            pl.FOV_Main = Mathf.Lerp(pl.FOV_Main, pl.FOV_MainDefault, Time.deltaTime * 16);
         
        }

    }



   


    void Shoot()
    {
        
            if (Aiming) FinalGunSpread = CarGunInHandItem.GunSpread / 5f;
            else FinalGunSpread = CarGunInHandItem.GunSpread;

        


        if (AttackDuration > Time.fixedTime && GunInHandItem._Guntype == Item.Guntype.knife)
            GunCollision();

        if (ReloadTimer > 0 || ShotDurationTimer > 0 || CurrentGunID == -1 ) return;
        

        

      

        if (!pl.IM.Fire || pl.CollidingDialog != null) return;

        
        if (pl.HandObject != null)
        {
            _Menu.PlaySoundsPitched(_Menu.ErrorClip, 1);
            return;
        }
        //pl.DropHandObject();

        // pl.Gun_Ray = GameObject.Find("GunTrigger").GetComponent<CollList>().rayhit;
        pl.Gun_Ray = new List<GameObject>();
        GunHipPositions = new List<Vector3>();

        if (GunInHandItem._Guntype == Item.Guntype.knife )
        {
            pl.Gun_Ray = MeleeColl.coll_obj;
        }
        else
        {
            for (int i = 0; i < inv.GetItemInDatabase(CurrentGunID).BulletsInShot; i++)
            {
               
                if (Physics.Raycast(GunTrigger.transform.position,
                                GunTrigger.transform.forward +
                                GunTrigger.transform.up * UnityEngine.Random.Range(0, FinalGunSpread / 40f) +
                                GunTrigger.transform.right * UnityEngine.Random.Range(-FinalGunSpread / 40f, FinalGunSpread / 40f), out gunrayhit, 100, GunMask))
                {


                    pl.Gun_Ray.Add(gunrayhit.collider.gameObject);
                    GunHipPositions.Add(gunrayhit.point);



                    Debug.DrawRay(GunTip.transform.position, GunTrigger.transform.forward, new Color(1, 1, 1, 1));

                    pl.Gun_Ray.Add(gunrayhit.collider.gameObject);
                    GunHipPositions.Add(gunrayhit.point);
                }
                
            }
        }

      //  GunTrigger.localRotation = Quaternion.Euler(Random.Range(-GunInHandItem.GunSpread, GunInHandItem.GunSpread), 0, 0);



        if (inv.GetItem(CurrentGunID).AmmoInGun < 1 && inv.GetItem(CurrentGunID)._Guntype != Item.Guntype.knife)
        {
            if (inv.GetItem(CurrentAmmoID) == null)
            {
                GunSound.clip = inv.GetItemInDatabase(CurrentGunID).EmptyShotClip;
                GunSound.Play();

            }
                Reload();
            return;

        }

    
        GunSound.clip = inv.GetItemInDatabase(CurrentGunID).ShootClips[UnityEngine.Random.Range(0, inv.GetItemInDatabase(CurrentGunID).ShootClips.Length)];

        GunSound.Play();
        //Flash.SetActive(true);
        /*
        if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.pistol)
        {

            if (!Aiming)
                pl.Pl_Anim.PlayBodyAnim("PistolShooting_NoAim");
            else pl.Pl_Anim.PlayBodyAnim("PistolShooting");
        }
        if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.shotgun)
        {

            if (!Aiming)
                pl.Pl_Anim.PlayBodyAnim("ShotgunShooting_NoAim");
            else pl.Pl_Anim.PlayBodyAnim("ShotgunShooting");
        }

        if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.rifle)
        {

            if (!Aiming)
                pl.Pl_Anim.PlayBodyAnim("RifleShooting_NoAim");
            else pl.Pl_Anim.PlayBodyAnim("RifleShooting");
        }

        if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.knife)
        {
            pl.Pl_Anim.PlayBodyAnim("KnifeAttack");
            AttackDuration = Time.fixedTime + 1;
        }
        */
        if (inv.GetItemInDatabase(CurrentGunID)._Guntype != Item.Guntype.knife)
            inv.GetItem(CurrentGunID).AmmoInGun -= 1;

        if (inv.GetItemInDatabase(CurrentGunID).EffectToCast != null)
        {
            if (ShootEffect != null)
            {
                if (ShootEffect.name != inv.GetItemInDatabase(CurrentGunID).EffectToCast.name)
                {
                    Destroy(ShootEffect);
                    ShootEffect = Instantiate(inv.GetItemInDatabase(CurrentGunID).EffectToCast, GunTip.transform.position, GunTip.transform.parent.rotation, GunTip.transform.parent);

                }
            }
            else ShootEffect = Instantiate(inv.GetItemInDatabase(CurrentGunID).EffectToCast, GunTip.transform.position, GunTip.transform.parent.rotation, GunTip.transform.parent);


            ShootEffect.GetComponent<Animator>().Play("Main", 0);
        }

        if (GunInHandItem._Guntype != Item.Guntype.knife)
        {
            
         
                pl.SetCamShakeTimer(0.2f, 0.002f);
            
            GunCollision();
        }
        ShotDurationTimer = 0.5f;


    }


    void GunCollision()
    {

        for (int i = 0; i < pl.Gun_Ray.Count; i++)
        {
            if (pl.Gun_Ray[i] != null)
            {
                if (pl.Gun_Ray[i].GetComponent<StatsControll>() != null ||
                    pl.Gun_Ray[i].GetComponent<EnemyBodyPart>() != null)
                {
                    StatsControll _Stats;
                    Attack _Attack;
                    EnemyBodyPart _Bodypart;

                    int HeadShotBuff = 0;
                    if (pl.Gun_Ray[i].GetComponent<EnemyBodyPart>() != null)
                    {
                        _Bodypart = pl.Gun_Ray[i].GetComponent<EnemyBodyPart>();

                        _Stats = _Bodypart._ParentStart;
                        _Attack = _Bodypart._ParentStart.GetComponent<Attack>();

                        if (_Bodypart.Bodypart == Slot.bodypart.Head)
                        {
                            if (_Stats.GetComponent<MoveBetweenSpots>() != null && !_Attack.Lying)
                                _Stats.GetComponent<MoveBetweenSpots>().AddSlowdown(1);
                            HeadShotBuff = 1;
                        }
                        
                    }
                    else
                    {
                        _Stats = pl.Gun_Ray[i].GetComponent<StatsControll>();
                        _Attack = pl.Gun_Ray[i].GetComponent<Attack>();

                    }

                    if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.pistol)
                        _Stats.GetDamage(pl.PistolDamage + 1 + HeadShotBuff);

                    if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.shotgun)
                        _Stats.GetDamage(pl.ShotgunDamage + 2 + HeadShotBuff);

                    if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.rifle)
                        _Stats.GetDamage(pl.RifleDamage + 3 + HeadShotBuff);

                    if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.cannon)
                        _Stats.GetDamage(pl.RifleDamage + 3 + HeadShotBuff);

                    if (inv.GetItemInDatabase(CurrentGunID)._Guntype == Item.Guntype.knife)
                        _Stats.GetDamage(1);
                    if (_Attack != null)
                        _Attack.StartChasing();

                    AttackDuration = -1;
                    SetBloodEffect(pl.Gun_HitPos);

                }
                else
                {
                    print("pl.Gun_Ray.Count " + pl.Gun_Ray.Count);
                    if (GunHipPositions.Count > 0)
                    {
                        if (inv.GetItemInDatabase(CurrentGunID)._Guntype != Item.Guntype.cannon)
                            SetWallEffect(GunHipPositions[i], ref WallEffects);
                        else
                            SetWallEffect(GunHipPositions[i], ref WallCannonEffects);
                    }
                }
            }
        }
    }
    void Reload()
    {
        if (inv.GetItem(CurrentAmmoID) ==null) return;

        if (inv.GetItem(CurrentAmmoID).Count <= 0) return;
        if (ReloadTimer > 0) return;
        if (inv.GetItem(CurrentGunID).AmmoInGun >= pl.MagCapacity)
        {
            ReloadTimer = 1;
            ActionReload();
            return;
        }

        int addtomag = pl.MagCapacity - inv.GetItem(CurrentGunID).AmmoInGun;

        if (inv.GetItem(CurrentAmmoID).Count < addtomag) addtomag = inv.GetItem(CurrentAmmoID).Count;

        inv.GetItem(CurrentGunID).AmmoInGun += addtomag;
        inv.ReduceItemCount(CurrentAmmoID, addtomag);
        ActionReload();
        

        ReloadTimer = 1;
    }

    public void SetGunID(int ID, int durability, int ammo)
    {

     

        if (ID <= -1)
        {

        
            GunIDInHand = -1;
            GunDurability = 0;
            GunInHandItem = new Item();
           
            GunInHandMesh = null;

            return;
        }



        GunInHandItem = inv.GetItemInDatabase(ID);
        GunInHandMesh = inv.GetItemInDatabase(ID).PrefabObject.GetComponent<MeshFilter>().sharedMesh;
        GunInHandMaterial = inv.GetItemInDatabase(ID).PrefabObject.GetComponent<MeshRenderer>().sharedMaterial;


        GunIDInHand = ID;
        GunInHandAmmo = inv.GetItemInDatabase(ID).AmmoID;

        GunDurability = durability;

      
        pl.MagCapacity = inv.GetItemInDatabase(ID).AmmoInGun;

         
        DamageAmount = inv.GetItemInDatabase(ID).DamageAmount;
        print("pl.MagCapacity  " + pl.MagCapacity);
      

        /* for (int i = 0; i < BulletsForThisGun.Count; i++) Destroy(BulletsForThisGun[i]);
            BulletsForThisGun = new List<GameObject>();

            if (GetItemInDatabase(CurrentGunID).MagicObjectToCast != null && GetItemInDatabase(CurrentGunID).MagicObjectToCast.Length > 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (BulletsForThisGun.Count < 20)
                    {
                        GameObject b = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Objects/" + GetItemInDatabase(CurrentGunID).MagicObjectToCast));
                        b.transform.position = new Vector3(9999, 9999);
                        BulletsForThisGun.Add(b);
                    }
                }
            }
            */



       

    }

    public void SetGunItem(Item gun)
    {



        if (gun.itemID <= -1)
        {


            GunIDInHand = -1;
            GunDurability = 0;
            GunInHandItem = new Item();

            GunInHandMesh = null;
            GunObjectMeshFilter.mesh = null;

            return;
        }


        GunInHandItem = gun;
        GunInHandMesh = gun.PrefabObject.GetComponent<MeshFilter>().sharedMesh;
        GunInHandMaterial = gun.PrefabObject.GetComponent<MeshRenderer>().sharedMaterial;


        GunIDInHand = gun.itemID;

        GunDurability = gun.Durability;


        DamageAmount = gun.DamageAmount;



    }

    void DropGun()
    {
        if (!pl.IM.exit_b || GunIDInHand<=-1) return;

        GameObject dropped = Instantiate(GunObject);
        dropped.transform.position = GunObject.transform.position;
        dropped.transform.parent = null;
        dropped.name = pl.inv.GetItemInDatabase(GunIDInHand).itemNames[0];


        dropped.AddComponent<Rigidbody>();
        dropped.AddComponent<BoxCollider>();
        dropped.GetComponent<Rigidbody>().useGravity = true;
        dropped.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        dropped.AddComponent<GetItem>();
        dropped.GetComponent<GetItem>().ItemNum = GunIDInHand;

        BodySlotsUI.RemoveSlotItem(GunIDInHand, Slot.bodypart.Hand);


        for (int i = 0; i < pl.SL.SaveLoadCurrent.ObjectsToDestroy.Count; i++)
        {
            if (pl.SL.SaveLoadCurrent.ObjectsToDestroy[i] == dropped.name)
            {
                pl.SL.SaveLoadCurrent.ObjectsToDestroy.RemoveAt(i);
          
            }
        }

        SetGunID(-1, 0, 0);
        

    }



    public void SetCarFrontGunID(int ID, int durability, int ammo)
    {
        if (ID == -1)
        {
           
            CarFrontGunID = -1;

            if (CarBackGunID <= -1)
                CarGunInHandItem = new Item();
            else CarGunInHandItem = inv.GetItemInDatabase(CarBackGunID);



            return;
        }
 
        CarFrontGunID = ID;
        GunInHandAmmo = inv.GetItemInDatabase(ID).AmmoID;
        CarGunInHandItem = inv.GetItemInDatabase(ID);


        pl.MagCapacity = inv.GetItemInDatabase(ID).AmmoInGun;
        DamageAmount = inv.GetItemInDatabase(ID).DamageAmount;

       
    }

    public void SetCarBackGunID(int ID, int durability, int ammo)
    {
        if (ID == -1)
        {
      
            CarBackGunID = -1;

            if (CarFrontGunID<=-1)
            CarGunInHandItem = new Item();
            else CarGunInHandItem = inv.GetItemInDatabase(CarFrontGunID);

            return;
        }
  
        CarBackGunID = ID;
        GunInHandAmmo = inv.GetItemInDatabase(ID).AmmoID;
        CarGunInHandItem = inv.GetItemInDatabase(ID);

        pl.MagCapacity = inv.GetItemInDatabase(ID).AmmoInGun;
        DamageAmount = inv.GetItemInDatabase(ID).DamageAmount;
        
    }


    public void SetCarMeleeID(int ID, int durability, int ammo)
    {
        if (ID <= -1)
        {
        
            CarMeleeID = -1;
            return;
        }

     
        CarMeleeID = ID;

        //GunDurability = durability;


        pl.MagCapacity = inv.GetItemInDatabase(ID).AmmoInGun;
        DamageAmount = inv.GetItemInDatabase(ID).DamageAmount;
      
        

    }


    private void GunUI()
    {
        if(_Menu.Language==1)
        AmmoT = "Патрони: ";
        if (_Menu.Language == 0)
            AmmoT = "Ammo: ";

        if (CurrentGunID <= -1)
        {
            AmmoText.text = AmmoT + 0 + " / none";
            return;
        }


        if (inv.GetItem(CurrentGunID) == null) return;

        if(inv.GetItem(CurrentAmmoID) !=null)
        AmmoText.text = AmmoT + inv.GetItem(CurrentGunID).AmmoInGun + " / " + 
                inv.GetItem(CurrentAmmoID).Count;
        else AmmoText.text =
               AmmoT + inv.GetItem(CurrentGunID).AmmoInGun + " / none";
      
    }

    void Timers()
    {
        if(ReloadTimer>0)
        ReloadTimer -= Time.deltaTime;

        if (ShotDurationTimer > 0)
            ShotDurationTimer -= Time.deltaTime;

        if (AimDurationTimer > 0)
            AimDurationTimer -= Time.deltaTime;

        
    }

    public void SetBloodEffect(Vector3 Pos)
    {
        for (int i = 0; i < BloodEffects.Count; i++)
        {
            if (!BloodEffects[i].GetComponent<MeshRenderer>().enabled)
            {
                BloodEffects[i].transform.position = Pos;
                BloodEffects[i].transform.LookAt(new Vector3(pl.transform.position.x, pl.transform.position.y, pl.transform.position.z));
                BloodEffects[i].GetComponent<Animator>().Play("Main", 0);
                return;
            }
        }
    }

   


    public void SetWallEffect(Vector3 Pos, ref List<GameObject> _Effects)
    {

        for (int i = 0; i < _Effects.Count; i++)
        {
            if (!_Effects[i].GetComponent<MeshRenderer>().enabled)
            {
                print("SetWallEffect");
                _Effects[i].transform.position = Pos;

                Vector3 v3 = pl.transform.position - _Effects[i].transform.position;

                Quaternion fixedRotation = Quaternion.Euler(new Vector3(90, 1, 1));

                _Effects[i].transform.rotation = Quaternion.LookRotation(new Vector3(v3.x, v3.y, v3.z)) * fixedRotation;
     
                _Effects[i].GetComponent<Animator>().Play("Main", 0);
                _Effects[i].GetComponent<MeshRenderer>().enabled = true;
                return;
            }
        }
    }

    public void SetGunTip(GameObject Tip, Vector3 Pos)
    {
        GunTip = Tip;
        GunTip.transform.localPosition = Pos;
    }

    void AimPointControll()
    {
        if (_Menu.HideUI) 
        {
            AimPoint.SetActive(false);
            return;
        }


        if (pl.PlayerPause()) AimPoint.SetActive(false);
        else AimPoint.SetActive(true);

        AimPoint.transform.position = Canvas_TR.position;
        

        if (CurrentGunID <= -1)
        {
            CentralPointParts.SetActive(true);

            for (int i = 0; i < AimPointParts.Count; i++)
            {
                AimPointParts[i].gameObject.SetActive(false);
            }

            return;
        }

        if (GunInHandItem._Guntype == Item.Guntype.knife )
        {
            CentralPointParts.SetActive(true);
            for (int i = 0; i < AimPointParts.Count; i++)
            {
                AimPointParts[i].gameObject.SetActive(false);
            }
            return;
        }

        CentralPointParts.SetActive(false);


        float scalsefactor = 1 + FinalGunSpread * 3;

        for (int i = 0; i < AimPointParts.Count; i++)
        {
            if (Aiming )
            {
                AimPointParts[i].gameObject.SetActive(false);
            }
            else
                AimPointParts[i].gameObject.SetActive(true);

            if (i == 0)
                AimPointParts[i].localPosition = new Vector3(0,
                    Mathf.Lerp(AimPointParts[i].localPosition.y, 9 * scalsefactor, Time.deltaTime * 5), 0);
            if (i == 1)
                AimPointParts[i].localPosition = new Vector3(
                    Mathf.Lerp(AimPointParts[i].localPosition.x, 9 * scalsefactor, Time.deltaTime * 5), 0, 0);
            if (i == 2)
                AimPointParts[i].localPosition = new Vector3(0,
                    Mathf.Lerp(AimPointParts[i].localPosition.y, -9 * scalsefactor, Time.deltaTime * 5), 0);
            if (i == 3)
                AimPointParts[i].localPosition = new Vector3(
                    Mathf.Lerp(AimPointParts[i].localPosition.x, -9 * scalsefactor, Time.deltaTime * 5), 0, 0);

        }

    }



    void ActionReload()
    {

        //  CrossObject.SetActive(false);
        //  MechanicsObject.SetActive(false);

        GunSound.clip = inv.GetItemInDatabase(CurrentGunID).ReloadClip;
        GunSound.Play();

        pl.Pl_Anim.PlayBodyAnim(inv.GetItemInDatabase(CurrentGunID).itemNames[0] + "Reload", 0.1f);

    }


    void QuiсkWeaponSwap()
    {
        if (pl.inv.showinvent || pl.inv.showjournal || pl.showMap) return;
        // print("QuiсkWeaponSwap 0 " + pl.IM.scrollWheel);


        Item.itemtype Type;

         Type = Item.itemtype.gun;

        Slot.bodypart Part = Slot.bodypart.Hand;
       
        if (Input.GetKeyDown(KeyCode.V))
        {

            for (int x = 0; x < BodySlotsUI.Slots.Length; x++)
            for (int y = 0; y < BodySlotsUI.Slots[x].Slot.Length; y++)
            if (BodySlotsUI.Slots[x].SlotScrips[y]._bodypart == Part)
            {
                Item Buffer = inv.DeepCopyItem(
                    BodySlotsUI.Slots[x].items[y].itemID,
                    BodySlotsUI.Slots[x].items[y].Count,
                    BodySlotsUI.Slots[x].items[y].Durability,
                    BodySlotsUI.Slots[x].items[y].AmmoInGun);

                inv.AddItemNOAUDIO(Buffer.itemID, Buffer.Count, Buffer.Durability, Buffer.AmmoInGun, new Vector2(9999, 9999));

                BodySlotsUI.Slots[x].items[y] = new Item();

                        SetGunItem(new Item());
                        SetGunID(new Item().itemID, new Item().Durability, new Item().AmmoInGun);

            }

            return;
        
        
        }
 


        if (pl.IM.scrollWheel > 0.05f || pl.IM.RightTrigger)
        {
            print("scrollWheel >0");
            bool choosenext = false;

            for (int i = 0; i < inv.inventory.Count; i++)
            {
                choosenext = true;
                if (GunInHandItem.itemID == -1) choosenext = true;
                if (choosenext)
                    ScrollWeaponBody(i, Type);
            }

        }


        if (pl.IM.scrollWheel < -0.05f || pl.IM.LeftTrigger)
        {
            print("scrollWheel < 0");
            bool choosenext = false;

            for (int i = inv.inventory.Count - 1; i > -1; i--)
            {
                choosenext = true;
                if (GunInHandItem.itemID == -1) choosenext = true;
                if (choosenext)
                    ScrollWeaponBody(i, Type);
            }
        }




    }


    void ScrollWeaponBody(int i, Item.itemtype Type)
    {
        if (GunQuickSwitchTimer > Time.fixedTime) return;
        if (inv.inventory[i].itemID <= -1) return;

        Item Buffer;
        Item ItemToAdd;
        Slot.bodypart Part = Slot.bodypart.Hand;



        if (inv.inventory[i]._itemtype == Type && CurrentGunID != pl.inv.inventory[i].itemID)
        {

            ItemToAdd = inv.DeepCopyItem(
         inv.inventory[i].itemID,
         inv.inventory[i].Count,
         inv.inventory[i].Durability,
           inv.inventory[i].AmmoInGun);


            for (int x = 0; x < BodySlotsUI.Slots.Length; x++)
                for (int y = 0; y < BodySlotsUI.Slots[x].Slot.Length; y++)
                    if (BodySlotsUI.Slots[x].SlotScrips[y]._bodypart == Part)
                    {
                        Buffer = inv.DeepCopyItem(
                            BodySlotsUI.Slots[x].items[y].itemID,
                            BodySlotsUI.Slots[x].items[y].Count,
                            BodySlotsUI.Slots[x].items[y].Durability,
                            BodySlotsUI.Slots[x].items[y].AmmoInGun);


                        BodySlotsUI.AddSlotItem(ItemToAdd.itemID, ItemToAdd.Durability, ItemToAdd.AmmoInGun, x, y);
                        print("to the body AmmoInGun" + ItemToAdd.AmmoInGun);
                        SetGunItem(ItemToAdd);
                        SetGunID(ItemToAdd.itemID, ItemToAdd.Durability, ItemToAdd.AmmoInGun);

                        // UpgradesUI.Slots[x].items[y] = pl.inv.DeepCopyItem(ItemToAdd.itemID, 1, ItemToAdd.Durability);

                        //  inv.UP.AddUpgradeItem(inv.inventory[i].itemID, inv.inventory[i].Durability, x, y);
                        BodySlotsUI.GetComponent<BodySlots>().AddSubtractStats(Buffer.itemID, -1);



                        if (Buffer.itemID > -1)
                            inv.AddItemNOAUDIO_NOPickedNames(Buffer.itemID,
                                Buffer.Count,
                                Buffer.Durability, Buffer.AmmoInGun, new Vector2(2000, 2000));

                      
                            BodySlotsUI.SetGunOnBodyPosition(x, y);
                            BodySlotsUI.GetGunOnBody().AmmoInGun = ItemToAdd.AmmoInGun;
                        

                        //inv.ReduceItemCount(inv.inventory[i].itemID, 1);
                        inv.inventory[i] = new Item();


                        GunQuickSwitchTimer = Time.fixedTime + 0.1f;
                    }

        }





    }


}
