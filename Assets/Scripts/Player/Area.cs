using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Area : NetworkBehaviour
{
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
                        Debug.Log("Area aufruf 4");
                        t.SetTileFlags(v, TileFlags.None);
                        t.SetColor(v, gameManager.getColor(r.GetComponent<PlayerManager>().playerColor));
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

}
