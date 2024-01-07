using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Color> gameFarben; 
    [SerializeField] private List<StandardVolk> voelker;
    [SerializeField] private SaveHelper saveHelper;

    void Start() {
        foreach(StandardVolk volk in saveHelper.Voelker) {
            voelker.Add(volk);
        }
    }

    public Color getColor(int id) {
        if(id < gameFarben.Count) return gameFarben[id];
        return Color.white;
    }

    public StandardVolk getVolkByID(int id) {
        foreach(StandardVolk v in voelker) {
            if(v.id == id) return v;
        }
        return null;
    }
}
