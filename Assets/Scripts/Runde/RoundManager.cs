using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class RoundManager : NetworkBehaviour
{
    [SyncVar] private int round;
    [SyncVar] private int curTurn = 0;
    public readonly SyncList<string> list = new SyncList<string>();
    private static Random rand = new Random();

    [SerializeField] private TMP_Text activePlayer;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Button nextRoundBtn;

    [SerializeField] private GameManager gameManager;

    private bool isActive = false;


    

    void nextRound() {
        //Hier checken ob nächste Runde erlaubt ist:

        if(isActive) {
            CMDnextRound();
        }
    }

    void Start() {
        nextRoundBtn.onClick.AddListener(nextRound);
    }

    [Command(requiresAuthority=false)]
    public void startGame(List<string> listOfPlayers) {
        while(listOfPlayers.Count > 0) {
            int value = rand.Next(listOfPlayers.Count-1);
            list.Add(listOfPlayers[value]);
            listOfPlayers.Remove(listOfPlayers[value]);
        }

        activePlayer.text = list[0];
        round = 0;
        roundText.text = round + "";
        
        roundChange();
    }

    [ClientRpc]
    void roundChange() {
        roundText.text = round + "";
        activePlayer.text = list[curTurn];
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        //schauen ob Spieler aktiv
        if(activePlayer.text.Equals(pm.name)) {
            isActive = true;
            CMDchangeFarbe(pm.playerColor);
            if(round == 0){
                pm.setMainbuilding();
            }
        }else {
            isActive = false;
        }
    }

    [Command(requiresAuthority=false)]
    void CMDchangeFarbe(int id) {
        changeFarbe(id);
    }

    [ClientRpc]
    void changeFarbe(int id) {
        activePlayer.color = gameManager.getColor(id);
    }

    [Command(requiresAuthority=false)]
    void CMDnextRound() {
        if(++curTurn >= list.Count) {
            curTurn=0;
            round++;
        }
        activePlayer.text = list[curTurn];
        roundText.text = round + "";
        roundChange();
    }

}
