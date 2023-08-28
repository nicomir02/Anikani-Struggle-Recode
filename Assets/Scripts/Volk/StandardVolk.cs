using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardVolk : MonoBehaviour
{
    //allgemeine Gebäude für alle Völker(spezielle in spezifischen Volkskripten reinschreiben)
    [SerializeField] private Building hauptGebaeude;
    [SerializeField] private Building quarry;
    [SerializeField] private Building woodCutter;
    [SerializeField] private Building tower;
    [SerializeField] public int id;

    // Start is called before the first frame update
    public Building getMainBuilding() {
        return hauptGebaeude;
    }

    public GameObject getBuildingByID(int id) {
        if(id == 0) return hauptGebaeude.gameObject;
        if(id == 1) return woodCutter.gameObject;
        if(id == 2) return quarry.gameObject;
        if(id == 3) return tower.gameObject;
        return null;
    }

    public Dictionary<GameObject, int> getAllBuildingObjects() {
        Dictionary<GameObject, int> objects = new Dictionary<GameObject, int>();
        objects.Add(woodCutter.gameObject, 1);
        objects.Add(quarry.gameObject, 2);
        objects.Add(tower.gameObject, 3);
        
        return objects;
    }
}
