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

    private void Awake()
    {
        previosMenu = defaultPreviosMenu;
        nextMenu = defaultNextMenu;
        firstObjectSelected = defaultFirstObjectSelected;
    }

    public void GoNextMenu(PlayerInputHandler player)
    {
        MenuManager.Instance.OpenMenu(player, nextMenu);
    }

    public void GoPreviusMenu(PlayerInputHandler player)
    {
        MenuManager.Instance.OpenMenu(player, previosMenu);
    }

}
