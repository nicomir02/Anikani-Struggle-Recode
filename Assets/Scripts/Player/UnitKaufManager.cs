using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UnitKaufManager : Panel
{
    //[SerializeField] private Button gebaeudeKaufBtn;


    [SerializeField] private Transform scrollviewTransfrom;

    //KaufPanel
    [SerializeField] private GameObject prefab;
    private List<GameObject> generatedPrefabs = new List<GameObject>();

    //parent von Ausbildungsleiste
    [SerializeField] private GameObject ausbildungsleiste;

    //aktuell auszubildendeEinheiten Prefab
    [SerializeField] private GameObject prefabUnit;
    private List<GameObject> generatedPrefabsUnits = new List<GameObject>();

    
    TrainUnitBuilding saveBuilding;

    private bool offen = false;

    [SerializeField] private Pause pause;

    

    // Start is called before the first frame update
    void Start()
    {
        //unitKaufBtn.onClick.AddListener(openClosePanel);
    }

    public void addUnit(int id) {
        saveBuilding.addTrainUnit(PlayerHelperScript.findLocalPlayer().getVolk().getUnitByID(id).GetComponent<Unit>());
        reloadAusbildungen();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.U) && canPause()){
            if(!offen){
                TrainUnitBuilding building = null; 
                foreach(TrainUnitBuilding b in FindObjectsOfType<TrainUnitBuilding>()) {
                    if(b.playerManager == PlayerHelperScript.findLocalPlayer()) {
                        building = b;
                        break;
                    }
                }
                if(building == null) return;
                openPanel(building);
                offen = true;
            } else {
                closePanel();
                offen = false;
            }
        }
        if(Input.GetButtonDown("Cancel") && !pause.IsPause && offen) {
            closePanel();
        }
    }

    public void reloadAusbildungen() {
        foreach(GameObject o in generatedPrefabsUnits) {
            Destroy(o);
        }
        generatedPrefabsUnits = new List<GameObject>();
        //liste der aktuell am ausbildenden Einheiten innerhalb dieser Barracke
        List<int> sort = new List<int>(saveBuilding.Ausbildungen.Keys);
        sort.Sort();
        

        foreach(int i in sort){
            GameObject o = Instantiate(prefabUnit, transform.position, transform.rotation);
            o.transform.SetParent(ausbildungsleiste.GetComponent<Transform>(), false);
            o.transform.GetChild(0).GetComponent<Image>().sprite = saveBuilding.Ausbildungen[i].GetComponent<SpriteRenderer>().sprite;
            o.transform.GetChild(1).GetComponent<TMP_Text>().text = "" + (i - saveBuilding.Zahl);
            generatedPrefabsUnits.Add(o);
        }
    }

    public void openPanel(TrainUnitBuilding b, bool forceOpen=false) {
        if(!canPause() && !forceOpen) return;
        saveBuilding = b;
        panel.SetActive(true);
        
        reloadAusbildungen();

        //alle Einheiten auflisten als Scrollview innerhalb des Panels
        foreach(KeyValuePair<GameObject, int> kvp in PlayerHelperScript.findLocalPlayer().getVolk().getAllUnits()) {
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            o.transform.SetParent(scrollviewTransfrom, false);
            o.transform.GetChild(2).GetComponent<UnitID>().unit_ID = kvp.Value;
            o.transform.GetChild(0).GetComponent<Image>().sprite = kvp.Key.GetComponent<SpriteRenderer>().sprite;

            TMP_Text text = o.transform.GetChild(3).GetComponent<TMP_Text>();
            Unit u = PlayerHelperScript.findLocalPlayer().getVolk().getUnitByID(kvp.Value).GetComponent<Unit>();
            text.text = "";
            Dictionary<Ressource, float> unitKosten = u.getKosten();
            foreach(KeyValuePair<Ressource, float> kosten in unitKosten) {
                text.text += kosten.Key.name + ": " + kosten.Value + "\n";
            }

            o.transform.GetChild(2).GetComponent<UnitID>().Kosten = unitKosten;
            o.transform.GetChild(2).GetComponent<UnitID>().Building = b;

            generatedPrefabs.Add(o);
        }
    }

    public override void closePanel(){
        offen = false;
        panel.SetActive(false);
        foreach(GameObject gameObject in generatedPrefabs) {
            Destroy(gameObject);
        }
        generatedPrefabs = new List<GameObject>();
    }


    void openClosePanel(TrainUnitBuilding b){
        if(!offen ){
                openPanel(b);
                offen = true;
            } else {
                closePanel();
                offen = false;
            }
    }

    
}
