using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//
public class Unit : VolkObject
{
    [SerializeField] private List<Sprite> frontWalk = new List<Sprite>();
    [SerializeField] private List<Sprite> backWalk = new List<Sprite>();

    [SerializeField] private int maxSteps = 4;
    public int steps = 0; //Wieder private machen nach test
    public int Steps {
        get{return steps;}
    }

    [SyncVar] private int wieLangNichtGebiet = 0;
    public int WieLangNichtGebiet {
        get{return wieLangNichtGebiet;}
    }

    private MapBehaviour mapBehaviour;

    private Area area;
    private RoundManager rm;
    
    
    [SyncVar] public Vector3Int currentPosition; //Wieder private machen nach test

    [SerializeField] private int ausbildungszeit;


    [SerializeField] private GameObject lightObject;
    public int getAusbildungszeit() {
        return ausbildungszeit;
    }

    public Dictionary<Ressource, int> getKosten(){
        Dictionary<Ressource, int> result = new Dictionary<Ressource, int>();
        
        int anzahl = 0;

        /*
        Optimierungsvorschlg:
        für anzahl der gebäude: Beim PlayerManager speichern sobald Gebäude gebaut als dictinary     
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

    

    public override void Start() {
        base.Start();
        vorlieben = mm.getMoral(0, healValue > 0);
        
        area = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<Area>();
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        /*transform.position = getPosition(new Vector3Int(0,0,0));
        currentPosition = new Vector3Int(0,0,0);*/

        //Debug.Log(transform.position);
        currentPosition = tilemapHover.getVec3IntFromVec3(transform.position);
        //Debug.Log(currentPosition);
        transform.position = tilemapHover.getVec3FromVec3Int(currentPosition, true);
        //Debug.Log(transform.position);

        changeHealth(maxHealth);

        //Debug.Log("Start" + currentPosition);
        steps = 0;
        
        
    }

    [Command(requiresAuthority=false)]
    public void changeCurrentPosition() {
        GetComponent<Renderer>().sortingOrder = (int) transform.position.z + 3;
        RPCchangeCurrentPosition(currentPosition);
    }

    [ClientRpc]
    void RPCchangeCurrentPosition(Vector3Int vec) {
        currentPosition = vec;
        GetComponent<Renderer>().sortingOrder = (int) transform.position.z + 3;
    }


    public override void Update() {
        base.Update();
        if(currentPosition == new Vector3Int(0,0,0)) {
            changeCurrentPosition();
        }

        if(Input.GetMouseButtonDown(0) && SelectHelper.canSelect()) {
            if(!m_selected && tilemapHover.getMousePosition().x == currentPosition.x && tilemapHover.getMousePosition().y == currentPosition.y && transform.parent.GetComponent<PlayerManager>() == PlayerHelperScript.findLocalPlayer()) {
                selected();
            }else if(m_selected && steps > 0){
                Debug.Log("Try Schritte gehen");
                schritteGehen(tilemapHover.getMousePosition());
                deselect(); //nach gehen soll Einheit nicht noch selected sein
            }else{
                deselect();
            }
        }
    }

    public override void selected() {
        /*GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
        m_selected = true;*/
        base.selected();
    }

    //schauen ob position dieses objects mit dem übergebenen vector vec übereinstimmt
    public override bool isObjectAtPosition(Vector3Int vec) {
        Debug.Log("Object at " + currentPosition + " other at " + vec);
        return currentPosition.x == vec.x && currentPosition.y == vec.y;
    }

    //Nachdem neue Runde anfängt Aufladungsmethode
    [Command(requiresAuthority=false)]
    public void nextRound() {
        steps = maxSteps;
        if(area.isAreaByID(currentPosition, transform.parent.GetComponent<PlayerManager>().playerColor)) {
            wieLangNichtGebiet = 0;
        }else {
            wieLangNichtGebiet++;
        }
        changeSteps(steps);
        lastAttack++;
        lastHeal++;
    }

