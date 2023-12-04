using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Building : VolkObject
{
    //standardmäßg 3 gebäude radius (= 3x3 Feld)
    [SerializeField] protected int radius = 3;
    //ob Gebäude schon gesetzt
    private bool gesetzt = false;

    public bool buildingGesetzt {
        get{return gesetzt;}
    }
    
    //Mouseposition
    protected TilemapHover th;
    //mapBehaviour
    protected MapBehaviour mb;

    private Area area;

    //RadiusNeighborCheck
    protected RadiusNeighborCheck radiusNeighborCheck;

    //Spawn Helper
    private ObjectSpawnHelper spawnHelper;

    private PlayerManager pm;
    public PlayerManager playerManager {
        get{return pm;}
        set{pm = value;}
    }

    [SerializeField] private GameObject lightObject;

    //position des Gebäudes
    private readonly SyncList<Vector3Int> position = new SyncList<Vector3Int>();
    [SyncVar] protected Vector3Int middlePoint;

    //liste der zerstörten Ressourcen unter Gebäude
    private Dictionary<Ressource, float> ressourcen = new Dictionary<Ressource, float>();
    
    public Dictionary<Ressource, float> Ressourcen {
        get{return ressourcen;}
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        vorlieben = mm.getMoral(1);

        area = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<Area>();
        mb = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        radiusNeighborCheck = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<RadiusNeighborCheck>(); 

        spawnHelper = GameObject.Find("___InGame___/----Manager----/HelperScripts").GetComponent<ObjectSpawnHelper>();

        changeHealth(maxHealth);

        //Spielersuche
        if(pm == null) {
            foreach(PlayerManager playerManager in FindObjectsOfType<PlayerManager>()) {
                if(playerManager.name == rm.aktiverSpieler) {
                    pm = playerManager;
                    break;
                }
            }
        }
        if(pm == PlayerHelperScript.findLocalPlayer()){
            transparencyChange(0.5f);
        }else {
            transparencyChange(0f);
        }
        
    }

    

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        if(pm != PlayerHelperScript.findLocalPlayer()) return;
        
        Vector3Int vec = tilemapHover.getMousePosition();
        
        vec.x -= (radius-1)/2;
        vec.y -= (radius-1)/2;
        
        Vector3 newPos = tilemapHover.getVec3FromVec3Int(vec);

        newPos.z += 1;
        newPos.z += 1;

        if(!gesetzt){
            CMDtransparencyChange(0.5f, newPos);
            /*
            Leider ist setzbar() sehr ineffizient
            
            if(setzbar()) {
                Color tmp = GetComponent<SpriteRenderer>().color;
                tmp = Color.white;
                tmp.a = 0.5f;

                GetComponent<SpriteRenderer>().color = tmp;
            }else {
                Color tmp = GetComponent<SpriteRenderer>().color;
                tmp = Color.red;
                tmp.a = 0.5f;

                GetComponent<SpriteRenderer>().color = tmp;
            }*/
        }

        if(Input.GetMouseButtonDown(0) && !gesetzt && setzbar()){
            
            transform.position = newPos;
            
            vec.x += (radius-1)/2;
            vec.y += (radius-1)/2;
            
            gebaeudeBau(vec);
            gesetzt = true;
            CMDtransparencyChange(1f, newPos);
        }
        //if(!setzbar()) Debug.Log("nicht setzbar");

        if(Input.GetMouseButtonDown(0) && SelectHelper.canSelect()) {
            if(isSelected()) {
                //Debug.Log("gebäude wurde ausgewaehlt");
                //Debug.Log(gameObject.name);
                //hier auch infoboxen und upgrade/zerstörungs/bewegungs(bei wanderern) buttons aktivieren!
                selected();
            }else if(!isSelected() && m_selected){
                deselect();
            }
        }
    }
    
    
    public List<Vector3Int> getPosition() {
        return new List<Vector3Int>(position);
    }

    [Command(requiresAuthority=false)]
    public void CMDtransparencyChange(float f, Vector3 vec) {
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;

        Vector3 curPosition = transform.position;
        curPosition = vec;
        transform.position = curPosition;

        RPCtransparencyChange(f, vec);
    }

    [ClientRpc]
    public void RPCtransparencyChange(float f, Vector3 vec) {
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;

        Vector3 curPosition = transform.position;
        curPosition = vec;
        transform.position = curPosition;
    }

    private void transparencyChange(float f){
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    public override bool isObjectAtPosition(Vector3Int vec) {
        foreach(Vector3Int ve in position) {
            return ve.x == vec.x && ve.y == vec.y;
        }
        return false;
    }

    public virtual bool setzbar(){
        Vector3Int neuPos = tilemapHover.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, tilemapHover.tilemaps);
    }

    private void gebaeudeBau(Vector3Int pos){
        middlePoint = pos;
        if(!area.addArea(radiusNeighborCheck.getNeighborTiles(pos, radius*3), radiusNeighborCheck.getNeighborTiles(pos, radius))) return; //Hier ändern, falls anderer Radius

        pos.x -= (radius-1)/2;
        pos.y -= (radius-1)/2;
        for(int x = 0; x < radius; x++){
            for(int y = 0; y < radius; y++){
                addPosition(pos);
                pos.y += 1;
            }
            pos.y -= radius;
            pos.x += 1;
        }
        lightObject.SetActive(true);
        
        CMDressourcenZerstoeren();
    }

    [Command(requiresAuthority=false)]
    public void addPosition(Vector3Int vec) {
        position.Add(vec);
    }

    [Command(requiresAuthority=false)]
    public void CMDressourcenZerstoeren() {
        ressourcenZerstoeren();
    }

    [ClientRpc]
    private void ressourcenZerstoeren() {
        foreach(Vector3Int vec in position) {
            Ressource ress = mb.getRessource(vec);
            if(ress != null) {
                if(ressourcen.ContainsKey(ress)) {
                    ressourcen[ress] += 1;
                }else {
                    ressourcen.Add(ress, 1);
                }
            } 
            foreach(Tilemap tm in tilemapHover.tilemaps) {
                tm.SetTile(new Vector3Int(vec.x, vec.y, 1), null);
            }
        }
    }


    [Command(requiresAuthority=false)]
    public override void nextRound() {
        base.nextRound();
    }

    public Dictionary<Ressource, float> getKosten(){
        Dictionary<Ressource, float> result = new Dictionary<Ressource, float>();
        
        int anzahl = 0;

        /*
        Optimierungsvorschlg:
        für anzahl der gebäude: Beim PlayerManager speichern wen Gebäude gebaut als dictinary     
        */
        rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();
        
        foreach(Building b in FindObjectsOfType<Building>()) {
            string activePlayer = "";

            foreach(PlayerManager player in FindObjectsOfType<PlayerManager>()) {
                if(player.name == rm.aktiverSpieler) {
                    activePlayer = player.name;
                    break;
                }
            }

            if(b.name == name + " " + activePlayer) {
                anzahl++;
            }
        }

        for(int i = 0; i < RessourcenFuerKosten.Count; i++){
            float wert = anzahl*kostenMultiplierFuerRessource[i];
            float convert = Mathf.Round(wert * 10.0f) * 0.1f;
            result.Add(RessourcenFuerKosten[i], convert+kostenFuerRessource[i]); //wievielegebäudeDiesesTyps*multiplierfürverschiedeneRessourcen+kostenFuerRessource
        }
        return result;
    }

    private bool isSelected(){
        if(position.Contains(tilemapHover.getMousePosition())){
            return true;
        }
        return false;
    }
}
