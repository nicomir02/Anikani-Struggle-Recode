using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;
using System.Linq;
using System.Threading;
using System;

public class MarketBuilding : Building
{
    //für Moralauswirkungen StandardmaxKaeufe eingeführt
    [SerializeField] private int standardMaxKaeufe = 4;
    private int maxKaeufe = 4;  
    //anzahl wie weit in vergangenheit geschaut werden soll für die Preisanpassung
    [SerializeField] private int rueckblickAnzahl = 10;
    //Liste der letzten rueckblickAnzahl Käufe.
    private Dictionary<int, Unit> kaeufe = new Dictionary<int, Unit>();

    [SerializeField] private AudioClip buySound;
    
    //Anzahl der Käufe limitieren pro Runde?
    public bool istVoll {
        get{return kaeufe.Count >= maxKaeufe;}
    }
    //Kaufliste
    public Dictionary<int, Unit> Kaeufe {
        get{return kaeufe;}
    }

    private int zahl = 0;
    public int Zahl {
        get{return zahl;}
    }

    public override void Start(){
        base.Start();
        maxKaeufe = standardMaxKaeufe; //ändern falls es maxkäufe später gibt für die Moral
        //loadTrainEnd();
        Debug.Log("Start called");
    } 

    public override void selected() {
        base.selected();    //führt sachen aus selected Methode von Building aus.
        //Jetzt spezielle Aktionen für MarketBuilding
    }
    

    public override void nextRound() {
        base.nextRound();
        /*if(ausbildungen.Count > 0) zahl++;
        if(ausbildungen.ContainsKey(zahl)) {
            //Ausbildung gespawned;
            PlayerManager pm = PlayerHelperScript.findLocalPlayer();
            animationList = trainEnd;
            if(buySound != null) {
                GetComponent<AudioSource>().clip = buySound;
                GetComponent<AudioSource>().Play();
            }
            pm.setUnit(ausbildungen[zahl], transform);
            ausbildungen.Remove(zahl);//um dictionary zu pflegen und nicht größer werden zu lassen
            if(ausbildungen.Count <= 0) {
                zahl = 0;
            }
        }*/
    }
    //marketanimation
    /*
    [Command(requiresAuthority=false)]
    public void trainAnimation() {
        animationList = trainEnd;
        RpcTrainAnimation();
    }

    public void RpcTrainAnimation() {
        animationList = trainEnd;
        animationStart(0, 0);
    }
    */

    

    //private List<Sprite> trainEnd = new List<Sprite>();
   /* private void loadTrainEnd() {
        transform.GetChild(5).GetComponent<AniSpriteMerger>().aniMerge();
        trainEnd = transform.GetChild(5).GetComponent<AniSpriteMerger>().GetList(); 
    }*/

//kaufmethode
/*
    public bool addTrainUnit(Unit u) {
        if(kaeufe.Count >= maxKaeufe) return false;
        if(kaeufe.Count <= 0) {
            kaeufe.Add(u.getAusbildungszeit(), u);
        }else {
            int highestValue = kaeufe.Keys.Max();
            //Debug.Log("insert " + kaeufe.Count);

            kaeufe.Add(highestValue + u.getAusbildungszeit(), u);
        }
        return true;
    }*/

//Getter und Setter für Moralauswirkungen wenn später notwendig für maxkaeufe


    public int MaxKaeufe {
        get{return maxKaeufe;}
        set{maxKaeufe = value;}
    }
    public int StandardMaxKaeufe {
        get{return standardMaxKaeufe;}
    }
    
}
