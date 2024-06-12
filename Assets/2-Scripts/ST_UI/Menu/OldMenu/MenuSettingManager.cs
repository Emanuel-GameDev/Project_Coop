using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class MenuSettingManager : MonoBehaviour
{
    [Header("Description Settings")]
    [SerializeField]
    private LocalizeStringEvent settingDescriptionEvent;
    [SerializeField]
    private LocalizedString defaultString;

    [Header("Interactable Settings")]
    [SerializeField]
    private GameObject settersRoot;

    [SerializeField, ReorderableList]
    List<InteractableSetter> interactableSetters = new();

    private InteractableSetter lastActiveSetter;

    private void Awake()
    {
        InitialSetup();
    }

    private void InitialSetup()
    {
        if (settersRoot != null)
        {
            foreach (InteractableSetter setter in settersRoot.GetComponentsInChildren<InteractableSetter>())
            {
                if (!interactableSetters.Contains(setter))
                    interactableSetters.Add(setter);
            }
        }

        foreach (InteractableSetter setter in interactableSetters)
        {
            setter.MenuSettingManager = this;
        }
    }

    private void OnDisable()
    {
        DisableAllSetters();
    }

    public void DisableAllSetters()
    {
        foreach (InteractableSetter setter in interactableSetters)
        {
            setter.DisableInteract();
        }
        lastActiveSetter = null;
    }

    public void SetDescription(LocalizedString localizationString)
    {
        if (settingDescriptionEvent != null)
        {
            if (localizationString != null)
            {
                settingDescriptionEvent.StringReference = localizationString;
            }
            else
            {
                settingDescriptionEvent.StringReference = defaultString;
            }
        }
          
    }

    public void ChangeActiveSetter(InteractableSetter setter)
    {
        if (lastActiveSetter != null && lastActiveSetter != setter)
        {
            lastActiveSetter.DisableInteract();
        }
        else
            DisableAllSetters();

        lastActiveSetter = setter;
    }
}
