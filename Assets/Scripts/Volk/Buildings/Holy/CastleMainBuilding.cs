using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMainBuilding : Building
{
    public override bool setzbar() {
        Vector3Int neuPos = th.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, th.tilemaps, false);
    }
}
