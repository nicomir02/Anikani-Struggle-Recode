using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Random;

public class Biom : MonoBehaviour
{
    [SerializeField] public int id;

    [SerializeField] public float probability;
    [SerializeField] public float size;

    [SerializeField] public int minheight = 0;
    [SerializeField] public int maxheight = 0;

    [SerializeField] private Block mainBlock;

    private static System.Random rand = new System.Random();

    public Block getBlock() {
        return mainBlock;
    }
}
