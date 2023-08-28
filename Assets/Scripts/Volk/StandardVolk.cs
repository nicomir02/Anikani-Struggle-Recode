using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardVolk : MonoBehaviour
{
    //allgemeine Gebäude für alle Völker(spezielle in spezifischen Volkskripten reinschreiben)
    [SerializeField] private Building hauptGebaeude;
    [SerializeField] public int id;

    // Start is called before the first frame update
    public Building getMainBuilding() {
        return hauptGebaeude;
    }

    public GameObject getBuildingByID(int id) {
        if(id == 0) return hauptGebaeude.gameObject;
        return null;
    }
}
