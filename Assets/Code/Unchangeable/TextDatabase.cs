using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TextDatabase : MonoBehaviour
{
    public List<TextA> textEN = new List<TextA>();
    public List<TextA> textUA = new List<TextA>();


    void Awake()
    {

        string red = "#FF5D5D";
        string green = "#9DFF99";
        string yellow = "#FFF224";


        textEN.Add(
new TextA(9, true, "Note",
new StringList[]{
               new StringList(new string[]{
            "To room scan."
            })

}
));



        textEN.Add(
new TextA(10, true, "Note",
new StringList[]{
               new StringList(new string[]{
            "To another shooting range."
            })

}
));



    }
}
