using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class GenericBarScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;
    
    private float maxValue;
    private void OnEnable()
    {       
        slider = GetComponent<Slider>();
        fill = slider.fillRect.gameObject.GetComponent<Image>();
              
    }

    public void SetMaxValue(float maxValue)
    {                   
        
        slider.maxValue = maxValue;
        this.maxValue = maxValue;
        slider.value = maxValue;

       
        fill.color = gradient.Evaluate(1f);
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
    public void SetValue(float value) 
    {
        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void ResetValue()
    {
        slider.value = maxValue; 
    }
}
