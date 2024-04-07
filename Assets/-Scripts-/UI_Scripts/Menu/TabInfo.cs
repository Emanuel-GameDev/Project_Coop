using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject tabRoot;
    public GameObject TabRoot
    {
        get
        {
            if (!haveSubTabs)
            {
                return tabRoot;
            }
            else
            {
                return tabs[ActualSubTabIndex].SubTabRoot;
            }
        }
    }

    [SerializeField]
    private Button connectedButton;
    public Button ConnectedButton => connectedButton;

    [SerializeField]
    private GameObject defaultFirstObjectSelected;
    private GameObject firstObjectSelected;
    public GameObject FirstObjectSelected
    {
        get
        {
            if (!haveSubTabs)
            {
                if (firstObjectSelected == null)
                    return defaultFirstObjectSelected;
                else
                    return firstObjectSelected;
            }
            else
            {
                return tabs[ActualSubTabIndex].FirstObjectSelected;
            }
        }
        set
        {
            firstObjectSelected = value;
        }
    }

    [SerializeField]
    private bool haveSubTabs = false;
    public bool HaveSubTabs => haveSubTabs;

    [SerializeField, Tooltip("Imposta se puoi passare dall'ultima tab alla prima e viceversa oppure no")]
    private bool continuosNavigation = true;
    [SerializeField]
    private List<SubTabInfo> tabs = new();

    private int actualSubTabIndex = 0;
    private int ActualSubTabIndex
    {
        get => actualSubTabIndex;

        set
        {
            if (value < 0)
            {
                if (continuosNavigation)
                    actualSubTabIndex = tabs.Count - 1;
                else
                    actualSubTabIndex = 0;
            }
            else if (value >= tabs.Count)
            {
                if (continuosNavigation)
                    actualSubTabIndex = 0;
                else
                    actualSubTabIndex = tabs.Count - 1;
            }
            else
            {
                actualSubTabIndex = value;
            }
            Debug.Log($"Actual SubTab index: {actualSubTabIndex}, Value: {value}");
        }
    }

    public void GoPreviousSubTab()
    {
        tabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
        ActualSubTabIndex--;
        tabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
    }

    public void GoNextSubTab()
    {
        tabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
        ActualSubTabIndex++;
        tabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
    }

    public void GoToSubTab(SubTabInfo subTab)
    {
        int index = tabs.IndexOf(subTab);

        if (index > -1)
        {
            ActualSubTabIndex = index;
            tabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
        }
    }

}
