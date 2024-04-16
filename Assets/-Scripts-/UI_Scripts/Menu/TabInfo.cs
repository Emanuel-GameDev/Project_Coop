using System.Collections.Generic;
using UnityEngine;

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
                return subTabs[ActualSubTabIndex].SubTabRoot;
            }
        }
    }

    [SerializeField]
    private TabSelection connectedButton;
    public TabSelection ConnectedButton => connectedButton;

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
                return subTabs[ActualSubTabIndex].FirstObjectSelected;
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
    private int defaultSubTabIndex = 0;

    [SerializeField]
    private List<SubTabInfo> subTabs = new();

    private int actualSubTabIndex = 0;
    private int ActualSubTabIndex
    {
        get => actualSubTabIndex;

        set
        {
            if (value < 0)
            {
                if (continuosNavigation)
                    actualSubTabIndex = subTabs.Count - 1;
                else
                    actualSubTabIndex = 0;
            }
            else if (value >= subTabs.Count)
            {
                if (continuosNavigation)
                    actualSubTabIndex = 0;
                else
                    actualSubTabIndex = subTabs.Count - 1;
            }
            else
            {
                actualSubTabIndex = value;
            }
            Debug.Log($"Actual SubTab index: {actualSubTabIndex}, Value: {value}");
        }
    }

    public void Inizialize()
    {
        if (haveSubTabs)
        {
            foreach (SubTabInfo subTab in subTabs)
            {
                subTab.Inizialize();
                subTab.SubTabRoot.SetActive(false);
            }
        }
    }

    public void GoPreviousSubTab()
    {
        subTabs[ActualSubTabIndex].DeselectTabButton();
        subTabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
        ActualSubTabIndex--;
        subTabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
        subTabs[ActualSubTabIndex].SelectTabButton();
    }

    public void GoNextSubTab()
    {
        subTabs[ActualSubTabIndex].DeselectTabButton();
        subTabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
        ActualSubTabIndex++;
        subTabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
        subTabs[ActualSubTabIndex].SelectTabButton();
    }

    public void GoToSubTab(SubTabInfo subTab)
    {
        if(subTab == null)
        {
            GoDefaultSubTab();
            return;
        }

        int index = subTabs.IndexOf(subTab);

        if (index > -1)
        {
            subTabs[ActualSubTabIndex].DeselectTabButton();
            subTabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
            ActualSubTabIndex = index;
            subTabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
            subTabs[ActualSubTabIndex].SelectTabButton();
        }
    }

    public void GoDefaultSubTab()
    {
        if (defaultSubTabIndex < subTabs.Count)
        {
            subTabs[ActualSubTabIndex].DeselectTabButton();
            subTabs[ActualSubTabIndex].SubTabRoot.SetActive(false);
            ActualSubTabIndex = defaultSubTabIndex;
            subTabs[ActualSubTabIndex].SubTabRoot.SetActive(true);
            subTabs[ActualSubTabIndex].SelectTabButton();
        }
    }

    public void SelectTabButton()
    {
        if (connectedButton != null)
            connectedButton.Select();
    }
    public void DeselectTabButton()
    {
        if (connectedButton != null)
            connectedButton.Deselect();
    }

}
