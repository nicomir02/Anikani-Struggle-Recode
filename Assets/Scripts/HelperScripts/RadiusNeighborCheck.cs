using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RadiusNeighborCheck : MonoBehaviour
{
    private MapBehaviour mb; 
    void Start(){
        mb = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
    }

    //Check to see if tiles in the specifies radius are free/empty (for building placement i.e.)
    public bool checkRadius(int radius, Vector3Int currentPosition, List<Tilemap> tilemaps, bool checkArea=true){
        //if(radius % 2 == 0) return false;   //falls nur ungeraden radius nutzen und man in der mitte anfängt
        //in die untere linke ecke des zu überprüfenden quadrats gehen
        currentPosition.x -= (radius - 1) / 2;
        currentPosition.y -= (radius - 1) / 2;
        int z = mb.getZ(currentPosition);
        //nachbarsfelder anschauen mit allen tilemaps
        bool canBuild = !checkArea;

        for(int x = 0; x < radius; x++){
            for(int y = 0; y < radius; y++){
                //Debug.Log("current pos:" + currentPosition);
                //Debug.Log(checkOwnArea(currentPosition));
                if(mb.getZ(currentPosition) != z) return false;
                foreach(Tilemap tm in tilemaps){
                    if(!checkBaubar(currentPosition) || checkBuildings(currentPosition)){
                        return false;
                    }
                    if(checkArea && checkOwnArea(currentPosition)) {
                        checkArea = false;
                        canBuild = true;
                    }
                }
                currentPosition.y += 1;
            }
            currentPosition.y -= radius;
            currentPosition.x += 1;
        }
        return canBuild;
    }
    //schaut ob Wasser an der Position
    bool checkBaubar(Vector3Int currentPosition){
        return mb.getBiom(currentPosition).getBaubar();
    }
   
    //schaut ob Gebäude an der Position (wird schwieriger glaube ich)
    bool checkBuildings(Vector3Int currentPosition){
        foreach(Building building in FindObjectsOfType<Building>()) {
            if(building.getPosition().Contains(currentPosition)) return true;
        }
        return false;
    }

    //schaut ob im eigenen Gebiet
    bool checkOwnArea(Vector3Int currentPosition){
        return GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<Area>().isOwnArea(currentPosition);
    }

    //Nur ungerader Radius, damit Mitte gewählt werden kann 
    public List<Vector3Int> getNeighborTiles(Vector3Int start, int radius) {
        //Debug.Log(start);
        //Debug.Log(radius);
        List<Vector3Int> liste = new List<Vector3Int>();
        
        //in die ecke gehen, da start der mittelpunkt ist
        start.x -= (radius - 1) / 2;
        start.y -= (radius - 1) / 2;
        //Debug.Log(start);
        for(int x = 0; x < radius; x++){
            for(int y = 0; y < radius; y++){
                start.z = mb.getZ(start);
                liste.Add(start);
                start.y += 1;
            }
            start.y -= radius;
            start.x += 1;
        }
        return liste;
    }
}
