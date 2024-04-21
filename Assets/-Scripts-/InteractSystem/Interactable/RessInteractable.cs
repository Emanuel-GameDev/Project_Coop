using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessInteractable : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("Oggetto per mostrare il comando di interazione")]
    private GameObject interacterVisualization;

    private int triggerCount;


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
        
    }

    public void AbortInteraction(IInteracter interacter)
    {
        
    }
}
