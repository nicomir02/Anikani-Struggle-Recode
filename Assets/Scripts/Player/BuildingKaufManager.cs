using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingKaufManager : MonoBehaviour
{


    //[SerializeField] private Button gebaeudeKaufBtn;
    [SerializeField] private GameObject panel;

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
        GameObject o = Instantiate(prefab, transform.position, transform.rotation);
        o.transform.SetParent(GameObject.Find("___InGame___/Canvas/GebaeudeKaufPanel").transform, false);
        o.transform.GetChild(2).GetComponent<BuildingID>().building_id = 0;
        generatedPrefabs.Add(o);
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
