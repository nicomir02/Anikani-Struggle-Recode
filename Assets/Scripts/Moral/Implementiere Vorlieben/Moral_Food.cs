using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moral_Food : Moral
{
    public Moral_Food() {
        
        overviewName = "Food";

        positive = "Less hungry";
        neutral = "Normal eating";
        negative = "Always hungry";

        setCase();
    }
        
    
    public override float auswerten(VolkObject o) {
        float result = 0f;
        
        if(moral == 0) { //Ab 0.5 volle Moral
            result = o.CurrentNahrung*2;
        }else if(moral == 1) { //Ab >0.5 etwas Moral, 1 volle Moral
            result = (o.CurrentNahrung-0.5f)*2;
        }else { //Ab 1 etwas Moral, 2 volle Moral
            result = (o.CurrentNahrung-1f)*2;
        }
        result = Mathf.Min(Mathf.Max(result, 0f), 1f);
        Debug.Log(result);
        return result;
    }
}