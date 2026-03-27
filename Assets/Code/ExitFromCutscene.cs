using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitFromCutscene : MonoBehaviour
{
    public bool Exit;
    public string Location;
   
    void Update()
    {
        if (Exit)
        {
            SceneManager.LoadScene(Location);

        }
    }
}
