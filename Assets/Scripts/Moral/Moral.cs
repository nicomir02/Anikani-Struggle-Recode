using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Moral
{
    public string positive = "";
    public string neutral = "";
    public string negative = "";

    protected int moral = -1;

    private static System.Random rand = new System.Random();

    public int setCase(int newCase=-1) {
        if(newCase != -1 && newCase >= 0 && newCase <= 2) {
            if(isValidCase(newCase)) {
                moral = newCase;
                return moral;
            }
        }

        List<int> cases = new List<int>();
        if(isValidCase(0)) cases.Add(0);
        if(isValidCase(1)) {
            cases.Add(1);
            cases.Add(1);
            cases.Add(1);
        }
        if(isValidCase(2)) cases.Add(2);

        moral = cases[rand.Next(cases.Count)];

        return cases[rand.Next(cases.Count)];
    }

    public override string ToString() {
        if(moral==0) return positive;
        if(moral==1) return neutral;
        if(moral==2) return negative;

        return "Not Selected";
    }

    public bool isValidCase(int newCase) {
        return (newCase == 0 && positive != null && positive != "") || (newCase == 1 && neutral != null && neutral != "") || (newCase == 2 && negative != null && negative != "");
    }

    public abstract float auswerten(VolkObject o);
}
