using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
   

    public GameObject EnemySpawnPoint;

    

}

public class Enemy_Spawner : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject EnemyDeathPoint;
    public SpawnPoint[] EnemySpawnPoints;
    public AudioSource[] AS;

    private float Timer;
    public float Delay;
    private GameObject Spawned;
    private MoveBetweenSpots SpawnerMove;
    private Attack SpawnerAttack;

    private float AttackTimer;
    public float AttackDuration = 60;
    public float StartDelay = 60;

    private Player pl;
    private int spawnID = 0;

    private bool spawned;
    public bool RetreatIfHPIsLow;

    public bool DisableOnEnemyBack { get; set; }
    void Start()
    {
        Timer = StartDelay;
        pl = InitializeOnAwake.pl;
      
    }

    void Spawn()
    {
       
        if (Timer >= 0 || Spawned != null) return;
        

        Spawned = Instantiate(Enemy, EnemySpawnPoints[spawnID].EnemySpawnPoint.transform.position, Enemy.transform.rotation);
        SpawnerMove = Spawned.GetComponent<MoveBetweenSpots>();
        SpawnerAttack = Spawned.GetComponent<Attack>();
        SpawnerMove.waypoints = new Transform[1] { pl.transform };
        Spawned.GetComponent<Attack>().Target = pl.transform;

    

        AttackTimer = AttackDuration;
        Timer =  Delay + AttackDuration;
        spawned = true;
    }

    void PlayAudio( int spawnpointID)
    {
        if (Timer <= 0 ) return;
        if (AttackTimer <= 0) return;

        float FirstAudio = 3;
        float SecondAudio = 2;
        float ThirdAudio = 0.4f;

 
        if (Timer  <  FirstAudio)
        {
            if (!AS[0].isPlaying)
                AS[0].Play();
        }

        if (Timer < SecondAudio)
        {
            if (!AS[1].isPlaying)
                AS[1].Play();
            
        }

        if (Timer < ThirdAudio)
        {
            if (!AS[2].isPlaying)
                 AS[2].Play();
            
        }
        

    }

    void StopAudio(int spawnpointID)
    {
        print("StopAudio " + "Spawn ID " + spawnID);

        for (int i = 0; i < AS.Length; i++)
        {
            if (AS[i].isPlaying)
                AS[i].Stop();
        }
    }


    void SpawnEnd()
    {



        if (Spawned == null)
        {
            return;
        }

        if (RetreatIfHPIsLow)
        {
            if (Spawned.GetComponent<StatsControll>().HP < 8)
            {
                Spawned.GetComponent<StatsControll>().InvisTimer = 99;
                AttackTimer = -1;
                
            }
        }

        if (AttackTimer >= 0)
        return;


        SendEnemiesBack();

        if (Mathf.Abs(Spawned.transform.position.x - pl.transform.position.x) > 30 && Mathf.Abs(Spawned.transform.position.z - pl.transform.position.z) > 30)
            Spawned.transform.position = EnemyDeathPoint.transform.position;


        if (Mathf.Abs(Spawned.transform.position.x - EnemyDeathPoint.transform.position.x) < 4 && Mathf.Abs(Spawned.transform.position.z - EnemyDeathPoint.transform.position.z) < 4)
        {
            if (DisableOnEnemyBack) gameObject.SetActive(false);
            Spawned.GetComponent<StatsControll>().HP = 0;
          
        }
        

    }

    void SetspawnID()
    {
        float prevmag = 999999;
        for (int i = 0; i < EnemySpawnPoints.Length; i++)
        {
            if (Mathf.Abs((EnemySpawnPoints[i].EnemySpawnPoint.transform.position - pl.transform.position).magnitude) < prevmag)
            {
                spawnID = i;
                prevmag = Mathf.Abs((EnemySpawnPoints[i].EnemySpawnPoint.transform.position - pl.transform.position).magnitude);

            }
        }

        for (int i = 0; i < AS.Length; i++)
        {
            if(!spawned)
            AS[i].transform.position = EnemySpawnPoints[spawnID].EnemySpawnPoint.transform.position;
        }

    }


    void Update()
    {
        Timer -= Time.deltaTime;
        AttackTimer-= Time.deltaTime;

        SetspawnID();

        Spawn();

        PlayAudio(spawnID);

        SpawnEnd();

        if (spawned && Spawned == null)
        {
            StopAudio(spawnID);
            spawned = false;
        }
    }


    public void SendEnemiesBack()
    {

        if (Spawned == null)
        {
            return;
        }

        SpawnerMove.Attacking = false;
        SpawnerAttack.enabled = false; 
        StopAudio(spawnID);
        SpawnerMove.waypoints = new Transform[1] { EnemyDeathPoint.transform };
        SpawnerMove.speed = 80;
        AttackTimer = -1;


    }




}
