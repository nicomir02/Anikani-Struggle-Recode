using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    //Noch nicht funktionabel;
    private bool isPause = false;
    [SerializeField] GameObject _pause;
    [SerializeField] Button continueBtn;

    void Update()
    {
        if(Input.GetButtonDown("Cancel")) {
            updatePause();
        }
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
