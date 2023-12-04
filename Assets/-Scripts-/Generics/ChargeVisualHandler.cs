using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeVisualHandler : MonoBehaviour
{
    [SerializeField]
    RectTransform chargeVisual;
    [SerializeField]
    RectTransform barTransform;
    private CharacterClass characterClass;
    Vector2 direction => characterClass.GetLastNonZeroDirection();

    float maxValue;
    float minValue;
    float currentValue;
    float startTime;
    bool mustEnd;

    public void Inizialize(float min, float max)
    {
        maxValue = max;
        minValue = min;
        mustEnd = false;
        SetBar();
    }

    public void StartCharging(float startTime)
    {
        this.startTime = startTime;
        chargeVisual.gameObject.SetActive(true);
        mustEnd = false;
    }

    private void SetBar()
    {
        chargeVisual.localPosition = new Vector3(0, 0, maxValue/2);
        chargeVisual.sizeDelta = new Vector2(chargeVisual.sizeDelta.x, maxValue);
    }

    public void StopCharging()
    {
        mustEnd = true;
    }


    IEnumerator Charging()
    {
        while (!mustEnd)
        {
            float duration = Time.time - startTime;//Darivedere
            float topDistance = maxValue - minValue;
            barTransform.anchoredPosition = new Vector2(barTransform.anchoredPosition.x, topDistance);
        }
        yield return null;    
    }
}
