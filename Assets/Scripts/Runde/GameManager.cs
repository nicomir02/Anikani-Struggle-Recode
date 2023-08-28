using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Color> gameFarben;
    [SerializeField] private List<StandardVolk> voelker;

    public Color getColor(int id) {
        return gameFarben[id];
    }

    public StandardVolk getVolkByID(int id) {
        foreach(StandardVolk v in voelker) {
            if(v.id == id) return v;
        }
        return null;
    }
}
