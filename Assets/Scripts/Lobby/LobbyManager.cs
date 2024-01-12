using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Build.Content;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private NetworkManagerAnikani networkManager;
    [SerializeField] private TMP_Text[] playerlist;

    [SerializeField] private GameManager gameManager;

    private readonly SyncDictionary<string, bool> playernames = new SyncDictionary<string, bool>();

    private bool playerChosen = false;
    [SerializeField] private Button readyBtn;

    [SerializeField] private TMP_Dropdown tribeOptions;

    [SerializeField] private Button player1;
    [SerializeField] private Button player2;
    [SerializeField] private Button player3;
    [SerializeField] private Button player4;

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

    bool registered = false;

    void Start() {
        Ingame.SetActive(false);
        player1.onClick.AddListener(() => OnButtonClicked(player1));
        player2.onClick.AddListener(() => OnButtonClicked(player2));
        player3.onClick.AddListener(() => OnButtonClicked(player3));
        player4.onClick.AddListener(() => OnButtonClicked(player4));
        chooseColorBtn.onClick.AddListener(() => OnButtonClicked(chooseColorBtn));
        tribeOptions.onValueChanged.AddListener(delegate {tribeChange(tribeOptions);});

        red.interactable = false;
        red.gameObject.SetActive(false);
        red.onValueChanged.AddListener(delegate {valueChanged(red);});
        green.interactable = false;
        green.gameObject.SetActive(false);
        green.onValueChanged.AddListener(delegate {valueChanged(green);});
        blue.interactable = false;
        blue.gameObject.SetActive(false);
        blue.onValueChanged.AddListener(delegate {valueChanged(blue);});
        chooseColorBtn.gameObject.SetActive(false);

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
        if(buttonClicked == player1 || buttonClicked == player2 || buttonClicked == player3 || buttonClicked == player4) {
            if (playerChosen) {}
            else {
                playerChosen = true;
                TMP_Text buttonTextComponent = buttonClicked.transform.GetChild(0).GetComponent<TMP_Text>();
                PlayerManager pm = PlayerHelperScript.findLocalPlayer();
                if(!buttonTextComponent.text.Contains("-")) {
                    if(pm.playerCode != -1) {
                        cmdUpdateButtonText(pm.playerCode, "");
                    }

                    if(buttonClicked == player1) {
                        pm.playerCode = 0;
                        cmdUpdateButtonText(0, buttonTextComponent.text + " - " + pm.name);
                    }else if(buttonClicked == player2) {
                        pm.playerCode = 1;
                        cmdUpdateButtonText(1, buttonTextComponent.text + " - " + pm.name);
                    }else if(buttonClicked == player3) {
                        pm.playerCode = 2;
                        cmdUpdateButtonText(2, buttonTextComponent.text + " - " + pm.name);
                    }else if(buttonClicked == player4) {
                        pm.playerCode = 3;
                        cmdUpdateButtonText(3, buttonTextComponent.text + " - " + pm.name);
                    }
                }
                red.interactable = true;
                red.gameObject.SetActive(true);
                green.interactable = true;
                green.gameObject.SetActive(true);
                blue.interactable = true;
                blue.gameObject.SetActive(true);
                chooseColorBtn.gameObject.SetActive(true);
            }
        }
        if(buttonClicked == chooseColorBtn) colorChosing();
    }

    public void valueChanged(Slider changedSlider) {
        colorButtonUpdate();
    }

    private void colorChosing() {
        float redVal = red.value;
        float greenVal = green.value;
        float blueVal = blue.value;
        red.interactable = false;
        red.gameObject.SetActive(false);
        green.interactable = false;
        green.gameObject.SetActive(false);
        blue.interactable = false;
        blue.gameObject.SetActive(false);
        chooseColorBtn.gameObject.SetActive(false);

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
        if(text != "") {
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
        playername = "RandomName " + new System.Random().Next(0, 1000);
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
        if(pm.playerCode == -1) return;
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

        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if (pm.playerCode == -1 || !playerChosen) {
            //Textfeld please select Player number
        }
        else {
            int num = pm.playerCode;
            if (num == 0) player1.GetComponent<Image>().color = color;
            if (num == 1) player2.GetComponent<Image>().color = color;
            if (num == 2) player3.GetComponent<Image>().color = color;
            if (num == 3) player4.GetComponent<Image>().color = color;

            //set Color to gameFarben Liste
            gameManager.setColor(num, color);
        }
    }
}
