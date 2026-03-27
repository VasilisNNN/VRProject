using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int EnemyHealths = 100;

 void DeductPoints (int DamageAmount) {
  EnemyHealths -= DamageAmount;
 }

 void Update () {
  if (EnemyHealths <= 0) {
   Destroy(gameObject);
  }
 }
}
