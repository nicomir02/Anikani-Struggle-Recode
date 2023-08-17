using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHover : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Color selectColor;
    [SerializeField] private MapBehaviour mapBehaviour;

    Vector3Int oldVec;
    Color oldColor;

    // Update is called once per frame
    void Update()
    {
        Vector3Int newVec = getMousePosition();
        if(oldVec == null || oldVec != newVec) {
            Debug.Log(newVec);
            tilemap.SetColor(oldVec, oldColor);

            tilemap.SetTileFlags(newVec, TileFlags.None);
            oldColor = tilemap.GetColor(newVec);
            tilemap.SetColor(newVec, selectColor);
            oldVec = newVec;
        }
    }

    Vector3Int getMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 13;  // Set this to however far you want the point to be
        Vector3 vector = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int result = tilemap.WorldToCell(vector);
        /*result.x -= 7;
        result.y -= 7;
*/
        result.z = mapBehaviour.getZ(result);
        
        return result;
    }
}
