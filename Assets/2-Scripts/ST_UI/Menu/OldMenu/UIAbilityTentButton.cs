using UnityEngine;
using UnityEngine.Localization.Components;

public class UIAbilityTentButton : MonoBehaviour
{
    public AbilityMenu AbilityMenu {  get; set; }
    [SerializeField]
    private GameObject openedButtonObject;
    [SerializeField]
    private GameObject closedButtonObject;

    [Line]
    public LocalizeStringEvent AbilityName;
    public LocalizeStringEvent AbilityDescription;
    
    private void Awake()
    {
        if(AbilityMenu == null)
            AbilityMenu = GetComponentInParent<AbilityMenu>();
    }

    public void Activate()
    {
        if (AbilityMenu != null)
            AbilityMenu.CloseAll();

        SetOpened(true);
    }

    public void Deactivate()
    {
        SetOpened(false);
    }

    private void SetOpened(bool value)
    {
        openedButtonObject.SetActive(value);
        closedButtonObject.SetActive(!value);
    }

}
