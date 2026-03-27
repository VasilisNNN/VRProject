using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.AI;


public class Enemies : MonoBehaviour
{
    public GameObject[] EnemyObjects;
    private float timer;
    public float DelayBetweenEnemies = 10;
    public float DelayBetweenWaves = 10;
    public float DelayStart = -1;


    public Transform[] SpawnPositions;

    public GameObject Effect;
    [HideInInspector]
    public List<GameObject> BuildedPers = new List<GameObject>();

    

    private int EnemiesInWaveCount;
    public int EnemiesInWaveCountMAX = 10;
  
    
    private int WaveCount;
    public int WaveCountMAX = 3;

    
    private Player pl;

    public DayAndNight.DayCycle Day_Cycle = DayAndNight.DayCycle.Day;
    public bool NotInTheMorning = false;
    

    private StatsControll stats;
    private SpriteRenderer SPRT;
    private Animator Anim;
    void Start()
    {
        Anim = GetComponent<Animator>();
        stats = GetComponent<StatsControll>();
        SPRT = GetComponent<SpriteRenderer>();

        EnemiesInWaveCount = 0;

        pl = InitializeOnAwake.pl;
        if (DelayStart < 0) DelayStart = DelayBetweenWaves;

        timer = DelayStart;

        //EnemyTimer = GameObject.Find("EnemyTimer");

            /*  for (int i = 0; i < _constr.OBOnBoard.Count; i++)
              {
                  if (_constr.OBOnBoard[i].Name.Contains(EnemyObjects[0].name))
                  BuildedPers.Add(_constr.OBOnBoard[i].Object);

              }*/


    }

    

    void Update()
    {
        if (pl.StartLoading) return;

        timer -= Time.deltaTime * pl.Game_SPEED;

        AnimationAndColor();

        if (pl.DayNight == null)
        {
            CreateEnemy_Main();

            CheckEnemyNull();
            return;
        }


        if (NotInTheMorning && pl.DayNight.Day_Cycle == DayAndNight.DayCycle.Morning) return;

 

        if (pl.Game_SPEED > 0 && (pl.DayNight.Day_Cycle == Day_Cycle || Day_Cycle == DayAndNight.DayCycle.AllTime))
        {
          
            CreateEnemy_Main();

            CheckEnemyNull();
        }

    }


    void AnimationAndColor()
    {
        float col = 1 + (timer) / DelayBetweenEnemies * -1;

        if (SPRT != null)
            SPRT.color = new Color(col, col, col, 1);

        if (pl.DayNight == null) return;
        if (NotInTheMorning && pl.DayNight.Day_Cycle == DayAndNight.DayCycle.Morning) return;

   
    }



    void CreateEnemy_Main()
    {
      

  
        if (timer >= 0) return;

        if (transform.parent == pl.gameObject) return;
        
        if (BuildedPers.Count >= EnemiesInWaveCountMAX * WaveCountMAX) return;
       
        if (EnemiesInWaveCount < EnemiesInWaveCountMAX)
        {
 
            CreateEnemy();
            
        }
        else
        {
            
            WaveCount++;


            EnemiesInWaveCount = 0;
            timer = DelayBetweenWaves;


        }
        
    }



    void CheckEnemyNull()
    {
        for (int i = 0; i < BuildedPers.Count; i++)
        {
            if (BuildedPers[i] == null)
            {
                BuildedPers.RemoveAt(i);
                EnemiesInWaveCount--;

                // BuildedPersCount--;
            }
        }
    }

    void CreateEnemy()
    {
        int num = Random.Range(0, EnemyObjects.Length);

        
      /*  if (stats != null)
        {
            if (stats.DurabilityMax > -1)
                stats.Durability--;
        }
        */


        GameObject Enemy = Instantiate<GameObject>(EnemyObjects[num]);

        if(Anim!=null)
        Anim.Play("Start");


        BuildedPers.Add(Enemy);
        Enemy.name = EnemyObjects[num].name + BuildedPers.Count + Random.Range(0.1f, 100.1f) + Random.Range(0.1f, 100.1f) + Random.Range(0.1f, 100.1f) + Random.Range(0.1f, 100.1f);

    //    Enemy.GetComponent<MoveBetweenSpots>().waypoints = new Transform[1] { pl._transform };

        StatsControll Enemy_Stats = Enemy.GetComponent<StatsControll>();

        if (Enemy_Stats != null)
        {
      
            Enemy_Stats.SpawnPointName = name;

        }

        Vector3 EnemyPos = transform.position;
      


        if (SpawnPositions.Length == 0)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
                EnemyPos = hit.position;

        }
        else
        {
            int n = Random.Range(0, SpawnPositions.Length);
         
            

            EnemyPos = new Vector3(SpawnPositions[n].position.x,
                SpawnPositions[n].position.y, SpawnPositions[n].position.z);


              
        }


        Enemy.GetComponent<NavMeshAgent>().Warp(EnemyPos);
        Enemy.GetComponent<NavMeshAgent>().nextPosition = EnemyPos;
    
        if (Effect != null)
        {
            GameObject EffectOb = Instantiate<GameObject>(Effect);
            EffectOb.transform.position = Enemy.transform.position;
        }
       

        EnemiesInWaveCount++;

        timer = DelayBetweenEnemies;
        
    }
}
