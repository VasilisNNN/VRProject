using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bullet
{

    public float Lifetime;
    public Vector3 Direction;
    public Vector3 StartPos;
    public Bullet()
    {
    }
}

public class EnemyWeapon : MonoBehaviour
{
    private Player pl;
    private Transform _transform;


    private float ShootDelay;
    public Attack _Attack;
    public GameObject BulletPrefab;

    public List<Bullet> Bullets = new List<Bullet>();
    public List<GameObject> BulletsObj = new List<GameObject>();
    public float bulletSpeed = 20f;
    public Transform GunTip;
    public int Damage = 1;

    private GameObject GunTipPos;
    public bool _Rotate = true;


    void Start()
    {
        _transform = transform;
        pl = InitializeOnAwake.pl;

        GunTipPos = Instantiate(new GameObject());
        for (int i = 0; i < 10; i++)
        {
            GameObject B = Instantiate(BulletPrefab);
            Bullets.Add(new Bullet ());
            BulletsObj.Add(B);
        }

        for (int i = 0; i < 10; i++)
        {
           // Bullets[i].Object.transform.position = new Vector3(0,0,-999);
         
        }

    }


    void Update()
    {
        if (_Attack == null) return;
        Rotate();
        Shoot();
      

        SetDamage();
    }

    private void FixedUpdate()
    {
        BulletsControll();
    }
    private void OnDisable()
    {
        for (int i = 0; i < BulletsObj.Count; i++)
        {
            if (BulletsObj[i] != null && GunTipPos!=null)
            {
                BulletsObj[i].GetComponent<MeshRenderer>().enabled = false;
                if (BulletsObj[i].GetComponent<TrailRenderer>() != null)
                    BulletsObj[i].GetComponent<TrailRenderer>().emitting = false;

                Bullets[i].StartPos = GunTipPos.transform.position;
                Bullets[i].Direction = Vector3.zero;
                BulletsObj[i].transform.position = GunTipPos.transform.position;
                Bullets[i].Lifetime = 0;
          
            }
        }

        ShootDelay = 0;
    }




    void Rotate()
    {
        if (!_Rotate) return;

        _transform.LookAt(pl._transform);
    }


    void Shoot()
    {
        ShootDelay -= Time.deltaTime;
 

        GunTipPos.transform.position = GunTip.position;


        for (int i = 0; i < BulletsObj.Count; i++)
        {
        
            if (ShootDelay <= 0 && _Attack.attacking)
            {
                if (Bullets[i].Lifetime <= 0)
                {
                    //print(name + " BULLET  " + BulletsObj[i].name);

                    Bullets[i].Direction = (pl._transform.position - GunTipPos.transform.position).normalized;
                    Bullets[i].StartPos = GunTipPos.transform.position;
                    BulletsObj[i].transform.position = GunTipPos.transform.position;
                    Bullets[i].Lifetime = 0.7f;

                    print("BulletsObj[i].Name " + BulletsObj[i].name);
                    ShootDelay = 0.5f;
                    return;
                }


            }




        }


    }

    void SetDamage()
    {
        for (int i = 0; i < BulletsObj.Count; i++)
        {
            if (pl.Legscoll_obj.Contains(BulletsObj[i]))
                pl.GetDamage(Damage);

        }
    }

    void BulletsControll()
    {
        for (int i = 0; i < BulletsObj.Count; i++)
        {
            Bullets[i].Lifetime -= Time.deltaTime;

            if (Bullets[i].Lifetime <= 0)
            {
               
                BulletsObj[i].transform.position = GunTipPos.transform.position;
                Bullets[i].StartPos = GunTipPos.transform.position;

                BulletsObj[i].GetComponent<MeshRenderer>().enabled = false;
                if (BulletsObj[i].GetComponent<TrailRenderer>() != null)
                    BulletsObj[i].GetComponent<TrailRenderer>().emitting = false;

                BulletsObj[i].transform.position = GunTipPos.transform.position;



            }
            else
            {
                BulletsObj[i].transform.position += 
                    Bullets[i].Direction * Time.deltaTime * bulletSpeed;

                BulletsObj[i].transform.LookAt(Bullets[i].Direction);

                BulletsObj[i].GetComponent<MeshRenderer>().enabled = true;
                if (BulletsObj[i].GetComponent<TrailRenderer>() != null)
                    BulletsObj[i].GetComponent<TrailRenderer>().emitting = true;

            }
        }
    }


    private void OnDestroy()
    {
        if (BulletsObj.Count > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                print(name + "_BulletsDestroy");
                Destroy(BulletsObj[i]);
            }
        }

    }



}
