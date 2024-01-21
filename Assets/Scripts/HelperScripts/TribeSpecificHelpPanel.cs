using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TribeSpecificHelpPanel : MonoBehaviour
{
    public GameObject Panel;
    //public string tribename;
    //ScrollView Units
    [SerializeField] private Transform scrollviewUnitTransform;
    //UnitPanel
    [SerializeField] private GameObject unitPrefab;
    private List<GameObject> generatedUnitPrefabs = new List<GameObject>();
     //ScrollView Buildings
    [SerializeField] private Transform scrollviewBuildingTransform;
    //BuildingPanel
    [SerializeField] private GameObject buildingPrefab;
    private List<GameObject> generatedBuildingPrefabs = new List<GameObject>();

    public void fillPanel(){
        //Panel.transform.GetChild(0).GetComponent<TMP_Text>().text = tribename;
        fillUnitPanel();
        fillBuildingPanel();
    }
    public void fillUnitPanel(){
        int i = 0;
        //alle Einheiten auflisten als Scrollview innerhalb des Panels
        foreach(KeyValuePair<GameObject, int> kvp in PlayerHelperScript.findLocalPlayer().getVolk().getAllUnits()) {
            GameObject o = unitPrefab;
            if(i > 0) o = Instantiate(unitPrefab, transform.position, transform.rotation);
            Unit u = PlayerHelperScript.findLocalPlayer().getVolk().getUnitByID(kvp.Value).GetComponent<Unit>();
            
            o.transform.SetParent(scrollviewUnitTransform, false);
            o.transform.GetChild(4).GetComponent<UnitID>().unit_ID = kvp.Value;
            o.transform.GetChild(0).GetComponent<Image>().sprite = u.Icon;

            TMP_Text text = o.transform.GetChild(3).GetComponent<TMP_Text>();
            
            text.text = "";
            Dictionary<Ressource, float> unitKosten = u.getKosten();
            foreach(KeyValuePair<Ressource, float> kosten in unitKosten) {
                text.text += kosten.Key.name + ": " + kosten.Value + "\n";
            }

            o.transform.GetChild(4).GetComponent<UnitID>().Kosten = unitKosten;

            if(i++ > 0) generatedUnitPrefabs.Add(o);
        }
    }

    public void fillBuildingPanel(){
        int i = 0;
        foreach(KeyValuePair<GameObject, int> kvp in PlayerHelperScript.findLocalPlayer().getVolk().getAllBuildingObjects()) {
            GameObject o = buildingPrefab;
            if(i > 0) {
                o = Instantiate(buildingPrefab, transform.position, transform.rotation);
            }
            Building b = PlayerHelperScript.findLocalPlayer().getVolk().getBuildingByID(kvp.Value).GetComponent<Building>();

            o.transform.SetParent(scrollviewBuildingTransform, false);
            o.transform.GetChild(4).GetComponent<BuildingID>().building_id = kvp.Value;
            if(b.Icon != null) o.transform.GetChild(0).GetComponent<Image>().sprite = b.Icon;

            TMP_Text text = o.transform.GetChild(3).GetComponent<TMP_Text>();
            o.transform.GetChild(1).GetComponent<TMP_Text>().text = b.name;
            o.transform.GetChild(2).GetComponent<TMP_Text>().text = "Standard Health " + b.standardMaxHealth + "\n";
            text.text = "";
            Dictionary<Ressource, float> buildingKosten = b.getKosten();
            foreach(KeyValuePair<Ressource, float> kosten in buildingKosten) {
                text.text += kosten.Key.name + ": " + kosten.Value + "\n";
            }

            o.transform.GetChild(4).GetComponent<BuildingID>().Kosten = buildingKosten;

            if(i++ > 0) generatedBuildingPrefabs.Add(o);
        }
    }
}
