using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour {
	public List<Item> items = new List<Item>();

    void Awake()
    {
        items.Add(new Item(0, new string[2] { "Fuel", "Пальне" }, new string[2] { "Canister of Fuel", "Каністра з пальним" }));
        items[items.Count - 1].CanStack = true;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(1, new string[2] { "Food", "Їжа" }, new string[2] { "Pack of Food", "Пакунок з їжею" }));
        items.Add(new Item(2, new string[2] { "Transistor", "Транзистор" }, new string[2] { "Transistor for the Radio", "Транзистор для радіо" }));
        items.Add(new Item(3, new string[2] { "Lost cat", "Загублений кіт" }, new string[2] { "Lost cat", "Загублений кіт" }));
        items.Add(new Item(4, new string[2] { "Wood", "Деревина" }, new string[2] { "Wood", "Деревина" }));
        items.Add(new Item(5, new string[2] { "Musician case", "Чохол музиканта" }, new string[2] { "Musician case", "Чохол музиканта" }));
        items.Add(new Item(6, new string[2] { "Bread", "Хліб" }, new string[2] { "Bread", "Хліб" }));
        items.Add(new Item(7, new string[2] { "Seeds", "Насіння" }, new string[2] { "Seeds", "Насіння" }));



        items.Add(new Item(10, new string[2] { "Pistol ammo", "Патрони до пістолета" }, new string[2] { "Pistol ammo", "Патрони до пістолета" }));
        items[items.Count - 1].CanStack = true;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(11, new string[2] { "Shotgun ammo", "Патрони до дробовика" }, new string[2] { "Shotgun ammo", "Патрони до дробовика" }));
        items[items.Count - 1].CanStack = true;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(12, new string[2] { "Rifle ammo", "Патрони до гвинтівки" }, new string[2] { "Rifle ammo", "Патрони до гвинтівки" }));
        items[items.Count - 1].CanStack = true;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(15, new string[2] { "Cannon ammo", "Снаряди до гармати" }, new string[2] { "Cannon ammo", "Снаряди до гармати" }));
        items[items.Count - 1].CanStack = true;

        items.Add(new Item(20, new string[2] { "Corn", "Кукурудза" }, new string[2] { "Corn", "Свіжа кукурудза" }));
        items[items.Count - 1].CanStack = true;




        items.Add(new Item(99, new string[2] { "Knife", "Ніж" }, new string[] { "Knife", "Ніж" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Hand };
        items[items.Count - 1]._itemtype = Item.itemtype.gun;
        items[items.Count - 1]._Guntype = Item.Guntype.knife;


        items[items.Count - 1].ShootClips = new AudioClip[]
        {
            Resources.Load<AudioClip>("Sound/Hit/Swing_2")
        };

        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Military Weapon & Gun Sfx Pack/Handguns/Pistol/gun_one_shot_pistol_reload_handgun_glock_05");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Knife");

        items[items.Count - 1].DamageAmount = 2;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].GunSpread = 0.6f;



        items.Add(new Item(100, new string[] { "Pistol", "Пістолет" }, new string[] { "Pistol", "Пістолет" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Hand };
        items[items.Count - 1]._itemtype = Item.itemtype.gun;
        items[items.Count - 1]._Guntype = Item.Guntype.pistol;

        items[items.Count - 1].ShootClips = new AudioClip[]
        {
          Resources.Load<AudioClip>("Sound/Guns/Pistol/gun_one_shot_pistol_revolver_beefy_punchy_aggressive_3"),
          Resources.Load<AudioClip>("Sound/Guns/Pistol/gun_one_shot_pistol_revolver_beefy_punchy_aggressive_2"),
          Resources.Load<AudioClip>("Sound/Guns/Pistol/gun_one_shot_pistol_revolver_beefy_punchy_aggressive_1")
        };

        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Pistol");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/PistolEffect");

        items[items.Count - 1].AmmoID = 10;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].GunSpread = 0.6f;
        items[items.Count - 1].BulletsInShot = 1;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(101, new string[] { "Water Pistol", "Водяний пістолет" }, new string[] { "Water Pistol", "Водяний пістолет" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Hand };
        items[items.Count - 1]._itemtype = Item.itemtype.gun;
        items[items.Count - 1]._Guntype = Item.Guntype.pistol;

        items[items.Count - 1].ShootClips = new AudioClip[] {
        Resources.Load<AudioClip>("Sound/Ocean Game/Weapons_Toolkit/Ocean_Game_Weapons_Toolkit_Boom_10")
        };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/WaterPistol");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/WaterPistolEffect");

        items[items.Count - 1].AmmoID = 10;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].GunSpread = 0.6f;
        items[items.Count - 1].BulletsInShot = 1;
        items[items.Count - 1].CanBeDropped = false;

        items.Add(new Item(102, new string[] { "Rifle", "Гвинтівка" }, new string[] { "","" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Hand };
        items[items.Count - 1]._itemtype = Item.itemtype.gun;
        items[items.Count - 1]._Guntype = Item.Guntype.rifle;

        items[items.Count - 1].ShootClips = new AudioClip[] 
        { 
            Resources.Load<AudioClip>("Sound/Guns/Rifle_1") 
        };

        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Rifle");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/PistolEffect");

        items[items.Count - 1].AmmoID = 13;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 3;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].GunSpread = 1.2f;
        items[items.Count - 1].BulletsInShot = 1;
        items[items.Count - 1].CanBeDropped = false;


        items.Add(new Item(103, new string[] { "Shotgun","Дробовик" }, new string[] { "", "" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Hand };
        items[items.Count - 1]._itemtype = Item.itemtype.gun;
        items[items.Count - 1]._Guntype = Item.Guntype.shotgun;

        items[items.Count - 1].ShootClips = new AudioClip[] 
        { 
            Resources.Load<AudioClip>("Sound/Military Weapon & Gun Sfx Pack/Shotgun/Stock_Shots/designed_shotgun_beefy_punchy_aggressive_dry_distorted_01"),
            Resources.Load<AudioClip>("Sound/Military Weapon & Gun Sfx Pack/Shotgun/Stock_Shots/designed_shotgun_beefy_punchy_aggressive_dry_distorted_02"),
            Resources.Load<AudioClip>("Sound/Military Weapon & Gun Sfx Pack/Shotgun/Stock_Shots/designed_shotgun_beefy_punchy_aggressive_dry_distorted_03")
        };

        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Military Weapon & Gun Sfx Pack/Shotgun/Semi_Shotgun/designed_shotgun_semi_reloading_quick");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Shotgun");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/PistolEffect");

        items[items.Count - 1].AmmoID = 11;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 4;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].GunSpread = 1.2f;
        items[items.Count - 1].BulletsInShot = 1;
        items[items.Count - 1].CanBeDropped = false;


        items.Add(new Item(200, new string[] { "Car cannon","" }, new string[] { "", "" }));

        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Front };
        items[items.Count - 1]._itemtype = Item.itemtype.cargun;
        items[items.Count - 1]._Guntype = Item.Guntype.cannon;


        items[items.Count - 1].ShootClips = new AudioClip[] { Resources.Load<AudioClip>("Sound/Guns/Cannon_Shot") };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/TractorCannon");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/CannonEffect");

        items[items.Count - 1].AmmoID = 15;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].BulletsInShot = 1;

        items[items.Count - 1].GunSpread = 0;



        items.Add(new Item(201, new string[] { "Car cannon fat", "" }, new string[] { "", "" }));

        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Front };
        items[items.Count - 1]._itemtype = Item.itemtype.cargun;
        items[items.Count - 1]._Guntype = Item.Guntype.cannon;


        items[items.Count - 1].ShootClips = new AudioClip[] { Resources.Load<AudioClip>("Sound/Guns/Cannon_Shot")};
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/TractorCannonFat");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/CannonEffect");

        items[items.Count - 1].AmmoID = 15;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].BulletsInShot = 1;
        items[items.Count - 1].GunSpread = 0;


        items.Add(new Item(202, new string[] { "Car cannon spikes", "" }, new string[] { "", "" }));

        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Front };
        items[items.Count - 1]._itemtype = Item.itemtype.cargun;
        items[items.Count - 1]._Guntype = Item.Guntype.cannon;


        items[items.Count - 1].ShootClips = new AudioClip[] { Resources.Load<AudioClip>("Sound/Guns/Cannon_Shot") };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/TractorCannonSpikes");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/CannonEffect");

        items[items.Count - 1].AmmoID = 15;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].BulletsInShot = 3;
        items[items.Count - 1].GunSpread = 0;


        items.Add(new Item(300, new string[] { "Tractor mower", "" }, new string[] { "", "" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Melee };
        items[items.Count - 1]._itemtype = Item.itemtype.carmelee;

        items[items.Count - 1].ShootClips = new AudioClip[] { Resources.Load<AudioClip>("Sound/Guns/PistolShot") };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Tractor mower");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/PistolEffect");

        items[items.Count - 1].AmmoID = 1;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;



        items.Add(new Item(301, new string[] { "Tractor mower 2", "" }, new string[] { "", "" }));
        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Melee };
        items[items.Count - 1]._itemtype = Item.itemtype.carmelee;

        items[items.Count - 1].ShootClips = new AudioClip[] { Resources.Load<AudioClip>("Sound/Guns/PistolShot") };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/Tractor mower 2");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/PistolEffect");

        items[items.Count - 1].AmmoID = 1;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;



        items.Add(new Item(400, new string[] { "Car back cannon", "" }, new string[] { "", "" }));

        items[items.Count - 1].Gun = true;
        items[items.Count - 1]._bodypart = new Slot.bodypart[1] { Slot.bodypart.Back };
        items[items.Count - 1]._itemtype = Item.itemtype.cargun;
        items[items.Count - 1]._Guntype = Item.Guntype.cannon;

        items[items.Count - 1].ShootClips = new AudioClip[] {
        Resources.Load<AudioClip>("Sound/Guns/Cannon_Shot")
        };
        items[items.Count - 1].ReloadClip = Resources.Load<AudioClip>("Sound/Guns/Reload");
        items[items.Count - 1].EmptyShotClip = Resources.Load<AudioClip>("Sound/Guns/empty-gun-shot");

        items[items.Count - 1].PrefabObject = Resources.Load<GameObject>("Prefabs/Weapon/TractorCannon");
        items[items.Count - 1].EffectToCast = Resources.Load<GameObject>("Prefabs/Effects/CannonEffect");

        items[items.Count - 1].AmmoID = 15;
        items[items.Count - 1].AmmoInGun = 10;
        items[items.Count - 1].DamageAmount = 1;
        items[items.Count - 1].Durability = 99;
        items[items.Count - 1].BulletsInShot = 1;



        //-------------------------Quest--------------------------//

        items.Add(new Item(1000, new string[2] { "Secret key", "Секретний ключ" }, new string[2] { "Secret key", "Секретний ключ" }));
        items[items.Count - 1].CanStack = true;

        items.Add(new Item(1001, new string[2] { "Tools", "Інструменти" }, new string[2] { "Tools", "Інструменти" }));
        items[items.Count - 1].CanStack = true;

        items.Add(new Item(1002, new string[2] { "Car keys", "Ключі від машини" }, new string[2] { "Car keys", "Ключі від машини" }));
        items[items.Count - 1].CanStack = true;


    }


}
