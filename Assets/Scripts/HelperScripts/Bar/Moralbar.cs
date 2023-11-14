using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moralbar : Bar
{
    void Start()
    {
        fullWidth = topBar.rect.width; 
        newMaxValue();
        Value = MaxValue;
    }

    public override void newMaxValue() {
        MaxValue = 1.0f;
    }
}
