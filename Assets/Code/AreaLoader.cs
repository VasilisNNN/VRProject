using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaLoader : MonoBehaviour
{
    private Player pl;
    public float LoadDistance = 20;
    public float LoadDay = -1;
    public string SceneToLoad;
    public string SceneParrent;

    // Start is called before the first frame update
    void Start()
    {
        pl = InitializeOnAwake.pl;
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadDay > -1)
        {
            if (pl.SL.SaveLoadCurrent.DayNumber == LoadDay)
            {
                if (!SceneManager.GetSceneByName(SceneToLoad).isLoaded)
                    SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
            }
            else if (SceneManager.GetSceneByName(SceneToLoad).isLoaded)
                SceneManager.UnloadSceneAsync(SceneToLoad);
        }

        if (LoadDistance > -1)
        {
            if (Mathf.Abs((transform.position - pl._transform.position).magnitude) < LoadDistance)
            {
                if (!SceneManager.GetSceneByName(SceneToLoad).isLoaded)
                {
                    SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);

                }
                else GameObject.Find(SceneParrent).transform.position = transform.position;
            }
            else
            {
                if (SceneManager.GetSceneByName(SceneToLoad).isLoaded)
                    SceneManager.UnloadSceneAsync(SceneToLoad);

            }
        }
    }
}
