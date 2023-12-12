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
    Vector2 Direction => characterClass.GetLastNonZeroDirection();

    float maxValue;
    float minValue;
    float maxTime;
    float startTime;
    bool mustEnd;

    public void Inizialize(float min, float max, float time, CharacterClass character)
    {
        maxValue = max;
        minValue = min;
        maxTime = time;
        mustEnd = false;
        characterClass = character;
        SetBar();
        chargeVisual.gameObject.SetActive(false);
    }

    public void StartCharging(float startTime)
    {
        this.startTime = startTime;
        chargeVisual.gameObject.SetActive(true);
        mustEnd = false;
        StartCoroutine(Charging());
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
        while (!mustEnd) //da non usare il while
        {
            float duration = Time.time - startTime;
            float barLenght = Mathf.Lerp(minValue, maxValue, duration/maxTime);
            float topDistance = Mathf.Max(0, maxValue - barLenght);
            barTransform.offsetMin = new Vector2(barTransform.offsetMin.x, topDistance);
            Debug.Log($"top: {topDistance}");
            float angle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            yield return null;
        }
        chargeVisual.gameObject.SetActive(false);
    }
}
