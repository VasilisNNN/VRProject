using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPLevel : MonoBehaviour
{

    private GameObject HeartSource;
    private List<GameObject> Hearts = new List<GameObject>();
    private Player pl;

    public bool HP = true;
    public bool Fuel = false;


    void Start()
    {
        pl = InitializeOnAwake.pl;

        if (HP)
        {
            HeartSource = Resources.Load<GameObject>("Prefabs/Objects/Heart");
        }

        if (Fuel)
        {
            HeartSource = Resources.Load<GameObject>("Prefabs/Objects/Fuel");
        }

        for (int i = 0; i < 15; i++)
        {
            
                Hearts.Add(Instantiate<GameObject>(HeartSource, transform));

            
        }
    }


    void Update()
    {

        if (HP)
        {
            HeartsControll(pl.HP,0.2f);
           
        }

       
    }


    void HeartsControll(int parameter, float distance)
    {
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (i < parameter)
                Hearts[i].transform.localPosition = new Vector3(distance * i, 0, 0);
            else Hearts[i].transform.localPosition = transform.position + new Vector3(99999, 99999, 99999);
        }
    }
}
