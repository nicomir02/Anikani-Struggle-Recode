using System.Collections;
using System.Collections.Generic;
using UnityEngine; //

public class SaveHelper : MonoBehaviour
{
    //string[] notAllowed = {}; //nicht erlaubte Namen hier rein schreiben
    public new string name;

    [SerializeField] private List<StandardVolk> voelker = new List<StandardVolk>();

    public List<StandardVolk> Voelker {
        get{return voelker;}
    }


    public bool checkAndSave(string name){
        if(name.Length < 1) return false;
        this.name = name;
        return true; 
    }

    void Start() {
        foreach(StandardVolk volk in voelker) {
            GetComponent<NetworkManagerAnikani>().spawnPrefabs.Add(volk.HauptGebaeude.gameObject);
            foreach(Building b in volk.Buildings) {
                GetComponent<NetworkManagerAnikani>().spawnPrefabs.Add(b.gameObject);
            }
            foreach(Unit u in volk.Units) {
                GetComponent<NetworkManagerAnikani>().spawnPrefabs.Add(u.gameObject);
            }
        }
        
    }
}
