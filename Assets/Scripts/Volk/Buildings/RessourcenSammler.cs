using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class RessourcenSammler : Building
{
    [SerializeField] protected Ressource ressource;

    public virtual float howMany() {
        if(Ressourcen.ContainsKey(ressource)) {
            return Ressourcen[ressource]+1;
        }else {
            return 1;
        }
    }

    public override void nextRound() {
        base.nextRound();
        playerManager.addRessource(ressource, howMany() + ressourcenMoralAuswirkung);
    }

    private float ressourcenMoralAuswirkung = 0f;
    public float RessourcenMoralAuswirkung {
        get{return ressourcenMoralAuswirkung;}
        set{ressourcenMoralAuswirkung = value;}
    }

}