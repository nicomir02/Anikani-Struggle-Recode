using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHover : MonoBehaviour
{
    public List<Tilemap> tilemaps = new List<Tilemap>();
    private Tilemap tilemap; //derzeitige Tilemap
    public Tilemap oldtilemap; //alte Tilemap
    [SerializeField] private Color selectColor;
    [SerializeField] private MapBehaviour mapBehaviour;

    public Vector3Int oldVec;
    Color oldColor;

    public Color getSelectColor() {
        return selectColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(tilemaps.Count == 0) return;
        Vector3Int newVec = getMousePosition();
        if(tilemap == null) return;
        if(oldVec == null || oldVec != newVec || oldtilemap != tilemap) {
            if(oldVec != null && oldtilemap != null) oldtilemap.SetColor(oldVec, oldColor);

            tilemap.SetTileFlags(newVec, TileFlags.None);
            oldColor = tilemap.GetColor(newVec);
            tilemap.SetColor(newVec, selectColor);
            oldVec = newVec;
        }

        //if(Input.GetMouseButtonDown(0)) Debug.Log(new Vector3Int(oldVec.x,oldVec.y, mapBehaviour.getZ(oldVec)));
    }

    //nach Mapgenerierung einmal aufgerufen um eine Liste aller Tilemaps zu erstellen
    public void afterGeneration() {
        tilemaps = new List<Tilemap>();
        int z=mapBehaviour.maxZ;
        while(z >= 0) {
            if(GameObject.Find("Height"+z) != null) tilemaps.Add(GameObject.Find("Height"+z).transform.GetChild(0).GetComponent<Tilemap>());
            z--;
        }
    }

    public Vector3Int getMousePosition() {
        oldtilemap = tilemap;
        Vector3 mousePos = Input.mousePosition;
        Vector3 vector = Camera.main.ScreenToWorldPoint(mousePos);
        //vector.y += 0.8f * (tilemaps.Count-1);
        vector.z += 1f * (tilemaps.Count-1);
        List<Tilemap> hoeher = new List<Tilemap>(); //Speichert schon höhere Tilemaps um zu schauen ob dadrüber noch ein Tile ist
        foreach(Tilemap tm in tilemaps) {
            
            Vector3Int result = tm.transform.parent.GetComponent<Grid>().WorldToCell(vector);
            
            if(tm.HasTile(result)) {
                foreach(Tilemap tilem in hoeher) {
                    if(tilem.HasTile(result)) {
                        tilemap = tilem;
                        return result;
                    }
                }
                tilemap = tm;
                return result;
            }
            hoeher.Add(tm);
            vector.z -= 1f;
            //vector.y -= 0.8f;
        }
        
        return new Vector3Int(-1,-1,-1);
    }

    public Vector3 getVec3Mouse(){
        Vector3Int from = getMousePosition();
        
        return getVec3FromVec3Int(from);
    }

    public Vector3 getVec3FromVec3Int(Vector3Int from) {
        Vector3 v = transform.position;
        if(tilemap != null) {
            v = tilemap.CellToWorld(from);
        }else {
            v = tilemaps[0].CellToWorld(from);
        }
        v.z += 1;
        return v;
    }
}
