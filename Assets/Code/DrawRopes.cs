using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrawRopes : MonoBehaviour
{
 
   // public bool BGObject;

    private Player pl;
    private Transform _transform;
   
    public bool ShowRopes;
    public float linewidth = 0.1f;
    public int segmentLength = 30;

    public List<GameObject> Ropes = new List<GameObject>(); 
    
    public List<GameObject> ActiveteObjects = new List<GameObject>();

    void Start()
    {
      
        if (InitializeOnAwake.pl != null)
        {
            pl = InitializeOnAwake.pl;
     
        }

        _transform = transform;



      //  if (Detailes && DescriptionID == -1) Detailes = false;
    }
    private void OnDrawGizmos()
    {
        // if (NeedItem > -1) Gizmos.DrawIcon (transform.position, "Need Item" + NeedItem,true,new Color(1,1,1,1));
        if (_transform == null) _transform = transform;

        if (ActiveteObjects != null)
        {
            if (ActiveteObjects.Count > 0)
            {
                for (int i = 0; i < ActiveteObjects.Count; i++)
                {
                   

                    if (ActiveteObjects[i] != null)
                        Gizmos.DrawLine(_transform.position, ActiveteObjects[i].transform.position);
                }
            }
        }
   

       

    }
    // Update is called once per fr
    void Update()
    {

      

            if (ShowRopes)
            {
            
                if (GameObject.Find(gameObject.GetInstanceID() + "Rope") == null)
                {
                
                    for (int i = 0; i < ActiveteObjects.Count; i++)
                    {
                    if (ActiveteObjects[i] != null)
                    {
                       
                                Ropes.Add(Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Rope")));
                                Ropes[Ropes.Count - 1].name = gameObject.GetInstanceID() + "Rope";

                                Ropes[Ropes.Count - 1].GetComponent<Rope>().lineWidth = linewidth;

                               

                                Ropes[Ropes.Count - 1].GetComponent<Rope>().StartEnd = _transform;
                                Ropes[Ropes.Count - 1].GetComponent<Rope>().FinishEnd = ActiveteObjects[i].transform;
                                Ropes[Ropes.Count - 1].GetComponent<Rope>().segmentLength = segmentLength;

                        



                    }
                    }
                        

                 
                }
            }
            
    }

    public void OnOffObject(GameObject uiel, bool tf, float Alpha)
    {
        if (uiel != null)
        {
           

          
            if (uiel.GetComponent<BoxCollider>() != null)
            {
                uiel.GetComponent<BoxCollider>().enabled = tf;
            }

         
          
        }
    }

   
}
