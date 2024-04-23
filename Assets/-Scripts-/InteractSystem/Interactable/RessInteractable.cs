using System;
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

    private int triggerCount = 0;

    private int ressCount = 0;

    private float startingTime;

    private bool updateSlider = false;

    private void Update()
    {
        if (updateSlider)
        {
            float speedMultiplier = 1f + (ressSpeedUp * (ressCount - 1));

            float progress = (Time.time - startingTime) / (ressDuration / speedMultiplier);
            ressSlider.value = Mathf.Clamp01(progress);

            if (progress >= 1f)
            {
                OnSliderComplete();
            }
        }
        else
        {
            ressSlider.value = 0;
        }
    }

    private void OnSliderComplete()
    {
        character.Ress();
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

            if (interacterVisualization != null)
            {
                interacterVisualization.SetActive(true);
                triggerCount++;
            }


        }
        Debug.Log("Entra?");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);

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
        return null;
    }

    public void Interact(IInteracter interacter)
    {
        if (ressCount == 0)
        {
            updateSlider = true;
            startingTime = Time.time;
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
