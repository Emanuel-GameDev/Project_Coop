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

        foreach (AbilityMenuButton button in abilityButtons)
            button.AbilityMenu = this;
        
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

    private void OnEnable()
    {
        CloseAll();
        abilityButtons[0].Activate();
    }

}
