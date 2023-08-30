using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Building : NetworkBehaviour
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
    private MapBehaviour mb;
    //roundManager
    private RoundManager rm;

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

    //Leben/Haltbarkeit des Gebäudes
    [SerializeField] private int health;

    //position des Gebäudes
    private List<Vector3Int> position = new List<Vector3Int>();

    //liste der zerstörten Ressourcen unter Gebäude
    private Dictionary<Ressource, int> ressourcen = new Dictionary<Ressource, int>();
    
    public Dictionary<Ressource, int> Ressourcen {
        get{return ressourcen;}
    }

    //Kosten des Gebäudes
    [SerializeField] private List<Ressource> RessourcenFuerKosten;
    [SerializeField] private List<int> kostenFuerRessource;
    [SerializeField] private List<float> kostenMultiplierFuerRessource;


    // Start is called before the first frame update
    void Start()
    {
        th = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        mb = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        radiusNeighborCheck = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<RadiusNeighborCheck>(); 
        rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();

        spawnHelper = GameObject.Find("___InGame___/----Manager----/HelperScripts").GetComponent<ObjectSpawnHelper>();

        

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
    private void Update()
    {
        
        
        if(pm != PlayerHelperScript.findLocalPlayer()) return;
        
        Vector3Int vec = th.getMousePosition();
        vec.x -= (radius-1)/2;
        vec.y -= (radius-1)/2;
        Vector3 newPos = th.getVec3FromVec3Int(vec);
        newPos.z += 1;
        if(!gesetzt){
            transform.position = newPos;
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
    }
    
    public List<Vector3Int> getPosition() {
        return position;
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

    public virtual bool setzbar(){
        Vector3Int neuPos = th.getMousePosition();
        return radiusNeighborCheck.checkRadius(radius, neuPos, th.tilemaps);
    }

    private void gebaeudeBau(Vector3Int pos){
        PlayerHelperScript.findLocalPlayer().GetComponent<Area>().addArea(radiusNeighborCheck.getNeighborTiles(pos, radius*3)); //Hier ändern, falls anderer Radius

        pos.x -= (radius-1)/2;
        pos.y -= (radius-1)/2;
        for(int x = 0; x < radius; x++){
            for(int y = 0; y < radius; y++){
                position.Add(pos);
                pos.y += 1;
            }
            pos.y -= radius;
            pos.x += 1;
        }
        lightObject.SetActive(true);
        
        CMDressourcenZerstoeren();
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
            foreach(Tilemap tm in th.tilemaps) {
                tm.SetTile(new Vector3Int(vec.x, vec.y, 1), null);
            }
        }
    }

    public virtual void nextRound() {}

    public Dictionary<Ressource, int> getKosten(){
        Dictionary<Ressource, int> result = new Dictionary<Ressource, int>();
        
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
            int convert = (int) Mathf.Round(wert);
            result.Add(RessourcenFuerKosten[i], convert+kostenFuerRessource[i]); //wievielegebäudeDiesesTyps*multiplierfürverschiedeneRessourcen+kostenFuerRessource
        }
        return result;
    }
}
