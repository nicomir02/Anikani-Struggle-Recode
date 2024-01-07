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

    //noch MOralskala an ein Gameobject anfügen: [SerializeField] private Moralskala ms;

    void Start() {
        unitMoral.Add(new Moral_Heimweh());
        unitMoral.Add(new Moral_Attack());
        unitMoral.Add(new Moral_Optimism());
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
                vo.prevMoralAnpassung();    //Anpassung der Variable prevMoralWert
                //vo.moralAuswirkung();       //Auswirkungen durch Moralveränderung auf Attribute(erstmal nur Attribute)
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