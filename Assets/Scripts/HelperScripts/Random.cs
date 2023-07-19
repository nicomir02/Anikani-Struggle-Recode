using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random
{
    private static System.Random rand = new System.Random();

    public float NextFloat(float min, float max) {
        return (float)(rand.NextDouble() * (max - min) + min);
    }

    public float NextFloat() {
        return (float)(rand.NextDouble());
    }

    public int Next(int max) {
        return rand.Next(0, max);
    }

    public int Next(int min, int max) {
        return rand.Next(min, max);
    }
}
