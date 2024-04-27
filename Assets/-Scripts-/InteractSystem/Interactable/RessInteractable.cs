using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    PlayerCharacter character;
    [SerializeField, Tooltip("Oggetto per mostrare il comando di interazione")]
    private GameObject interacterVisualization;
    [SerializeField, Tooltip("Tempo per resuscitare il player")]
    private float ressDuration = 5f;
    [SerializeField, Tooltip("Velocità extra di ress, per player, in percentuale"), Range(0, 1)]
    private float ressSpeedUp = 0.15f;
    [SerializeField]
    private Slider ressSlider;

    private List<IInteracter> interacters = new();

    private int triggerCount = 0;

    private int ressCount = 0;

    private float elapsedTime;

    private bool updateSlider = false;

    private void Update()
    {
        if (updateSlider)
        {
            float speedMultiplier = 1f + (ressSpeedUp * (ressCount - 1));
            elapsedTime += Time.deltaTime * speedMultiplier;

            float progress = elapsedTime / ressDuration;
            ressSlider.value = Mathf.Clamp01(progress);

            if (progress >= 1f)
            {
                OnSliderComplete();
            }

            Debug.Log($"ressing; M: {speedMultiplier}, P : {progress}, E: {elapsedTime}");
        
        }
        else
        {
            ressSlider.value = 0;
        }
    }

    private void OnSliderComplete()
    {
        character.Ress();
        ResetRess();
    }

    private void ResetRess()
    {
        updateSlider = false;
        elapsedTime = 0;
        ressCount = 0;
        triggerCount = 0;
        interacterVisualization.SetActive(false);
        foreach (IInteracter interacter in interacters)
        {
            interacter.DisableInteraction(this);
        }
        interacters.Clear();
    }

    private void Start()
    {
        interacterVisualization.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
            interacters.Add(interacter);

            if (interacterVisualization != null)
            {
                interacterVisualization.SetActive(true);
                triggerCount++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
            interacters.Remove(interacter);

            if (interacterVisualization != null)
            {
                triggerCount--;
                if (triggerCount <= 0)
                    interacterVisualization.SetActive(false);
            }
        }
    }



    public void CancelInteraction(IInteracter interacter)
    {

    }

    public IInteracter GetFirstInteracter()
    {
        return interacters.Count > 0 ? interacters[0] : null;
    }

    public void Interact(IInteracter interacter)
    {
        if (ressCount == 0)
        {
            updateSlider = true;
            elapsedTime = 0;
        }

        ressCount++;
    }

    public void AbortInteraction(IInteracter interacter)
    {
        ressCount--;
        if (ressCount <= 0)
        {
            updateSlider = false;
        }
    }
}
