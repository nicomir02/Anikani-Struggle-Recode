using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moral_Food : Moral
{
    public Moral_Food() {
        
        overviewName = "Food";

        positive = "Always hungry";
        neutral = "Normal eating";
        negative = "Less hungry";

        setCase();
    }
        
    
    public override float auswerten(VolkObject o) {
        if(moral == 0) { //Ab 0.5 volle Moral
            return Mathf.Min(1f, o.CurrentNahrung*2);
        }else if(moral == 1) { //Ab >0.5 etwas Moral, 1 volle Moral
            return Mathf.Min(1f, (o.CurrentNahrung-0.5f)*2);
        }else { //Ab 1 etwas Moral, 2 volle Moral
            return Mathf.Min(1f, (o.CurrentNahrung-1f)*2);
        }
    }
}