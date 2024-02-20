using UnityEngine;
using UnityEngine.Events;

public class PressInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent OnInteract;
    
    public void Interact(IInteracter interacter)
    {
        OnInteract?.Invoke();
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
