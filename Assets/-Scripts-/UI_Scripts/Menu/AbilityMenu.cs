using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenu : MonoBehaviour
{
    [SerializeField] List<AbilityMenuButton> abilityButtons;

    public void CloseAll()
    {
        foreach (AbilityMenuButton button in abilityButtons)
        {
            button.Deactivate();
        }
    }

}
