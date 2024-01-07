using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using UnityEngine;

public class Moral_Attack : Moral
{
    public Moral_Attack()
    {
        overviewName = "moralAttack";
        positive = "Loves Attack";
        negative = "Hates Attack";

        setCase();
    }
        
    
    public override float auswerten(VolkObject o, SentimentType sentiment = SentimentType.Neutral) {
        int howlong = o.LastAttack;

        if(moral == 0) {
            if(howlong < 10) {
                return 1f-(float)howlong*0.1f;
            }else {
                return 0f;
            }
        }else if(moral == 2) {
            if(howlong < 10) {
                return (float)howlong*0.1f;
            }else {
                return 1f;
            }
        }

        return 0f;
    }
}
