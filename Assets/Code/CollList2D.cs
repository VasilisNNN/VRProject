using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollList2D : MonoBehaviour
{
    public List<GameObject> coll_obj = new List<GameObject>();

    private void OnTriggerStay2D(Collider2D c)
    {

        if (!coll_obj.Contains(c.gameObject))
        {
            coll_obj.Add(c.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D c)
    {

        if (coll_obj.Contains(c.gameObject))
        {
            coll_obj.Remove(c.gameObject);
        }

    }

    public List<GameObject> GetCollList()
    {
        return coll_obj;
    }
}
