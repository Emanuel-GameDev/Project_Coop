using UnityEngine;

public class AbilityMenuButton : MonoBehaviour
{
    public AbilityMenu AbilityMenu {  get; set; }
    [SerializeField]
    private GameObject openedButtonObject;
    [SerializeField]
    private GameObject closedButtonObject;
    
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
