using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Moralbar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void Change(float value) {
        slider.value += value;
    }
}
