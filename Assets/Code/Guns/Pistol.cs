using System.Collections.Generic;
using UnityEngine;

public class Pistol : GeneralGun
{


 
    void Start()
    {
        Grab = GetComponent<GrabObject>();


        pl = InitializeOnAwake.pl;
        IM = InitializeOnAwake.IM;
        Shootray = transform.Find("Shootray").GetComponent<CollList>();


        MagazineSize = 30;
        CurrentAmmo = 30;
        ReserveAmmo = 120;


        if (barrelLocation == null)
            barrelLocation = transform;


        Bullets.Add(Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation));


        for (int i = 0; i < Bullets.Count; i++)
            Bullets[i].SetActive(false);

    }

    void Update()
    {
        Reload();
        FireManager();

    }


    void FireManager()
    {

        if (!IM.Fire && pl.RightHandObject == gameObject) return;
        if (!IM.FireLeft && pl.LeftHandObject == gameObject) return;

        if (!Grab.inHand) return;

        gunAnimator.SetTrigger("Fire");

        if (CurrentAmmo < bullets_In_Shot)
        {
        
            gunAnimator.SetTrigger("Empty");
            return;
        }



        Shoot();

        if (Shootray.rayhit == null) return;

        for (int i = 0; i < Shootray.rayhit.Count; i++)
        {
            if (Shootray.rayhit[i] == null) continue;

            if (Shootray.rayhit[i].GetComponent<ShootTarget>() != null)
            {

                Shootray.rayhit[i].GetComponent<ShootTarget>().HP = 0;
            }
        }

    }








}
