using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteracter
{
    void InteractWith(IInteractable interactable);
    void CancelInteract(IInteractable interactable);

    void EnableInteraction(IInteractable interactable);
    void DisableInteraction(IInteractable interactable);

    void DisableOtherActions();

    void EnableAllActions();

    GameObject GetInteracterObject();
}
