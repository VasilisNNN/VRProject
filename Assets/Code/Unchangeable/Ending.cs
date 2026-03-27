using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    private Player pl;


    void Start()
    {
        pl = InitializeOnAwake.pl;
    }


    void Update()
    {
        if (pl.inv.CheckItem(101,1))
            SceneManager.LoadScene("Ending Good 0");
        else if (pl.inv.CheckItem(100, 1))
            SceneManager.LoadScene("Ending Bad 0");
        else
            SceneManager.LoadScene("Ending Good 0");
    }
}
