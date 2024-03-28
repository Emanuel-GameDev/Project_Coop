using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultPreviosMenu;
    [HideInInspector]
    public GameObject previosMenu;

    [SerializeField]
    private GameObject defaultNextMenu;
    [HideInInspector]
    public GameObject nextMenu;

    [SerializeField]
    private GameObject defaultFirstObjectSelected;
    [HideInInspector]
    public GameObject firstObjectSelected;

    private void Start()
    {
        ResetAll();
    }

    public void GoNextMenu(PlayerInputHandler player)
    {
        MenuManager.Instance.OpenMenu(player, nextMenu);
    }

    public void GoPreviusMenu(PlayerInputHandler player)
    {
        MenuManager.Instance.OpenMenu(player, previosMenu);
    }

    public GameObject GetFirstObjectSelected()
    {
        if (firstObjectSelected == null)
            return defaultFirstObjectSelected;

        return firstObjectSelected;
    }

    public void ResetNextMenu()
    {
        nextMenu = defaultNextMenu;
    }

    public void ResetPreviosMenu()
    {
        previosMenu = defaultPreviosMenu;
    }

    public void ResetFirstObjectSelected()
    {
        firstObjectSelected = defaultFirstObjectSelected;
    }

    public void ResetAll()
    {
        ResetNextMenu();
        ResetPreviosMenu();
        ResetFirstObjectSelected();
    }

}
