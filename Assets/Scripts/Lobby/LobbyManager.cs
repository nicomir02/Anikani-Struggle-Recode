using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private NetworkManagerAnikani networkManager;
    [SerializeField] private TMP_Text[] playerlist;

    public readonly SyncList<string> playernames = new SyncList<string>();

    [SerializeField] private Button readyBtn;

    [SerializeField] private GameObject Lobby;
    [SerializeField] private GameObject Ingame;
    [SerializeField] private MapBehaviour mapBehaviour;

    void Start() {
        readyBtn.onClick.AddListener(ready);
    }

    void ready() {
        //Neuer Code später
        Ingame.SetActive(true);
        mapBehaviour.createTerrain();
        Lobby.SetActive(false);
    }
}
