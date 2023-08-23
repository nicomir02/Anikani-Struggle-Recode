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

    // Update is called once per frame
    void Update()
    {
        if(tilemaps.Count == 0) return;
        Vector3Int newVec = getMousePosition();
        if(oldVec == null || oldVec != newVec || oldtilemap != tilemap) {
            if(oldVec != null && oldtilemap != null) oldtilemap.SetColor(oldVec, oldColor);

            tilemap.SetTileFlags(newVec, TileFlags.None);
            oldColor = tilemap.GetColor(newVec);
            tilemap.SetColor(newVec, selectColor);
            oldVec = newVec;
        }
    }

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
        foreach(Tilemap tm in tilemaps) {
            
            Vector3Int result = tm.transform.parent.GetComponent<Grid>().WorldToCell(vector);
            tilemap = tm;
            if(tm.HasTile(result)) {
                return result;
            }
            vector.z -= 1f;
            //vector.y -= 0.8f;
        }
        
        return new Vector3Int(-1,-1,-1);
    }

    public Vector3 getVec3Mouse(){
        Vector3Int from = getMousePosition();
        return tilemap.CellToWorld(from);
    }
}
