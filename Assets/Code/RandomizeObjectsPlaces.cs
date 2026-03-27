using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomizeObjectsPlaces : MonoBehaviour
{
    private List<GameObject> Objects = new List<GameObject>();
    private Vector3[] Places;
    private Quaternion[] Rotations;

    private Menu _Menu;

    private void Awake()
    {
        _Menu = InitializeOnAwake._Menu;
    }


    void Start()
    {
        if (_Menu.CurrentSlotLocations[_Menu.CurrentSlotNumber] == SceneManager.GetActiveScene().name)
            return;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null)
            {

                Objects.Add(transform.GetChild(i).gameObject);
            }
        }

        Places = new Vector3[Objects.Count];
        Rotations  = new Quaternion[Objects.Count];

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i] != null)
            {
                Places[i] = Objects[i].transform.position;
                Rotations[i] = Objects[i].transform.rotation;
            }
        }

        int rndplace = Random.Range(0, Objects.Count);

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i] != null)
            {
                if (rndplace >= Objects.Count) rndplace = 0;

                Objects[i].transform.position = Places[rndplace];
                Objects[i].transform.rotation = Rotations[rndplace];
                rndplace++;
            }
        }


    }


}
