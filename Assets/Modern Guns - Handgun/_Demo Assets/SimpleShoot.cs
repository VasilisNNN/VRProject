using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    public CollList Shootray;
    private InputMode IM;
    private GameObject tempFlash;

    public GrabObject Grab;
    public List<GameObject> Bullets = new List<GameObject>();


    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        IM = InitializeOnAwake.IM;
        Shootray = transform.Find("Shootray").GetComponent<CollList>();



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

        for (int i =0;i< Shootray.rayhit.Count;i++)
        {
            if (Shootray.rayhit[i] == null) continue;

            if (Shootray.rayhit[i].GetComponent<ShootTarget>() != null)
            {

                Shootray.rayhit[i].GetComponent<ShootTarget>().HP = 0;
            }
        }

    }



    

    void Shoot()
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
                if(Vector3.Distance(Bullets[i].transform.position, transform.position)>15)
                Bullets[i].SetActive(false);
              
            }

            if (!Bullets[i].activeInHierarchy)
            {
               
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


        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
      
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
      
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

   
        Destroy(tempCasing, destroyTimer);
    }

}
