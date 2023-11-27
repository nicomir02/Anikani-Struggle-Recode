using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class InfoboxManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllButtonsVolkObject;
    [SerializeField] private List<GameObject> AllButtonsUnit;
    [SerializeField] private List<GameObject> AllButtonsBuilding;

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject stopButton;

    // Main Infobox Panel
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject listBtns;

    [SerializeField] private TMP_Text allgemeineInfos;

    [SerializeField] private Pause pause;

    List<GameObject> instantiatedBtns = new List<GameObject>();

    bool bigInfobox = false;

    // Angeklicktes VolkObject
    VolkObject obj;
    bool destroyable = true;

    public void openRightBox(VolkObject o, bool canDestroy = true)
    {
        closeAllPanels();
        listBtns.SetActive(true);
        obj = o;
        panel.SetActive(true);
        destroyable = canDestroy;
        panel.transform.GetChild(0).gameObject.SetActive(true);

        // Instantiate buttons for volkobject, unit, and building
        InstantiateButtons(AllButtonsVolkObject);

        if (o.GetType() == typeof(Unit))
        {
            InstantiateButtons(AllButtonsUnit);
        }
        else if (o.GetType() == typeof(Building))
        {
            InstantiateButtons(AllButtonsBuilding);
        }

        // Instantiate specific buttons for the object
        InstantiateButtons(obj.SpecificButtonsInfobox);

        // Instantiate the new buttons
        InstantiateNewButton(startButton);
        InstantiateNewButton(stopButton);
    }

    private void InstantiateButtons(List<GameObject> buttonList)
    {
        foreach (GameObject ob in buttonList)
        {
            GameObject a = Instantiate(ob, listBtns.transform);
            instantiatedBtns.Add(a);
        }
    }

    private void InstantiateNewButton(GameObject buttonPrefab)
    {
        GameObject a = Instantiate(buttonPrefab, listBtns.transform);
        instantiatedBtns.Add(a);
    }

    public void openAllgemeineInfos(){
        bigInfobox = true;
        panel.transform.GetChild(1).gameObject.SetActive(true);

        allgemeineInfos.text = obj.description();
        

    }

    void LateUpdate() {
        if(Input.GetButtonDown("Cancel") && !pause.IsPause && bigInfobox) {
            closeInfo();
        }
    }

    public void destroyObject(){
        if(!destroyable) return;
        closeAllPanels();
        obj.destroyObject();
    }

    public void einheitenAusbildung(){
        GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<UnitKaufManager>().openPanel((TrainUnitBuilding) obj);
    }

    public void closeInfo() {
        bigInfobox = false;
        panel.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void useDrugs() {
        obj.useDrugs();
    }


    public void closeAllPanels() {
        foreach(GameObject o in instantiatedBtns) {
            Destroy(o);
        }
        instantiatedBtns = new List<GameObject>();
        listBtns.SetActive(false);
        //all children deactivate still to do
        panel.SetActive(false);
    }

    public void voiceCommand()
    {
        Debug.Log("voice button");
    }
}