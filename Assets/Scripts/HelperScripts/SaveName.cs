using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveName : MonoBehaviour
{
    string[] notAllowed = {}; //nicht erlaubte Namen hier rein schreiben
    public new string name;


    public bool checkAndSave(string name){
        foreach(string s in notAllowed) {
            if(name.Contains(s)) {
                return false;
            }
        }
        
        if(name.Length > 12) return false;
        this.name = name;
        return true; 
    }
}
