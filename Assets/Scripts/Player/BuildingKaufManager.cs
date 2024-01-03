using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingKaufManager : Panel
{

    [SerializeField] private Transform scrollviewTransfrom;

    [SerializeField] private GameObject prefab;

    private bool offen = false;

    private List<GameObject> generatedPrefabs = new List<GameObject>();

    [SerializeField] private Pause pause;

    // Start is called before the first frame update
    void Start()
    {
        //gebaeudeKaufBtn.onClick.AddListener(openClosePanel);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.B) && canPause()){
            if(!offen ){
                openPanel();
                offen = true;
            } else {
                closePanel();
                offen = false;
            }
        }
    }

    void openPanel(){
        if(!canPause()) return;
        offen = true;
        panel.SetActive(true);
        foreach(KeyValuePair<GameObject, int> kvp in PlayerHelperScript.findLocalPlayer().getVolk().getAllBuildingObjects()) {
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            o.transform.SetParent(scrollviewTransfrom, false);
            o.transform.GetChild(2).GetComponent<BuildingID>().building_id = kvp.Value;
            o.transform.GetChild(0).GetComponent<Image>().sprite = kvp.Key.GetComponent<SpriteRenderer>().sprite;

            TMP_Text text = o.transform.GetChild(3).GetComponent<TMP_Text>();
            Building b = PlayerHelperScript.findLocalPlayer().getVolk().getBuildingByID(kvp.Value).GetComponent<Building>();
            text.text = "";
            Dictionary<Ressource, float> buildingKosten = b.getKosten();
            foreach(KeyValuePair<Ressource, float> kosten in buildingKosten) {
                text.text += kosten.Key.name + ": " + kosten.Value + "\n";
            }

            o.transform.GetChild(2).GetComponent<BuildingID>().Kosten = buildingKosten;

            generatedPrefabs.Add(o);
        }
    }

    public override void closePanel(){
        base.closePanel();
        offen = false;
        foreach(GameObject gameObject in generatedPrefabs) {
            Destroy(gameObject);
        }
        generatedPrefabs = new List<GameObject>();
    }


    void openClosePanel(){
        if(!offen ){
            openPanel();
        } else {
            closePanel();
        }
    }

    
}
