using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingID : MonoBehaviour
{
    private int buildingID = -1;
    private Dictionary<Ressource, float> kosten;

    public int building_id {
        get{return buildingID;}
        set{buildingID = value;}
    }

    public Dictionary<Ressource, float> Kosten {
        get{return kosten;}
        set{kosten = value;}
    }

    void Start(){
        GetComponent<Button>().onClick.AddListener(click);
    }



    public void click(){
        if(buildingID == -1 || !GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>().isYourTurn()) return;
        PlayerManager pm = PlayerHelperScript.findLocalPlayer();
        if(pm.genugRessourceVorhanden(kosten)) {
            pm.setBuilding(buildingID);
            GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<BuildingKaufManager>().closePanel();
        }else {
            Debug.Log("[BuildingPanel] " + pm.name + " hat nicht genug Ressourcen");
        }
    }
}
