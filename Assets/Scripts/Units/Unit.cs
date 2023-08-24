using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class Unit : MonoBehaviour
{
    [SerializeField] private int maxSteps = 4;
    private int steps;
    private TilemapHover tilemapHover;
    private MapBehaviour mapBehaviour;
    private Vector3Int currentPosition;
    bool selected = false;

    void Start() {
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        transform.position = getPosition(new Vector3Int(0,0,0));
        currentPosition = new Vector3Int(0,0,0);
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            if(!selected && tilemapHover.getMousePosition() == currentPosition) {
                GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
            }else {
                schritteGehen(tilemapHover.getMousePosition());
            }
            selected = !selected;
        }
    }


    //Nachdem neue Runde anfängt Aufladungsmethode
    public void roundAufladen() {
        steps = maxSteps;
    }


    //Einheit soll sich bewegen
    void schritteGehen(Vector3Int end) {
        if(end.z == -1 && end.x == -1 && end.z == -1 || !mapBehaviour.insideField(end)) return;
        end.z = mapBehaviour.getZ(end);
        Pathfinding path = new Pathfinding(currentPosition, end, this);
        List<Vector3Int> liste = path.FindPath();
        if(path.getAllCost() < maxSteps) { //muss noch richtig gemacht werden
            Debug.Log(path.getAllCost());
            transform.position = getPosition(end); // Hier noch normale Bewegung machen
        }else {
            Debug.Log("Position kann nicht erreicht werden");
        }
    }

    //Sollten Umrechnungen gemacht werden müssen
    Vector3 getPosition(Vector3Int old) {
        Vector3 position = tilemapHover.getVec3FromVec3Int(old);
        return position;
    }

    //0.8 und 3
}
