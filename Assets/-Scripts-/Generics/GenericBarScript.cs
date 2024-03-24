using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class GenericBarScript : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private float maxValue;
    private void OnEnable()
    {       
        slider = GetComponent<Slider>();
    }

    public void Setvalue(float value)
    {                   
        if(slider == null)
            GetComponent<Slider>();

        slider.maxValue = value;
        maxValue = value;
        slider.value = value;
    }
    public float AddValue(float value)
    {
        slider.value += value;
        if(slider.value > maxValue)
            slider.value = maxValue;

        return slider.value;
    }
    public float DecreaseValue(float value) 
    { 
        slider.value -= value;
        if(slider.value <= 0)
            slider.value = 0;

        return slider.value;
    }

    public void ResetValue()
    {
        slider.value = maxValue; 
    }
}
