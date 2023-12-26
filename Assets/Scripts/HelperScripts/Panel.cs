using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] public GameObject panel;

    public virtual void closePanel() {
        panel.SetActive(false);
    }

    public bool canPause() {
        foreach(Panel o in GameObject.FindObjectsOfType<Panel>()) {
            if(o.panel.gameObject.activeSelf) {
                return false;
            }
        }
        return true;
    }
}
