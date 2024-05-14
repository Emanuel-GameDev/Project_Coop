using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(IInteracter interacter);
    void CancelInteraction(IInteracter interacter);

    void AbortInteraction(IInteracter interacter);

    IInteracter GetFirstInteracter();
}
