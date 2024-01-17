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

    [SerializeField] private TMP_Text steamLobbyText;
    [SerializeField] private GameObject lobbyListElement;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";

    public void loadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    void Start() {
        hostBtn.onClick.AddListener(hostServer);
        connectBtn.onClick.AddListener(connectServer);
        usingSteamBtn.onClick.AddListener(changeUsingSteam);
        if(!SteamManager.Initialized) {
            steamLobbyText.text = "Steam is not active";
            return;
        }
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        reloadList();
    }

    public void reloadList() {
        SteamAPI.Init();
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        int listAdd = 0;
        for (int i = 0; i < friendCount; i++)
        {
            // Get the Steam ID of the friend
            CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);

            // Get information about the friend
            FriendGameInfo_t friendGameInfo;
            if (SteamFriends.GetFriendGamePlayed(friendSteamID, out friendGameInfo)) {
                // Check if the friend is in a lobby
                if (friendGameInfo.m_steamIDLobby != CSteamID.Nil) {
                    Debug.Log("Friend " + friendSteamID + " is in lobby " + friendGameInfo.m_steamIDLobby);
                    GameObject o = null;
                    if(listAdd==0) {
                        o = lobbyListElement;
                        o.SetActive(true);
                    }else {
                        o = Instantiate(lobbyListElement, lobbyListElement.transform.parent);                        
                    }
                    o.GetComponent<Button>().onClick.AddListener(() => join(friendGameInfo.m_steamIDLobby));
                    o.transform.GetChild(0).GetComponent<TMP_Text>().text = "Join " + SteamFriends.GetFriendPersonaName(friendSteamID);

                    listAdd++;
                }
            }
        }
    }

    void join(Steamworks.CSteamID id) {
        manager.GetComponent<SaveHelper>().checkAndSave(SteamFriends.GetPersonaName().ToString());
        manager.changeTransport(steam);
        SteamMatchmaking.JoinLobby(id);
        Debug.Log("Request To Join Lobby");
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
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 10);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK) {Debug.Log("no"); return;}
        Debug.Log("Lobby created");
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        if(!manager.GetComponent<SaveHelper>().checkAndSave(SteamFriends.GetPersonaName().ToString())) return;
        manager.GetComponent<SaveHelper>().checkAndSave(SteamFriends.GetPersonaName().ToString());
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
        manager.changeTransport(steam);
        manager.GetComponent<SaveHelper>().checkAndSave(SteamFriends.GetPersonaName().ToString());
        HostLobby();
        /*if(usingSteamText.text == "using Steam") {
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
                manager.GetComponent<SaveHelper>().checkAndSave(chooseName.text);
                connect.SetActive(false);
                ipAdress.readOnly = true;
                ipAdress.text = "wait for invitation...";
                manager.changeTransport(steam);
            }
        }*/
    }
}
