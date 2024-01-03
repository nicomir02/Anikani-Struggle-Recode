using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Pause : Panel
{
    //Noch nicht funktionabel;
    private bool isPause = false;
    public bool IsPause {
        get{return isPause;}
    }
    [SerializeField] Button continueBtn;

    [SerializeField] private List<GameObject> cannotPause = new List<GameObject>();

    void Update()
    {
        if(Input.GetButtonDown("Cancel")) {
            bool b = false;
            foreach(Panel o in GameObject.FindObjectsOfType<Panel>()) {
                if(o.panel.gameObject.activeSelf) {
                    o.closePanel();
                    b = true;
                    break;
                }
            }
            Debug.Log(b);
            if(!b) updatePause();
        }
    }


    void updatePause() {
        isPause = !isPause;
        if(isPause) {
            panel.SetActive(true);
            Time.timeScale = 0f;
        }else {
            closePanel();
        }
    }

    public override void closePanel() {
        base.closePanel();
        isPause = false;
        Time.timeScale = 1f;
    }

    public void disconnect() {
        NetworkClient.Disconnect();
    }
}
