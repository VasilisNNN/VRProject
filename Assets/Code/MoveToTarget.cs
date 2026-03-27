using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : MonoBehaviour
{
    private NavMeshAgent Agent;
    public Transform Target;

    public Animator Anim;
    private string LasBodyAnim;

    private Player pl;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        pl = InitializeOnAwake.pl;
    }


    void Update()
    {
        if (pl._Menu.MenuONOFF || pl.inv.showinvent || pl.inv.showjournal)
        {
         
            Agent.speed = 0;
            PlayBodyAnim("Standing");
            return;
        }

        Agent.speed = 1;
        Agent.SetDestination(Target.position);

        PlayBodyAnim("Walking");
    }


    public void PlayBodyAnim(string animname)
    {
        if (LasBodyAnim == animname) return;
        Anim.CrossFade(animname, 0.07f);
        LasBodyAnim = animname;

    }
}
