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
            if (!haveTabs)
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
    private InteractableSetter interactableSetter;
    public InteractableSetter InteractableSetter => interactableSetter;

    [Header("Tabs Settings")]

    [SerializeField]
    private bool haveTabs = false;
    public bool HaveTabs => haveTabs;
    public bool HaveSubTabs => tabs[ActualTabIndex].HaveSubTabs;

    [SerializeField, Tooltip("Imposta se puoi passare dall'ultima tab alla prima e viceversa oppure no")]
    private bool continuosNavigation = true;

    [SerializeField]
    private int defaultTabIndex = 0;

    [SerializeField]
    private List<TabInfo> tabs = new();

    private int actualTabIndex = 0;

    private int ActualTabIndex
    {
        get => actualTabIndex;

        set
        {
            if (value < 0)
            {
                if (continuosNavigation)
                    actualTabIndex = tabs.Count - 1;
                else
                    actualTabIndex = 0;
            }
            else if (value >= tabs.Count)
            {
                if (continuosNavigation)
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

    public void Inizialize() 
    {
        if (haveTabs)
        {
            foreach (TabInfo tab in tabs)
            {
                tab.Inizialize();
                tab.TabRoot.SetActive(false);
            }
        }
    }

    public void GoPreviousTab()
    {
        tabs[ActualTabIndex].TabRoot.SetActive(false);
        tabs[ActualTabIndex].DeselectTabButton();
        ActualTabIndex--;
        tabs[ActualTabIndex].TabRoot.SetActive(true);
        tabs[ActualTabIndex].SelectTabButton();
    }

    public void GoNextTab()
    {
        tabs[ActualTabIndex].TabRoot.SetActive(false);
        tabs[ActualTabIndex].DeselectTabButton();
        ActualTabIndex++;
        tabs[ActualTabIndex].TabRoot.SetActive(true);
        tabs[ActualTabIndex].SelectTabButton();
    }

    public void GoToTab(TabInfo tab)
    {
        if(tab == null)
        {
            GoDefaultTab();
        }

        int index = tabs.IndexOf(tab);

        if (index > -1)
        {
            tabs[ActualTabIndex].TabRoot.SetActive(false);
            tabs[ActualTabIndex].DeselectTabButton();
            ActualTabIndex = index;
            tabs[ActualTabIndex].TabRoot.SetActive(true);
            tabs[ActualTabIndex].SelectTabButton();
        }
    }

    public void GoDefaultTab()
    {
        if (defaultTabIndex < tabs.Count)
        {
            tabs[ActualTabIndex].TabRoot.SetActive(false);
            ActualTabIndex = defaultTabIndex;
            tabs[ActualTabIndex].TabRoot.SetActive(true);
            tabs[ActualTabIndex].SelectTabButton();
        }
    }


    public void CloseAllTab()
    {
        foreach (TabInfo tab in tabs)
        {
            tab.DeselectTabButton();
            tab.TabRoot.SetActive(false);
        }
    }

    public void GoNextSubTab()
    {
        tabs[ActualTabIndex].GoNextSubTab();
    }

    public void GoPreviousSubTab()
    {
        tabs[ActualTabIndex].GoPreviousSubTab();
    }
}
