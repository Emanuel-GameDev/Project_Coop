using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityMenu : MonoBehaviour, IVisualizationChanger
{
    [SerializeField] 
    List<AbilityMenuButton> abilityButtons;

    [SerializeField]
    VisualizationChangerHandler visualizationChangerHandler;

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
        MenuManager.Instance.SetActiveVisualizationChanger(visualizationChangerHandler);
    }

    public void ChangeVisualization()
    {
        if(visualizationChangerHandler == null)
        {
            Debug.LogError("VisualizationChangerHandler is null");
            return;
        }

        visualizationChangerHandler.ChangeVisualization();
    }


}
