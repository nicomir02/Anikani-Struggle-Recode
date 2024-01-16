using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardVolk : MonoBehaviour
{
    //allgemeine Gebäude für alle Völker(spezielle in spezifischen Volkskripten reinschreiben)
    [SerializeField] private Building hauptGebaeude;
    [SerializeField] private AudioClip backgroundMusicClip;

    [SerializeField][TextArea(3, 10)] private string description;

    public AudioClip BackgroundMusicClip {
        get {
            return backgroundMusicClip;
        }
    }

    public Building HauptGebaeude {
        get{return hauptGebaeude;}
    }

    [SerializeField] private List<Building> buildings = new List<Building>();

    public List<Building> Buildings {
        get {
            return buildings;
        }
    }

    [SerializeField] private int standardMaxInteractions = 2;
    public int StandardMaxInteractions {
        get {
            return standardMaxInteractions;
        }
    }
    
    [SerializeField] public int id;

    [SerializeField] private List<Unit> units;

    public List<Unit> Units {
        get {
            return units;
        }
    }

    // Start is called before the first frame update
    public Building getMainBuilding() {
        return hauptGebaeude;
    }

    public GameObject getBuildingByID(int id) {
        if(id == 0) return hauptGebaeude.gameObject;
        if(buildings[id-1] != null) return buildings[id-1].gameObject;
        return null;
    }

    public Dictionary<GameObject, int> getAllBuildingObjects() {
        Dictionary<GameObject, int> objects = new Dictionary<GameObject, int>();
        
        int i = 1;
        foreach(Building b in buildings) {
            objects.Add(b.gameObject, i++);
        }

        return objects;
    }

    public GameObject getUnitByID(int id) {
        return units[id].gameObject;
    }

    public int getUnitID(Unit u) {
        for(int i=0;i<units.Count;i++) {
            if(u == units[i]) return i;
        }
        return -1;
    }

    public Dictionary<GameObject, int> getAllUnits() {
        Dictionary<GameObject, int> objects = new Dictionary<GameObject, int>();
        for(int i=0; i<units.Count;i++) {
            objects.Add(units[i].gameObject, i);
        }
        
        return objects;
    }
}
