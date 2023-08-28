using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Building : NetworkBehaviour
{
    //standardmäßg 3 gebäude radius (= 3x3 Feld)
    [SerializeField] private int radius = 3;
    //ob Gebäude schon gesetzt
    private bool gesetzt = false;
    
    //Mouseposition
    private TilemapHover th;
    //mapBehaviour
    private MapBehaviour mb;

    //RadiusNeighborCheck
    private RadiusNeighborCheck radiusNeighborCheck;

    private PlayerManager pm;
    public PlayerManager playerManager {
        get{return pm;}
        set{pm = value;}
    }

    //Leben/Haltbarkeit des Gebäudes
    [SerializeField] private int health;

    //position des Gebäudes
    private List<Vector3Int> position = new List<Vector3Int>();

    //liste der zerstörten Ressourcen unter Gebäude
    private Dictionary<Ressource, int> ressourcen = new Dictionary<Ressource, int>();

    // Start is called before the first frame update
    void Start()
    {
        th = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        mb = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        radiusNeighborCheck = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<RadiusNeighborCheck>(); 

        transparencyChange(0.5f);
    }

    // Update is called once per frame
    private void Update()
    {
        if(pm == null || pm != PlayerHelperScript.findLocalPlayer()) return;
        Vector3Int vec = th.getMousePosition();
        vec.x -= (radius-1)/2;
        vec.y -= (radius-1)/2;
        Vector3 newPos = th.getVec3FromVec3Int(vec);
        newPos.z += 1;
        if(!gesetzt){
            transform.position = newPos;
        }

        if(Input.GetMouseButtonDown(0) && !gesetzt && setzbar()){
            transform.position = newPos;
            vec.x += (radius-1)/2;
            vec.y += (radius-1)/2;
            gebaeudeBau(vec);
            gesetzt = true;
            CMDtransparencyChange(1f);
        }
        //if(!setzbar()) Debug.Log("nicht setzbar");
    }
    
    public List<Vector3Int> getPosition() {
        return position;
    }

    [Command(requiresAuthority=false)]
    public void CMDtransparencyChange(float f, GameObject o) {
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;
        RPCtransparencyChange(f);
        NetworkServer.Spawn(o);
    }

    [ClientRpc]
    public void RPCtransparencyChange(float f) {
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    private void transparencyChange(float f){
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    private bool setzbar(){
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
}
