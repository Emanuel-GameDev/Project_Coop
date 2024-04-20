using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class PressInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent<IInteracter> OnOnePlayerInteract;
    [SerializeField] UnityEvent OnAllPlayersInteract;
    [SerializeField] TextMeshProUGUI interactersCount;
    [SerializeField] LocalizedString localizedString;
    List<IInteracter> interacters = new List<IInteracter>();

    private void Start()
    {
        if (interactersCount != null)
            interactersCount.gameObject.SetActive(false);
    }

    public void NotifyInteraction(IInteracter interacter)
    {
        InteractionNotificationHandler.Instance.ActivateNotification(interacter, localizedString, this);
    }

    public void Interact(IInteracter interacter)
    {
        if (!interacters.Contains(interacter))
        {
            interacters.Add(interacter);

            OnOnePlayerInteract?.Invoke(interacter);
        }

        if(interactersCount != null)
        {
            interactersCount.gameObject.SetActive(true);
            interactersCount.text = $"{interacters.Count}/{GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>().Length}";
        }

        if (interacters.Count >= CoopManager.Instance.GetActiveHandlers().Count)
        {
            OnAllPlayersInteract?.Invoke();

            if (interactersCount != null)
                interactersCount.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
            
        }
    }

}
