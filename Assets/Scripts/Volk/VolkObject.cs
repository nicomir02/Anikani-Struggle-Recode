using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class VolkObject : NetworkBehaviour
{
    [SyncVar] public float health;

    [SerializeField] private Sprite icon;
    public Sprite Icon {
        get {
            return icon;
        }
    }
    
    [SyncVar] public float attackValue = 0;
   
    [SyncVar] public float healValue = 0;

//vorher war SerializeField eine SyncVar und hieß maxHealth statt standardmaxHealth. jetzt test für moralauswirkungen auf attribute
    [SerializeField] public int standardMaxHealth = 100;
    [SyncVar] public int maxHealth = 100;   //vielleicht als float?

    

//ähnlich wie bei maxhealth:
    [SerializeField] public int standardAttackRange = 1;
    [SyncVar] public int attackRange = 1;

    [SerializeField] protected Healthbar healthbar;
    [SerializeField] protected Moralbar moralbar;
    public Moralbar Moralb {
        get{return moralbar;}
    }

    [SerializeField] private AudioClip bauAusbildSound;
    public AudioClip BauSound {
        get {return bauAusbildSound;}
    }

    [SerializeField] private AudioClip todesSound;
    /*public AudioClip TodesSound {
        get {return todesSound;}
    }*/

//für die Moralskala. Bei Auswahl fuer den Aufruf der Moralskala benutzt
    private float[] moralskalaArray = {0f,0.1f,0.25f,0.5f,0.75f,0.90f,1f};

    [SyncVar] protected int lastAttack = 0;
    public int LastAttack {
        get{return lastAttack;}
    }

    [SyncVar] private float moralBoost = 1f;
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

    //Moral der vorherigen RUnde
    private float prevMoralWert = 1f;


    protected List<Moral> vorlieben = new List<Moral>();
    public List<Moral> Vorlieben {
        get{return vorlieben;}
    }

    protected PlayerManager pm;
    public PlayerManager playerManager {
        get{
            if(pm == null) {
                return transform.parent.GetComponent<PlayerManager>();
            }
            return pm;
        }
        set{pm = value;}
    }

    protected MoralManager mm;

    //Kosten des Objekts
    [SerializeField] public List<Ressource> RessourcenFuerKosten;
    [SerializeField] public List<int> kostenFuerRessource;
    [SerializeField] public List<float> kostenMultiplierFuerRessource;

    public void ApplyMoraleChange(float change)
    {
        moralWert += change;
        moralWert = Mathf.Clamp(moralWert, 0f, 1f);
        if (Moralb != null) Moralb.Change(moralWert);
        mm.gesamtMoralBerechnung(playerManager);
        Debug.Log($"[VolkObject] New Morale Value: {moralWert}");
    }

    protected InfoboxManager im;

    //special Volkobject InfoboxButtons
    [SerializeField] private List<GameObject> specificButtonsInfobox;

    public List<GameObject> SpecificButtonsInfobox {
        get{return specificButtonsInfobox;}
    }

    protected TilemapHover tilemapHover;

    
    protected List<Sprite> animationList = new List<Sprite>();
    [SerializeField] private float animationAbstand = 0.2f;
    protected List<Sprite> idle = new List<Sprite>();

    private void loadAnimations() {
        transform.GetChild(4).GetComponent<AniSpriteMerger>().aniMerge();
        idle = transform.GetChild(4).GetComponent<AniSpriteMerger>().GetList(); 
    }

    public bool m_selected = false;

    protected RoundManager rm;

    Sprite safe;

    protected float currentNahrung = 0f;
    public float CurrentNahrung {
        get{return currentNahrung;}
        set{currentNahrung = value;}
    }

    private readonly SyncList<int> suchtLvl = new SyncList<int>(); 

    [SyncVar] private float neededFood = -1f;

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

    public Dictionary<Ressource, float> getKosten(){
        Dictionary<Ressource, float> result = new Dictionary<Ressource, float>();
        
        int anzahl = 0;

        /*
        Optimierungsvorschlg:
        für anzahl der gebäude: Beim PlayerManager speichern wen Gebäude gebaut als dictinary     
        */
        rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();
        
        foreach(Building b in FindObjectsOfType<Building>()) {
            string activePlayer = "";

            foreach(PlayerManager player in FindObjectsOfType<PlayerManager>()) {
                if(player.name == rm.aktiverSpieler) {
                    activePlayer = player.name;
                    break;
                }
            }

            if(b.name == name + " " + activePlayer) {
                anzahl++;
            }
        }

        for(int i = 0; i < RessourcenFuerKosten.Count; i++){
            float wert = anzahl*kostenMultiplierFuerRessource[i];
            float convert = Mathf.Round(wert * 10.0f) * 0.1f;
            result.Add(RessourcenFuerKosten[i], convert+kostenFuerRessource[i]); //wievielegebäudeDiesesTyps*multiplierfürverschiedeneRessourcen+kostenFuerRessource
        }
        return result;
    }

    public float boostAuswertung() {
        float otherBoost = 0f; //Später bspw. Anikani Ressource
        float drugboost = 0f;

        if(suchtLvl.Count > 1) {
            if(suchtLvl[0] > 0) {
                drugboost = (suchtLvl[0]-suchtLvl[1]) * 0.1f;
            }else {
                drugboost = 0f;
            }
            
        }
        moralBoost = drugboost + otherBoost + 1f;

        return moralBoost;
    }

    public float moralAuswertung(bool gesamtauswertung=true) {
        float moral = 0f;
        int howMany = 0;

        boostAuswertung();

        foreach(Moral m in vorlieben) {
            howMany++;

            moral += m.auswerten(this);
            Debug.Log(name + " " + m);
        }

        float old = MoralWert;
        MoralWert = Mathf.Min((moral / howMany)*MoralBoost,1f);
        if(Moralb != null) Moralb.Change(MoralWert);
        if(gesamtauswertung) mm.gesamtMoralBerechnung(playerManager);
        return MoralWert;
    }

    public virtual string description() {
        string result = 
        "Name: "+name.Replace(transform.parent.name, "")+
        "\nLeben: " + Mathf.Round(health*10)*0.1f + "/" + Mathf.Round(maxHealth*10)*0.1f +
        "\nMoral: " + Mathf.Round(MoralWert*10)*0.1f + "/1" +
        "\nNahrung: " + Mathf.Round(currentNahrung*10)*0.1f + "/" +NeededFood;
        if(currentNahrung < NeededFood*0.25) result += " - starvation"; 
        result += "\nMoralboost: " + boostAuswertung();

        /*result += "\n\nVorlieben:\n";
        int i =0;
        foreach(Moral m in Vorlieben) {
            if(i++ > 0) {
                result += "\n";
            }
            result += m + ": " + m.auswerten(this) + "/1";
        }*/
        return result;
    }

    public virtual void Start() {
        CMDStart();
        Debug.Log("[" + name + "] instanziert");
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        im = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<InfoboxManager>();
        mm = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<MoralManager>();
        rm = GameObject.Find("___InGame___/----Manager----/RoundManager").GetComponent<RoundManager>();
        loadAnimations();
        animationList = idle;
        safe = GetComponent<SpriteRenderer>().sprite;
        StartCoroutine(animationStart(0));
        //für moralsuwirkungen erstmal zu testen
        maxHealth = standardMaxHealth;
        attackRange = standardAttackRange;
        
    }

    [Command(requiresAuthority=false)]
    public void CMDStart() {
        suchtLvl.Add(-1);
        suchtLvl.Add(-1);
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
        if(suchtLvl.Count > 1 && suchtLvl[0] > -1) {
            suchtLvl[0] = suchtLvl[0]-1;
        }
        if(rm != null && rm.Round > 0) changeHealth(maxHealth*0.01f);
        
    }

    //in MoralManager aufgerufen. Jede Runde wird der prevMoralWert angepasst auf den aktuellen
    public void prevMoralAnpassung(){
        prevMoralWert = moralWert;
    }
    

    public IEnumerator animationStart(int cur, bool change=false) {
        if(animationList.Count == 0) animationList.Add(safe);

        if(++cur >= animationList.Count) {
            cur = 0;
        }
        if(cur==0 && change==false) {
            animationList = idle;
        }else if(change == true) {
            cur = 0;
        }
        
        GetComponent<SpriteRenderer>().sprite = animationList[cur];

        yield return new WaitForSecondsRealtime(animationAbstand);

        StartCoroutine(animationStart(cur, change));
    }

    public virtual bool isObjectAtPosition(Vector3Int vec) {
        return false;
    }

    [Command (requiresAuthority=false)]
    public void changeHealth(float value){
        
        if((health+value) > maxHealth) {
            value = maxHealth-health;
            health = maxHealth;
        }else health += value;
        if(healthbar != null) healthChange(value);
        
        if(health <= 0) destroyObject();
    }

    [ClientRpc]
    public void healthChange(float value) {
        healthbar.Change(value);
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
        if(GetComponent<AudioSource>() == null) {
            gameObject.AddComponent<AudioSource>();
        }
        if(todesSound == null) return;
        GetComponent<AudioSource>().clip = todesSound;
        GetComponent<AudioSource>().Play();
    }

    [Command(requiresAuthority=false)]
    public void useDrugs() {
        suchtLvl[0] += 5;
        suchtLvl[1] += 2;
        moralAuswertung();
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
    
    //schauen ob neuer Checkpunkt in Moralwert erreicht wurde und Attribute sich wieder verändern (0%, 10%, 25%, 50%, 75%, 90%, 100%)
    private void moralskalaAusfuehrung(){
        
        //durch array schauen solange kleiner ist mroalwert um zu guckenin welchem bereich
        //dann scwitch case und dann schauen in moralskala skript
        int count = 0;
        for (int i = 0; i < moralskalaArray.Length; i++){
            if(moralWert < moralskalaArray[i]) break;
            count++;
        }
        switch(count) {
            case 0:
            //zugriff auf moralskala? an volkobject anhägen? über moralmanager? noch überlegen wie am besten
                //ms.klasse1(this);
                break;
            /* case 1:
                ms.klasse2(this);
                break;
            case 2:
                ms.klasse3(this);
                break;
            case 3:
                ms.klasse4(this);
                break;
            case 4:
                ms.klasse5(this);
                break;
            case 5:
                ms.klasse6(this);
                break; */
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        
    }

    //Moralauswirkungen geplante Attribute pro Volksobjekt
    /*private void moralAuswirkung(){
        //andere Möglichkeit wäre mit Moralwert zu multiplizieren, und standardwert zu nehmen für attribute
        float veraenderungsanteil = moralWert / prevMoralWert;
        if (fabs(veraenderungsanteil) - 0,0001 <= 1f) return;        //nah an 1, also kaum veränderung
        //allgemeine Attribute von allen VOlkobjekten:
        //Health:
        float healthchange = maxHealth * veraenderungsanteil;
        maxHealth += healthchange;
        health += healthchange; //aktuelles leben auch verändern oder nur maxLeben?
        //hier mal andere Variante
        attackRange = round(standardAttackRange * moralWert);   //runden auf nächsten integer

        if(obj.GetType() == Building){ //schauen ob das so funktioniert

        } else if(obj.GetType() == Unit){

        }
        }*/


        //Getter und Setter für Variablen für Moralskala
        
    public float Health {
        get{return health;}
        set{health = value;}
    }
    public float AttackValue {
        get{return attackValue;}
        set{attackValue = value;}
    }
        public float HealValue {
        get{return healValue;}
        set{healValue = value;}
    }
    public int MaxHealth {
        get{return maxHealth;}
        set{maxHealth = value;}
    }
    public int StandardMaxHealth {
        get{return standardMaxHealth;}
    }
    public int AttackRange {
        get{return attackRange;}
        set{attackRange = value;}
    }
    public int StandardAttackRange {
        get{return standardAttackRange;}
    }

}

