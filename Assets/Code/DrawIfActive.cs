using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class DrawIfActive : MonoBehaviour
{
    public GameObject[] SPRTs;
    public GameObject[] NotActiveSPRTs;
    public bool ActionOnOneOf;
    private int[] Numbs;
    private int[] NoActiveNumbs;

    public bool TargetBool;

    public int QuestID = -1;
    public int DoneQuestID = -1;
    private Player pl;

    public bool _destroy;
    public bool OnlyAnimation;
    
    private bool Draw = true;
    public bool DoOnes = false;

    private bool Done;
    public float ShakeCamera = 0;
    public bool PlayAudio = true;

    private bool TF = true;

    public bool StartOff;
    void Start()
    {
        pl = InitializeOnAwake.pl;
     
        Numbs = new int[SPRTs.Length];

        if(NotActiveSPRTs!=null)
        NoActiveNumbs = new int[NotActiveSPRTs.Length];

        if(StartOff)
            OnOffObject(gameObject, false);
        TF = true;

    }
    private void OnDrawGizmos()
    {
        if (SPRTs != null)
        {
            for (int i = 0; i < SPRTs.Length; i++)
            {
                Gizmos.color = new Color(0.2f, 1, 0.2f);
                Gizmos.DrawLine(transform.position, SPRTs[i].transform.position);
              
            }
        }
    }
        // Update is called once per frame
    void Update()
    {
     
        if (Done) return;

        if (QuestID > -1)
        {
            if(!pl.inv.CheckQuestDone(QuestID))
            return;

            if (SPRTs.Length == 0 && !Done)
            {
                if (_destroy) Destroy(gameObject);
                OnOffObject(gameObject, TargetBool);

                Done = true;
                
            }



        }


        if (SPRTs != null)
        {
            for (int i = 0; i < SPRTs.Length; i++)
            {
                if (SPRTs[i].GetComponent<MeshRenderer>().enabled) Numbs[i] = 1;
                else Numbs[i] = 0;

              
            }
        }


        if (NotActiveSPRTs != null)
        {
            for (int i = 0; i < NotActiveSPRTs.Length; i++)
            {
                if (NotActiveSPRTs[i] != null)
                {
                    if (NotActiveSPRTs[i].activeInHierarchy)
                    {
                        if(!ActionOnOneOf)
                        NoActiveNumbs[i] = 0;
                    }
                    else
                    {
                        if (ActionOnOneOf)
                        {
                            for (int j = 0; j < NotActiveSPRTs.Length; j++) NoActiveNumbs[j] = 1;
                        }
                        else NoActiveNumbs[i] = 1;
                    }
                }
                else
                {
                    if (ActionOnOneOf)
                    {
                        for (int j = 0; j < NotActiveSPRTs.Length; j++) NoActiveNumbs[j] = 1;
                    }
                    else NoActiveNumbs[i] = 1;
                }
                
            }


           
        }



        if (Numbs.Length > 0)
        {
            if (Numbs.Sum() >= Numbs.Length)
            {
                if (TF)
                {
                    if (ShakeCamera > 0) pl.SetCamShakeTimer(ShakeCamera, 1f);

                    if (DoOnes) Done = true;

                    OnOffObject(gameObject, TargetBool);
                    TF = false;
                }
            }
            else if (!TF)
            {
                OnOffObject(gameObject, !TargetBool);
                TF = true;
            }
        }


        if (NoActiveNumbs.Length > 0)
        {

            if (NoActiveNumbs.Sum() == NoActiveNumbs.Length)
            {
                if (DoneQuestID > -1) pl.inv.DoneQuest(DoneQuestID);
                if (ShakeCamera > 0) pl.SetCamShakeTimer(ShakeCamera, 1f);

                if (_destroy)
                {
                    if (GetComponent<DamageHeal>() != null)
                    {
                        JoyCheck.RemoveJoyObject(gameObject);
                    }

                    Destroy(gameObject);
                }
                if (DoOnes) Done = true;
              
                    OnOffObject(gameObject, TargetBool);
                    TF = false;
                
            }
            else
            {
                if (!TF)
                {
                    OnOffObject(gameObject, !TargetBool);
                    TF = true;
                }
            }
        }
        
    }








    private void OnOffObject(GameObject uiel, bool tf)
    {
        if (_destroy) return;
        
        if (uiel == null) return;

        TurnComponentsONOFF(uiel.gameObject, tf);

        ToggleThroughChild(uiel.transform, tf);


        
    }


    void ToggleThroughChild(Transform parent, bool TF)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            TurnComponentsONOFF(child.gameObject, TF);
            ToggleThroughChild(child, TF);
        }

    }




    void TurnComponentsONOFF(GameObject uiel, bool tf)
    {

        if (uiel.GetComponent<Animator>() != null && !tf)
        {
            uiel.GetComponent<Animator>().Play("Back", 0);
        }


        if (uiel.GetComponent<Animator>() != null && tf)
        {
            uiel.GetComponent<Animator>().Play("Main", 0);
        }

        if (OnlyAnimation) return;



        if (uiel.GetComponent<StatsControll>() != null)
        {
            uiel.GetComponent<StatsControll>().enabled = tf;
        }


        if (uiel.GetComponent<Attack>() != null)
        {
            uiel.GetComponent<Attack>().enabled = tf;
        }

        if (uiel.GetComponent<MoveBetweenSpots>() != null)
        {
            uiel.GetComponent<MoveBetweenSpots>().enabled = tf;
        }
        if (uiel.GetComponent<NavMeshAgent>() != null)
        {
            uiel.GetComponent<NavMeshAgent>().enabled = tf;
        }

        if (uiel.GetComponent<Door>() != null)
        {
            uiel.GetComponent<Door>().enabled = tf;
        }


        if (uiel.GetComponent<AudioSource>() != null && tf && PlayAudio)
        {
            uiel.GetComponent<AudioSource>().Play();
        }

        if (uiel.GetComponent<Enemy_Spawner>() != null)
        {
            uiel.GetComponent<Enemy_Spawner>().enabled = tf;
        }

        if (uiel.GetComponent<TilemapRenderer>() != null)
        {
            uiel.GetComponent<TilemapRenderer>().enabled = tf;
        }

        if (uiel.GetComponent<MeshRenderer>() != null)
        {
            uiel.GetComponent<MeshRenderer>().enabled = tf;
        }

        if (uiel.GetComponent<SkinnedMeshRenderer>() != null)
        {
            uiel.GetComponent<SkinnedMeshRenderer>().enabled = tf;
        }



        if (uiel.GetComponent<Image>() != null)
        {
            uiel.GetComponent<Image>().enabled = tf;


            for (int i = 0; i < uiel.transform.childCount; i++)
            {

                if (uiel.transform.GetChild(i).GetComponent<Image>() != null)
                {
                    uiel.transform.GetChild(i).GetComponent<Image>().enabled = tf;
                }
            }
        }
        

        if (uiel.GetComponent<SpriteRenderer>() != null)
        {
            uiel.GetComponent<SpriteRenderer>().enabled = tf;
        }
        if (uiel.GetComponent<BoxCollider2D>() != null)
        {
            uiel.GetComponent<BoxCollider2D>().enabled = tf;
        }

        if (uiel.GetComponent<BoxCollider>() != null)
        {
            uiel.GetComponent<BoxCollider>().enabled = tf;
        }

        if (uiel.GetComponent<PolygonCollider2D>() != null)
        {
            uiel.GetComponent<PolygonCollider2D>().enabled = tf;
        }

        if (uiel.GetComponent<Dialog>() != null)
        {
            uiel.GetComponent<Dialog>().enabled = tf;
        }
        if (uiel.GetComponent<Light>() != null)
        {
            uiel.GetComponent<Light>().enabled = tf;
        }
    }





}
