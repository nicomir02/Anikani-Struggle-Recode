using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VolkObject : NetworkBehaviour
{
    [SyncVar] public int health;
    
    [SyncVar] public int attackValue = 0;
    [SyncVar] public int healValue = 0;

    [SerializeField] public int maxHealth = 100;

    [SerializeField] public int attackRange = 1;

    //Kosten des Objekts
    [SerializeField] public List<Ressource> RessourcenFuerKosten;
    [SerializeField] public List<int> kostenFuerRessource;
    [SerializeField] public List<float> kostenMultiplierFuerRessource;


    public virtual bool isObjectAtPosition(Vector3Int vec) {
        return false;
    }

    [Command (requiresAuthority=false)]
    public void changeHealth(int value){
        health += value;
        if(health > maxHealth) health = maxHealth;
        if(health <= 0) destroyObject();
    }

    //einheit zerstören

    [Command (requiresAuthority = false)]
    public void destroyObject(){
        NetworkServer.Destroy(gameObject);
    }

    public bool tryAttack(Vector3Int v) {
        foreach(VolkObject obj in FindObjectsOfType<VolkObject>()) {
            if(obj.isObjectAtPosition(v)) {
                if(obj.transform.parent == transform.parent) {
                    obj.changeHealth(healValue);
                    return true;
                }else {
                    obj.changeHealth(-attackValue);
                    return true;
                }
            }
        }
        return false;
    }
}
