using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MovementVR : MonoBehaviour
{
    private InputMode IM;
    private Transform _transform;
    private Player pl;
    private Vector3 movepos;
    private NavMeshHit hit;

    private Vector3 forward ;
    private Vector3 right ;
    private Vector3 nextstep;

    void Start()
    {
        pl = InitializeOnAwake.pl;
        _transform = transform;
        IM = InitializeOnAwake.IM;

        movepos = Vector3.zero;
    }


    void Update()
    {
    
       
             Vector3 plpos = new Vector3(
            pl._transform.position.x,
         GetClosestNavMeshY(pl._transform.position) ,
              pl._transform.position.z);

       
            
       forward = pl.MainCamera.transform.forward;
       right = pl.MainCamera.transform.right;


        nextstep = forward * IM._vertical + right * IM._horizontal;
        
        Vector3 nextstepcorrected = new Vector3(
            Mathf.Clamp(nextstep.x, -3f, 3f), 0,
               Mathf.Clamp(nextstep.z, -3f, 3f)) ;

          if (NavMesh.SamplePosition(plpos + movepos + nextstepcorrected, out hit, 0.1f, NavMesh.AllAreas))

        if (!pl._Menu.MenuONOFF)
            movepos += nextstepcorrected  * Time.deltaTime * 3;


        if (IM.exit_b || IM.menu_b)
        {
            movepos = Vector3.zero;
            return;
        }


  
        
            _transform.position = plpos + movepos + new Vector3(0,0.1f,0);


        if (Mathf.Abs(IM._vertical) < 0.01f &&
      Mathf.Abs(IM._horizontal) < 0.01f &&
      movepos != Vector3.zero)
        {

          //  pl._transform.position += movepos;
            pl.GetComponent<Rigidbody>().position += movepos;
            movepos = Vector3.zero;
            return;
        }



    }

    public float GetClosestNavMeshY(Vector3 worldPos)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(worldPos, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position.y;
        }

        return worldPos.y; // fallback if no NavMesh found
    }


}
