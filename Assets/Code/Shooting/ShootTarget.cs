using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootTarget : MonoBehaviour
{
    public int HP = 1;
    private Player pl;
    public float RespawnDelay = 0;
    private float RespawnTimer;
    private MeshRenderer _Mesh;
    private BoxCollider _Box;
    private int StartHP;
    void Start()
    {
        StartHP = HP;
        pl = InitializeOnAwake.pl;
    }

    void Update()
    {
        Respawn();
        DeathManager();

    }

    void Respawn()
    {
        if (RespawnTimer < Time.fixedTime && 
            HP<=0 && 
            RespawnDelay>0)
        {
            HP = StartHP;
            _Mesh.enabled = true;
            _Box.enabled = true;
        }
    }




    void DeathManager()
    {
        if (HP > 0) return;

        if (RespawnDelay == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            RespawnTimer = Time.fixedTime + RespawnDelay;
            _Mesh.enabled = false;
            _Box.enabled = false;
        }


        pl.MoveExplosion(transform.position);
        

    }




}
