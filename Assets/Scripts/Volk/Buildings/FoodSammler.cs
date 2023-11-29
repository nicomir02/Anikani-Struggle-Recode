using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSammler : RessourcenSammler
{
    // Start is called before the first frame update
    int howmany = -1;

    public override float howMany() {
        if(howmany == -1) fieldAusrechnen();
        return howmany;
    }

    public void fieldAusrechnen() {
        List<Vector3Int> vecs = radiusNeighborCheck.getNeighborTiles(middlePoint, 7);
        int waterCount = 1;
        foreach(Vector3Int vec in vecs) {
            Vector3Int v = vec;
            v.z = mb.getZ(vec);
            if(mb.getBiom(v) != null && mb.getBiom(v).GoodForField) waterCount++; 
        }

        howmany = waterCount;
    }
    public override void nextRound() {
        base.nextRound();
        currentNahrung = 0.5f;
    }
}
