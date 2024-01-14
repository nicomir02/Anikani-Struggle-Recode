using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoralManager : NetworkBehaviour
{
    private Dictionary<Moral, Sprite> unitMoral = new Dictionary<Moral, Sprite>();
    private Dictionary<Moral, Sprite> buildingMoral = new Dictionary<Moral, Sprite>();

    private Dictionary<Moral, Sprite> allgemeinMoral = new Dictionary<Moral, Sprite>();

    private Dictionary<Moral, Sprite> healer = new Dictionary<Moral, Sprite>();

    [SerializeField] RoundManager rm;

    [SerializeField] Ressource anikani;

    [SerializeField] private Sprite heimweh;
    [SerializeField] private Sprite attack;
    [SerializeField] private Sprite optimism;
    [SerializeField] private Sprite schaden;
    [SerializeField] private Sprite food;
    [SerializeField] private Sprite heal;

    float gesamtMoral = 0.5f;
    [SerializeField] private Moralbar moralbar;

    //noch MOralskala an ein Gameobject anfügen: [SerializeField] private Moralskala ms;

    void Start() {
        unitMoral.Add(new Moral_Heimweh(), heimweh);
        unitMoral.Add(new Moral_Attack(), attack);
        unitMoral.Add(new Moral_Optimism(), optimism);
        allgemeinMoral.Add(new Moral_Schaden(), schaden);
        allgemeinMoral.Add(new Moral_Food(), food);

        healer.Add(new Moral_Heal(), heal);
    }

    public Sprite getSprite(Moral m) {
        foreach(KeyValuePair<Moral, Sprite> kvp in unitMoral) {
            if(kvp.Key.overviewName == m.overviewName) return kvp.Value;
        }
        foreach(KeyValuePair<Moral, Sprite> kvp in buildingMoral) {
            if(kvp.Key.overviewName == m.overviewName) return kvp.Value;
        }
        foreach(KeyValuePair<Moral, Sprite> kvp in allgemeinMoral) {
            if(kvp.Key.overviewName == m.overviewName) return kvp.Value;
        }
        foreach(KeyValuePair<Moral, Sprite> kvp in healer) {
            if(kvp.Key.overviewName == m.overviewName) return kvp.Value;
        }
        return null;
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
        moralbar.Change(newGesamtMoral);
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
            foreach(Moral m in unitMoral.Keys) {
                auswahl.Add(m);
            }
        }else if(volkObject==1){
            foreach(Moral m in buildingMoral.Keys) {
                auswahl.Add(m);
            }
        }
        foreach(Moral m in allgemeinMoral.Keys) {
            auswahl.Add(m);
        }

        if(heal) {
            foreach(Moral m in healer.Keys) {
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