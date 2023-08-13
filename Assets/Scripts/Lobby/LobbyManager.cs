using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private NetworkManagerAnikani networkManager;
    [SerializeField] private TMP_Text[] playerlist;

    public readonly SyncDictionary<string, bool> playernames = new SyncDictionary<string, bool>();

    [SerializeField] private Button readyBtn;

    [SerializeField] private GameObject Lobby;
    [SerializeField] private GameObject Ingame;
    [SerializeField] private MapBehaviour mapBehaviour;
   
    [Command(requiresAuthority=false)]
    public void serverPlayerConnect(string playername) {
        playernames.Add(playername, false);
    }

    
    /*[ClientRpc]
    void syncDictionary() {
        int i=0;
        foreach(KeyValuePair<string, bool>kvp in playernames) {
            playerlist[i].text = kvp.Key;
            if(kvp.Value == true) playerlist[i].text += " - ready";
            i++;
        }
    }*/ 

    void ready() {
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.playerColor == -1) return;
    }

}
