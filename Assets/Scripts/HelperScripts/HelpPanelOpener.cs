using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanelOpener : MonoBehaviour
{
    public GameObject Panel;
    public GameObject closePrevPanel;
    //open and close Panel
	public void openPanel(){
		if (Panel != null){
			bool isActive = Panel.activeSelf;
			Panel.SetActive(!isActive);
		}
        if (closePrevPanel != null){
            bool isActive = closePrevPanel.activeSelf;
			closePrevPanel.SetActive(!isActive);
        }

	}
}
