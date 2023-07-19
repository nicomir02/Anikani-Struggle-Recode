using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static System.Random;

public class Block : MonoBehaviour
{
    [SerializeField] private TileBase[] tiles;
    [SerializeField] public int id;

    private static System.Random rand = new System.Random();

    public TileBase getTile() {
        return tiles[rand.Next(0, tiles.Length)];
    }

    public void setTile(Tilemap tilemap, Vector3Int vec) {
        for(int i=0; i<tiles.Length; i++) {
            tilemap.SetTile(vec, tiles[i]);
            vec.z += 1;
        }
    }
}
