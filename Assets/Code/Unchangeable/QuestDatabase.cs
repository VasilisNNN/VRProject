using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestDatabase : MonoBehaviour
{
    public List<Quest> QuestsEN = new List<Quest>();
    // public List<Quest> QuestsUA = new List<Quest>();

    void Awake()
    {


        QuestsEN.Add(new Quest(0, 0,
        "Visit uncle",
        new string[1]{
        "Visit uncle"
        }
        ));

        QuestsEN.Add(new Quest(1, 0,
        "Proceed to the house",
        new string[1]{
                "Proceed to the house"
        }
        ));

        QuestsEN.Add(new Quest(2, 0,
       "Ride the tractor",
        new string[1]{
                  "Ride the tractor"
        }
        ));

        QuestsEN.Add(new Quest(3, 0,
       "Collect crops",
       new string[1]{
        "Collect 15 corn"
       }
       ));


        QuestsEN.Add(new Quest(4, 0,
        "Run from the Teddy",
        new string[1]{
                    "Run from the Teddy"
        }
        ));

        QuestsEN.Add(new Quest(5, 0,
        "Practice shooting",
        new string[1]{
                        "Practice shooting"
        }
        ));

        QuestsEN.Add(new Quest(6, 0,
        "Investigate Teddy the tractor (Talk to the mayor)",
        new string[1]{
                          "Investigate Teddy the tractor (Talk to the mayor)"
        }
        ));




        //---------------------------------Vedana------------------------------//
        QuestsEN.Add(new Quest(20, 0,
                  "Bring food to Vedana",
                   new string[1]{
                  "Find food and bring it to Vedana"
                   }
                   ));

        //---------------------------------Bajan ------------------------------//
        QuestsEN.Add(new Quest(21, 0,
                  "Bring radio antenna to Bajan",
                   new string[1]{
                  "Find radio antenna and bring it to Bajan"
                   }
                   ));


        //---------------------------------Lucy ------------------------------//
        QuestsEN.Add(new Quest(22, 0,
                  "Bring lost cat to Lucy",
                   new string[1]{
                  "Find lost cat and bring it to Lucy"
                   }
                   ));

        //---------------------------------Lev ------------------------------//
        QuestsEN.Add(new Quest(23, 0,
                  "Bring wood to Lev",
                   new string[1]{
                  "Find wood and bring it to Lev"
                   }
                   ));

        //---------------------------------Orpheus ------------------------------//
        QuestsEN.Add(new Quest(24, 0,
                  "Bring musician case to Orpheus",
                   new string[1]{
                  "Find musician case and bring it to Orpheus"
                   }
                   ));

        //---------------------------------Yava ------------------------------//
        QuestsEN.Add(new Quest(25, 0,
                  "Bring bread to Yava",
                   new string[1]{
                  "Find bread and bring it to Yava"
                   }
                   ));

        //---------------------------------Finnegan------------------------------//
        QuestsEN.Add(new Quest(26, 0,
                  "Bring seeds to Finnegan",
                   new string[1]{
                  "Find seeds and bring it to Finnegan"
                   }
                   ));


        //-------------------------------Side Quests----------------------------------//


        //DAY0

        QuestsEN.Add(new Quest(99, 0,
      "Fight a tractorhead",
      new string[1]{
        "Fight a tractorhead."
      }
      ));

    



        //DAY1

        QuestsEN.Add(new Quest(100, 0,
      "Fight a camp of tracktorheads",
      new string[1]{
                          "Fight a camp of tracktorheads."
      }
      ));

        QuestsEN.Add(new Quest(101, 0,
     "Track a secret traktorhead",
     new string[1]{
                          "Track a secret traktorhead."
     }
     ));

        QuestsEN.Add(new Quest(102, 0,
    "Prevent a child to become a traktorhead",
    new string[1]{
                              "Prevent a child to become a traktorhead."
    }
    ));


        //DAY2


        QuestsEN.Add(new Quest(103, 0,
   "Visit the village mayor",
   new string[1]{
      "Visit the village mayor."
   }
   ));

        QuestsEN.Add(new Quest(104, 0,
 "Three safes",
 new string[1]{
      "Brake three safes Get papers inside and bring back. Of you didn’t bring them back you can use three land property papers later."
 }
 ));

        QuestsEN.Add(new Quest(105, 0,
"Barn blaze",
new string[1]{
      "Extinguish a fire that has started in the barn before it spreads."
}
));

        //DAY3



        QuestsEN.Add(new Quest(106, 0,
"Bandit Brawl",
new string[1]{
      "Defeat 10 bandits that have been raiding the village."
}
));

        QuestsEN.Add(new Quest(107, 0,
"Lost Child",
new string[1]{
      "Find and safely return a child who has wandered off into the forest."
}
));


        QuestsEN.Add(new Quest(108, 0,
"Wounded Animal",
new string[1]{
      "Find and heal a wounded animal in the forest."
}
));


        //DAY4


        QuestsEN.Add(new Quest(109, 0,
"Fence Fortification",
new string[1]{
      "Reinforce the village fences against incoming tractorheads."
}
));

        QuestsEN.Add(new Quest(110, 0,
"Rogue Tractorhead",
new string[1]{
      "Track down and disable a rogue tractorhead. Collect item on it."
}
));

        QuestsEN.Add(new Quest(111, 0,
"Village Dispute",
new string[1]{
      "Mediate a conflict between two villagers over stolen goods."
}
));






        //DAY5


        QuestsEN.Add(new Quest(112, 0,
"Herbal Remedy",
new string[1]{
      "Find and use a rare herb to create a remedy for a sick villager."
}
));

        QuestsEN.Add(new Quest(113, 0,
"New cannon",
new string[1]{
      "Help the blacksmith craft new tools to protect the village."
}
));

        QuestsEN.Add(new Quest(114, 0,
"Thieves",
new string[1]{
      "Catch a group of thieves breaking into city shops."
}
));



        //DAY6


        QuestsEN.Add(new Quest(115, 0,
"Cut grass",
new string[1]{
      "."
}
));

        QuestsEN.Add(new Quest(116, 0,
"Chop some trees",
new string[1]{
      "."
}
));

        QuestsEN.Add(new Quest(117, 0,
"Find wood doll in the forest",
new string[1]{
      "."
}
));













        //-------------------------------- FINAL ---------------------------------//

        QuestsEN.Add(new Quest(4000000, 0,
       "Fight a final hive mind boss",
        new string[1]{
               "Fight a final hive mind boss"
        }
        ));




    }
}