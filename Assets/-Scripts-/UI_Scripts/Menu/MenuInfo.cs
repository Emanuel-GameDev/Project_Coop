using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject menuRoot;
    public GameObject MenuRoot
    {
        get
        {
            if (!haveTabs)
            {
                return menuRoot; 
            }
            else
            {
                return tabs[ActualTabIndex].TabRoot;
            }
        }
    }

    [SerializeField]
    private GameObject defaultFirstObjectSelected;
    private GameObject firstObjectSelected;
    public GameObject FirstObjectSelected
    {
        get
        {
            if(!haveTabs)
            {
                if (firstObjectSelected == null)
                    return defaultFirstObjectSelected;
                else
                    return firstObjectSelected;
            }
            else
            {
                return tabs[ActualTabIndex].FirstObjectSelected;
            }

            
        }
        set
        {
            firstObjectSelected = value;
        }
    }

    [SerializeField]
    private MenuInfo defaultPreviosMenu;
    private MenuInfo previousMenu;
    public MenuInfo PreviousMenu
    {
        get
        {
            if (previousMenu == null)
                return defaultPreviosMenu;
            else
                return previousMenu;
        }
        set
        {
            previousMenu = value;
        }
    }

    [SerializeField]
    private bool haveTabs = false;
    public bool HaveTabs => haveTabs;

    [SerializeField]
    private bool continuosNavigation = true;

    [SerializeField]
    private List<TabInfo> tabs = new();

    private int actualTabIndex = 0;

    private int ActualTabIndex
    {
        get => actualTabIndex;

        set
        {
            if(value < 0)
            {
                if(continuosNavigation)
                    actualTabIndex = tabs.Count - 1;
                else
                    actualTabIndex = 0;
            }   
            else if (value >= tabs.Count)
            {
                if(continuosNavigation)
                    actualTabIndex = 0;
                else
                    actualTabIndex = tabs.Count - 1;
            }
            else
            {
                actualTabIndex = value;
            }
            Debug.Log($"Actual tab index: {actualTabIndex}, Value: {value}");
        }
    }

    public void GoPreviousTab()
    {
        tabs[ActualTabIndex].TabRoot.SetActive(false);
        ActualTabIndex--;
        tabs[ActualTabIndex].TabRoot.SetActive(true);
    }

    public void GoNextTab()
    {
        tabs[ActualTabIndex].TabRoot.SetActive(false);
        ActualTabIndex++;
        tabs[ActualTabIndex].TabRoot.SetActive(true);
    }

    public void GoToTab(TabInfo tab)
    {
        int index = tabs.IndexOf(tab);

        if (index > -1)
        {
            ActualTabIndex = index;
            tabs[ActualTabIndex].TabRoot.SetActive(true);
        }
    }

    public void CloseAllTab()
    {
        foreach (TabInfo tab in tabs)
        {
            tab.TabRoot.SetActive(false);
        }
    }
}
