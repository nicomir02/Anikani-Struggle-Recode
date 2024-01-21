using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;
using System.Linq;
using System.Threading;
using System;

public class TrainUnitBuilding : Building
{
    //für Moralauswirkungen StandardmaxAubildungen eingeführt
    [SerializeField] private int standardMaxAusbildungen = 4;
    private int maxAusbildungen = 4;  

    //Liste der aktuellen auszubildenden Einheiten und die Anzahl der noch benötigten Runden.
    private Dictionary<int, Unit> ausbildungen = new Dictionary<int, Unit>();

    [SerializeField] private AudioClip trainEndSound;
    
    public bool istVoll {
        get{return ausbildungen.Count >= maxAusbildungen;}
    }

    public Dictionary<int, Unit> Ausbildungen {
        get{return ausbildungen;}
    }

    private int zahl = 0;
    public int Zahl {
        get{return zahl;}
    }

    public override void Start(){
        base.Start();
        maxAusbildungen = standardMaxAusbildungen;
        loadTrainEnd();
        Debug.Log("Start called");
    } 

    public override void selected() {
        base.selected();    //führt sachen aus selected Methode von Building aus.
        //Jetzt spezielle Aktionen für TrainUnitBuildings wie z.B. Barracke
    }
    

    public override void nextRound() {
        base.nextRound();
        if(ausbildungen.Count > 0) zahl++;
        if(ausbildungen.ContainsKey(zahl)) {
            //Ausbildung gespawned;
            PlayerManager pm = PlayerHelperScript.findLocalPlayer();
            animationList = trainEnd;
            if(trainEndSound != null) {
                GetComponent<AudioSource>().clip = trainEndSound;
                GetComponent<AudioSource>().Play();
            }
            pm.setUnit(ausbildungen[zahl], transform);
            ausbildungen.Remove(zahl);//um dictionary zu pflegen und nicht größer werden zu lassen
            if(ausbildungen.Count <= 0) {
                zahl = 0;
            }
        }
    }

    /*
    Erklärung für nextROundMethode:
    zahl++
    im dictionary:
    3, Knight
    6, Knight
    10, Angel
    */

    private List<Sprite> trainEnd = new List<Sprite>();
    private void loadTrainEnd() {
        transform.GetChild(5).GetComponent<AniSpriteMerger>().aniMerge();
        trainEnd = transform.GetChild(5).GetComponent<AniSpriteMerger>().GetList(); 
    }

    public bool addTrainUnit(Unit u) {
        if(ausbildungen.Count >= maxAusbildungen) return false;
        if(ausbildungen.Count <= 0) {
            ausbildungen.Add(u.getAusbildungszeit(), u);
        }else {
            int highestValue = ausbildungen.Keys.Max();
            //Debug.Log("insert " + ausbildungen.Count);

            ausbildungen.Add(highestValue + u.getAusbildungszeit(), u);
        }
        return true;
    }

//Getter und Setter für Moralauswirkungen
    public int MaxAusbildungen {
        get{return maxAusbildungen;}
        set{maxAusbildungen = value;}
    }
    public int StandardMaxAusbildungen {
        get{return standardMaxAusbildungen;}
    }
}
