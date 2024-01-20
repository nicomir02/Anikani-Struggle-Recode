using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TribeSpecificHelpPanel : MonoBehaviour
{
    public GameObject Panel;
    public string tribename;

    public void fillPanel(){
        Panel.transform.GetChild(0).GetComponent<TMP_Text>().text = tribename;

    }
}
