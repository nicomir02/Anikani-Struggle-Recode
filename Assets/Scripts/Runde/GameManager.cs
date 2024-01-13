using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Dictionary<int, Color> gameFarben = new Dictionary<int, Color>(); 
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

    public void setColor(int id, Color color) {
        Debug.Log(id);
        Debug.Log(color);
        if(gameFarben.ContainsKey(id)) {
            gameFarben[id] = color;
        }else {
            gameFarben.Add(id, color);
        }
        float[] array = new float[3];
        array[0] = color.r;
        array[1] = color.g;
        array[2] = color.b;
        newColorCMD(id, array);
    }

    [Command(requiresAuthority=false)]
    public void newColorCMD(int id, float[] colorUtility) {
        newColorRPC(id, colorUtility);
    }

    [ClientRpc]
    public void newColorRPC(int id, float[] colorUtility) {
        Color color = new Color(colorUtility[0], colorUtility[1], colorUtility[2], 255);
        if(gameFarben.ContainsKey(id)) {
            gameFarben[id] = color;
        }else {
            gameFarben.Add(id, color);
        }
    }

    public StandardVolk getVolkByID(int id) {
        foreach(StandardVolk v in voelker) {
            if(v.id == id) return v;
        }
        return null;
    }
}
