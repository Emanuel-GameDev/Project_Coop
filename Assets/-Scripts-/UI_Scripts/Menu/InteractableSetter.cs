using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableSetter : MonoBehaviour
{
    [SerializeField, ReorderableList]
    List<Selectable> interactables = new();

    public void EnableInteract()
    {
        foreach (Selectable selectable in interactables) selectable.interactable = true;
    }

    public void DisableInteract()
    {
        foreach (Selectable selectable in interactables) selectable.interactable = false;
    }

}
