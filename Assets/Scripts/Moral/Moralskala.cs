using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moralskala : MonoBehaviour
{

    /*Skalenverteilung:
        klasse1: 0 - 10% Moral
        klasse2: 10 - 25% Moral
        klasse3: 25 - 50% Moral
        klasse4: 50 - 75% Moral
        klasse5: 75 - 90% Moral
        klasse6: 90 - 100% Moral
    */

    public void checkSkala(VolkObject vo, int moralkategoriewert){
        switch(moralkategoriewert) {
            case 1:
                klasse1(vo);
                break;
            //noch klasse1 methode für alle kategoien kopieren und anpassen(auf balance achten)
            /*case 2:
                klasse2(vo);
                break;
            case 3:
                klasse3(vo);
                break;
            case 4:
                klasse4(vo);
                break;
            case 5:
                klasse5(vo);
                break;
            case 6:
                klasse6(vo);
                break;*/
            default:
                Debug.Log("Ausserhalb Moralskala. Irgendetwas schief gelaufen");
                break;
        }
        //zufallsaenderung(vo, moralkategoriewert);     //immer ausführen, egal was für ein case
    }

    //Alkohol Boost auch noch beachten!, als Parameter übergeben?
    //was ist beim leben falls kategoiewechsel? aktuelles leben auch anpassen? runter falls maxhealth niedriger als aktuelles leben, höher wenn maxhealth wieder steigt?
    public void klasse1(VolkObject vo){
        //Allgemeine Volkobjekt Variablen:
        vo.MaxHealth = (int)(vo.StandardMaxHealth * 0.5f);
        if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
        vo.AttackValue = vo.StandardAttackValue * 0.5f;
        vo.HealValue = vo.StandardHealValue * 0.5f;
        vo.AttackRange = (int)(vo.StandardAttackRange * 0.5f);
        Debug.Log("Typ= " + vo.GetType());
        //Speziell für Gebäude or Einheiten:
        /*
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
        } */
    }



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
