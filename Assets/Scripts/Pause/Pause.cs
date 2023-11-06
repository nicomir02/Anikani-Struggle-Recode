using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    //Noch nicht funktionabel;
    private bool isPause = false;
    public bool IsPause {
        get{return isPause;}
    }
    [SerializeField] GameObject _pause;
    [SerializeField] Button continueBtn;

    [SerializeField] private List<GameObject> cannotPause = new List<GameObject>();

    void Update()
    {
        if(Input.GetButtonDown("Cancel") && canPause()) {
            updatePause();
        }
    }

    bool canPause() {
        foreach(GameObject o in cannotPause) {
            if(o.activeSelf) {
                return false;
            }
        }
        return true;
    }

    void updatePause() {
        isPause = !isPause;
        if(isPause) {
            _pause.SetActive(true);
            Time.timeScale = 0f;
        }else {
            _pause.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
