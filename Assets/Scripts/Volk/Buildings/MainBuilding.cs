using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building
{
    public override bool setzbar() {
        Vector3Int neuPos = tilemapHover.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, tilemapHover.tilemaps, false);
    }

    public override void selected() {
        /*Debug.Log("[" + name + "]" + " Selected");
        GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
        im.openRightBox(this, false);
        m_selected = true;*/
        base.selected();
    }
    
}
