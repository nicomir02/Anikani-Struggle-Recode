using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private readonly SyncDictionary<int, float[]> gameFarben = new SyncDictionary<int, float[]>();
    [SerializeField] private List<StandardVolk> voelker;
    [SerializeField] private SaveHelper saveHelper;

    void Start() {
        foreach(StandardVolk volk in saveHelper.Voelker) {
            voelker.Add(volk);
        }
    }

    public Color getColor(int id) {
        if(gameFarben.ContainsKey(id)) {
            Debug.Log(gameFarben[id][0] + " " + gameFarben[id][1] + " " + gameFarben[id][2]);
            return new Color(gameFarben[id][0], gameFarben[id][1], gameFarben[id][2], 255);
        } 
        return Color.white;
    }

    [Command(requiresAuthority=false)]
    public void setColor(int id, Color color) {
        float[] array = new float[3];
        array[0] = color.r;
        array[1] = color.g;
        array[2] = color.b;
        if(gameFarben.ContainsKey(id)) {
            gameFarben[id] = array;
        }else {
            gameFarben.Add(id, array);
        }

        
        newColorCMD(id, array);
    }

    [Command(requiresAuthority=false)]
    public void newColorCMD(int id, float[] colorUtility) {
        if(gameFarben.ContainsKey(id)) {
            gameFarben[id] = colorUtility;
        }else {
            gameFarben.Add(id, colorUtility);
        }
    }

    public Color getColorByName(string playername) {
        foreach(PlayerManager players in FindObjectsOfType<PlayerManager>()) {
            if(players.name == playername) {
                return getColor(players.playerCode);
            }
        }
        return Color.white;
    }

    public StandardVolk getVolkByID(int id) {
        foreach(StandardVolk v in voelker) {
            if(v.id == id) return v;
        }
        return null;
    }
}
