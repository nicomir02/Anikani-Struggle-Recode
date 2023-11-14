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

    void Start() {
        unitMoral.Add(new Moral_Heimweh());
        unitMoral.Add(new Moral_Attack());

        allgemeinMoral.Add(new Moral_Schaden());

        healer.Add(new Moral_Heal());
    }

    [Command(requiresAuthority=false)]
    public void nextRound(PlayerManager pm) {
        float moral = 0f;
        int howMany = 0;
        foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
            if(vo.transform.parent.GetComponent<PlayerManager>() == pm) {
                moral = 0f;
                howMany = 0;
                foreach(Moral m in vo.Vorlieben) {
                    howMany++;
                    moral += m.auswerten(vo);
                }
                float old = vo.MoralWert;
                vo.MoralWert = Mathf.Min((moral / howMany)*vo.MoralBoost,1f);
                if(vo.Moralb != null) vo.Moralb.Change(vo.MoralWert-old);
                
            }
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

        return auswahl; //später zufällig 2-3 Vorlieben..
        
    }
}