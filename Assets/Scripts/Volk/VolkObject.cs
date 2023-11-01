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


    [SerializeField] private List<Sprite> animationList = new List<Sprite>();
    [SerializeField] private float animationAbstand = 0.2f;

    private InfoboxManager im;

    //special Volkobject InfoboxButtons
    [SerializeField] private List<GameObject> specificButtonsInfobox;

    public List<GameObject> SpecificButtonsInfobox {
        get{return specificButtonsInfobox;}
    }

    protected TilemapHover tilemapHover;

    public virtual void Start() {
        Debug.Log("[" + name + "] instanziert");
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        im = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<InfoboxManager>();

        if(animationList.Count > 0) StartCoroutine(animationStart(0));
    }

    public IEnumerator animationStart(int cur) {
        Debug.Log("In Enumerator: " + cur);
        if(cur >= animationList.Count) cur = 0;
        GetComponent<SpriteRenderer>().sprite = animationList[cur];
        cur++;
        Debug.Log("a");
        yield return new WaitForSecondsRealtime(animationAbstand);
        animationStart(cur);
    }

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
    
    //methode die alle aktionen listet die ausgeführt werden sollen sobald ein VolkObject selected ist.
    public virtual void selected() {
        Debug.Log("[" + name + "]" + " Selected");
        GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
        im.openRightBox(this);
    }

    public virtual void deselect(){
        GetComponent<SpriteRenderer>().color = Color.white;
        im.closeAllPanels();
    }
    
    public void tryAttack(Vector3Int v, VolkObject obj) {
        if(healValue > 0 && obj.transform.parent == transform.parent) {  //eigene Einheit heilen falls man heilen kann
            obj.changeHealth(healValue);
        }else {
            obj.changeHealth(-attackValue);
        }
    }
}
