using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CollList))]
[RequireComponent(typeof(Rigidbody))]


public class CollideWithObjects : MonoBehaviour
{
    public GameObject[] ObjectsToCollideWith;
    public GameObject[] ObjectsToConnect;
    private GameObject ConnectedObject;
    private CollList Coll;
    public GameObject[] ResultObjects;

    private int[] Res;

    private Player pl;

    private MeshRenderer _MRenderer;

    private AudioSource AS;
    private AudioClip EndClip, SwitchClip;


    void Start()
    {
        Coll = GetComponent<CollList>();
        Res = new int[ObjectsToCollideWith.Length];
        pl = InitializeOnAwake.pl;

        _MRenderer = GetComponent<MeshRenderer>();


        AS = GetComponent<AudioSource>();

        EndClip = Resources.Load<AudioClip>("Sound/UI/Click");
        SwitchClip = Resources.Load<AudioClip>("Sound/UI/Typing_1");

    }

    void Update()
    {
        if (ConnectedObject == null)
        {

            if (_MRenderer != null) _MRenderer.enabled = true;
        }
        else
        {
            if (ConnectedObject.GetComponent<MeshRenderer>() != null)
            {
                if (ConnectedObject.GetComponent<MeshRenderer>().enabled)
                    if (_MRenderer != null) _MRenderer.enabled = false;


            }
            else
               if (_MRenderer != null) _MRenderer.enabled = false;

        }

        for (int i = 0; i < ObjectsToConnect.Length; i++)
        {
            if (Coll.GetCollList().Contains(ObjectsToConnect[i]))
            {
                if (ObjectsToConnect[i] != pl.HandObject)
                {
                    if (ConnectedObject == null)
                    {
                        if (ObjectsToConnect[i].GetComponent<Rigidbody>() != null)
                        {

                            ObjectsToConnect[i].GetComponent<Rigidbody>().isKinematic = true;
                        }

                        ConnectedObject = ObjectsToConnect[i];
                        PlaySoundForced(SwitchClip);

                        ObjectsToConnect[i].transform.SetPositionAndRotation(transform.position, transform.rotation);
                    }
                }
                else
                {



                    if (ObjectsToConnect[i] == ConnectedObject)
                    {
                        ConnectedObject = null;
                        PlaySoundForced(EndClip);
                    }
                }


            }
            else if (ObjectsToConnect[i] == ConnectedObject)
            {
                ConnectedObject = null;
                PlaySoundForced(EndClip);
            }

        }




        for (int i = 0; i < ObjectsToCollideWith.Length; i++)
        {
            if (Coll.GetCollList().Contains(ObjectsToCollideWith[i]))
            {
                for (int j = 0; j < ResultObjects.Length; j++)
                {
                    ResultObjects[j].SetActive(true);
             
                }

                Res[i] = 1;
               
            }else Res[i] = 0;
        }

        if (Res.Sum() <= 0)
        {



            for (int j = 0; j < ResultObjects.Length; j++)
            {
                ResultObjects[j].SetActive(false);

      
            }
        }
    }




    void PlaySound(AudioClip clip)
    {
        if (AS == null) return;
        if (AS.isPlaying) return;

        AS.clip = clip;
        AS.Play();
    }

    void PlaySoundForced(AudioClip clip)
    {
        if (AS == null) return;
        AS.clip = clip;
        AS.Play();
    }

    void StopSoundForced(AudioClip clip)
    {
        if (AS == null) return;
        if (!AS.isPlaying || AS.clip != clip) return;

        AS.Stop();
    }


}
