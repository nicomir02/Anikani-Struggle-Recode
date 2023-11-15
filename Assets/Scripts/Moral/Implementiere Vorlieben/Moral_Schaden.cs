using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moral_Schaden : Moral
{
    public Moral_Schaden() {
        positive = "Loves damage"; //auf 100% nicht okay
        neutral = "Okay with damage"; // Neutral: Bis 50% okay
        negative = "Hates damage"; // 100% super, 50%=0

        setCase();
    }
        
    
    public override float auswerten(VolkObject o) {
        if(moral == 0) {
            if(o.health >= o.maxHealth) {
                return 0f;
            }else {
                return 1f;
            }
        }else if(moral == 1) {
            if(o.health >= o.maxHealth/2) {
                return 1f;
            }else {
                return (float)(o.health/(o.maxHealth/2));
            }
        }else if(moral == 2) {
            if(o.health >= o.maxHealth) return 1f;
            if(o.health-o.maxHealth*0.5f > 0) return ((o.health-o.maxHealth*0.5f)/o.maxHealth)*2;
            return 0f;
        }

        return 0f;
    }
}
