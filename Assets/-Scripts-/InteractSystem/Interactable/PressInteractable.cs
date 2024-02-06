using UnityEngine;
using UnityEngine.Events;

public class PressInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent OnInteract;
    
    public void Interact(IInteracter interacter)
    {
        OnInteract?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
        }
    }

}
