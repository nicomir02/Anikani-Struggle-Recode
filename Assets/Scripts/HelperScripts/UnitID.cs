using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitID : MonoBehaviour
{
    private int unitID = -1;
    private Dictionary<Ressource, int> kosten;

    public int unit_ID {
        get{return unitID;}
        set{unitID = value;}
    }

    public Dictionary<Ressource, int> Kosten {
        get{return kosten;}
        set{kosten = value;}
    }

    void Start(){
        GetComponent<Button>().onClick.AddListener(click);
    }



    public void click(){
        if(unitID == -1) return;
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.genugRessourceVorhanden(kosten)) {
            GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<UnitKaufManager>().addUnit(unitID);
            Debug.Log("UnitPanel] " + pm.name + " hat Unit gekauft");
        }else {
            Debug.Log("[UnitPanel] " + pm.name + " hat nicht genug Ressourcen");
        }
    }
}
