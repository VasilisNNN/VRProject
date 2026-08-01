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


    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    private GameObject tempFlash;
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] private float shotPower = 500f;

    [SerializeField] private float ejectPower = 150f;
    private GameObject tempCasing;
    public CollList ReloadColl;
    public int AmmoID = -1;
    private GameObject AmmoCase;
    public GameObject MagazineParent;


    public void Shoot()
    {
        if (muzzleFlashPrefab)
        {


            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);


            Destroy(tempFlash, destroyTimer);
        }

       

        if (!bulletPrefab)
        { return; }

        for (int i = 0; i < Bullets.Count; i++)
        {
            if (Bullets[i].activeInHierarchy)
            {
                if (Vector3.Distance(Bullets[i].transform.position, transform.position) > 15)
                    Bullets[i].SetActive(false);

            }

            if (!Bullets[i].activeInHierarchy)
            {
                CasingRelease();
                Bullets[i].SetActive(true);
                Bullets[i].GetComponent<Rigidbody>().position = barrelLocation.position;
                Bullets[i].GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
                return;
            }


        }




    }


    void CasingRelease()
    {

        if (!casingExitLocation || !casingPrefab)
        { return; }


        
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;

        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);

        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);


        Destroy(tempCasing, destroyTimer);
    }

    public void Reload()
    {
        if (ReloadColl == null) return;

        for (int i = 0; i < ReloadColl.coll_obj.Count; i++)
        {
            if (ReloadColl.coll_obj[i].GetComponent<GrabObject>().ID == AmmoID)
            {
                if (ReloadColl.coll_obj[i].GetComponent<GrabObject>().inHand)
                {
                    ReloadColl.coll_obj[i].GetComponent<Rigidbody>().isKinematic = true;
                    ReloadColl.coll_obj[i].transform.parent = MagazineParent.transform.parent;
                    ReloadColl.coll_obj[i].transform.localPosition = Vector3.zero;
                    AmmoCase = ReloadColl.coll_obj[i];
                    ReloadColl.coll_obj[i].GetComponent<GrabObject>().inHand = false;
                }

            }
        }



        if (IM.Reload)
        {
            AmmoCase.transform.SetParent(null);
            AmmoCase.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
