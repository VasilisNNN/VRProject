using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;



public class MouseController : MonoBehaviour
{
    

    public bool ObjectColl(GameObject Object)
    {
        Vector2 Mouth = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 Min = (Vector2)Object.GetComponent<BoxCollider2D>().bounds.min ;
        Vector2 Max = (Vector2)Object.GetComponent<BoxCollider2D>().bounds.max;

        if (Mouth.x > Min.x && Mouth.y > Min.y && Mouth.x < Max.x && Mouth.y < Max.y)
        {
            return true;

        }
        else return false;
    }


    public bool UIColl(GameObject Button)
    {
        Vector2 Mouth = Input.mousePosition;
        if (Button == null) return false;

        if (Button.GetComponent<BoxCollider2D>()==null) return false;

        Vector2 Min = (Vector2)Button.GetComponent<BoxCollider2D>().bounds.min -
            Button.GetComponent<RectTransform>().sizeDelta / 2;
        Vector2 Max = (Vector2)Button.GetComponent<BoxCollider2D>().bounds.max + Button.GetComponent<RectTransform>().sizeDelta / 2;

        if (Mouth.x > Min.x && Mouth.y > Min.y && Mouth.x < Max.x && Mouth.y < Max.y && Button.GetComponent<BoxCollider2D>().enabled && Button.activeInHierarchy)
        {
            return true;

        }
        else return false;
       
    }

    private void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameObject.Find("Canvas").transform.position.z);

    }
    void ForTests()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene("RideCutScene");

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene("Meds_Boss");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene("Detective_Board");

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SceneManager.LoadScene("Delivery_Boss");

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SceneManager.LoadScene("Map");

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SceneManager.LoadScene("IT_Boss");

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SceneManager.LoadScene("SampleScene");
    }

}
