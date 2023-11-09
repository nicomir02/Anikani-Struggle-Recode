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
        if(isValidCase(1)) cases.Add(1);
        if(isValidCase(2)) cases.Add(2);

        return cases[rand.Next(cases.Count)];
    }

    public bool isValidCase(int newCase) {
        return (newCase == 0 && positive != null && positive != "") || (newCase == 1 && neutral != null && neutral != "") || (newCase == 2 && negative != null && negative != "");
    }

    public abstract float auswerten(Unit u);
}
