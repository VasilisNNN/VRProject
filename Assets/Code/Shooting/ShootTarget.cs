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
        if (HP <= 0)
        {
            Destroy(gameObject);
            pl.MoveExplosion(transform.position);
        }

    }
}
