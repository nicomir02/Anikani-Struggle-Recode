using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class OnlineMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text usingSteamText;
    [SerializeField] private Button usingSteamBtn;

    [SerializeField] private Transport steam;
    [SerializeField] private Transport kcp;

    [SerializeField] private NetworkManagerAnikani manager;

    [SerializeField] private Button hostBtn;

    [SerializeField] private Button connectBtn;
    [SerializeField] private GameObject connect;
    
    [SerializeField] private GameObject lobbyManager;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject onlineMenu;

    [SerializeField] private TMP_InputField chooseName;
    [SerializeField] private TMP_InputField ipAdress;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";

    public void loadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    void Start() {
        hostBtn.onClick.AddListener(hostServer);
        connectBtn.onClick.AddListener(connectServer);
        usingSteamBtn.onClick.AddListener(changeUsingSteam);
        if(!SteamManager.Initialized) return;
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    void connectServer() {
        if(usingSteamText.text == "using Steam") {
            Debug.Log("noch nicht verfügbar");
        }else {
            kcpConnect();
        }
    }

    void hostServer() {
        if(usingSteamText.text == "using Steam") {
            HostLobby();
        }else {
            kcpHost();
        }
    }


    public void HostLobby() {
        Debug.Log("lobby creation");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK) {Debug.Log("no"); return;}
        Debug.Log("Lobby created");
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        manager.changeTransport(steam);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        Debug.Log("Request To Join Lobby");
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if(NetworkServer.active) {return;} 
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        
        manager.StartClient();
        
        
    }

    void connected() {
        //onlineMenu.SetActive(false);
    }




    void kcpConnect() {
        if(manager.GetComponent<SaveHelper>().checkAndSave(chooseName.text)) {
            if(ipAdress.text.Length > 0) {
                manager.networkAddress = ipAdress.text;
            }else {
                manager.networkAddress = "localhost";
            }
            connected();
            manager.StartClient();
            
        }else {
            chooseName.text = "Name is not allowed";
        }
        
    }

    void kcpHost() {
        if(manager.GetComponent<SaveHelper>().checkAndSave(chooseName.text)) {
            connected();
            manager.StartHost();
        }else {
            chooseName.text = "Name is not allowed";
        }
    }

    void changeUsingSteam() {
        
        if(usingSteamText.text == "using Steam") {
            usingSteamText.text = "don't use Steam";
            chooseName.readOnly = false;
            ipAdress.readOnly = false;
            connect.SetActive(true);
            ipAdress.text = "";
            manager.changeTransport(kcp);
        }else {
            if(SteamManager.Initialized) {
                usingSteamText.text = "using Steam";
                chooseName.readOnly = true;
                chooseName.text = SteamFriends.GetPersonaName().ToString();
                connect.SetActive(false);
                ipAdress.readOnly = true;
                ipAdress.text = "wait for invitation...";
                manager.changeTransport(steam);
            }
        }
    }
}
