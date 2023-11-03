using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectHelper
{
    public static bool canSelect() {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        return EventSystem.current.currentSelectedGameObject == null;
    }
}
