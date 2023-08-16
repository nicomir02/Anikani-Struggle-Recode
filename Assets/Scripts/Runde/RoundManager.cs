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
        if(activePlayer.text.Equals(PlayerHelperScript.findLocalPlayer().name)) {
            isActive = true;
        }else {
            isActive = false;
        }
    }

    [Command(requiresAuthority=false)]
    void CMDnextRound() {
        round++;
        if(++curTurn >= list.Count) {
            curTurn=0;
        }
        activePlayer.text = list[curTurn];
        roundText.text = round + "";
        roundChange();
    }

}
