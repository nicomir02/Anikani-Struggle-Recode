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

    //Alkohol Boost auch noch beachten!, als Parameter übergeben?
    public void klasse1(VolkObject vo){
        //Allgemeine Volkobjekt Variablen:
        vo.MaxHealth = (int)(vo.StandardMaxHealth * 0.5f);
        if(vo.Health > vo.MaxHealth) vo.Health = vo.MaxHealth;  //Leben nur verringern, falls maxleben niedriger wird als jetziges leben
        //vo.AttackValue = vo.StandardAttackValue * 0.5f;
        //vo.HealValue = vo.StandardHealValue * 0.5f;
        vo.AttackRange = (int)(vo.StandardAttackRange * 0.5f);
        //Speziell für Gebäude or Einheiten:
        /* klappt noch nicht ganz:
        if(vo.GetType() == RessourcenSammler){
            vo.RessourcenMoralAuswirkung = -1f;
        }else if(vo.GetType() == TrainUnitBuilding){
            vo.MaxAusbildungen = vo.StandardMaxAusbildungen - 3;
        }else if(vo.GetType() == FoodSammler){
            //ähnlich wie RessourcenSammler anpassen!
        }else if(vo.GetType() == Unit){
            vo.MaxSteps = vo.StandardMaxSteps - 3;
            if(vo.MaxSteps <= 0) vo.MaxSteps = 0;
            vo.Ausbildungszeit = vo.StandardAusbildungszeit + 3;
            //Kosten der Ausbildung auch noch anpassen
        } */
    }



    //Zufallsänderungen
    public void zufallsaenderung(VolkObject vo){
        //erst schauen ob ein attribut geändert wird durch zufallsgenerator
        //zufallsinteger berechnen für attributauswahl
        
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
