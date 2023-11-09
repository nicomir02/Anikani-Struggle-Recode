using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoralManager : MonoBehaviour
{
    private List<Moral> unitMoral = new List<Moral>();

    void Start() {
        unitMoral.Add(new Moral_Heimweh());
    }
}
