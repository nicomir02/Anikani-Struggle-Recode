using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;

public class Pause : Panel
{
    //Noch nicht funktionabel;
    
    [SerializeField] Button continueBtn;

    [SerializeField] private List<GameObject> cannotPause = new List<GameObject>();

    [SerializeField] private InputManager inputManager;

    


    public void updatePause() {
        if(!isPause) {
            panel.SetActive(true);
            isPause = true;
            Time.timeScale = 0f;
        }else {
            closePanel();
        }
    }

    public override void closePanel() {
        base.closePanel();
        isPause = false;
        Time.timeScale = 1f;
        closeSettings();
    }

    public void disconnect() {
        NetworkClient.Disconnect();
    }

    void Update() {
        if(inputManager.GetButtonDown("Cancel")) {
            if(settingsMenu.activeSelf) {
                closeSettings();
                return;
            }

            bool b = false;
            foreach(Panel o in GameObject.FindObjectsOfType<Panel>()) {
                if(o.panel.gameObject.activeSelf) {
                    o.closePanel();
                    b = true;
                    break;
                }
            }
            if(!b) updatePause();
        }
    }


    [SerializeField] private GameObject settingsMenu;

    public void openSettings() {
        settingsMenu.SetActive(true);
    }

    public void closeSettings() {
        settingsMenu.SetActive(false);
    }
}
