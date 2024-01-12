using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoralManager : NetworkBehaviour
{
    private List<Moral> unitMoral = new List<Moral>();
    private List<Moral> buildingMoral = new List<Moral>();

    private List<Moral> allgemeinMoral = new List<Moral>();

    private List<Moral> healer = new List<Moral>();

    [SerializeField] RoundManager rm;

    [SerializeField] Ressource anikani;

    float gesamtMoral = 0.5f;
    [SerializeField] private Moralbar moralbar;

    [SerializeField] private Moralskala moralskala;


    void Start() {
        unitMoral.Add(new Moral_Heimweh());
        unitMoral.Add(new Moral_Attack());

        allgemeinMoral.Add(new Moral_Schaden());
        allgemeinMoral.Add(new Moral_Food());

        healer.Add(new Moral_Heal());
    }

    [Command(requiresAuthority=false)]
    public void nextRound(PlayerManager pm) {
        if(rm.Round == 0) return;
        float moral = 0f;
        int howMany = 0;
        
        foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
            if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                moral += vo.moralAuswertung();
                howMany++;
                //neu dazu für Moralauswirkungen
                
                if (vo.prevMoralAnpassung()){    //Anpassung der Variable prevMoralWert
                    //vo.moralAuswirkung();       //Auswirkungen durch Moralveränderung auf Attribute(erstmal nur Attribute) methode direkt in moralskala implementiert momentan
                    moralskala.checkSkala(vo, vo.MoralSkalenKategorieWert);    //getter für moralKategoiewert übergeben an moralskala skript
                }
            }
        }
        //für alle inherited classes auch, da casten (vo as Unit) nicht klappt:
        //Unit
        foreach(Unit u in FindObjectsOfType<Unit>()){
            if(u.transform.parent.GetComponent<PlayerManager>() == pm) {
                Debug.Log(u.transform.parent);
                Debug.Log("Unit");
                Debug.Log(u.GetType());
                if (u.prevMoralAnpassung()){    //Anpassung der Variable prevMoralWert
                    moralskala.checkSkalaUnit(u, u.MoralSkalenKategorieWert);    //getter für moralKategoiewert übergeben an moralskala skript
                }
            }
        }
        //Building
        foreach(MainBuilding mb in FindObjectsOfType<Building>()){
            if(mb.transform.parent.GetComponent<PlayerManager>() == pm) {
                Debug.Log(mb.transform.parent);
                Debug.Log("Building");
                Debug.Log(mb.GetType());
                /*if (mb.prevMoralAnpassung()){    //Anpassung der Variable prevMoralWert
                    moralskala.checkSkalaMainBuilding(mb, mb.MoralSkalenKategorieWert);    //getter für moralKategoiewert übergeben an moralskala skript
                }*/
            }
        }

        //Ressourcensammler
        foreach(RessourcenSammler rs in FindObjectsOfType<RessourcenSammler>()){
            if(rs.transform.parent.GetComponent<PlayerManager>() == pm) {
                Debug.Log(rs.transform.parent);
                Debug.Log("RessourcenSammler");
                Debug.Log(rs.GetType());
                if (rs.prevMoralAnpassung()){    //Anpassung der Variable prevMoralWert
                    moralskala.checkSkalaRessourcenSammler(rs, rs.MoralSkalenKategorieWert);    //getter für moralKategoiewert übergeben an moralskala skript
                }
            }
        }

        //TrainUnitBuilding
        foreach(TrainUnitBuilding tub in FindObjectsOfType<TrainUnitBuilding>()){
            if(tub.transform.parent.GetComponent<PlayerManager>() == pm) {
                Debug.Log(tub.transform.parent);
                Debug.Log("TrainUnitBuilding");
                Debug.Log(tub.GetType());
                if (tub.prevMoralAnpassung()){    //Anpassung der Variable prevMoralWert
                    moralskala.checkSkalaTrainUnitBuilding(tub, tub.MoralSkalenKategorieWert);    //getter für moralKategoiewert übergeben an moralskala skript
                }
            }
        }

        float newGesamtMoral = moral/howMany;
        moralbar.Change(newGesamtMoral-gesamtMoral);
        gesamtMoral = newGesamtMoral;

        if(newGesamtMoral > 0.9f) {
            pm.addRessource(anikani, 0.5f);
        }
    }

    public List<Moral> getMoral(int volkObject, bool heal=false) {
        //volkObject = 0 = Unit
        //volkObject = 1 = Building
        List<Moral> auswahl = new List<Moral>(); 
        if(volkObject==0) {
            foreach(Moral m in unitMoral) {
                auswahl.Add(m);
            }
        }else if(volkObject==1){
            foreach(Moral m in buildingMoral) {
                auswahl.Add(m);
            }
        }
        foreach(Moral m in allgemeinMoral) {
            auswahl.Add(m);
        }

        if(heal) {
            foreach(Moral m in healer) {
                auswahl.Add(m);
            }
        }
        
        foreach(Moral m in auswahl) {
            m.setCase();
        }

        List<Moral> result = new List<Moral>(); 
        foreach(Moral m in auswahl) {
            result.Add((Moral) m.Clone());
        }



        return result; //später zufällig 2-3 Vorlieben..
        
    }
}