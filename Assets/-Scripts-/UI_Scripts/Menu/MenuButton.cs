using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField]
    private MenuInfo nextMenu;
    [SerializeField]
    private TabInfo tabToOpen;

    public void GoNextMenu()
    {
        if(nextMenu == null) return;
        if(tabToOpen == null)
            MenuManager.Instance.OpenMenu(nextMenu);
        else
            MenuManager.Instance.OpenMenu(nextMenu, tabToOpen);
    }
}