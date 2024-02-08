using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider Slider;

    public void SetSliderValue(float hp)
    {
        Slider.value = hp;
    }

    public void SetSliderMaxValue(float hp)
    {
        Slider.maxValue = hp;
        Slider.value = hp;
    }
}
