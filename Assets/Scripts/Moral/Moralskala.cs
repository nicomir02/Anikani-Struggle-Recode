using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moralskala : MonoBehaviour
{
 
    //Serialize Fields für Balancing Aspekte zum Anpassen
    [SerializeField] private float faktorCase1 = 0.25f;
    [SerializeField] private float faktorCase2 = 0.5f;
    [SerializeField] private float faktorCase3 = 0.75f;
    [SerializeField] private float faktorCase4 = 1f;
    [SerializeField] private float faktorCase5 = 1.25f;
    [SerializeField] private float faktorCase6 = 1.5f;
    [SerializeField] private float faktorCase7 = 2f;

    [SerializeField] private int maxStepsAenderung1 = -3;
    [SerializeField] private int maxStepsAenderung2 = -2;
    [SerializeField] private int maxStepsAenderung3 = -1;
    [SerializeField] private int maxStepsAenderung4 = 0;
    [SerializeField] private int maxStepsAenderung5 = 1;
    [SerializeField] private int maxStepsAenderung6 = 2;
    [SerializeField] private int maxStepsAenderung7 = 3;


    /*Skalenverteilung:
        klasse1: 0 - 10% Moral
        klasse2: 10 - 25% Moral
        klasse3: 25 - 45% Moral
        Klasse4: 45 - 55% Moral
        klasse5: 55 - 75% Moral
        klasse6: 75 - 90% Moral
        klasse7: 90 - 100% Moral
    */

    //allgemeine Volkobjekt attribute //nicht mehr genutzt
    
    public void checkSkala(VolkObject vo, int moralkategoriewert){
        switch(moralkategoriewert) {
            case 1:
               
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase1);
                if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                vo.AttackValue = vo.StandardAttackValue * faktorCase1;
                vo.HealValue = vo.StandardHealValue * faktorCase1;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase1);
                //Debug.Log("Typ= " + vo.GetType());
                break;
            //noch klasse1 methode für alle kategoien kopieren und anpassen(auf balance achten)
            case 2:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase2);
                if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                vo.AttackValue = vo.StandardAttackValue * faktorCase2;
                vo.HealValue = vo.StandardHealValue * faktorCase2;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase2);
                break;
            case 3:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase3);
                if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                vo.AttackValue = vo.StandardAttackValue * faktorCase3;
                vo.HealValue = vo.StandardHealValue * faktorCase3;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase3);
                break;
            case 4:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase4);
                
                vo.AttackValue = vo.StandardAttackValue * faktorCase4;
                vo.HealValue = vo.StandardHealValue * faktorCase4;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase4);
                break;
            case 5:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase5);
                
                vo.AttackValue = vo.StandardAttackValue * faktorCase5;
                vo.HealValue = vo.StandardHealValue * faktorCase5;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase5);
                break;
            case 6:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase6);
                
                vo.AttackValue = vo.StandardAttackValue * faktorCase6;
                vo.HealValue = vo.StandardHealValue * faktorCase6;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase6);
                break;
            case 7:
                vo.MaxHealth = (int)(vo.StandardMaxHealth * faktorCase7);
                
                vo.AttackValue = vo.StandardAttackValue * faktorCase7;
                vo.HealValue = vo.StandardHealValue * faktorCase7;
                vo.AttackRange = (int)(vo.StandardAttackRange * faktorCase7);
                break;
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        //zufallsaenderung(vo, moralkategoriewert);     //immer ausführen, egal was für ein case, eventuell nur unten bei den spezialisierungen/inherited classes?
    }
    

    //spezielle VOlkobjekte

    //Unit
    public void checkSkalaUnit(Unit u, int moralkategoriewert){
        switch(moralkategoriewert) {
            case 1:
               /*
               //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 0.25f);
                if(u.Health > u.MaxHealth) u.Health = u.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                u.AttackValue = u.StandardAttackValue * 0.25f;
                u.HealValue = u.StandardHealValue * 0.25f;
                u.AttackRange = (int)(u.StandardAttackRange * 0.25f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps - 3;
                if(u.MaxSteps <= 0) u.MaxSteps = 0;
                //Kosten der Ausbildung auch noch anpassen

                break;
            //noch klasse1 methode für alle kategoien kopieren und anpassen(auf balance achten)
            case 2:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 0.5f);
                if(u.Health > u.MaxHealth) u.Health = u.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                u.AttackValue = u.StandardAttackValue * 0.5f;
                u.HealValue = u.StandardHealValue * 0.5f;
                u.AttackRange = (int)(u.StandardAttackRange * 0.5f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps - 2;
                if(u.MaxSteps <= 0) u.MaxSteps = 0;
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 3:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 0.75f);
                if(u.Health > u.MaxHealth) u.Health = u.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                u.AttackValue = u.StandardAttackValue * 0.75f;
                u.HealValue = u.StandardHealValue * 0.75f;
                u.AttackRange = (int)(u.StandardAttackRange * 0.75f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps - 1;
                if(u.MaxSteps <= 0) u.MaxSteps = 0;
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 4:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 1f);
                
                u.AttackValue = u.StandardAttackValue * 1f;
                u.HealValue = u.StandardHealValue * 1f;
                u.AttackRange = (int)(u.StandardAttackRange * 1f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps - 0;
                if(u.MaxSteps <= 0) u.MaxSteps = 0;
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 5:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 1.25f);
               
                u.AttackValue = u.StandardAttackValue * 1.25f;
                u.HealValue = u.StandardHealValue * 1.25f;
                u.AttackRange = (int)(u.StandardAttackRange * 1.25f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps + 1;
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 6:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 1.5f);
                
                u.HealValue = u.StandardHealValue * 1.5f;
                u.AttackRange = (int)(u.StandardAttackRange * 1.5f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps + 2;
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 7:
                /*
                //allgemeine Attribute
                u.MaxHealth = (int)(u.StandardMaxHealth * 2f);
                
                u.AttackValue = u.StandardAttackValue * 2f;
                u.HealValue = u.StandardHealValue * 2f;
                u.AttackRange = (int)(u.StandardAttackRange * 2f);
                */
                //speziell für Unit
                u.MaxSteps = u.StandardMaxSteps + 3;
                //Kosten der Ausbildung auch noch anpassen

                break;
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        //zufallsaenderung(u, moralkategoriewert);     //immer ausführen, egal was für ein case
    }


//Ressourcensammler
    public void checkSkalaRessourcenSammler(RessourcenSammler rs, int moralkategoriewert){
        switch(moralkategoriewert) {
            case 1:
            /*
               //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 0.25f);
                if(rs.Health > rs.MaxHealth) rs.Health = rs.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                rs.AttackValue = rs.StandardAttackValue * 0.25f;
                rs.HealValue = rs.StandardHealValue * 0.25f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 0.25f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            //noch klasse1 methode für alle kategoien kopieren und anpassen(auf balance achten)
            case 2:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 0.5f);
                if(u.Health > rs.MaxHealth) rs.Health = rs.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                rs.AttackValue = rs.StandardAttackValue * 0.5f;
                rs.HealValue = rs.StandardHealValue * 0.5f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 0.5f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 3:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 0.75f);
                if(u.Health > rs.MaxHealth) rs.Health = rs.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
                rs.AttackValue = rs.StandardAttackValue * 0.75f;
                rs.HealValue = rs.StandardHealValue * 0.75f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 0.75f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 4:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 1f);
                
                rs.AttackValue = rs.StandardAttackValue * 1f;
                rs.HealValue = rs.StandardHealValue * 1f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 1f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 5:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 1.25f);
               
                rs.AttackValue = rs.StandardAttackValue * 1.25f;
                rs.HealValue = rs.StandardHealValue * 1.25f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 1.25f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 6:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 1.5f);
                
                rs.HealValue = rs.StandardHealValue * 1.5f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 1.5f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 7:
                /*
                //allgemeine Attribute
                rs.MaxHealth = (int)(rs.StandardMaxHealth * 2f);
                
                rs.AttackValue = rs.StandardAttackValue * 2f;
                rs.HealValue = rs.StandardHealValue * 2f;
                rs.AttackRange = (int)(rs.StandardAttackRange * 2f);
                */
                //speziell für Ressourcensammler
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        //zufallsaenderung(rs, moralkategoriewert);     //immer ausführen, egal was für ein case
    }

//TrainUnitBuilding
    public void checkSkalaTrainUnitBuilding(TrainUnitBuilding tub, int moralkategoriewert){
        switch(moralkategoriewert) {
            case 1:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            //noch klasse1 methode für alle kategoien kopieren und anpassen(auf balance achten)
            case 2:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 3:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 4:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 5:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 6:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            case 7:
                //speziell für TrainunitBuilding
                
                //Kosten der Ausbildung auch noch anpassen

                break;
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        //zufallsaenderung(tub, moralkategoriewert);     //immer ausführen, egal was für ein case
    }


    //Alkohol Boost auch noch beachten!, als Parameter übergeben? oder ist der nur für den Moralwert?
    //was ist beim leben falls kategoiewechsel? aktuelles leben auch anpassen? runter falls maxhealth niedriger als aktuelles leben, höher wenn maxhealth wieder steigt?
   
/* 
    public void klassetmpalt(VolkObject vo){
        //Allgemeine Volkobjekt Variablen:
        vo.MaxHealth = (int)(vo.StandardMaxHealth * 0.5f);
        if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
        vo.AttackValue = vo.StandardAttackValue * 0.5f;
        vo.HealValue = vo.StandardHealValue * 0.5f;
        vo.AttackRange = (int)(vo.StandardAttackRange * 0.5f);
        Debug.Log("Typ= " + vo.GetType());
        //Speziell für Gebäude or Einheiten:
        if(vo.GetType() == typeof(RessourcenSammler)){
            vo.RessourcenMoralAuswirkung = -1f;
        }else if(vo.GetType() == typeof(TrainUnitBuilding)){
            vo.MaxAusbildungen = vo.StandardMaxAusbildungen - 3;
        }else if(vo.GetType() == typeof(FoodSammler)){
            //ähnlich wie RessourcenSammler anpassen!
        }else if(vo.GetType() == typeof(Unit)){
            vo.MaxSteps = vo.StandardMaxSteps - 3;
            if(vo.MaxSteps <= 0) vo.MaxSteps = 0;
            vo.Ausbildungszeit = vo.StandardAusbildungszeit + 3;
            //Kosten der Ausbildung auch noch anpassen
        }
    }
*/



    //Zufallsänderungen, aber was ist bei kategoriewechsel? alte änderungen merken?
    public void zufallsaenderung(VolkObject vo, int moralkategoriewert){
        //erst schauen ob ein attribut geändert wird durch zufallsgenerator bei 0 wird kein attribut geändert, sonst ja
        if(UnityEngine.Random.Range(0,2) == 0) return;  //whrsch anpassen möglich. nun sind ergebnisse 0 und 1 möglich, also 50% whrsch ein zufallsattribut zu verändern
        //zufallsinteger berechnen für attributauswahl
        //hier ähnlich wie in den klasseX-methoden. 
        //generisches attribut ändern, ja oder nein?
        if(UnityEngine.Random.Range(0,2) == 1){  //wenn ja, dann noch schauen welches generische volksobjekt attribut
            int tmp = UnityEngine.Random.Range(0,4);    //momentan 4 generische attribute, auch noch anhand attribut ein Random.Range(0f,xf) schauen wie stark die maxanpassung sein kann, eventuell auch noch auf kategoien anpassen!
            switch(tmp) {
                case 1: //Maxhealth

                    break;
                case 2: //AttackValue
                    
                    break;
                case 3: //HealValue
                    
                    break;
                case 4: //AttackRange
                    
                    break;                  
                default:
                    break;
            }
        } else {    //wenn nicht generisches attribut, dann vielleicht spezielles attribut
            if(UnityEngine.Random.Range(0,2) == 1){  //wenn ja, dann noch schauen welches spezielle attribut
                //hier erstmal schauen was für ein Typ das Objekt ist!
                //.....
            }
        }
        //dann über eine switch case liste anhand zufallsintegers attribut aussuchen
        //zufallsfloat berechnen für prozentuale anpassung(attributspezifische range!)
        //dann attribut/variable anhand zufallsfloat anpassen

    }

    //weitere Auswirkungen
    public void weitereAuswirkungen(VolkObject vo){
        //Bewegung
        //Angriff
        //übergang zu Gegner
        //KI generiertes Antworten/Motzen xD
    }


}
