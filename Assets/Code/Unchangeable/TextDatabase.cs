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
new TextA(9, true, "Mum",
new StringList[]{
               new StringList(new string[]{
            "There is a lot of joy here.",
            "I need to pick all joyful toys on the playground to go home early."
            })

}
));

        textEN.Add(
new TextA(10, true, "Mum",
new StringList[5]{
               new StringList(new string[]{
            "Mum, can I play outside? "
            }),
               new StringList(new string[]{
            "Have you finished all the homework?"
            }),
               new StringList(new string[]{
            "I did! "
            }),   
               new StringList(new string[]{
            "You can go then, but don’t make me call you back home."
            }),
              new StringList(new string[]{
            "I’ll be back at 9"
            })

}
));



        textEN.Add(
    new TextA(15, true, "MrBest",
    new StringList[]{
    new StringList(new string[]{
        "Hi, I never saw you here."
    }),
    new StringList(new string[]{
        "Hello, boy! This place is awesome, one of the few true relics of the past.",
        "You live here?"
    }),
    new StringList(new string[]{
        "I do!"
    }),
    new StringList(new string[]{
        "What is your name?"
    }),
    new StringList(new string[]{
        "Roman."
    }),
    new StringList(new string[]{
        "Nice to meet you Roman, I’m Best.",
        "Tell me, Roman. Where are your parents?"
    }),
    new StringList(new string[]{
        "Mum is at home…Again."
    }),
    new StringList(new string[]{
        "You seems sad, is everything ok with your mum?"
    }),
    new StringList(new string[]{
        "Not great, I guess…"
    }),
    new StringList(new string[]{
        "What happened?"
    }),
    new StringList(new string[]{
        "She lost her job and she is very sad.",
        "Like she is not going out much."
    }),
    new StringList(new string[]{
        "It does not matter how slowly you go as long as you do not stop. Maybe I have a great opportunity for her.",
        "What’s your apartment?"
    }),
    new StringList(new string[]{
        "Number seven."
    }),
    new StringList(new string[]{
        "I’ll talk to her. Meanwhile, take a candy!"
    }),
    new StringList(new string[]{
        "Candy! Thank you!"
    })
    }
    ));
        textEN.Add(
new TextA(30, true, "Gopnik",
new StringList[]{
               new StringList(new string[]{
            "Hello!"
            }),
               new StringList(new string[]{
            "Hey, kid. What you want?"
            }),
               new StringList(new string[]{
            "Is your friend ok?"
            }),
               new StringList(new string[]{
            "He is more than ok. He is in heavens. Now go away."
            })
}
));

        textEN.Add(
new TextA(31, true, "Gopnik",
new StringList[]{
               new StringList(new string[]{
            "..."
            }),
               new StringList(new string[]{
            "I told you. Go! "
            })
               
}
));

        textEN.Add(
       new TextA(32, true, "Gopnik",
       new StringList[]{
               new StringList(new string[]{
            "..."
            }),
               new StringList(new string[]{
            "..."
            })

       }
       ));


        textEN.Add(
      new TextA(33, false, "Gopnik",
      new StringList[]{
               new StringList(new string[]{
            "You again.",
            "Look at these toy cars, bro.",
            "There is something wrong with them.",
            "I was watching those cars for some time.",
            "I swear, bro, they juggle around. ",
            "Switch places, like grasshoppers.",
            "Not only cars, though, some other toys do this too.",
            "Crazy, right? "
            })

      }
      ));

        textEN.Add(
new TextA(34, false, "Gopnik",
new StringList[]{
               new StringList(new string[]{
            "Bro, you saw those lines around toy?",
            "That is weird.",
            "Every time I try to see them - I lose some joy.",
            "Not great, but if you lost something it helps.",
            "(Press F to show Item on the playground. You will pay some joy for that) "
            })

}
));

        textEN.Add(
new TextA(40, true, "Vova",
new StringList[]{
               new StringList(new string[]{
            "Hi!"
            }),
               new StringList(new string[]{
            "Hello, how are you? How is your mum?"
            }),
               new StringList(new string[]{
            "She is fine. Want me to study all day.",
            "But it is summer. There is not school."
            }),
               new StringList(new string[]{
            "Maybe she wants you to be the smartest boy!"
            }),
              new StringList(new string[]{
            "I think she is bored."
            }),
              new StringList(new string[]{
            "Did she manage to find a new job?"
            }),
              new StringList(new string[]{
            "No. Still nothing. It’s the forth month."
            }),
              new StringList(new string[]{
            "Hm… Did you have your dinner?"
            }),
              new StringList(new string[]{
            "Yeah, soup.",
            "Yak."
            }),
              new StringList(new string[]{
            "Soup is not that bad. Listen, if you promise me to be a good boy, I can bring you some candies tomorrow. "
            }),
              new StringList(new string[]{
            "Candies!",
            "I will be the best-behaved boy in the city, Mr. Malik!  "
            }),
              new StringList(new string[]{
            "Ha-ha. Good.",
            "Oh, one more thing. I have new domino at home, if you want, I can bring it tomorrow too."
            }),
              new StringList(new string[]{
            "Sure! Let’s play!"
            })
}
));


        textEN.Add(
      new TextA(60, true, "Gopnik",
      new StringList[]{
               new StringList(new string[]{
            "That clock. I have to collect all joy for me and mum before it gets too late."
            })

      }
      ));

        
        
        //----------------------------Level 1---------------------------//

        textEN.Add(
    new TextA(100, true, "Mum",
    new StringList[]{
               new StringList(new string[]{
            "Mum, I going to the playground!"
            }),
               new StringList(new string[]{
            "Have you finished all the homework?"
            }),
               new StringList(new string[]{
            "I did!"
            }),
               new StringList(new string[]{
            "Good. I saw something in there. "
            }),
               new StringList(new string[]{
            "New toys? "
            }),
               new StringList(new string[]{
            "Yeah. Like in pairs or in groups. Someone left them.",
            "Be careful."
            }),
               new StringList(new string[]{
            "Sure, mummy!"
            })
    }
    ));



        textEN.Add(
   new TextA(110, false, "MrBest",
   new StringList[]{
               new StringList(new string[]{
            "Yes, yes. See, this is the last plot of land and we can offer you really good compensation."
            }),
               new StringList(new string[]{
            "But I lived here my whole life."
            }),
               new StringList(new string[]{
            "I do understand that but did you consider to move to some more calm place, like near the magnificent blue lakes. "
            }),
               new StringList(new string[]{
            "Lakes are poisoned for past five years, because of the gold mine. No fish, no birds."
            }),
               new StringList(new string[]{
            "Well maybe you can consider another place. "
            }),
               new StringList(new string[]{
            "Your offer is nice, but I have to think about it. "
            }),
               new StringList(new string[]{
            "You can think but remember, once we make a deal with the most home owners the deal will be much worth. "
            }),
               new StringList(new string[]{
            "Sure, have a nice day. "
            })
   }
   ));



        textEN.Add(
 new TextA(120, true, "Vova",
 new StringList[]{
               new StringList(new string[]{
            "Hello! "
            }),
               new StringList(new string[]{
             "Oh, hi! Look at you, growing with each day! ",
             "How are you doing? "
            }),
               new StringList(new string[]{
            "Not bad, I fell this is going to be a great day!"
            }),
               new StringList(new string[]{
            "Did you heard about the Redface?",
            "I have a bad feeling about that beast.",
            "Just walking like that. You think we should destroy it? "
            }),
               new StringList(new string[]{
            "I don’t know. Mum said it is not hostile."
            }),
               new StringList(new string[]{
            "Maybe she is right.",
            "Do you want to play?"
            }),
               new StringList(new string[]{
            "I am going to win you over so hard!"
            })
 }
 ));

        //----------------------------Level 2---------------------------//

        textEN.Add(
new TextA(200, false, "Mum",
new StringList[]{
               new StringList(new string[]{
            "How is your homework? How is school?"
            }),
               new StringList(new string[]{
             "Peter pushed me today."
            }),
               new StringList(new string[]{
            "He what? How did that happen?"
            }),
               new StringList(new string[]{
            "Well, I called him stinky."
            }),
               new StringList(new string[]{
            "Why would you do that?"
            }),
               new StringList(new string[]{
            "I just don’t like him."
            }),
               new StringList(new string[]{
            "Honey, please don’t do that.",
            "I can’t deal…",
            "Just don’t push him first, ok? "
            }),
               new StringList(new string[]{
            "I’ll try.",
            "Can I go now?"
            }),
               new StringList(new string[]{
            "You can go. "
            })

        }
        ));

        textEN.Add(
new TextA(210, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hello, child!"
    }),
    new StringList(new string[]{
        "Hi…"
    }),
    new StringList(new string[]{
        "I have a little challenge for you.",
        "See those pedestals with toys on them.",
        "I want you to pick one of the toys, pay attention, some of them are better than other.",
        "You have to pick one."
    }),
    new StringList(new string[]{
        "Why are you here?"
    }),
    new StringList(new string[]{
        "What?"
    }),
    new StringList(new string[]{
        "Why are you here? You don’t live in our apartment block.",
        "Oh, I am just a simple entrepreneur, who want to make this place better.",
        "I am going to give you some new toys and candies.",
        "A lot of candies!"
    }),
    new StringList(new string[]{
        "Candies! I like candies."
    }),
    new StringList(new string[]{
        "Great, you are a very smart boy. Please, take a look at new toys I brought!"
    })
}
));
        //----------------------------Level 3---------------------------//

        textEN.Add(
new TextA(300, true, "Mum",
new StringList[]{
    new StringList(new string[]{
        "I’m going to play."
    }),
    new StringList(new string[]{
        "Ok. Did you talked to that new guy?",
        "He has a wonky name."
    }),
    new StringList(new string[]{
        "Best?",
        "Yeah. Umm…",
        "You think he is ok?"
    }),
    new StringList(new string[]{
        "He gave me candies."
    }),
    new StringList(new string[]{
        "You took them?",
        "I told you not to take anything from strangers."
    }),
    new StringList(new string[]{
        "He is a neighbor, so I thought that’s ok to take candies from him."
    }),
    new StringList(new string[]{
        "Well, if you see him tell that I want to chat with him."
    }),
    new StringList(new string[]{
        "Ok, mum!"
    })
}
        ));


        textEN.Add(
new TextA(320, true, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Hello!"
    }),
    new StringList(new string[]{
        "Oh. You out to play!",
        "Your mum is sad again?"
    }),
    new StringList(new string[]{
        "She is sad all the time since the dad is gone.",
        "I have to get her some joy."
    }),
    new StringList(new string[]{
        "I see…",
        "You know two days ago I saw a crow caring a walnut.",
        "The crow set on the fence, observing the road.",
        "Just before the red car passes - the crow flew and dropped the nut. ",
        "And what do you think?! ",
        "It cracked! The crow quickly dropped on the road, catch the nut and was gone.",
        "Even such small creature could figure out how to deal with that problem.",
        "Things will be fine.",
        "I always can help you if you need some joy.",
        "You can check my table any day."
    }),
    new StringList(new string[]{
        "You are very kind, thank you, Mr. Malik."
    }),
    new StringList(new string[]{
        "Just call me Vova. Go play now!"
    })
}
));



        //----------------------------Level 4---------------------------//


        textEN.Add(
new TextA(400, true, "Mum",
new StringList[]{
    new StringList(new string[]{
        "Mum?"
    }),
    new StringList(new string[]{
        "Yeah."
    }),
    new StringList(new string[]{
        "Are you ok?"
    }),
    new StringList(new string[]{
        "The joy.",
        "I need the joy.",
        "Can bring me some?"
    }),
    new StringList(new string[]{
        "I…",
        "I don’t know."
    }),
    new StringList(new string[]{
        "You play with toys right?",
        "You can get me some joy.",
        "I can use some of your joy.",
        "Please help me!"
    }),
    new StringList(new string[]{
        "Mumy, I’ll do all I can do."
    })
}
));


        textEN.Add(
new TextA(410, true, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hello."
    }),
    new StringList(new string[]{
        "Oh, you again!",
        "Have you seen your neighbor from 8th apartment?"
    }),
    new StringList(new string[]{
        "No."
    }),
    new StringList(new string[]{
        "I thing he is missing!",
        "Maybe you can find some clues where can he be."
    }),
    new StringList(new string[]{
        "Me?"
    }),
    new StringList(new string[]{
        "Well, you know him. I don’t."
    }),
    new StringList(new string[]{
        "Ok, I’ll do my best."
    })
}
));


        textEN.Add(
new TextA(420, false, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Where is your neighbor, Roman?"
    }),
    new StringList(new string[]{
        "OhI don’t know. Haven’t seen her today. "
    }),
    new StringList(new string[]{
        "Me neither.",
        "She used to say hello to me every morning.",
        "But I haven’t seen her for three days.",
        "If you happened to see Adelyn – tell me."
    })
}
));

        //----------------------------Level 5---------------------------//

        textEN.Add(
new TextA(510, true, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Why are you in the water?"
    }),
    new StringList(new string[]{
        "I was looking for my watch."
    }),
    
    new StringList(new string[]{
        "There is a huge one on the building."
    }),
    new StringList(new string[]{
        "Well… I still need my watch. ",
        "Did you hear the news?"
    }),
    new StringList(new string[]{
        "What news?"
    }),
    new StringList(new string[]{
        "Redface. He came back."
    }),
    new StringList(new string[]{
        "No way…"
    }),
    new StringList(new string[]{
        "Yeah. He was spotted on the Forest island and probably will be here tomorrow."
    }),
    new StringList(new string[]{
        "I hate that thing. When he looks at me its like the worst sadness I’ve experienced."
    }),
    new StringList(new string[]{
        "Be aware. I’m going on vacation for next three days."
    }),
    new StringList(new string[]{
        "Good for you."
    }),
    new StringList(new string[]{
        "Take care of yourself!",
        "I’ll leave you some candies across the playground."
    })
}
));

        textEN.Add(
new TextA(520, false, "Vova",
new StringList[]{
    new StringList(new string[]{
         "The sky!",
        "It is burning.",
        "But who burn it?",
        "Can he be among us?"
    })
}
));


        //-----------------------------Level 6------------------------------//

        textEN.Add(
new TextA(600, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "It is watching us!",
        "It is watching!",
        "Don’t let it look at you."
    }),
   new StringList(new string[]{
        "She is so depressed. I have to do something."
    }),
}
));




        textEN.Add(
new TextA(610, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hello, son, I need your help!",
        "I need to open the gate to the garage.",
        "Seems like there is some kind of riddle. Do you have any clue what it means?"
    }),
     new StringList(new string[]{
        "There are notes there. Can’t you read?"
    }),

      new StringList(new string[]{
        "I know, but if you want to leave early, you need to help me.",
        "Oh, and be careful around cars. ",
        "There are some drunks under the cars.",
        "Probably failed mechanics."
    }),
     new StringList(new string[]{
        "Sure."
    })
}
));

        textEN.Add(
new TextA(611, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "I am going to give away three-year food supply to three winners of my contest. Here are the rules:",
        "Fist: you have to be at least twenty years old",
        "Second: You have to live in Lunovka district",
        "And the final you have to be ready to spend thirty days on the top of a water tower and been filmed. ",
        "If you are ready for this challenge call +30551242311!",
        "You can do better and Mr. Best can help you!"
    })
}
));




        textEN.Add(
new TextA(620, false, "Vova",
new StringList[]{
    new StringList(new string[]{
        "You know, I used to work at the factory.",
        "I was an engineer.",
        "We used to make satellites.",
        "But then the space agency just closed the program.",
        "“We don’t need space”, - they said. In my face. “It was always a stupid dream.”",
        "And then they just sold everything to this soda company.",
        "Now they launch commercials into orbit.",
        "What a knife in the back."
    })
}
));



        textEN.Add(
new TextA(630, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "They hide under the cars. Be careful!"
    })
}
));

        //-----------------------------Level 7------------------------------//


        textEN.Add(
new TextA(700, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "Listen!"
    }),
    new StringList(new string[]{
        "I don’t hear anything…"
    }),
    new StringList(new string[]{
        "The boxes are ticking, someone made them dangerous. "
    })


}));


        textEN.Add(
new TextA(710, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "I have some great news!",
        "This will make you so happy!",
        "I move in!",
        "We live together now!",
        "Are you happy?"
    }),
    new StringList(new string[]{
        "..."
    }),
    new StringList(new string[]{
        "I know you are happy deep inside, despite your sad face.",
        "So. We need to carry my stuff.",
        "Those carriers brought a lot of boxes, but I am not sure which are mine. ",
        "You need to sort them out and bring the correct boxes to the house.",
        "I think there was some kind of list, but I lost it."
        
    })


}));


        textEN.Add(
new TextA(711, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Those boxes are a mess.",
        "Our first point is that weird Mr. Best. Is that a real name?",
        "Anyway, I marked his boxes with a circle sticker."
    })


}));




        textEN.Add(
new TextA(720, false, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Oh, hi kid!"
    }),
    new StringList(new string[]{
        "Hello!"
    }),
}
));


        //-----------------------------Level 8------------------------------//


        textEN.Add(
new TextA(800, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "..."
    })


}));


        textEN.Add(
new TextA(810, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "This day is the best day in our beautiful lives!",
        "Are you ready, Roman? ",
        "I made the biggest candy in the world! ",
        "Look at this! ",
        "Exciting!",
        "But I lost the key to our sweet tender treat!",
        "Come on. Roman!",
        "Open that door!"
    })

}));

        textEN.Add(
new TextA(820, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Something clean"
    })


}));

        textEN.Add(
new TextA(821, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Something for your closet."
    })


}));

        textEN.Add(
new TextA(822, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Something to smell nice."
    })


}));

        textEN.Add(
new TextA(823, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Something sharp."
    })


}));

        textEN.Add(
new TextA(824, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Something to be clean."
    })


}));

        //-----------------------------Level 9------------------------------//


        textEN.Add(
new TextA(900, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "..."
    })


}));

        textEN.Add(
new TextA(910, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Oh, look at you! So big!",
        "You grown up while I was absent.",
        "So many thing happened.",
        "How have you been?"
        
    }),
     new StringList(new string[]{
        "Helping you all the time."
    }),
      new StringList(new string[]{
        "Well, been busy is useful for a young lad, like you!",
        "Listen, Roman. You want to earn real money?",
        "Help me and I’ll reward you!"
    }),

      new StringList(new string[]{
        "..."
    }),

       new StringList(new string[]{
        "I think Redface changed something here. See those gates?",
        "They reveal parts of the playground.",
        "Every time I stepped into that thing something new appear.",
        "Maybe you will be able to rub your head around this nonsense."
    })


}));
        textEN.Add(
new TextA(920, false, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Oh, hi kid!"
    }),
    new StringList(new string[]{
        "Hello!"
    }),
     new StringList(new string[]{
        "Someone burned the theater yesterday.",
        "Your mum used to work there."
    })
}));



        textEN.Add(
new TextA(940, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Coffee!",
        "Some of them are lying!",
        "Find those with peace inside."
    })


}));




        //-----------------------------Level 10------------------------------//

        textEN.Add(
new TextA(1010, true, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hi! Mum wants to talk to you."
    }),
    new StringList(new string[]{
        "Thank you, Roman! "
    })

}));

        textEN.Add(
new TextA(1030, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "There were two lovers.",
        "They were so in love that they could not be apart even for a day.",
        "But one day the shadow looked into their house.",
        "They start to argue every day. ",
        "The scene was in the kitchen.",
        "She grabbed a plate while he grabbed a pot.",
        "Objects were flying.",
        "The shadow was satisfied, but she realized she could not understand who was who now. ",
        "Pick the girl doll!"
    })
}
));

        textEN.Add(
      new TextA(1035, false, "Note",
      new StringList[]{
    new StringList(new string[]{
        "One day the girl understood she had a child.",
        "Lovers found a common ground and peace.",
        "Young couple need money, so the boy went to play in a local theater. ",
        "The job was hard, but not the worst",
        "One day he got a good role",
        "But wat the role was? I can’t remember.",
        "Was it a lord or a servant?"
    })
      }
      ));


        textEN.Add(
   new TextA(1040, false, "Note",
   new StringList[]{
    new StringList(new string[]{
        "The work at the theater was hard.",
        "The boy become a man.",
        "The girl gave birth to a smart son. ",
        "They were happy.",
        "But with maturity comes sadness.",
        "He start to drink.",
        "Heavily. ",
        "One day the boy went into the pub and he met his old friend.",
        "That friend had dark eyes and a black as night hair. ",
        "Which one if the friend? "
    })
   }
   ));


        textEN.Add(
   new TextA(1045, false, "Note",
   new StringList[]{
    new StringList(new string[]{
        "The man and the friend talk about their lives.",
        "The job at the theater was not enough to rase a child.",
        "So, the friend told the man that the city if growing and he can pay for a job. ",
        "The man need to drop a bag in the pocket of a rival. ",
        "The rival was a criminal just like the friend.",
        "He wear a black jacket and good-looking trousers while he goes to the gym every Sunday.",
        "Where is the rival?"
    })
   }
   ));




        //-----------------------------Level 11------------------------------//


        textEN.Add(
new TextA(1110, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hey, kid. Those addicts they put this trap.",
        "I think my leg is broken. ",
        "I was going to show them where they belong.",
        "Now I’m here. ",
        "If only you can help me. "
        
    }),
      new StringList(new string[]{
        "I can call for help. "
    }),
      new StringList(new string[]{
        "No. Redface is watching.",
        "Look you need to help me to finish a small business.",
        "We cannot let them take over our playground.",
        "I saw sewer hatches got loose.",
        "You need to lead those addicts onto the hatches. Don’t be wary, you are too light to drop down. ",
        "The rest is my problem."
    }),
      new StringList(new string[]{
        "You want me lead them on the carousel? "
    }),
      new StringList(new string[]{
        "Yes, that is what I need.",
        "Please!"
    })
}
));

        textEN.Add(
new TextA(1120, true, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Hi!"
    }),
     new StringList(new string[]{
        "Hello! That face. Horrifying.",
        "Why did it walk here.",
        "We haven’t seen this creature for years.",
        "I hope its going to be gone soon."
    })

}
));


        textEN.Add(
new TextA(1130, false, "Gopnik",
new StringList[]{
    new StringList(new string[]{
        "Oh. You again.",
        "My brothers are not themselves anymore.",
        "Someone has been drugging them, and now they are mindless addicts.",
        "Be careful!",
        "Once I stepped through that gate, Fedir appeared.",
        "Almost got me.",
        "Seems like they pop out when you go through that thing. To what time would you prefer to set the call?"
    })
}
));



        //-----------------------------Level 12------------------------------//

        textEN.Add(
new TextA(1210, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hello, Roman!",
        "Congratulations!",
        "It is your birthday today! ",
        "I brought you a lot of different presents!",
        "I hid some of them on the playground.",
        "But first you need to learn something. ",
        "Small but important life lesson! ",
        "You need to put necessary thing on the concrete block. ",
        "You do this – I open the gates and let you into the real deal!"
    })
}
));



        textEN.Add(
new TextA(1240, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "Tom got his first job and now it is time for dramatic realization. He have to manage his finances and pick only Necessary things. ",
        "You have to help him and collect those things and put them on the pedestal."
    })
}
));


        //-----------------------------Level 13------------------------------//
        textEN.Add(
new TextA(1300, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "Roman.",
        "I am getting married tomorrow."
    }),
    new StringList(new string[]{
        "What does it mean? "
    }),
    new StringList(new string[]{
        "We will be a new family. "
    }),
    new StringList(new string[]{
        "Don’t you love dad? "
    }),
    new StringList(new string[]{
        "He is gone. Forever.",
        "I cannot live in greave all the time."
    }),
    new StringList(new string[]{
        "I understand."
    })
}
));


        textEN.Add(
         new TextA(1320, false, "Vova",
         new StringList[]{
            new StringList(new string[]{
                "They are getting married. Aren’t they?"
            }),
            new StringList(new string[]{
                "Mum and Best? "
            }),
            new StringList(new string[]{
                "Yes, that Best and your mum. I think he want to getting rid of us to build a monstrosity on the top of our lovely house. "
            }),
             new StringList(new string[]{
                "It is al though."
            }),
             new StringList(new string[]{
                "It is classic. We need to preserve things. We can not demolish everything every five years."
            }),
              new StringList(new string[]{
                "I don’t know. "
            }),
               new StringList(new string[]{
                "All right, Roman.",
                "Go help your mum."
            }),
         }
         ));






        textEN.Add(
   new TextA(1330, false, "Note",
   new StringList[]{
    new StringList(new string[]{
        "One stool is danger, and another is humiliation. What would you sit on and what would you set your mum on?"
    })
   }
   ));

        textEN.Add(
   new TextA(1340, false, "Note",
   new StringList[]{
    new StringList(new string[]{
        "The paper about the murder in the theater. It is very old.",
        "Blood spilled on the scene! Great tragedy happened in the theater when young actress shoots a prompt gun, which happened to be a real weapon of murder.",
        "The bullet got just near the heart of her husband. He died the same moment.",
        "Young actress was crushed. The police department investigates this tragedy.",
        "Some say the gun was intentionally replaced, but who wanted to kill the poor actor?"
    })
   }
   ));



        //-----------------------------Level 14------------------------------//

        textEN.Add(
new TextA(1400, false, "Mum",
new StringList[]{
    new StringList(new string[]{
        "Roman, this is very important day for me!",
        "I need you to de good and do what I say.",
        "Can we do that?"
    }),
    new StringList(new string[]{
        "Sure.",
        "You really like that guy so much? "
    }),
      new StringList(new string[]{
        "I do.",
        "Besides he is kind and well put.",
        "I am not getting younger. This is a good person for both of us.",
        ""
    }),
        new StringList(new string[]{
        "I get it. "
    }),
           new StringList(new string[]{
        "I need a little help today. ",
        "Small tasks. "
    }),
              new StringList(new string[]{
        "I’ll help you. "
    })

}
));


        textEN.Add(
new TextA(1420, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "One doll is hungry and the other is not. Which one should you feed?"
    })
}
));

        textEN.Add(
new TextA(1421, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "One doll is sad and one if not. Which one is sad?"
    })
}
));
        textEN.Add(
new TextA(1422, false, "Note",
new StringList[]{
    new StringList(new string[]{
        "One doll is rich and one if not. Which one is poor?"
    })
}
));

        textEN.Add(
      new TextA(1423, false, "Note",
      new StringList[]{
    new StringList(new string[]{
        "One doll set in the kitchen and another set on the balcony. Which was in the kitchen?"
    })
      }
      ));


        textEN.Add(
    new TextA(1424, false, "Note",
    new StringList[]{
    new StringList(new string[]{
        "One doll was silent but another screamed at some point."
    })
    }
    ));







        //-----------------------------Level 15------------------------------//



        textEN.Add(
  new TextA(1510, false, "MrBest",
  new StringList[]{
    new StringList(new string[]{
        "This is the saddest day we experienced for the past years.",
        "Volodymyr was the beam of the light for me, he gave every person here reason, cheers and hope.",
        "He tough me how to play domino and read me his poem every evening.",
        "Such a beautiful person.",
        "But even the greatest people have flows",
        "His flow was alcohol.",
        "Yesterday he was found dead here on this playground and today we gather to say our final words top him. ",
        "One by one people from our lovely house disappeared and now the most important member of our society passed away.",
        "What a tragedy.",
        "Let’s pray for his soul."
    })
  }
  ));


        textEN.Add(
 new TextA(1520, false, "Person",
 new StringList[]{
    new StringList(new string[]{
        "Hello!"
    }),

      new StringList(new string[]{
        "Hi! You want some candy? "
    }),

        new StringList(new string[]{
        "I don’t think I like it anymore. "
    }),

          new StringList(new string[]{
        "Anyway, can you please collect all good candies from the table? "
    }),

            new StringList(new string[]{
        "Probably. "
    }),

 }
 ));





        //-----------------------------Level 16------------------------------//




        textEN.Add(
 new TextA(1600, true, "Mum",
 new StringList[]{
    new StringList(new string[]{
        "Mum!"
    }),
     new StringList(new string[]{
        "Hello, Roman.",
        "Come. We have a new home now!"
    }),
     new StringList(new string[]{
        "Don’t you think this Best man is evil?"
    }),
     new StringList(new string[]{
        "He cares about us. See. The new apartment."
    }),
     new StringList(new string[]{
        "But he did something to all the people around.",
        "Since he came everything is weird.",
        "The homes, the sky.",
        "Vova is dead!"
    }),
    new StringList(new string[]{
        "He was an old gentleman. It happens to old people. They pass away.",
        "Nothing else changed much accept the new houses built."
    }),
    new StringList(new string[]{
        "I don’t know.",
        "I don’t trust him."
    }),

     new StringList(new string[]{
        "I understand, but he is a good person. I am sure you will get a common ground.",
        "He likes you and he want you two to be friends.",
        "Can you do that. Can you accept Best?"
    }),
    new StringList(new string[]{
        "I don’t know.",
        "Maybe yes, maybe not."
    })

 }
 ));


        textEN.Add(
new TextA(1610, false, "MrBest",
new StringList[]{
    new StringList(new string[]{
        "Hello, son!",
        "This is it. Our new home.",
        "We were working together on this grand project.",
        "You helped me a lot!",
        "Now we can live here together.",
        "Me. Your mum and you! ",
        "Are you happy? "
    }),

      new StringList(new string[]{
        "You destroyed our home?!"
    }),

        new StringList(new string[]{
        "What home? That old rotting house.",
        "It is more than forty years old.",
        "Now we have a beautiful house."
    }),

      new StringList(new string[]{
        "It is ugly.",
        "I want my home!"
    }),

     new StringList(new string[]{
        "Listen, this is our new time with the new family!",
        "Please, come home! "
    })

}
));

        textEN.Add(
new TextA(1620, true, "Vova",
new StringList[]{
    new StringList(new string[]{
        "You alive?",
        "I tricked everyone.",
        "We need to kill this bustard. He destroyed our home and he will destroy the rest.",
        "Take this and put into his coffee."
    })
}
));

        textEN.Add(
new TextA(1700, true, "Vova",
new StringList[]{
    new StringList(new string[]{
        "Mr Best is not with us.",
        "Skyscrapers were demolished.",
        "I like it this way. ",
         "Me and my mum, collecting joy, living the regular life. ",
        "Noone else. ",
         "No destructions. ",
        "Everything is calm and quite",
        "I will bring you more jot, my dear mum!"
    })
}
));

        textEN.Add(
        new TextA(1710, true, "Vova",
        new StringList[]{
    new StringList(new string[]{
        "We live in a new house. ",
        "It is big.",
        "Best is not that bad. He buys me toys and candies.",
        "Mum is enlightened from inside, but she pays me so little attention.",
        "Rarely looks at me. ",
        "At least she is happy. ",
        "I spend most of the time outside or at school.",
        "I’m thinking about majoring in theater, like my dad."
    })
        }
        ));



        textEN.Add(
        new TextA(1720, true, "Vova",
        new StringList[]{
    new StringList(new string[]{
        "I don’t what to deal with that Best and Mum and everything.",
        "Its too much.",
        "I decided to visit my aunt.",
        "She lives in country site.",
        "Look how beautiful it is.",
        "I am definitely not coming back."
    })
        }
        ));












    }
}
