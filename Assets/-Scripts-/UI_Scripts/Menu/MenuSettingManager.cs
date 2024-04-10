using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class MenuSettingManager : MonoBehaviour
{
    [SerializeField]
    private LocalizeStringEvent settingDescriptionEvent;
    [SerializeField]
    private LocalizedString defaultString;

    private LocalizeStringEvent currentLocalizationEvent;

    [SerializeField, ReorderableList]
    List<InteractableSetter> interactableSetters = new();

    public void DisableAllSetters()
    {
        foreach (InteractableSetter setter in interactableSetters)
        {
            setter.DisableInteract();
        }
    }

    public void SetCurrentLocalizationEvent(LocalizeStringEvent localizationString)
    {
        currentLocalizationEvent = localizationString;

        if (currentLocalizationEvent == null)
        {
            settingDescriptionEvent.StringReference = defaultString;
        }
        else
        {
            settingDescriptionEvent.StringReference = currentLocalizationEvent.StringReference;
        }
    }


}
