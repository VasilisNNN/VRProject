using UnityEngine;
using System.Collections.Generic;



public class GeneralGun : Weapon
{
    public int max_Ammo;
    public int bullets_In_Shot;

    public int MagazineSize = 30;
    public int CurrentAmmo = 30;
    public int ReserveAmmo = 120;


    public CollList Shootray;

    public List<GameObject> Bullets = new List<GameObject>();

    public Animator gunAnimator;

    public Transform barrelLocation;
    public Transform casingExitLocation;


  
 }
