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
        List<Vector3Int> vecs = radiusNeighborCheck.getNeighborTiles(middlePoint, 5);
        int waterCount = 1;
        foreach(Vector3Int vec in vecs) {
            Debug.Log(mb.getBiom(vec));
            if(mb.getBiom(vec) != null && mb.getBiom(vec).GoodForField) waterCount++; 
        }

        howmany = waterCount;
    }
}
