using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]

public class Item {
    public string[] itemNames;
    public string[] itemDesc;
    public int itemID =-1;
    

    public enum Soundtype { regular, sword, club, axe};
    public Soundtype _Soundtype;


    public enum itemtype { item, gun, melee, cargun, carmelee, clothes };
    public itemtype _itemtype;

    public enum Guntype {  pistol, shotgun, rifle, cannon, knife};
    public Guntype _Guntype;

    public Slot.bodypart[] _bodypart;

    
    public int Count;

    public int DamageAmount;
    public int BulletDamageAmount;
    public int MaxHP = 0;
    public int HP = 0;
    public int Intellect = 0;
    public int Speed;
    public int DashDuration;
    public int Vision;

    public int Stamina;
    public int MaxStamina = 0;
    public int StaminaUse;
    public int StaminaRecoverySpeed;

    public int BulletsInShot;

    public string MagicObjectToCast;
    public GameObject EffectToCast;

    public int MagicDamage;
    public int FireDamage;
    public int IceDamage;
    public int MechanicDamage;

    public int MagicDefense;
    public int NatureDefense;
    public int FireDefense;

    public bool CanStack;
    public bool Gun;
    public int Durability = 99;

    public GameObject PrefabObject;

    public int AmmoID = -1;
    public int AmmoInGun;
    public int MagCapacity;

    public AudioClip[] ShootClips;
    public AudioClip ReloadClip;
    public AudioClip EmptyShotClip;

    public float GunSpread = 0;

    public bool CanBeDropped;

    public int Satiety;
    public int DamageBuff;
    public GameObject MagicEffectToCast;


    public int Cost;
    public bool Food;
    public bool CanNOTBeRemovedFromTheBody;



    public int AddSlots;
    public int Protection;

    public Item(int id, string[] itemnames,  string[] itemdesc)
    {
        itemNames = itemnames;
        itemID = id;
        itemDesc = itemdesc;
    }

 
   
    
    public Item()
	{
		itemID = -1;
        
    }
    
}
