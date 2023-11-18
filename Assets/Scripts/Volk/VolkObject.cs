using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VolkObject : NetworkBehaviour
{
    [SyncVar] public float health;
    
    [SyncVar] public float attackValue = 0;
    [SyncVar] public float healValue = 0;

    [SerializeField] public int maxHealth = 100;

    [SerializeField] public int attackRange = 1;

    [SerializeField] protected Healthbar healthbar;
    [SerializeField] protected Moralbar moralbar;
    public Moralbar Moralb {
        get{return moralbar;}
    }

    [SyncVar] protected int lastAttack = 0;
    public int LastAttack {
        get{return lastAttack;}
    }

    private float moralBoost = 1f;
    public float MoralBoost {
        get{return moralBoost;}
    }

    [SyncVar] protected int lastHeal = 0;
    public int LastHeal {
        get{return lastHeal;}
    }

    [SerializeField] [SyncVar] private float moralWert = 1f;
    public float MoralWert {
        get{return moralWert;}
        set{moralWert = value;}
    }

    protected List<Moral> vorlieben = new List<Moral>();
    public List<Moral> Vorlieben {
        get{return vorlieben;}
    }

    protected MoralManager mm;

    //Kosten des Objekts
    [SerializeField] public List<Ressource> RessourcenFuerKosten;
    [SerializeField] public List<int> kostenFuerRessource;
    [SerializeField] public List<float> kostenMultiplierFuerRessource;


    

    protected InfoboxManager im;

    //special Volkobject InfoboxButtons
    [SerializeField] private List<GameObject> specificButtonsInfobox;

    public List<GameObject> SpecificButtonsInfobox {
        get{return specificButtonsInfobox;}
    }

    protected TilemapHover tilemapHover;

    
    protected List<Sprite> animationList = new List<Sprite>();
    [SerializeField] private float animationAbstand = 0.2f;
    [SerializeField] public List<Sprite> idle = new List<Sprite>();

    public bool m_selected = false;

    Sprite safe;

    private float currentNahrung = 0f;
    public float CurrentNahrung {
        get{return currentNahrung;}
        set{currentNahrung = value;}
    }

    private float neededFood = -1f;

    public float NeededFood {
        get{
            if(neededFood != -1f) return neededFood;
            foreach(Moral m in vorlieben) {
                if(m.overviewName == "Food") {
                    if(m.m_Moral == 0) {
                        neededFood = 0.5f;
                        return 0.5f;
                    }else if(m.m_Moral == 1) {
                        neededFood = 1f;
                        return 1f;
                    }else if(m.m_Moral == 2) {
                        neededFood = 2f;
                        return 2f;
                    }
                    break;
                }
            }
            return 0f;
        }
    }

    public virtual string description() {
        string result = 
        "Name: "+name.Replace(transform.parent.name, "")+
        "\nLeben: " + Mathf.Round(health*10)*0.1f + "/" + Mathf.Round(maxHealth*10)*0.1f +
        "\nMoral: " + Mathf.Round(MoralWert*10)*0.1f + "/1" +
        "\nNahrung: " + Mathf.Round(currentNahrung*10)*0.1f + "/" +NeededFood;
        if(currentNahrung < NeededFood*0.25) result += " - starvation"; 

        result += "\n\nVorlieben:\n";
        int i =0;
        foreach(Moral m in vorlieben) {
            if(i++ > 0) {
                result += "\n";
            }
            result += m;
        }
        return result;
    }

    public virtual void Start() {
        Debug.Log("[" + name + "] instanziert");
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        im = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<InfoboxManager>();
        mm = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<MoralManager>();
        animationList = idle;
        //Debug.Log(animationList.Count);
        safe = GetComponent<SpriteRenderer>().sprite;
        StartCoroutine(animationStart(0));
    }
    
    public virtual void Update() {
        if(!m_selected) return;
        if(Input.GetButtonDown("CycleObjects")) {
            cycleObjectMethod();
        }
    }

    [Command(requiresAuthority=false)]
    public virtual void nextRound() {
        currentNahrung = 0f;
    }
    

    public IEnumerator animationStart(int cur) {
        if(animationList.Count == 0) animationList.Add(safe);

        if(++cur >= animationList.Count) cur = 0;
        
        GetComponent<SpriteRenderer>().sprite = animationList[cur];

        yield return new WaitForSecondsRealtime(animationAbstand);

        StartCoroutine(animationStart(cur));
    }

    public virtual bool isObjectAtPosition(Vector3Int vec) {
        return false;
    }

    [Command (requiresAuthority=false)]
    public void changeHealth(float value){
        health += value;
        if(healthbar != null) healthbar.Change(value);
        if(health > maxHealth) health = maxHealth;
        if(health <= 0) destroyObject();
    }

    //einheit zerstören

    [Command (requiresAuthority = false)]
    public void destroyObject(){
        destroyedObject();
        NetworkServer.Destroy(gameObject);
        
    }

    [ClientRpc]
    public void destroyedObject() {
        if(m_selected) deselect();
    }
    
    //methode die alle aktionen listet die ausgeführt werden sollen sobald ein VolkObject selected ist.
    //Kamera bewegt sich zum selected Objekt
    public virtual void selected() {
        Debug.Log("[" + name + "]" + " Selected");
        GetComponent<SpriteRenderer>().color = tilemapHover.getSelectColor();
        im.openRightBox(this);
        m_selected = true;
    }

    public virtual void deselect(){
        Debug.Log("[" + name + "]" + " Deselected");
        GetComponent<SpriteRenderer>().color = Color.white;
        im.closeAllPanels();
        m_selected = false;
    }
    
    public void tryAttack(Vector3Int v, VolkObject obj) {
        if(healValue > 0 && obj.transform.parent == transform.parent) {  //eigene Einheit heilen falls man heilen kann
            obj.changeHealth(healValue);
            lastHeal = 0;
        }else {
            obj.changeHealth(-attackValue);
            lastAttack = 0;
        }
    }

    public void cycleObjectMethod(){
        //Debug.Log("cycleObjectMethod");
        if(Input.GetAxisRaw("CycleObjects") > 0) {  //Next Object durch .
            //Debug.Log("NextObjektMethod");
            //Debug.Log("TypAusgewähltesObjektNex" + this.GetType());
            nextObject();
        }else if(Input.GetAxisRaw("CycleObjects") < 0) { // Previous Object durch ,
            //Debug.Log("TypAusgewähltesObjektPrev" + this.GetType());
            //Debug.Log("PreviousObjektMethod");
            previousObject();
            
        }
    }
    
    public void nextObject(){
        bool vorbei = false;
        VolkObject first = null;
        foreach(VolkObject o in FindObjectsOfType<VolkObject>()) {
            //Debug.Log("found Object next" + o.GetType());

            //Debug.Log(o.transform.parent.GetComponent<PlayerManager>() != transform.parent.GetComponent<PlayerManager>());
            //Debug.Log(o.GetType() == GetType());
            
            if(o.transform.parent.GetComponent<PlayerManager>() != transform.parent.GetComponent<PlayerManager>() || o.GetType() != GetType()) continue; //nur Objekte von diesem Spieler relevant
            
            if(first == null) first = o;//setzen des ersten Objektes von diesem Typ als Variable first
            if(vorbei) { //am nächsten Objekt angekommen von diesem Typ, falls es keins gibt, dann wird die foreach schleife ienfach bis zum ende durchgegangen
                //Debug.Log(vorbei);
                //Debug.Log("thisName:" + this.name);
                deselect();
                o.selected();
                //Debug.Log("selectedObjectName: " + o.name);
                break;
            }
            if(o == this) vorbei = true; //am jetzigen Objekt angekommen und nun im nächsten Schritt vorbeigegangen
        }
        if(m_selected) { //falls dieses Objekt noch selected ist, dann gab es kein FolgeObjekt von diesem Typ
            ///Debug.Log("after selected next maybe same?");
            deselect();
            //if(first == null) Debug.Log("first ist null");
            first.selected(); //daher springt man zum ersten Objekt dieses Typs (könnte auch dieses hier sein, was egal ist)
        }
    }

    public void previousObject(){
        VolkObject previous = null;
        
        foreach(VolkObject o in FindObjectsOfType<VolkObject>()) {
            //Debug.Log("cur Object in foreach" + o.GetType());
            if(o.transform.parent.GetComponent<PlayerManager>() != transform.parent.GetComponent<PlayerManager>() || o.GetType() != GetType()) continue; //nur Objekte von diesem Spieler relevant
            //Debug.Log("found Object prev" );
            if(o == this && previous != null) { 
                //Debug.Log("previous found");
                deselect();
                previous.selected();
                //Debug.Log(previous.name);
                break;
            }
            previous = o;   //falls this das erste Objekt war, dann wird previous auf dieses gesetzt aber die Schleife geht durch bis zum Ende ohne this zu deselecten und previous wird das letzte Objekt der Schleife
        }
        if(m_selected) {    //falls dieses Objekt weiterhin selected ist, springen wir zu unserem previous(kann das letzte Objekt sein falls this das erste war)
            //Debug.Log("after selected previous maybe same?"); 
            deselect();
            previous.selected();
        }
    }
}
