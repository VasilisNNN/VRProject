using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTarget : MonoBehaviour
{
    public int HP = 1;
    private Player pl;
    void Start()
    {
        pl = InitializeOnAwake.pl;
    }

    void Update()
    {
        DeathManager();

    }

    void DeathManager()
    {
        if (HP > 0) return;
        
        Destroy(gameObject);
        pl.MoveExplosion(transform.position);
        

    }
}
