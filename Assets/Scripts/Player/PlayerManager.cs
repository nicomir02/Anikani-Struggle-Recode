using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour {
    /*
    Speicher alles was Spieler spezifisch ist
    PlayerFind Methode findet sich in einem HelperScript "PlayerHelperScript"
    */

    /*
    Volk des Spielers
    0 = Order of Seraph
    1 = Curse Bearer
    ....
    */

    /*
    Idee für Sprite speichern: 
    Scripts für einzelne Völker
    z.B. OrderOfSeraphSprites
        Speicherort für alle Knight Sprites
        Speicherort für alle Archer Sprites
        ...
    player ruft spezifisches Script auf 
        Methode mit Volk übergabe und switch case
    einheitliche speicherart oder ähnliches für aufruf/rückgabe von Spritelists für animationlist
    im script animerger und merger aufrufe für die sprites und bei objekt erstellung teamfarbe übergeben 

    ggf. wenn irgendwann überlaufen der Einheiten möglich noch ein weiteres Script EnemySprites, 
    um alle feindlichen einheiten in eigener Farbe zu generieren 
    möglichkeit 1: get other players, get SpriteSaver, getUnitsInColor()
    möglichkeit 2: keine Ahnung irgendwas anderes 
    */

    [SyncVar] private int volkID = -1;
    private AudioSource backgroundMusic;

    public int playerVolk {
        get{return volkID;}
        set{
            volkID = value;
            Debug.Log(volkID);
            if(backgroundMusic == null) backgroundMusic = GetComponent<AudioSource>();
            foreach(StandardVolk v in FindObjectsOfType<SaveHelper>()[0].Voelker) {
                if(v.id == volkID) {
                    volk = v;
                    if(volk.BackgroundMusicClip != null) {
                        backgroundMusic.clip = volk.BackgroundMusicClip;
                    }else {
                        //Standard Musik
                    }
                    if(backgroundMusic.clip != null) backgroundMusic.Play();
                }
            }
        }
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

    public int interactions = 0;

    public int maxInteractions = 2;

    private ObjectSpawnHelper spawnHelper;

    private TMP_Text woodCounter;
    private TMP_Text stoneCounter;
    private TMP_Text nahrungsCounter;
    private TMP_Text anikaniCounter;

    
    [SyncVar][SerializeField] private int code = -1;
    public int playerCode {
        get{return code;}
        set{changePlayerCode(value);}
    }

    [Command(requiresAuthority=false)]
    public void changePlayerCode(int value) {
        code = value;
    }

    //testen ob Obere Leiste schon geladen für die RessourcenCounter
    private bool eingetragen = false;

    //Hauptgebäude schon gesetzt
    private bool hauptGebaeudeIstGesetzt = false;
    public bool playerHauptgebaeude {
        get{return hauptGebaeudeIstGesetzt;}
    }


    [SerializeField] private List<Ressource> alleRessourcen = new List<Ressource>();
    [SerializeField] private Ressource nahrung;

     //Ressourcen die Spieler hat
    private readonly SyncDictionary<int, float> playerRessourcen = new SyncDictionary<int, float>();
    
    
    public Dictionary<Ressource, float> PlayerRessourcen {
        get{
            Dictionary<Ressource, float> result = new Dictionary<Ressource, float>();
            foreach(KeyValuePair<int, float> kvp in playerRessourcen) {
                foreach(Ressource r in alleRessourcen) {
                    if(r.Id == kvp.Key) {
                        result.Add(r, kvp.Value);
                        break;
                    }
                }
            }
            return result;
        }
    }

    //abfrage ob genug Ressourcen für Kauf vorhanden
    public bool genugRessourceVorhanden(Dictionary<Ressource, float> ress){
        List<int> result1 = new List<int>();
        List<float> result2 = new List<float>();
        foreach(KeyValuePair<Ressource,float> kvp in ress){
            result1.Add(kvp.Key.Id);
            result2.Add(kvp.Value);
            if(kvp.Value == 0) continue;
            if(!playerRessourcen.ContainsKey(kvp.Key.Id) || playerRessourcen[kvp.Key.Id] < kvp.Value){
                return false;
            }
        }
        ressourcenAbzug(result1, result2);
        return true;
    }
    [Command(requiresAuthority=false)]
    public void ressourcenAbzug(List<int> l1, List<float> l2){
        for(int i=0; i<l1.Count; i++){
            if(l2[i] == 0) continue;
            playerRessourcen[l1[i]] -= l2[i];
        }
        ressourcenReload();
    }

    
    public void addRessource(Ressource r, float value){
        if(PlayerHelperScript.findLocalPlayer() != this) return;
        ServerAddRessource(r.Id, value);
    }

    [Command(requiresAuthority=false)]
    private void ServerAddRessource(int r, float value){
        if(playerRessourcen.ContainsKey(r)){ 
            playerRessourcen[r] += value;
        } else {
            playerRessourcen.Add(r, value);
        }
        ressourcenReload();
    }

//Veränderung der Ressource in der oberen Leiste
    [ClientRpc]
    private void ressourcenReload() {
        if(PlayerHelperScript.findLocalPlayer() != this) return;
        foreach(KeyValuePair<int,float> kvp in playerRessourcen){
            foreach(Ressource r in alleRessourcen) {
                if(r.Id == kvp.Key) {
                    Debug.Log(r.name + " " + kvp.Value);
                    if(r.Id == 0) woodCounter.text = Mathf.Round(kvp.Value * 10.0f) * 0.1f + "";
                    if(r.Id == 1) stoneCounter.text = Mathf.Round(kvp.Value * 10.0f) * 0.1f + "";
                    if(r.Id == 2) nahrungsCounter.text = Mathf.Round(kvp.Value * 10.0f) * 0.1f + "";
                    if(r.Id == 3) anikaniCounter.text = Mathf.Round(kvp.Value * 10.0f) * 0.1f + "";
                }
            }
        }
    }


    public StandardVolk getVolk() {
        return volk;
    }

    public void nextRound(RoundManager rm) {
        interactions = 0;
        if(rm.Round == 0) {
            interactions = maxInteractions - 1;
        }
    }

    public void setBuilding(int buildingID) {
        if(!isLocalPlayer) return;
        spawnHelper.spawnBuilding(volk.id, buildingID, transform, volk.getBuildingByID(buildingID).name + " " + name, code);
    }

    public void setUnit(Unit unit, Transform t){
        if(!isLocalPlayer) return;
        spawnHelper.spawnUnit(volk.id, volk.getUnitID(unit), t, unit.name + " " + name, code);
    }
    

    void Start() {
        if(isLocalPlayer) {
            int id = new Random().Next(0, 1000);
            while(true) {
                foreach(PlayerManager players in FindObjectsOfType<PlayerManager>()) {
                    if(players.playerCode == id) {
                        id = new Random().Next(0, 9999);
                        continue;
                    }
                }
                break;
            }

            foreach(StandardVolk v in FindObjectsOfType<SaveHelper>()[0].Voelker) {
                if(v.id == volkID) {
                    volk = v;
                    if(volk.BackgroundMusicClip != null) {
                        backgroundMusic.clip = volk.BackgroundMusicClip;
                    }else {
                        //Standard Musik
                    }
                    if(backgroundMusic.clip != null) backgroundMusic.Play();
                }
            }
            
            playerCode = id;
            backgroundMusic = GetComponent<AudioSource>();
            changePlayername(GameObject.Find("NetworkManager").GetComponent<SaveHelper>().name);
            spawnHelper = GameObject.Find("___InGame___/----Manager----/HelperScripts").GetComponent<ObjectSpawnHelper>();
        }
    }

    void Update() {
        if(!eingetragen && GameObject.Find("___InGame___/Canvas/LeisteOben/WoodCounter") != null) {
            woodCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/WoodCounter").GetComponent<TMP_Text>();
            nahrungsCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/NahrungsCounter").GetComponent<TMP_Text>();
            stoneCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/StoneCounter").GetComponent<TMP_Text>();
            anikaniCounter = GameObject.Find("___InGame___/Canvas/LeisteOben/AnikaniCounter").GetComponent<TMP_Text>();
            eingetragen = true;
            addRessource(nahrung, 10f);
            maxInteractions = volk.StandardMaxInteractions;
        }

        if(volk == null && volkID != -1) {
            foreach(StandardVolk v in FindObjectsOfType<SaveHelper>()[0].Voelker) {
                if(v.id == volkID) {
                    volk = v;
                    if(volk.BackgroundMusicClip != null) {
                        backgroundMusic.clip = volk.BackgroundMusicClip;
                    }else {
                        //Standard Musik
                    }
                    if(backgroundMusic.clip != null) backgroundMusic.Play();
                }
            }
        }
        
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
    
