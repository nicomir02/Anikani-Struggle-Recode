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
    private RoundManager rm;
    
    private Vector3Int currentPosition;
    bool selected = false;

    [SerializeField] private int ausbildungszeit;
    public int getAusbildungszeit() {
        return ausbildungszeit;
    }


    [SerializeField] private List<Ressource> RessourcenFuerKosten;
    [SerializeField] private List<int> kostenFuerRessource;
    [SerializeField] private List<float> kostenMultiplierFuerRessource;

    public Dictionary<Ressource, int> getKosten(){
        Dictionary<Ressource, int> result = new Dictionary<Ressource, int>();
        
        int anzahl = 0;

        /*
        Optimierungsvorschlg:
        für anzahl der gebäude: Beim PlayerManager speichern wen Gebäude gebaut als dictinary     
        */
        rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();
        
        foreach(Unit u in FindObjectsOfType<Unit>()) {
            string activePlayer = "";

            foreach(PlayerManager player in FindObjectsOfType<PlayerManager>()) {
                if(player.name == rm.aktiverSpieler) {
                    activePlayer = player.name;
                    break;
                }
            }

            if(u.name == name + " " + activePlayer) {
                anzahl++;
            }
        }

        for(int i = 0; i < RessourcenFuerKosten.Count; i++){
            float wert = anzahl*kostenMultiplierFuerRessource[i];
            int convert = (int) Mathf.Round(wert);
            result.Add(RessourcenFuerKosten[i], convert+kostenFuerRessource[i]); //wievieleEinheitenDiesesTyps*multiplierfürverschiedeneRessourcen+kostenFuerRessource
        }
        return result;
    }

    void Start() {
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        /*transform.position = getPosition(new Vector3Int(0,0,0));
        currentPosition = new Vector3Int(0,0,0);*/
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            if(!selected && tilemapHover.getMousePosition() == currentPosition) {
                Debug.Log("selected" + name);
                GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
            }else {
                schritteGehen(tilemapHover.getMousePosition());
            }
        }
    }


    //Nachdem neue Runde anfängt Aufladungsmethode
    public void nextRound() {
        steps = maxSteps;
    }


    //Einheit soll sich bewegen
    void schritteGehen(Vector3Int end) {
        if(end.z == -1 && end.x == -1 && end.z == -1 || !mapBehaviour.insideField(end)) return;
        end.z = mapBehaviour.getZ(end);
        Pathfinding path = new Pathfinding(currentPosition, end, this);
        if(path == null || path.FindPath() == null) {
            selected = !selected;
            GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }
        List<Vector3Int> liste = path.FindPath();
        if(liste.Count <= maxSteps) { //muss noch richtig gemacht werden
            Debug.Log(path.getAllCost());
            transform.position = getPosition(end); // Hier noch normale Bewegung machen
            end.z = 0;
            currentPosition = end;
        }else {
            selected = !selected;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //Sollten Umrechnungen gemacht werden müssen
    Vector3 getPosition(Vector3Int old) {
        Vector3 position = tilemapHover.getVec3FromVec3Int(old);
        return position;
    }

    //0.8 und 3
}
