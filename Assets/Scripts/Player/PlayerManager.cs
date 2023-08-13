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
    Farbe des Spielers:
    0 = Blau
    1 = Rot
    2 = Grün
    3 = Lila
    */
    private int color = -1;
    public int playerColor {
        get{return color;}
        set{color = value;}
    }

    void Start() {
        //if(isLocalPlayer) name = GameObject.Find("NetworkManager").GetComponent<SaveName>().name;
    }

}
