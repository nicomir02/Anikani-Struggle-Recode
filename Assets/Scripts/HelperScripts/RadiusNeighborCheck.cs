using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RadiusNeighborCheck : MonoBehaviour
{
    //Check to see if tiles in the specifies radius are free/empty (for building placement i.e.)
    public bool checkRadius(int radius, Vector3Int currentPosition, List<Tilemap> tilemaps){
        if(radius % 2 == 0) return false;   //nur ungeraden radius nutzen
        //in die untere linke ecke des zu überprüfenden quadrats gehen
        //currentPosition.x -= (radius - 1) / 2;
        //currentPosition.y -= (radius - 1) / 2;

        //nachbarsfelder anschauen mit allen tilemaps
        for(int x = 0; x < radius; x++){
            for(int y = 0; y < radius; y++){
                currentPosition.x += x;
                currentPosition.y += y;
                foreach(Tilemap tm in tilemaps){
                    if(tm.HasTile(currentPosition) || checkWasser(currentPosition) 
                        || checkRessourcen(currentPosition) || checkBuildings(currentPosition)
                        || checkOwnArea(currentPosition)){
                        return false;
                    }
                }
            }
        }
        return true;
    }
    //schaut ob Wasser an der Position
    bool checkWasser(Vector3Int currentPosition){
        return false;
    }
    //schaut ob Ressource an der Position
    bool checkRessourcen(Vector3Int currentPosition){
        return false;
    }
    //schaut ob Gebäude an der Position (wird schwieriger glaube ich)
    bool checkBuildings(Vector3Int currentPosition){
        //alle gameobjects nach building tag durchsuchen und dann deren public methode(für deren positionkoordinaten abfragen)
        //oder irgendwo Liste mit allen durch gebäude besetzte koordinaten speichern und durchschauen
        return false;
    }

    //schaut ob im eigenen Gebiet
    bool checkOwnArea(Vector3Int currentPosition){
        //Hilfssckript für Area machen und in GameManager packen
        //Area ownArea = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<Area>();
        return false;
    }
}
