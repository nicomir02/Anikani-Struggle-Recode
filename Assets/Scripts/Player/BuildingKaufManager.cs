using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingKaufManager : MonoBehaviour
{


    //[SerializeField] private Button gebaeudeKaufBtn;
    [SerializeField] private GameObject panel;

    [SerializeField] private Transform scrollviewTransfrom;

    [SerializeField] private GameObject prefab;

    private bool offen = false;

    private List<GameObject> generatedPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //gebaeudeKaufBtn.onClick.AddListener(openClosePanel);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
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
        panel.SetActive(true);
        foreach(KeyValuePair<GameObject, int> kvp in PlayerHelperScript.findLocalPlayer().getVolk().getAllBuildingObjects()) {
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            o.transform.SetParent(scrollviewTransfrom, false);
            o.transform.GetChild(2).GetComponent<BuildingID>().building_id = kvp.Value;
            o.transform.GetChild(0).GetComponent<Image>().sprite = kvp.Key.GetComponent<SpriteRenderer>().sprite;
            generatedPrefabs.Add(o);
        }
    }

    public void closePanel(){
        panel.SetActive(false);
        foreach(GameObject gameObject in generatedPrefabs) {
            Destroy(gameObject);
        }
        generatedPrefabs = new List<GameObject>();
    }


    void openClosePanel(){
        if(!offen ){
                openPanel();
                offen = true;
            } else {
                closePanel();
                offen = false;
            }
    }
}
