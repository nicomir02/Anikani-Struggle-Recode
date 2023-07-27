using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBiom : Biom
{
    //Dieses Script ist ein Unterskript von Biome
    //Manche Biome sollen nur innerhalb bestimmter Biome generieren, bzw. unter bestimmten Voraussetzungen
    //Beispielsweise sollen Berge nicht random da spawnen auf Höhe 3 sein, wenn alles andere auf Höhe 2 ist

    [SerializeField] public bool mussNachbarnSelbeHoehe;

    public bool isNachbarSelbeHoehe(Vector3Int vec) {
        return true;
    }
}
