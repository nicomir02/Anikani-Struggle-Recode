using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMainBuilding : Building
{
    public override bool setzbar() {
        Vector3Int neuPos = tilemapHover.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, tilemapHover.tilemaps, false);
    }
    
}
