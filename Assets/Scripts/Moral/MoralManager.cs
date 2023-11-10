using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoralManager : NetworkBehaviour
{
    private List<Moral> unitMoral = new List<Moral>();
    private List<Moral> buildingMoral = new List<Moral>();

    private List<Moral> allgemeinMoral = new List<Moral>();

    void Start() {
        unitMoral.Add(new Moral_Heimweh());
        unitMoral.Add(new Moral_Attack());

        allgemeinMoral.Add(new Moral_Schaden());
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

                vo.MoralWert = moral / howMany;
                Debug.Log(vo.MoralWert);
                
            }
        }
    }

    public List<Moral> getMoral(int volkObject) {
        //volkObject = 0 = Unit
        //volkObject = 1 = Building
        List<Moral> auswahl = new List<Moral>(); 
        if(volkObject==0) {
            foreach(Moral m in unitMoral) {
                auswahl.Add(m);
            }
        }else {
            foreach(Moral m in buildingMoral) {
                auswahl.Add(m);
            }
        }
        foreach(Moral m in allgemeinMoral) {
            auswahl.Add(m);
        }

        if(auswahl.Count < 3) {
            return auswahl;
        }

        return auswahl; //später zufällig 2-3 Vorlieben..
        
    } 
}
