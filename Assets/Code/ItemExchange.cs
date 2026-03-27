using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExchange : MonoBehaviour
{

    public int ItemToGet;
    public int ItemToGetCount;

    public int NeedItem;
    public int NeedItemCount;


    private Inventory inv;
    private Player pl;

    private Outline _Outline;

    void Start()
    {
        _Outline = GetComponent<Outline>();
        _Outline.OutlineColor = new Color(0, 0, 0, 0);

        inv = InitializeOnAwake.pl.GetComponent<Inventory>();
        pl = InitializeOnAwake.pl;
    }

    
    void Update()
    {
        if (_Outline != null)
        {
            if (pl.ViewColl(gameObject))
            {
                _Outline.OutlineColor = new Color(1, 1, 1, 1);
            }
            else _Outline.OutlineColor = new Color(0, 0, 0, 0);

        }

        if (pl.ViewColl(gameObject) && pl.IM.enter_b)
        {
            if (inv.GetItem(NeedItem) != null)
            {
                if (inv.GetItem(NeedItem).Count >= NeedItemCount)
                {
                    inv.AddItem(ItemToGet, ItemToGetCount, 99, 0);
                    inv.ReduceItemCount(NeedItem, NeedItemCount);
                }

            }
            
        }


    }



}
