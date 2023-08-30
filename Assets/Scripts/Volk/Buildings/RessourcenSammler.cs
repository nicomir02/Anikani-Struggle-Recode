using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class RessourcenSammler : Building
{
    [SerializeField] private Ressource ressource;

    public override void nextRound() {
        if(Ressourcen.ContainsKey(ressource)) {
            playerManager.addRessource(ressource, Ressourcen[ressource]+1);
        }else {
            playerManager.addRessource(ressource, 1);
        }
        
    }

}