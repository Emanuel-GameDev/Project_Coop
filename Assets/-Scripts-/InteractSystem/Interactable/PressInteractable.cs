using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class PressInteractable : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("Oggetto per mostrare il comando di interazione")]
    private GameObject interacterVisualization;
    [SerializeField, Tooltip("Stringa da visualizzare nella notifica di interazione")]
    private LocalizedString localizedString;
    [SerializeField]
    private bool showNotification = true;
    [SerializeField]
    private bool disableInteracterActions = true;
    [SerializeField]
    private bool isOnePlayerInteractable = false;

    [SerializeField] UnityEvent<IInteracter> OnOnePlayerInteract;
    [SerializeField] UnityEvent<IInteracter> OnOnePlayerCancelInteract;
    [SerializeField] UnityEvent OnAllPlayersInteract;

    List<IInteracter> interacters = new List<IInteracter>();

    private int triggerCount;

    private void Start()
    {
        if (interacterVisualization != null)
            interacterVisualization.SetActive(false);
    }

    private void NotifyInteraction(IInteracter interacter)
    {
        InteractionNotificationHandler.Instance.ActivateNotification(interacter, localizedString, this);
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

    public void Interact(IInteracter interacter)
    {
        if (isOnePlayerInteractable)
        {
            OnOnePlayerInteract?.Invoke(interacter);

            if (disableInteracterActions)
                interacter.DisableOtherActions();

            if (showNotification)
                NotifyInteraction(interacter);

        }
        else if (!interacters.Contains(interacter))
        {
            interacters.Add(interacter);

            if (showNotification)
                NotifyInteraction(interacter);

            if (disableInteracterActions)
                interacter.DisableOtherActions();

            if (interacters.Count >= CoopManager.Instance.GetActiveHandlers().Count)
            {
                OnAllPlayersInteract?.Invoke();

                if (interacterVisualization != null)
                    interacterVisualization.SetActive(false);
            }
        }

    }

    public void CancelAllInteraction()
    {
        List<IInteracter> interactersCopy = new List<IInteracter>(interacters);
        foreach (IInteracter interacter in interactersCopy)
        {
            CancelInteraction(interacter);
        }
    }

    public void CancelInteraction(IInteracter interacter)
    {
        if (interacters.Contains(interacter))
        {
            interacters.Remove(interacter);
            OnOnePlayerCancelInteract?.Invoke(interacter);

            if (showNotification)
            {
                if (interacters.Count == 0)
                    InteractionNotificationHandler.Instance.DeactivateNotification(this);
                else
                    NotifyCancelInteraction(interacter);
            }


            if (disableInteracterActions)
                interacter.EnableAllActions();
        }

    }

    private void NotifyCancelInteraction(IInteracter interacter)
    {
        InteractionNotificationHandler.Instance.CancelNotification(interacter, this);
    }


    public IInteracter GetFirstInteracter()
    {
        if (interacters.Count == 0)
            return null;

        return interacters[0];
    }

    public void AbortInteraction(IInteracter interacter)
    {

    }
}
