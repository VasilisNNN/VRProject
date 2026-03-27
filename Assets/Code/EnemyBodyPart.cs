using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    public StatsControll _ParentStart;
    public Slot.bodypart Bodypart;

    private void Start()
    {
        _ParentStart.Colliders.Add(gameObject);
    }
}
