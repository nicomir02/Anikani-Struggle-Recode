using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpPanelOpener : MonoBehaviour
{
    public GameObject Panel;
	public int isMainHelpPanel = 0;
	public int isTribeMainPanel = 0;
    public GameObject closePrevPanel;
    //open and close Panel
	public void openPanel(){
		if (Panel != null){
			bool isActive = Panel.activeSelf;
			Panel.SetActive(!isActive);
		}
		//falls TribePanel geöffnet wird braucht man noch Infos
		if(isTribeMainPanel == 1 && Panel.activeSelf == true){
			//tribePanelOpen();	//klappt momentan nicht
		}
        if (closePrevPanel != null){
            bool isActive = closePrevPanel.activeSelf;
			closePrevPanel.SetActive(!isActive);
        }

	}
	public void LateUpdate()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && Panel != null){
			Panel.SetActive(false);
		}
		if(isMainHelpPanel == 1 && Input.GetKeyDown(KeyCode.H) && Panel != null){
			bool isActive = Panel.activeSelf;
			Panel.SetActive(!isActive);
		}
	}

	public void tribePanelOpen(){
		Debug.Log("Starting Tribe Description");
		//nächster Debug.Log klappt nicht
		Debug.Log(GameObject.Find("___Everywhere___/----NetworkManager----").GetComponent<SaveHelper>().Voelker[0].Description);
		GameObject tmp = Panel.transform.GetChild(6).gameObject;
		tmp = tmp.transform.GetChild(0).gameObject;
		tmp = tmp.transform.GetChild(0).gameObject;
		tmp.GetComponent<TextMeshPro>().text = GameObject.Find("___Everywhere___/----NetworkManager----").GetComponent<SaveHelper>().Voelker[0].Description; 
		Panel.transform.Find("CurseBearersDescription/Viewport/Content").GetComponent<TextMeshPro>().text = GameObject.Find("___Everywhere___/----NetworkManager----").GetComponent<SaveHelper>().Voelker[1].Description;
	}
}
