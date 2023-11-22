using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building
{
    [SerializeField] private Ressource nahrung;

    public override bool setzbar() {
        Vector3Int neuPos = tilemapHover.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, tilemapHover.tilemaps, false);
    }

    public override void selected() {
        base.selected();
        im.closeInfo();
        im.openRightBox(this, false);
    }

    public override void nextRound() {
        base.nextRound();
        currentNahrung = 0.5f;
    }
    
}
