using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteracter
{
    void InteractWith(IInteractable interactable);

    void EnableInteraction(IInteractable interactable);
    void DisableInteraction(IInteractable interactable);

    GameObject GetInteracterObject();
}
