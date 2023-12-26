using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private NetworkManagerAnikani networkManager;
    [SerializeField] private TMP_Text[] playerlist;

    private readonly SyncDictionary<string, bool> playernames = new SyncDictionary<string, bool>();

    [SerializeField] private Button readyBtn;

    [SerializeField] private TMP_Dropdown tribeOptions;

    [SerializeField] private Button blueBtn;
    [SerializeField] private Button redBtn;
    [SerializeField] private Button greenBtn;
    [SerializeField] private Button purpleBtn;

    [SerializeField] private GameObject Lobby;
    [SerializeField] private GameObject Ingame;
    [SerializeField] private GameObject onlineMenu;
    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private RoundManager roundManager;

    [SerializeField] private SaveHelper saveAttributes;

    bool registered = false;

    void Start() {
        Ingame.SetActive(false);
        blueBtn.onClick.AddListener(() => OnButtonClicked(blueBtn));
        redBtn.onClick.AddListener(() => OnButtonClicked(redBtn));
        greenBtn.onClick.AddListener(() => OnButtonClicked(greenBtn));
        purpleBtn.onClick.AddListener(() => OnButtonClicked(purpleBtn));
        tribeOptions.onValueChanged.AddListener(delegate {tribeChange(tribeOptions);});

        

        foreach(StandardVolk volk in saveAttributes.Voelker) {
            string volkname = "";

            char old = '1';
            foreach(char c in volk.name) {
                if(c >= 'A' && c <= 'Z' && old != '1') volkname += " ";
                volkname += c;
                old = c;
            }

            tribeOptions.options.Add(new TMP_Dropdown.OptionData(volkname));
        }

        readyBtn.onClick.AddListener(ready);
    }

    void tribeChange(TMP_Dropdown change) {
        string volkname = "";

        int options = change.value;

        if(options == 0) {
            Random rand = new Random();
            options = rand.Next(1, tribeOptions.options.Count);
        }

        foreach(char c in tribeOptions.options[options].text) {
            if(c != ' ') volkname += c;
        }

        foreach(StandardVolk volk in saveAttributes.Voelker) {
            if(volk.name == volkname) {
                PlayerHelperScript.findLocalPlayer().playerVolk = volk.id;
                break;
            }
        }
    }

    public void OnButtonClicked(Button buttonClicked) {
        if(buttonClicked == blueBtn || buttonClicked == redBtn || buttonClicked == greenBtn || buttonClicked == purpleBtn) {

            TMP_Text buttonTextComponent = buttonClicked.transform.GetChild(0).GetComponent<TMP_Text>();
            PlayerManager pm = PlayerHelperScript.findLocalPlayer();
            if(!buttonTextComponent.text.Contains("-")) {
                if(pm.playerColor != -1) {
                    cmdUpdateButtonText(pm.playerColor, "");
                }

                if(buttonClicked == blueBtn) {
                    pm.playerColor = 0;
                    cmdUpdateButtonText(0, buttonTextComponent.text + " - " + pm.name);
                }else if(buttonClicked == redBtn) {
                    pm.playerColor = 1;
                    cmdUpdateButtonText(1, buttonTextComponent.text + " - " + pm.name);
                }else if(buttonClicked == greenBtn) {
                    pm.playerColor = 2;
                    cmdUpdateButtonText(2, buttonTextComponent.text + " - " + pm.name);
                }else if(buttonClicked == purpleBtn) {
                    pm.playerColor = 3;
                    cmdUpdateButtonText(3, buttonTextComponent.text + " - " + pm.name);
                }
            }
        }
    }

    [Command(requiresAuthority=false)]
    void cmdUpdateButtonText(int btn, string text) {
        rpcUpdateButtonText(btn, text);
    }

    [ClientRpc]
    void rpcUpdateButtonText(int btn, string text) {
        if(text != "") {
            if(btn == 0) {
                blueBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 1) {
                redBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 2) {
                greenBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 3) {
                purpleBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }
        }else {
            if(btn == 0) {
                blueBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = blueBtn.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 1) {
                redBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = redBtn.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 2) {
                greenBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = greenBtn.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 3) {
                purpleBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = purpleBtn.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }
        }
    }

    void Update() {
        if(!registered && NetworkClient.ready) {
            serverPlayerConnect(networkManager.GetComponent<SaveHelper>().name);
            registered = true;
            tribeChange(tribeOptions);
        }
    }
   
    [Command(requiresAuthority=false)]
    public void serverPlayerConnect(string playername) {
        playernames.Add(playername, false);
        syncDictionary();
    }

    
    [ClientRpc]
    void syncDictionary() {
        int i=0;
        foreach(KeyValuePair<string, bool>kvp in playernames) {
            playerlist[i].text = kvp.Key;
            if(kvp.Value == true) playerlist[i].text += " - ready";
            i++;
        }
    }
 

    void ready() {
        

        tribeOptions.interactable = false;

        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.playerColor == -1) return;
        readyPlayer(pm.name);
    }

    [Command(requiresAuthority=false)]
    void readyPlayer(string playername) {
        playernames[playername] = true;
        syncDictionary();
        List<string> saveDictionary = new List<string>();
        foreach(KeyValuePair<string, bool> kvp in playernames) {
            if(kvp.Value == false) return;
            saveDictionary.Add(kvp.Key);
        }

        

        startGame();
        mapBehaviour.startGame();
        roundManager.startGame(saveDictionary);
    }

    [ClientRpc]
    void startGame() {
        Lobby.SetActive(false);
        Ingame.SetActive(true);
        onlineMenu.SetActive(false);
    }

}
