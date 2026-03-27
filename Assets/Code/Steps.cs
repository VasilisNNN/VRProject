using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Steps : MonoBehaviour
{
    public Transform playerTransform;
    public Terrain CurrentTerrain;
    private Player pl;
    public int posX;
    public int posZ;
    public float[] textureValues;
    public AudioClip[] clips;
    public AudioClip[] woodclips;
    public AudioClip[] concreteclips;
    public AudioClip[] carpetclips;
    public AudioClip[] snowclips;
    public AudioClip[] grassclips;
    public AudioClip[] metalclips;
    public AudioClip[] sandclips;
    public AudioClip[] waterclips;
    public AudioClip[] mudclips;

    public AudioSource[] AS;

    public float Delay = 0.5f;
    private float StepTimer;

    public List<ParticleSystem> WheelsSmoke;
    public bool InCarMode;
    private int CurrentAS;
 
    void Start()
    {
        pl = InitializeOnAwake.pl;
        if(GameObject.Find("MainTerrain")!=null)
        CurrentTerrain = GameObject.Find("MainTerrain").GetComponent<Terrain>();
      //  playerTransform = gameObject.transform;
        AS = new AudioSource[] { transform.Find("Left").GetComponent<AudioSource>(), transform.Find("Right").GetComponent<AudioSource>() };
    }

    void Update()
    {
        // For better performance, move this out of update 
        // and only call it when you need a footstep.

        if (pl.PlayerPause()) return;


        GetTerrainTexture();

        if (!pl.LockCamera) PlaySound();
    }

    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }

    private void PlaySound()
    {
        if (( pl.CutSceneMode) && !InCarMode)
        {
            AudioSourcePlay().Stop();
            return;
        }


        if (pl.IM._horizontal != 0 || pl.IM._vertical != 0)
        {
            CollisionSteps();
            TerrainSteps();
        }
        else
        {
            StopEffects();
          //  AudioSourcePlay().Stop();
        }


        if (StepTimer < Time.fixedTime)
        {
            AudioSourcePlay().Stop();
        }
        
       
    }

    void PlayEffects()
    {
       
        for (int i = 0; i < WheelsSmoke.Count; i++)
        {
            if (!WheelsSmoke[i].isPlaying)
                WheelsSmoke[i].Play();
        }
            

       
    }

    void StopEffects()
    {
        for (int i = 0; i < WheelsSmoke.Count; i++)
            WheelsSmoke[i].Stop();
    }


    void TerrainSteps()
    {
        if (StepTimer >= Time.fixedTime) return;

        for (int i = 0; i < textureValues.Length; i++)
        {
            if (textureValues[0] > 0)
            {
                PlayEffects();

                if (!AudioSourcePlay().isPlaying && StepTimer < Time.fixedTime)
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = grassclips[Random.Range(0, grassclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

            }

            if (textureValues[1] > 0)
            {
                StopEffects();
           
                if (!AudioSourcePlay().isPlaying && StepTimer < Time.fixedTime)
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = sandclips[Random.Range(0, sandclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

            }


            if (textureValues[2] > 0)
            {
                PlayEffects();
                // print("TextureTerrain" + i);
                if (!AudioSourcePlay().isPlaying && StepTimer < Time.fixedTime)
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = carpetclips[Random.Range(0, carpetclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

            }



            if (textureValues[3] > 0)
            {
                StopEffects();
                if (!AudioSourcePlay().isPlaying && StepTimer < Time.fixedTime)
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = sandclips[Random.Range(0, sandclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

            }
        }
    }
    void CollisionSteps()
    {


        if (GetComponent<CollList>() == null || pl.inv.showinvent || pl.inv.showjournal)
            return;

        for (int w = 0; w < GetComponent<CollList>().coll_obj.Count; w++)
        {
            if (GetComponent<CollList>().coll_obj[w] != null && StepTimer < Time.fixedTime)
            {
                if (GetComponent<CollList>().coll_obj[w].tag == "Carpet")
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = carpetclips[Random.Range(0, carpetclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Wood")
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = woodclips[Random.Range(0, woodclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Concrete")
                {
                    StopEffects();
                    SetCurrentAS();
                    AudioSourcePlay().clip = concreteclips[Random.Range(0, concreteclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }
                if (GetComponent<CollList>().coll_obj[w].tag == "Grass")
                {
                    PlayEffects();
                    SetCurrentAS();
                    AudioSourcePlay().clip = grassclips[Random.Range(0, grassclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Metal")
                {
                    PlayEffects();
                    SetCurrentAS();
                    AudioSourcePlay().clip = metalclips[Random.Range(0, metalclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Snow")
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = snowclips[Random.Range(0, snowclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Water")
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = waterclips[Random.Range(0, waterclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();
                   
                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

                if (GetComponent<CollList>().coll_obj[w].tag == "Mud")
                {
                    SetCurrentAS();
                    AudioSourcePlay().clip = mudclips[Random.Range(0, mudclips.Length)];
                    AudioSourcePlay().pitch = 1 + 0.2f * (float)Random.Range(-1, 1);
                    AudioSourcePlay().Play();

                    StepTimer = Time.fixedTime + Delay;
                    break;
                }

            } 

        }

        
    }


    void ConvertPosition(Vector3 playerPosition)
    {
        if (CurrentTerrain != null)
        {
            Vector3 terrainPosition = playerPosition - CurrentTerrain.transform.position;

            Vector3 mapPosition = new Vector3
            (terrainPosition.x / CurrentTerrain.terrainData.size.x, 0,
            terrainPosition.z / CurrentTerrain.terrainData.size.z);

            float xCoord = mapPosition.x * CurrentTerrain.terrainData.alphamapWidth;
            float zCoord = mapPosition.z * CurrentTerrain.terrainData.alphamapHeight;

            posX = (int)xCoord;
            posZ = (int)zCoord;
        }
    }

    void CheckTexture()
    {
        if (CurrentTerrain != null)
        {
            if (posZ < 0) return;

            float[,,] aMap = CurrentTerrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);
            textureValues[0] = aMap[0, 0, 0];
            textureValues[1] = aMap[0, 0, 1];
            textureValues[2] = aMap[0, 0, 2];
            textureValues[3] = aMap[0, 0, 3];
        }
    }

    void SetCurrentAS()
    {
        CurrentAS++;
        if (CurrentAS >= AS.Length) CurrentAS = 0;
    }
    AudioSource AudioSourcePlay()
    {
       
        return AS[CurrentAS];
    
    }


}
