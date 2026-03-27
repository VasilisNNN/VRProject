using UnityEngine;
using System.Collections;

[System.Serializable]

public class StringList
{
    public string[] line;
    public StringList(string[] l)
    {
        line = l;
    }
}

public class TextA
{
    public int ID;
    public string[] SourceText;

    public StringList[] line;

    public int PrefabsLine = -1;
    public string PrefabName;
    public string IconName;

    public bool PlayerTurn;
    public TextA(int id, bool playersturn, string iconName, StringList[] _line)
    {
        ID = id;
        line = _line;
        IconName = iconName;
        PlayerTurn = playersturn;

    }


}
