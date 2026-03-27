using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    private RectTransform RT;
    private RectTransform MainCanvas;
    private Player pl;

    public bool Play;
    void Start()
    {
        pl = InitializeOnAwake.pl;
        MainCanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        RT = GetComponent<RectTransform>();
    }

 
    void Update()
    {

        DialogAnimations();
    }

    void DialogAnimations()
    {
      
        float height = MainCanvas.rect.height;
        float width = MainCanvas.rect.width;

        if (Play) RT.localPosition = new Vector3(0,
            Mathf.Lerp(RT.localPosition.y, 0, Time.deltaTime * 10), 0);
        else RT.localPosition = new Vector3(0,
            Mathf.Lerp(RT.localPosition.y,- height , Time.deltaTime * 10), 0);
    }

}
