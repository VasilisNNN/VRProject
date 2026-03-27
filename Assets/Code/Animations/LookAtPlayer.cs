using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtPlayer : MonoBehaviour
{
    private LookAtConstraint LookAt;
    void Start()
    {
        LookAt = GetComponent<LookAtConstraint>();
        ConstraintSource s = new ConstraintSource();
        s.sourceTransform = InitializeOnAwake.pl.transform;
        s.weight = 1;
        LookAt.AddSource(s);
    }

}
