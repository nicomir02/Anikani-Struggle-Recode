using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Area : NetworkBehaviour
{
    public readonly SyncDictionary<Vector3Int, int> area = new SyncDictionary<Vector3Int, int>();

    [SerializeField] private GameManager gameManager;
    [SerializeField] private MapBehaviour mapBehaviour;

    [SerializeField] private RoundManager roundManager;
    [SerializeField] private Button showAreaButton;
    bool AreaShown = false;

    bool build = false; //Hauptgebäude gesetzt: true or false?

    void Start() {
        showAreaButton.onClick.AddListener(showArea);
    }
    
    public bool addArea(List<Vector3Int> newArea, List<Vector3Int> buildingArea){
        foreach(Vector3Int vec in buildingArea) {
            if(isEnemyArea(vec)) return false;
        }
        build = true;
        CMDaddArea(newArea);
        return true;
    }

    [Command(requiresAuthority=false)]
    public void CMDaddArea(List<Vector3Int> newArea) {
        foreach(Vector3Int vec in newArea) {
            if(!area.ContainsKey(vec)) {
                area.Add(new Vector3Int(vec.x, vec.y, mapBehaviour.getZ(vec)), roundManager.ActiveID);
            }
        }
        //Debug.Log(roundManager.ActiveID);
        RpcUpdateArea();
    }

    [ClientRpc]
    public void RpcUpdateArea() {
        areaUpdate();
    }

//dient als Überprüfung damit man nur in eigener Area bauen und in der Runde in der man dran ist
    public bool isOwnArea(Vector3Int vec) {
        vec.z = mapBehaviour.getZ(vec);
        
        return PlayerHelperScript.findLocalPlayer().playerCode == roundManager.ActiveID && ((area.ContainsKey(vec) && area[vec] == roundManager.ActiveID) || (!build && !area.ContainsKey(vec)));
    }

    public bool isAreaByID(Vector3Int vec, int id) {
        vec.z = mapBehaviour.getZ(vec);
        return area.ContainsKey(vec) && area[vec] == id;
    }

    public bool isEnemyArea(Vector3Int vec) {
        vec.z = mapBehaviour.getZ(vec);
        
        return area.ContainsKey(vec) && area[vec] != roundManager.ActiveID;
    }

    
    void showArea(){

        if(!AreaShown){
            AreaShown = true;
            areaUpdate();
        }else {
            foreach(KeyValuePair<Vector3Int, int> kvp in area) {
                Vector3Int vec = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
                foreach(Tilemap t in FindObjectsOfType<Tilemap>()){
                    if(gameManager.getColor(kvp.Value) != Color.white) t.SetColor(vec, Color.white);
                }
            }
            AreaShown = false;
        }
    }

    void areaUpdate() {
        if(!AreaShown) return;
        foreach(KeyValuePair<Vector3Int, int> kvp in area) {
            Vector3Int vec = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
            foreach(Tilemap t in FindObjectsOfType<Tilemap>()){
                t.SetTileFlags(vec, TileFlags.None);
                if(gameManager.getColor(kvp.Value) != Color.white) t.SetColor(vec, gameManager.getColor(kvp.Value));
                Debug.Log(kvp.Value + " " + gameManager.getColor(kvp.Value));
            }
        }
    }

    /*
    
    public readonly SyncHashSet<Vector3Int> area = new SyncHashSet<Vector3Int>();
    private Button showAreaButton;
    bool AreaShown = false;
    bool canvasGeladen = false;
    private GameManager gameManager;
    private MapBehaviour mapBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<GameManager>();
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("___InGame___/Canvas/ShowAreaBtn") != null && !canvasGeladen) {
            showAreaButton = GameObject.Find("___InGame___/Canvas/ShowAreaBtn").GetComponent<Button>();
            showAreaButton.onClick.AddListener(showArea);

            canvasGeladen = true;
        }
    }

    public SyncHashSet<Vector3Int> playerArea {
        get{return area;}
    }

    public void addArea(List<Vector3Int> newArea){
        foreach(Vector3Int vec in newArea){
            bool isClaimed = false;
            foreach(Area r in FindObjectsOfType<Area>()){
                if(r.playerArea.Contains(vec)) {
                    isClaimed = true;
                    break;
                }
            }
            if(!isClaimed) addVec(vec);
        }
    }

    public bool isOwnArea(Vector3Int vec) {
        vec.z = mapBehaviour.getZ(vec);
        return area.Contains(vec);
    }

    [Command(requiresAuthority=false)]
    void addVec(Vector3Int vec) {
        area.Add(vec);
    }

    void showArea(){

        if(!AreaShown){
            Debug.Log("Area aufruf");
            foreach(Area r in FindObjectsOfType<Area>()){
                Debug.Log("Area aufruf 2");
                foreach(Vector3Int vec in r.playerArea){
                    Debug.Log("Area aufruf 3");
                    Vector3Int v = vec;
                    v.z = 0;
                    foreach(Tilemap t in FindObjectsOfType<Tilemap>()){
                        Debug.Log("Area aufruf 4 " + r.GetComponent<PlayerManager>().playerColor);
                        t.SetTileFlags(v, TileFlags.None);
                        if(gameManager.getColor(r.GetComponent<PlayerManager>().playerColor) != Color.white) t.SetColor(v, gameManager.getColor(r.GetComponent<PlayerManager>().playerColor));
                    }
                }
            }
            AreaShown = true;
        }else {
            foreach(Area r in FindObjectsOfType<Area>()){
                foreach(Vector3Int vec in r.playerArea){
                    Vector3Int v = vec;
                    v.z = 0;
                    foreach(Tilemap t in FindObjectsOfType<Tilemap>()){
                        t.SetColor(v, Color.white);
                    }
                }
            }
            AreaShown = false;
        }
    }

    */

}
