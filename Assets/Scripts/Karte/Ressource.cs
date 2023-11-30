using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ressource : MonoBehaviour
{
    [SerializeField] private int id = -1;
    public int Id {
        get{return id;}
    }

    [SerializeField] private TileBase[] tilebase;
    
    [SerializeField] private List<Biom> biom;
    [SerializeField, Range(0, 1)] private List<float> probability;
    [SerializeField, Range(0, 1)] private float standardProbality = 0f;

    public void setTile(Tilemap tilemap, Vector3Int vec) {
        foreach(TileBase tb in tilebase) {
            tilemap.SetTile(vec, tb);
            vec.z++;
        }
    }

    public float getProbality(Biom b) {
        if(b == null) return standardProbality;
        for(int i=0; i<biom.Count; i++) {
            if(b == biom[i] && i < probability.Count) {
                return probability[i];
            }
        }
        return standardProbality;
    }
}
