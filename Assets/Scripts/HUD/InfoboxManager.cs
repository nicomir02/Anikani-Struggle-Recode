using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoboxManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllButtonsVolkObject;
    [SerializeField] private List<GameObject> AllButtonsUnit;
    [SerializeField] private List<GameObject> AllButtonsBuilding;
    //Main Infobox Panel
    [SerializeField] private GameObject panel;

    //angeklicktes VolkObject
    VolkObject obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openRightBox(VolkObject o) {
        obj = o;
        panel.SetActive(true);
        panel.transform.GetChild(0).gameObject.SetActive(true);
        //volkobject alle buttons noch
        if(o.GetType() == typeof(Unit)) {
            Debug.Log("Unit aktiv");
            //unit buttonds noch
        }else if(o.GetType() == typeof(Building)){
            Debug.Log("Building aktiv");
            //buildingbuttons noch
        }
        //specific volkobject buttons noch aus volkobject o holen
    }   

    public void openAllgemeineInfos(){
        panel.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void destroyObject(){
        obj.destroyObject();
    }

    public void einheitenAusbildung(){
        GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<UnitKaufManager>().openPanel((TrainUnitBuilding) obj);
    }


    public void closeAllPanels(){
        //all children deactivate still to do
        panel.SetActive(false);
    }
}