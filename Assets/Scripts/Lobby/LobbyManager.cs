using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using System.Text;
using Unity.VisualScripting;
//using UnityEditor.Build.Content;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private NetworkManagerAnikani networkManager;

    [SerializeField] private GameManager gameManager;

    private Dictionary<string, TMP_Text> playerlist = new Dictionary<string, TMP_Text>();
    private readonly SyncDictionary<string, bool> playernames = new SyncDictionary<string, bool>();
    [SerializeField] private TMP_Text examplePlayerlistElement;
    [SerializeField] private Transform playerlistContent;

    [SyncVar] public int connectedPlayers = 0;

    private bool playerChosen = false;
    [SerializeField] private Button readyBtn;

    [SerializeField] private TMP_Dropdown tribeOptions;

    [SerializeField] private Button chooseColorBtn;
    [SerializeField] private Slider red;
    [SerializeField] private Slider green;
    [SerializeField] private Slider blue;

    [SerializeField] private GameObject Lobby;
    [SerializeField] private GameObject Ingame;
    [SerializeField] private GameObject onlineMenu;
    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private RoundManager roundManager;

    [SerializeField] private SaveHelper saveAttributes;

    [SerializeField] private Sprite randomTribe;

    bool registered = false;

    void Start() {
        Ingame.SetActive(false);
        chooseColorBtn.onClick.AddListener(() => OnButtonClicked(chooseColorBtn));
        tribeOptions.onValueChanged.AddListener(delegate {tribeChange(tribeOptions);});

        red.onValueChanged.AddListener(delegate {valueChanged(red);});
        green.onValueChanged.AddListener(delegate {valueChanged(green);});
        blue.onValueChanged.AddListener(delegate {valueChanged(blue);});
        int i = 1;
        //tribeOptions.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i++).GetComponent<Image>().sprite = randomTribe;
        
        foreach(StandardVolk volk in saveAttributes.Voelker) {
            string volkname = "";

            char old = '1';
            foreach(char c in volk.name) {
                if(c >= 'A' && c <= 'Z' && old != '1') volkname += " ";
                volkname += c;
                old = c;
            }

            tribeOptions.options.Add(new TMP_Dropdown.OptionData(volkname));
            //if(volk.tribeIcon != null) tribeOptions.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i++).GetComponent<Image>().sprite = volk.tribeIcon;
        }

        readyBtn.onClick.AddListener(ready);
    }

    [Command(requiresAuthority=false)]
    public void newPlayer(string player) {
        RPCnewPlayer(player);
        syncDictionary();
    }

    [ClientRpc]
    public void RPCnewPlayer(string player) {
        GameObject o = Instantiate(examplePlayerlistElement.gameObject, playerlistContent);

        TMP_Text text = o.GetComponent<TMP_Text>();
        text.text = player;
        playerlist.Add(player, text);
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
        if(buttonClicked == chooseColorBtn) colorChosing();
    }

    public void valueChanged(Slider changedSlider) {
        colorButtonUpdate();
    }

    private void colorChosing() {
        float redVal = red.value;
        float greenVal = green.value;
        float blueVal = blue.value;
        /*red.interactable = false;
        red.gameObject.SetActive(false);
        green.interactable = false;
        green.gameObject.SetActive(false);
        blue.interactable = false;
        blue.gameObject.SetActive(false);
        chooseColorBtn.gameObject.SetActive(false);*/

        

        chooseColor(redVal, greenVal, blueVal);
    }

    private void colorButtonUpdate() {
        float redVal = red.value;
        float greenVal = green.value;
        float blueVal = blue.value;

        Color color = new Color(red.value, green.value, blue.value, 1f);
        chooseColorBtn.image.color = color;
    }

    [Command(requiresAuthority=false)]
    void cmdUpdateButtonText(int btn, string text) {
        rpcUpdateButtonText(btn, text);
    }

    [ClientRpc]
    void rpcUpdateButtonText(int btn, string text) {
        /*if(text != "") {
            if(btn == 0) {
                player1.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 1) {
                player2.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 2) {
                player3.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }else if(btn == 3) {
                player4.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            }
        }else {
            if(btn == 0) {
                player1.transform.GetChild(0).GetComponent<TMP_Text>().text = player1.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 1) {
                player2.transform.GetChild(0).GetComponent<TMP_Text>().text = player2.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 2) {
                player3.transform.GetChild(0).GetComponent<TMP_Text>().text = player3.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }else if(btn == 3) {
                player4.transform.GetChild(0).GetComponent<TMP_Text>().text = player4.transform.GetChild(0).GetComponent<TMP_Text>().text.Split(" -")[0];
            }
        }*/
    }

    void Update() {
        if(!registered && NetworkClient.ready) {
            string pname = networkManager.GetComponent<SaveHelper>().name;
            if(pname == "" || pname == null) pname = "Random Name " + new Random().Next(0, 1000);
            PlayerHelperScript.findLocalPlayer().changePlayername(pname);
            serverPlayerConnect(pname);

            registered = true;
            tribeChange(tribeOptions);
            if(++connectedPlayers > 1) {
                int i = 0;
                foreach(KeyValuePair<string, bool> kvp in playernames) {
                    if(kvp.Key != pname && i > 0) {
                        GameObject o = Instantiate(examplePlayerlistElement.gameObject, playerlistContent);

                        TMP_Text text = o.GetComponent<TMP_Text>();
                        text.text = kvp.Key;

                        text.color = gameManager.getColorByName(kvp.Key);

                        if(kvp.Value == true) {
                            text.text += " - ready";
                        }

                        playerlist.Add(kvp.Key, text);
                    }else if(kvp.Key != pname && i == 0) {
                        examplePlayerlistElement.text = kvp.Key;
                        playerlist.Add(kvp.Key, examplePlayerlistElement);

                        if(kvp.Value == true) {
                            examplePlayerlistElement.text += " - ready";
                        }
                    }
                    i++;
                }
                newPlayer(pname);
            }else {
                examplePlayerlistElement.text = PlayerHelperScript.findLocalPlayer().name;
                playerlist.Add(PlayerHelperScript.findLocalPlayer().name, examplePlayerlistElement);
                //newPlayer(PlayerHelperScript.findLocalPlayer().name);
            }
            
        }
    }
   
    [Command(requiresAuthority=false)]
    public void serverPlayerConnect(string playername) {      
        playernames.Add(playername, false);

        foreach(PlayerManager pm in FindObjectsOfType<PlayerManager>()) {
            changeName(pm.playerCode, pm.name);
        }
    }

    [ClientRpc]
    public void changeName(int code, string pn) {
        foreach(PlayerManager pm in FindObjectsOfType<PlayerManager>()) {
            if(pm.playerCode == code) {
                pm.name = pn;
            }
        }
    }

    [Command(requiresAuthority=false)]
    void CMDsyncDictionary() {
        syncDictionary();
    }
    
    [ClientRpc]
    void syncDictionary() {
        int i=0;
        foreach(KeyValuePair<string, bool>kvp in playernames) {
            if(!playerlist.ContainsKey(kvp.Key)) continue;
            if(gameManager.getColorByName(kvp.Key) != null) playerlist[kvp.Key].color = gameManager.getColorByName(kvp.Key);
            playerlist[kvp.Key].text = kvp.Key;
            if(kvp.Value == true) playerlist[kvp.Key].text += " - ready";
            i++;
        }
    }
 

    void ready() {
        

        tribeOptions.interactable = false;

        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.playerCode == -1 || !playerChosen) return;
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

    void chooseColor(float red, float green, float blue) {
        Color color = new Color (red, green, blue, 1f);
        playerChosen = true;
        gameManager.setColor(PlayerHelperScript.findLocalPlayer().playerCode, color);
        CMDsyncDictionary();
    }
}
