using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public void neueBelegung() {
        if(PlayerPrefs.GetString("Button_Cancel") == null || PlayerPrefs.GetString("Button_Cancel") == "") PlayerPrefs.SetString("Button_Cancel", KeyCode.Escape.ToString());
        PlayerPrefs.Save();
    }

    
    public bool GetButtonDown(string btnName) {
        neueBelegung();
        btnName = PlayerPrefs.GetString("Button_"+btnName);
        return Input.GetKeyDown(btnName.ToLower());
    }

    public void SetButtonKey(string keyCode) {
        PlayerPrefs.SetString("Button_"+keyCode, keyCode.ToString());
    }

    KeyCode StringToKeyCode(string keyString) {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))  {
            if (keyString.ToUpper() == keyCode.ToString()) {
                return keyCode;
            }
        }
        return KeyCode.None;
    }
}
