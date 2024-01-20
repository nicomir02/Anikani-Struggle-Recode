using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoboxManager : Panel
{
    [SerializeField] private List<GameObject> AllButtonsVolkObject;
    [SerializeField] private List<GameObject> AllButtonsUnit;
    [SerializeField] private List<GameObject> AllButtonsBuilding;

    [SerializeField] private GameObject listBtns;
    public GameObject ListBtns {
        get {return listBtns}
    }

    [SerializeField] private TMP_Text allgemeineInfos;

    [SerializeField] private Pause pause;


    List<GameObject> instantiatedBtns = new List<GameObject>();

    [SerializeField] private GameObject moralElement;
    [SerializeField] private TilemapHover tilemapHover;
    [SerializeField] private MoralManager moralManager;

    private List<GameObject> instantiatedMoral = new List<GameObject>();

    bool bigInfobox = false;
    public GameObject BigInfobox {
        get {return bigInfobox}
    }

    //angeklicktes VolkObject
    VolkObject obj;
    bool destroyable = true;


    [SerializeField] private Sprite positive;
    [SerializeField] private Sprite neutral;
    [SerializeField] private Sprite negative;

    public void openRightBox(VolkObject o, bool canDestroy=true) {
        closeAllPanels();
        listBtns.SetActive(true);
        obj = o;
        panel.SetActive(true);
        destroyable = canDestroy;
        panel.transform.GetChild(0).gameObject.SetActive(true);
        //volkobject alle buttons noch
        foreach(GameObject ob in AllButtonsVolkObject) {
            GameObject a = Instantiate(ob);
            a.transform.SetParent(listBtns.transform);
            instantiatedBtns.Add(a);
        }

        if(o.GetType() == typeof(Unit)) {
            foreach(GameObject ob in AllButtonsUnit) {
                GameObject a = Instantiate(ob);
                a.transform.SetParent(listBtns.transform);
                instantiatedBtns.Add(a);
            }
            
        }else if(o.GetType() == typeof(Building)){
            foreach(GameObject ob in AllButtonsBuilding) {
                GameObject a = Instantiate(ob);
                a.transform.SetParent(listBtns.transform);
                instantiatedBtns.Add(a);
            }
        }
        foreach(GameObject ob in obj.SpecificButtonsInfobox) {
            GameObject a = Instantiate(ob);
            a.transform.SetParent(listBtns.transform);
            instantiatedBtns.Add(a);
        }
    }   

    public void openAllgemeineInfos(){
        if(instantiatedMoral.Count > 0) closeInfo();
        bigInfobox = true;
        panel.transform.GetChild(1).gameObject.SetActive(true);

        allgemeineInfos.text = obj.description();


        int i = 0;
        foreach(Moral m in obj.Vorlieben) {
            float f = m.auswerten(obj);
            GameObject o = moralElement;
            if(i>0) {
                o = Instantiate(moralElement, moralElement.transform.parent);
                instantiatedMoral.Add(o);
            }else moralElement.SetActive(true);

            o.transform.GetChild(0).GetComponent<Image>().sprite = moralManager.getSprite(m);

            if(m.m_Moral == 0) {
                o.transform.GetChild(1).GetComponent<Image>().sprite = positive;
            }else if(m.m_Moral == 1) {
                o.transform.GetChild(1).GetComponent<Image>().sprite = neutral;
            }else {
                o.transform.GetChild(1).GetComponent<Image>().sprite = negative;
            }
            o.transform.GetChild(2).GetComponent<TMP_Text>().text = Mathf.Round(f*10)*0.1f + "/1 \n";
            i++;
        }
        

    }


    void Update() {
        if(!bigInfobox && instantiatedBtns.Count == 0) {
            foreach(VolkObject vo in FindObjectsOfType<VolkObject>()) {
                if(vo.GetComponent<SpriteRenderer>().color == tilemapHover.getSelectColor()) {
                    openRightBox(vo);
                    return;
                }
            }
        }
    }

    public void destroyObject(){
        if(!destroyable) return;
        closeAllPanels();
        obj.destroyObject();
    }

    public void einheitenAusbildung(){
        Debug.Log("test");
        GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<UnitKaufManager>().openPanel((TrainUnitBuilding) obj, (!isPause && !bigInfobox));
    }

    public void closeInfo() {
        if(!bigInfobox) return;
        bigInfobox = false;
        panel.transform.GetChild(1).gameObject.SetActive(false);

        foreach(GameObject o in instantiatedMoral) {
            Destroy(o);
        }
    }

    public void useDrugs() {
        obj.useDrugs();
    }


    public override void closePanel() {
        closeAllPanels();
    }

    public void closeAllPanels() {
        foreach(GameObject o in instantiatedBtns) {
            Destroy(o);
        }
        panel.SetActive(false);
        instantiatedBtns = new List<GameObject>();
        listBtns.SetActive(false);
        //all children deactivate still to do
        closeInfo();
    }
}