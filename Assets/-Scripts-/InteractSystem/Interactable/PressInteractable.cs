using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent OnAllPlayersInteract;

    List<IInteracter> interacters = new List<IInteracter>();

    public void Interact(IInteracter interacter)
    {
        if(!interacters.Contains(interacter))
            interacters.Add(interacter);

        if (interacters.Count >= CoopManager.Instance.GetActiveHandlers().Count)
            OnAllPlayersInteract?.Invoke();
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
