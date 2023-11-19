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

    [SyncVar] private int activeID = -1;
    public int ActiveID {
        get{return activeID;}
    }

    public readonly SyncList<string> list = new SyncList<string>();
    private static Random rand = new Random();

    [SerializeField] private TMP_Text activePlayer;

    public string aktiverSpieler {
        get{return activePlayer.text;}
    }

    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Button nextRoundBtn;

    [SerializeField] private UnityEngine.Rendering.Universal.Light2D fogOfWar;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private MoralManager moralManager;

    [SerializeField] private Ressource nahrungsRessource;

    private bool isActive = false;


    public bool isYourTurn() {
        return isActive;
    }

    

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
        if(round == 1) {
            fogOfWar.color = Color.black;
        }
        if(activePlayer.text.Equals(pm.name)) {
            isActive = true;
            changeActivePlayerID(pm.playerColor);
            CMDchangeFarbe(pm.playerColor);
            if(round == 0){
                pm.setBuilding(0);  //0 ist die ID für MainBuilding
                //alexänderung
                /* if(!pm.playerHauptgebaeude){
                    isActive = false;
                    pm.setBuilding(0);
                } else {
                    isActive = true;
                } */
            }
            //count für Nahrung
            float count = 0f;
            float nahrung = 0f;
            if(pm.PlayerRessourcen.ContainsKey(nahrungsRessource)) {
                nahrung = pm.PlayerRessourcen[nahrungsRessource];
            }
            
            float testNahrung = nahrung;

            foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                    vo.nextRound();
                    
                    float f = vo.NeededFood;
                    count += f;

                    if(nahrung >= 0.5f && vo.CurrentNahrung < 0.5f) {
                        vo.CurrentNahrung = 0.5f;
                        nahrung -= 0.5f;
                    }
                    
                }
            }

            
            //Automatische Nahrungsverteilung Später auch manuelle lösung......
            if(count <= testNahrung) {
                nahrung = testNahrung;
                foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                    if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                        vo.CurrentNahrung = vo.NeededFood;
                        nahrung -= vo.NeededFood;
                    }
                }
            }else {
                if(nahrung > 0.5f) {
                    foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                        if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                            if(nahrung >= 0.5f && vo.NeededFood > 0.5f) {
                                vo.CurrentNahrung += 0.5f;
                                nahrung -= 0.5f;
                            }else {
                                break;
                            }
                        }
                    }
                    if(nahrung > 1f) {
                        foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                            if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                                if(nahrung >= 1f && vo.NeededFood > 1f) {
                                    vo.CurrentNahrung += 1f;
                                    nahrung -= 1f;
                                }else {
                                    break;
                                }
                            }
                        }
                    }
                }

                if(nahrung > 0f) {
                    foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                        if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                            if(nahrung >= 0f && vo.NeededFood > vo.CurrentNahrung) {
                                float berechnen = Mathf.Min(nahrung, vo.NeededFood-vo.CurrentNahrung);
                                vo.CurrentNahrung += berechnen;
                                nahrung -= berechnen;
                            }else {
                                break;
                            }
                        }
                    }
                }
            }
            Debug.Log("abzug:"+(testNahrung-nahrung));
            Dictionary<Ressource, float> abzug = new Dictionary<Ressource, float>();
            abzug.Add(nahrungsRessource, testNahrung-nahrung);
            pm.genugRessourceVorhanden(abzug);

            foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                    //5 Abschnitte jeweils 3% von maxhealth verlieren, maximal 15%
                    if(vo.CurrentNahrung < vo.NeededFood*0.25f) vo.changeHealth(Mathf.Round(-vo.maxHealth*0.03f));
                    if(vo.CurrentNahrung < vo.NeededFood*0.2f) vo.changeHealth(Mathf.Round(-vo.maxHealth*0.03f));
                    if(vo.CurrentNahrung < vo.NeededFood*0.15f) vo.changeHealth(Mathf.Round(-vo.maxHealth*0.03f));
                    if(vo.CurrentNahrung < vo.NeededFood*0.1f) vo.changeHealth(Mathf.Round(-vo.maxHealth*0.03f));
                    if(vo.CurrentNahrung < vo.NeededFood*0.05f) vo.changeHealth(Mathf.Round(-vo.maxHealth*0.03f));
                        
                        
                }
            }

            //Ende Nahrungsverteilung



            

            



            moralManager.nextRound(pm);
        }else {
            isActive = false;
        }
    }

    [Command(requiresAuthority=false)]
    public void changeActivePlayerID(int id) {
        activeID = id;
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
