using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Color> gameFarben;

    public Color getColor(int id) {
        return gameFarben[id];
    }
}
