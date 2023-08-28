using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    
    private int color = -1;
    public int playerColor {
        get{return color;}
        set{color = value;}
    }

    public StandardVolk getVolk() {
        return volk;
    }


    public void setBuilding(int buildingID){
        if(!isLocalPlayer) return;
        spawnHelper.spawnBuilding(volk.id, buildingID, transform, "Building " + buildingID + " " + name);
    }
    

    void Start() {
        if(isLocalPlayer) {
            changePlayername(GameObject.Find("NetworkManager").GetComponent<SaveName>().name);
            spawnHelper = GameObject.Find("___InGame___/----Manager----/HelperScripts").GetComponent<ObjectSpawnHelper>();
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
