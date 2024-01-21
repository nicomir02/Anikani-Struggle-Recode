using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


//
public class Unit : VolkObject
{
    private List<Sprite> frontWalk = new List<Sprite>();
    private List<Sprite> backWalk = new List<Sprite>();
    [SyncVar] public bool lastMoveWasAttack = false;

    private List<Sprite> frontAttack = new List<Sprite>();
    private List<Sprite> backAttack = new List<Sprite>();

    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip attackSound;

    private void loadMoreAnimations() {
        transform.GetChild(5).GetComponent<AniSpriteMerger>().aniMerge();
        frontWalk = transform.GetChild(5).GetComponent<AniSpriteMerger>().GetList(); 
        transform.GetChild(6).GetComponent<AniSpriteMerger>().aniMerge();
        backWalk = transform.GetChild(6).GetComponent<AniSpriteMerger>().GetList(); 

        transform.GetChild(7).GetComponent<AniSpriteMerger>().aniMerge();
        frontAttack = transform.GetChild(7).GetComponent<AniSpriteMerger>().GetList(); 
        transform.GetChild(8).GetComponent<AniSpriteMerger>().aniMerge();
        backAttack = transform.GetChild(8).GetComponent<AniSpriteMerger>().GetList(); 
    }

//StandardMaxSteps für Moralauswirkungen eingeführt
    [SerializeField] private int standardMaxSteps = 4;
    private int maxSteps = 4;
    private int steps = 0; 
    public int Steps {
        get{return steps;}
    }

    [SyncVar] private int wieLangNichtGebiet = 0;

    [SerializeField] private TMP_Text reichweitenText;

    public int WieLangNichtGebiet {
        get{return wieLangNichtGebiet;}
    }

    private MapBehaviour mapBehaviour;

    private Area area;

    private bool facingRight = true;
    
    
    [SyncVar] public Vector3Int currentPosition; //Wieder private machen nach test

    //Standardausbildungszeit für moralauswirkungen eingeführt
    [SerializeField] private int standardAusbildungszeit;
    private int ausbildungszeit;

    [SerializeField] private GameObject lightObject;
    public int getAusbildungszeit() {
        if (ausbildungszeit <= 0) ausbildungszeit = standardAusbildungszeit;
        return ausbildungszeit;
    }

