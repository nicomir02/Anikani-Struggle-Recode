using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    public void neueBelegung() {
        if(PlayerPrefs.GetString("Button_CANCEL") == null || PlayerPrefs.GetString("Button_CANCEL") == "") PlayerPrefs.SetString("Button_CANCEL", KeyCode.Escape.ToString());
        if(PlayerPrefs.GetString("Button_KARTEHOCH") == null || PlayerPrefs.GetString("Button_KARTEHOCH") == "") PlayerPrefs.SetString("Button_KARTEHOCH", KeyCode.Keypad8.ToString());
        if(PlayerPrefs.GetString("Button_KARTERUNTER") == null || PlayerPrefs.GetString("Button_KARTERUNTER") == "") PlayerPrefs.SetString("Button_KARTERUNTER", KeyCode.Keypad5.ToString());
        PlayerPrefs.Save();
    }

    
    public bool GetButtonDown(string btnName) {
        neueBelegung();
        btnName = btnName.ToUpper();

        btnName = PlayerPrefs.GetString("Button_"+btnName);
        return Input.GetKeyDown(StringToKeyCode(btnName));
    }

    public void SetButtonKey(string keyCode) {
        keyCode = keyCode.ToUpper();
        PlayerPrefs.SetString("Button_"+keyCode, keyCode.ToString());
    }

    KeyCode StringToKeyCode(string keyString) {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))  {
            if (keyString.ToUpper() == keyCode.ToString().ToUpper()) {
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    GameObject o = null;
    string sname = null;

    public void changeByButton() {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            // Get the clicked object's name
            string objectName = hit.collider.gameObject.name;
            string settings = hit.collider.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;

            if(settings == "Click Button") {
                hit.collider.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = sname;
            }else {
                sname = settings;
                o = hit.collider.gameObject;
                hit.collider.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Click Button";
            }
        }
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(o == null) return;
        if (Input.anyKeyDown) {
            
            if(Physics.Raycast(ray, out hit) && (hit.collider.gameObject == null || hit.collider.gameObject.transform.GetChild(0) == null || hit.collider.gameObject.transform.GetChild(0).GetComponent<TMP_Text>() == null || hit.collider.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text != "Click Button")) {
                SetButtonKey(o.name);
                o.transform.GetChild(0).GetComponent<TMP_Text>().text = Input.anyKeyDown.ToString();
            }
        }
    }


}
