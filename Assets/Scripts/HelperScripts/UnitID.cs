using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitID : MonoBehaviour
{
    private int unitID = -1;
    private Dictionary<Ressource, float> kosten;
    private TrainUnitBuilding building;

    public TrainUnitBuilding Building {
        get{return building;}
        set{building = value;}
    }

    public int unit_ID {
        get{return unitID;}
        set{unitID = value;}
    }

    public Dictionary<Ressource, float> Kosten {
        get{return kosten;}
        set{kosten = value;}
    }

    void Start(){
        GetComponent<Button>().onClick.AddListener(click);
    }



    public void click(){
        if(unitID == -1 || building.istVoll) return;
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.genugRessourceVorhanden(kosten)) {
            
            GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<UnitKaufManager>().addUnit(unitID);
            Debug.Log("UnitPanel] " + pm.name + " hat Unit gekauft");
        }else {
            Debug.Log("[UnitPanel] " + pm.name + " hat nicht genug Ressourcen");
        }
    }
}
