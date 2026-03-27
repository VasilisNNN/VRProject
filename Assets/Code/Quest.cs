using UnityEngine;
using System.Collections;

[System.Serializable]

public class Quest{
    public int ID;
    public string NAME;
    public string[] Description;
    public bool Started;
    public bool Done;
    public int QuestItemID;

    public Quest(int id, int quest_item_id, string name, string[] desription)
	{
        ID = id;
        QuestItemID = quest_item_id;
        NAME = name;
        Description = desription;
    

    }
}
