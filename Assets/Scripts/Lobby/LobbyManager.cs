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

    //Spieler muss auf Server registriert werden, OnStartClient und ähnliches warf Fehler aus
    private bool registered = false;

    void Update() {
        readyBtn.onClick.AddListener(ready);
        if(NetworkServer.active && !registered) {
            playerConnect(networkManager.GetComponent<SaveName>().name);
            registered = true;
        }
    }

    [Server]
    void playerConnect(string playername) {
        playernames.Add(playername, false);
    }


    void ready() {
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.playerColor == -1) return;
    }

}
