using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moral_Heimweh : Moral
{
    public Moral_Heimweh() {
        positive = "Heimweh";
        negative = "Fernweh";

        setCase();
    }
        
    
    public override float auswerten(Unit u) {
        int howlong = u.WieLangNichtGebiet;

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
