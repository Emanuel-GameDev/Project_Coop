using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class InteractableSetter : MonoBehaviour
{
    [SerializeField]
    LocalizedString interactableDescription;

    [SerializeField, ReorderableList]
    List<Selectable> interactables = new();

    public MenuSettingManager MenuSettingManager { get; set; }

    public void EnableInteract()
    {
        if (MenuSettingManager != null)
            MenuSettingManager.ChangeActiveSetter(this);

        if (interactableDescription != null)
            MenuSettingManager.SetDescription(interactableDescription);

        foreach (Selectable selectable in interactables) selectable.interactable = true;
    }

    public void DisableInteract()
    {
        foreach (Selectable selectable in interactables) selectable.interactable = false;
    }

}
