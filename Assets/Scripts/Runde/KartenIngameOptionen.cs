using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartenIngameOptionen : MonoBehaviour
{
    [SerializeField] private MapBehaviour mapBehaviour;
    private int curHeight;

    private List<GameObject> hoehen = new List<GameObject>();


    void Start() {
        curHeight = mapBehaviour.maxZ;
        
    }


    void Update() {
        if(Input.GetButtonDown("KarteHochRunter")) {
            if(Input.GetAxis("KarteHochRunter") > 0) {
                changeEbene(true);
            }else if(Input.GetAxis("KarteHochRunter") < 0) {
                changeEbene(false);
            }
        }
    }

    /*
    Ebene Change
    true = hoch
    false = runter
    */
    void changeEbene(bool hoch) {
        if(hoch) {
            if(hoehen.Count > 0) {
                hoehen[hoehen.Count-1].SetActive(true);
                hoehen.Remove(hoehen[hoehen.Count-1]);
                curHeight++;
            }
        }else if(!hoch && curHeight > 0) {
            GameObject now = GameObject.Find("Height"+curHeight);
            hoehen.Add(now);
            now.SetActive(false);
            curHeight--;
        }
    }
}
