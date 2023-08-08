using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ressource : MonoBehaviour
{
    [SerializeField] private TileBase tilebase;

    public void setTile(Tilemap tilemap, Vector3Int vec) {
        tilemap.SetTile(vec, tilebase);
    } 
}