    [ClientRpc]
    void changeSteps(int m_steps) {
        steps = m_steps;
    }




    //Einheit soll sich bewegen
    [Command(requiresAuthority=false)]
    void schritteGehen(Vector3Int end) {
        
        foreach(PlayerManager pm in FindObjectsOfType<PlayerManager>()) {
            if(PlayerHelperScript.findLocalPlayer() != transform.parent.GetComponent<PlayerManager>()) {
                lightObject.SetActive(false);
            }else {
                lightObject.SetActive(true);
            }
        }
        Debug.Log("Try Schritte gehen zu " + end);
        //klappt noch nicht ganz. auch abfragen dass nur der spieler der dran ist sich bewegen kann.
        if(end.z == -1 && end.x == -1 && end.z == -1 || !mapBehaviour.insideField(end)) return;
        end.z = mapBehaviour.getZ(end);
        Vector3Int start = currentPosition;
        start.z = mapBehaviour.getZ(start);
        Pathfinding path = new Pathfinding(start, end, this);
        if(path == null || path.FindPath() == null) {
            Debug.Log("ww" + path);
            return;
        }

        //Animationsbewegung über pfad in dieser liste
        List<Vector3Int> liste = path.FindPath();

        //schauen ob es sich um einen Angriff handelt
        foreach(VolkObject obj in FindObjectsOfType<VolkObject>()) {
            if(obj.isObjectAtPosition(end)) { //falls dort ein Object ist
                Debug.Log("Liste " + liste.Count);
                Debug.Log("Range " + attackRange);
                if(attackRange == liste.Count) { //und man von der jetzigen Position das gegnerische Object erreicht mit der angriffsreichweite
                    Debug.Log("test");
                    tryAttack(end, obj);   //probieren ob man dieses Object attackieren kann
                    steps = 0;      //übrige Aktions/Schrittanzahl auf 0 gesetzt
                    
                }
                return; //auf kein Objekt draufgehen(erstmal!!)
            }
        }

        if(liste[liste.Count-1] != end) liste.Add(end);

        StartCoroutine(MoveToPosition(liste, 0.5f, currentPosition));

        currentPosition = end;
        RPCchangeCurrentPosition(end);

        steps-= path.endkosten;
        changeSteps(steps);
    }

    
    public IEnumerator MoveToPosition(List<Vector3Int> positions, float timeToMove, Vector3Int from)
    {
        Vector3 startingPosition;
        for(int i=-1; i<positions.Count-1; i++) {
            float elapsedTime = 0f;

            if(i == -1) {
                startingPosition = tilemapHover.getVec3FromVec3Int(from);
            }else {
                startingPosition = tilemapHover.getVec3FromVec3Int(positions[i]);
            }

            Vector3 position = tilemapHover.getVec3FromVec3Int(positions[i+1]);

            if(startingPosition.x > position.x) {
                animationList = frontWalk;
            }else if(startingPosition.x < position.x) {
                animationList = backWalk;
            }else if(startingPosition.y > position.y) {
                animationList = frontWalk;
            }else if(startingPosition.y < position.y) {
                animationList = backWalk;
            }

            while (elapsedTime < timeToMove) {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / timeToMove);
                transform.position = Vector3.Lerp(startingPosition, position, t);
                GetComponent<Renderer>().sortingOrder = (int) transform.position.z + 1;
                yield return null;
            }
        }

        transform.position = tilemapHover.getVec3FromVec3Int(positions[positions.Count-1]);
        animationList = idle;

    }

    
    //Sollten Umrechnungen gemacht werden müssen
    Vector3 getPosition(Vector3Int old) {
        Vector3 position = tilemapHover.getVec3FromVec3Int(old);
        return position;
    }

    //0.8 und 

    //Angriff/Heilung auf fremde Einheit
    //........

    //Angriff/Heilung auf eigene Einheit
    //.......

    //angegriffen und geheilt
}
