using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class GenericBarScript : MonoBehaviour
{
    private float sliderValue;
    private float maxValue;
    private void Awake()
    {
        sliderValue = GetComponent<Slider>().value;
    }

    public void Setvalue(float value)
    {
        sliderValue=value;      
        maxValue = value;
        
       
    }
    public float AddValue(float value)
    {
        sliderValue += value;
        if(sliderValue > maxValue)
            sliderValue = maxValue;

        return sliderValue;
    }
    public float DecreaseValue(float value) 
    { 
        sliderValue -= value;
        if(sliderValue <= 0)
            sliderValue = 0;

        return sliderValue;
    }
}
