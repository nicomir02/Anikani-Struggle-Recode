using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelperScript : MonoBehaviour
{
    //Find LocalPlayer GameObject das Script PlayerManager
    public static PlayerManager findLocalPlayer() {
        foreach(PlayerManager pm in GameObject.FindObjectsOfType<PlayerManager>()) {
            if(pm.isLocalPlayer) return pm;
        }
        return null;
    }

}
