using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string Name;

    public int Damage;
    public float Attack_Delay;

 
    public float FireRate;
    public float Range;

    protected float NextFireTime;
    
    public InputMode IM { get; set; }
    public GrabObject Grab { get; set; }
    public Player pl { get; set; }
    public virtual bool CanFire()
    {
        return Time.time >= NextFireTime;
    }

    public virtual void Fire()
    {
        NextFireTime = Time.time + FireRate;
    }



    public virtual void Equip() { }

    public virtual void Unequip() { }

}
