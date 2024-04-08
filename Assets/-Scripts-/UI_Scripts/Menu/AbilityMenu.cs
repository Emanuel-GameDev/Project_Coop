using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityMenu : MonoBehaviour, IVisualizatorChanger
{
    [SerializeField] List<AbilityMenuButton> abilityButtons;

    private void Awake()
    {
        abilityButtons = new();
        abilityButtons = GetComponentsInChildren<AbilityMenuButton>().ToList<AbilityMenuButton>();
    }

    public void CloseAll()
    {
        foreach (AbilityMenuButton button in abilityButtons)
        {
            button.Deactivate();
        }
    }

    public void SetActiveVisualizationChanger()
    {
        MenuManager.Instance.SetActiveVisualizationChanger(this);
    }

    public void ChangeVisualization()
    {
        //TODO
        Debug.Log($"Funziona? {transform.parent.parent.parent.name}");
    }


}
