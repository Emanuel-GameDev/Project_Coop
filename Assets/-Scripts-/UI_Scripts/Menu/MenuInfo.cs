using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject menuRoot;
    public GameObject MenuRoot => menuRoot;

    [SerializeField]
    private GameObject defaultFirstObjectSelected;
    private GameObject firstObjectSelected;
    public GameObject FirstObjectSelected
    {
        get
        {
            if (firstObjectSelected == null)
                return defaultFirstObjectSelected;
            else
                return firstObjectSelected;
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
    private bool haveTab = false;
    public bool HaveTab => haveTab;
    [SerializeField]
    private MenuInfo NextTabMenu;
    public MenuInfo NextTab => NextTabMenu;
    [SerializeField]
    private MenuInfo PreviousTabMenu;
    public MenuInfo PreviousTab => PreviousTabMenu;

}