    public override void Start() {
        base.Start();
        loadMoreAnimations();
        vorlieben = mm.getMoral(0, healValue > 0);
        //Standardwerte für moralauswirkungen eingeführt
        maxSteps = standardMaxSteps;
        ausbildungszeit = standardAusbildungszeit;
        
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
        reichweitenText.text = 0+"";
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
    public override void nextRound() {
        base.nextRound();
        steps = maxSteps;
        reichweitenText.text = maxSteps +"";
        if(area.isAreaByID(currentPosition, transform.parent.GetComponent<PlayerManager>().playerCode)) {
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
        reichweitenText.text = steps+"";
    }

    [Command(requiresAuthority=false)]
    void cmdChangeSteps(int m_steps) {
        changeSteps(m_steps);
    }




    //Einheit soll sich bewegen
    [Command(requiresAuthority=false)]
    public void schritteGehen(Vector3Int end) {
        
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
                if(attackRange <= liste.Count || mapBehaviour.getZ(start)-mapBehaviour.getZ(end)+attackRange <= liste.Count) { //und man von der jetzigen Position das gegnerische Object erreicht mit der angriffsreichweite
                    Debug.Log("test");
                    tryAttack(end, obj);   //probieren ob man dieses Object attackieren kann
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
    

    [ClientRpc]
    public void schritteSound() {
        if(GetComponent<AudioSource>() == null) {
            gameObject.AddComponent<AudioSource>();
        }
        if(moveSound == null) return;
        GetComponent<AudioSource>().clip = moveSound;
        GetComponent<AudioSource>().Play();
    }

    public void tryAttack(Vector3Int v, VolkObject obj) {
        if(healValue > 0 && obj.transform.parent == transform.parent) {  //eigene Einheit heilen falls man heilen kann
            obj.changeHealth(healValue);
            lastHeal = 0;

            if(GetComponent<AudioSource>() == null) {
                gameObject.AddComponent<AudioSource>();
            }
            if(healSound == null) return;
            GetComponent<AudioSource>().clip = healSound;
            GetComponent<AudioSource>().Play();

            steps = 0;      //übrige Aktions/Schrittanzahl auf 0 gesetzt
            reichweitenText.text = steps+"";
            changeSteps(0);
            lastMoveWasAttack = true;
        }else if(obj.transform.parent != transform.parent){
            
            if(GetComponent<AudioSource>() == null) {
                gameObject.AddComponent<AudioSource>();
            }
            lastAttack = 0; 
            angriffAnimation(this, obj);
            obj.changeHealth(-attackValue);

            steps = 0;      //übrige Aktions/Schrittanzahl auf 0 gesetzt
            reichweitenText.text = steps+"";
            changeSteps(0);
            lastMoveWasAttack = true;

            if(attackSound == null) return;
            GetComponent<AudioSource>().clip = attackSound;
            GetComponent<AudioSource>().Play();
        }
    }

    [Command(requiresAuthority=false)]
    public void angriffAnimation(VolkObject v, VolkObject obj) {
        RpcangriffAnimation(this, obj);
    }

    [ClientRpc]
    public void RpcangriffAnimation(VolkObject v, VolkObject obj) {

        if(v.transform.position.x >= obj.transform.position.x && v.transform.position.y >= obj.transform.position.y) {
            animationList = frontAttack;
        }else if(v.transform.position.x >= obj.transform.position.x && v.transform.position.y <= obj.transform.position.y) {
            animationList = backAttack;
        }else if(v.transform.position.x <= obj.transform.position.x && v.transform.position.y >= obj.transform.position.y) {
            animationList = frontAttack;
        }else if(v.transform.position.x <= obj.transform.position.x && v.transform.position.y <= obj.transform.position.y) {
            animationList = backAttack;
        }
        animationStart(0, 0);
    }

    

    void Flip(){ //0 = right, 1 = left;
        facingRight = !facingRight;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    [Command(requiresAuthority=false)]
    public void moveAnimation(Vector3 v, Vector3 obj) {
        RpcMoveAnimation(v, obj);
    }

    [ClientRpc]
    public void RpcMoveAnimation(Vector3 startingPosition, Vector3 position) {

        List<Sprite> nextAnimation = animationList;
        if(startingPosition.x >= position.x && startingPosition.y >= position.y) {
            nextAnimation = frontWalk;
            if(!facingRight) Flip();
        }else if(startingPosition.x >= position.x && startingPosition.y <= position.y) {
            nextAnimation = backWalk;
            if(!facingRight) Flip();
        }else if(startingPosition.x <= position.x && startingPosition.y >= position.y) {
            nextAnimation = frontWalk;
            if(facingRight) Flip();
        }else if(startingPosition.x <= position.x && startingPosition.y <= position.y) {
            nextAnimation = backWalk;
            if(facingRight) Flip();
        }
        if(nextAnimation != animationList) {
            animationList = nextAnimation;
            animationStart(0, -1);
        }
       
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
            moveAnimation(startingPosition, position);
            
            

            while (elapsedTime < timeToMove) {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / timeToMove);
                transform.position = Vector3.Lerp(startingPosition, position, t);
                GetComponent<Renderer>().sortingOrder = (int) transform.position.z + 1;
                yield return null;
            }
            animationStart(idle.Count+1, 0);
        }

        transform.position = tilemapHover.getVec3FromVec3Int(positions[positions.Count-1]);
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

    //Getter und Setter für Moralauswirkungen
    public int MaxSteps {
        get{return maxSteps;}
        set{maxSteps = value;}
    }
    public int StandardMaxSteps {
        get{return standardMaxSteps;}
    }
    public int Ausbildungszeit {
        get{
            if (ausbildungszeit <= 0) ausbildungszeit = standardAusbildungszeit;
            return ausbildungszeit;
            }
        set{ausbildungszeit = value;}
    }
    public int StandardAusbildungszeit {
        get{return standardAusbildungszeit;}
    }
}
