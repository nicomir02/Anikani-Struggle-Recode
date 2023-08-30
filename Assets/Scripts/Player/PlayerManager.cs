using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour
{

   

    /*
    Speicher alles was Spieler spezifisch ist
    PlayerFind Methode findet sich in einem HelperScript "PlayerHelperScript"
    */


    /*
    Volk des Spielers
    0 = Order of Seraphs
    1 = Ancient Wanderers
    ....

    */

    private int volkID = -1;

    public int playerVolk {
        get{return volkID;}
        set{volkID = value;}
    }
    //nur zum testen. später in der lobby setzen
    [SerializeField] private StandardVolk volk;

    /*
    Farbe des Spielers:
    0 = Blau
    1 = Rot
    2 = Grün
    3 = Lila
    */

    private ObjectSpawnHelper spawnHelper;

    private TMP_Text woodCounter;
    private TMP_Text stoneCounter;

    
    private int color = -1;
    public int playerColor {
        get{return color;}
        set{color = value;}
    }

    //testen ob Obere Leiste schon geladen für die RessourcenCounter
    private bool eingetragen = false;

    //Hauptgebäude schon gesetzt
    private bool hauptGebaeudeIstGesetzt = false;
    public bool playerHauptgebaeude {
        get{return hauptGebaeudeIstGesetzt;}
    }

     //Ressourcen die Spieler hat
    private Dictionary<Ressource, int> playerRessourcen = new Dictionary<Ressource, int>();
    //abfrage ob genug Ressourcen für Kauf vorhanden
    public bool genugRessourceVorhanden(Dictionary<Ressource, int> ress){
        foreach(KeyValuePair<Ressource,int> kvp in ress){
            if(kvp.Value == 0) continue;
            if(!playerRessourcen.ContainsKey(kvp.Key) || playerRessourcen[kvp.Key] < kvp.Value){
                return false;
            }
        }
        ressourcenAbzug(ress);
        return true;
    }
    
    private void ressourcenAbzug(Dictionary<Ressource, int> ress){
        foreach(KeyValuePair<Ressource,int> kvp in ress){
            if(kvp.Value == 0) continue;
            playerRessourcen[kvp.Key] -= kvp.Value;
        }
        ressourcenReload();
    }

    public void addRessource(Ressource r, int value){
        if(playerRessourcen.ContainsKey(r)){ 
            playerRessourcen[r] += value;
        } else {
            playerRessourcen.Add(r, value);
        }
        ressourcenReload();
    }

//Veränderung der Ressource in der oberen Leiste
    private void ressourcenReload() {
        foreach(KeyValuePair<Ressource,int> kvp in playerRessourcen){
            if(kvp.Key.name == "Tree") {
                woodCounter.text = kvp.Value + "";
            }else if(kvp.Key.name == "Stone") {
                stoneCounter.text = kvp.Value + "";
            }
        }
    }


    public StandardVolk getVolk() {
        return volk;
    }


    public void setBuilding(int buildingID){
        if(!isLocalPlayer) return;
        spawnHelper.spawnBuilding(volk.id, buildingID, transform, volk.getBuildingByID(buildingID).name + " " + name);
    }
    

    void Start() {
        if(isLocalPlayer) {
            changePlayername(GameObject.Find("NetworkManager").GetComponent<SaveName>().name);
            spawnHelper = GameObject.Find("___InGame___/----Manager----/HelperScripts").GetComponent<ObjectSpawnHelper>();
        }
    }

    void Update() {
        if(!eingetragen && GameObject.Find("___InGame___/Canvas/LeisteOben/WoodCounter") != null) {
            woodCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/WoodCounter").GetComponent<TMP_Text>();
            stoneCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/StoneCounter").GetComponent<TMP_Text>();
            eingetragen = true;
        }

        //alexänderung
        /* if(!hauptGebaeudeIstGesetzt){
            RoundManager rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();
            foreach(Building b in FindObjectsOfType<Building>()) {
                if(b.GetComponent<CastleMainBuilding>() == null) continue;
                string activePlayer = "";
                if(name == rm.aktiverSpieler) {
                    activePlayer = name;
                    break;
                }
                if(b.name == name + " " + activePlayer) {
                    hauptGebaeudeIstGesetzt = true;
                }
            }
        } */


    }

    [Command(requiresAuthority=false)]
    public void changePlayername(string pname) {
        name = pname;
        rpcChangeName(pname);
    }

    [ClientRpc]
    public void rpcChangeName(string pname) {
        name = pname;
    }

}
