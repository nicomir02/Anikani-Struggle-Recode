using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHover : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Color selectColor;
    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private float offset = 13f;

    Vector3Int oldVec;
    Color oldColor;

    // Update is called once per frame
    void Update()
    {
        Vector3Int newVec = getMousePosition();
        if(oldVec == null || oldVec != newVec) {
            tilemap.SetColor(oldVec, oldColor);

            tilemap.SetTileFlags(newVec, TileFlags.None);
            oldColor = tilemap.GetColor(newVec);
            tilemap.SetColor(newVec, selectColor);
            oldVec = newVec;
        }
    }

    Vector3Int getMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = offset;
        Vector3 vector = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int result = tilemap.LocalToCell(vector);
        /*result.x -= 7;
        result.y -= 7;*/
        result.z = mapBehaviour.getZ(result);
        
        return result;
    }
}
