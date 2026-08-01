using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : GeneralGun
{


    void Start()
    {
        Grab = GetComponent<GrabObject>();

     
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
        FireManager();

    }


    void FireManager()
    {

        if (!IM.Fire) return;
        print("FIRE " + name);

        if (!Grab.inHand) return;
        print("Grab " + name);

        gunAnimator.SetTrigger("Fire");
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
