using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : Bar
{
    // Start is called before the first frame update
    void Start()
    {
        fullWidth = topBar.rect.width; 
        newMaxValue();
        Value = MaxValue;
    }

    public override void newMaxValue() {
        MaxValue = gameObject.transform.parent.GetComponent<VolkObject>().maxHealth;
    }
}
