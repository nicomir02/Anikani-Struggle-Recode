using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingID : MonoBehaviour
{
    public int buildingID = -1;

    public int building_id {
        get{return buildingID;}
        set{buildingID = value;}
    }

    void Start(){
        GetComponent<Button>().onClick.AddListener(click);
    }
    public void click(){
        if(buildingID == -1) return;
        PlayerHelperScript.findLocalPlayer().setBuilding(buildingID);
        GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<BuildingKaufManager>().closePanel();
    }
}
