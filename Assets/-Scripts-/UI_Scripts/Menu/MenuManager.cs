using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("MenuManager");
                    _instance = singletonObject.AddComponent<MenuManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject optionMenu;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void SetPlayerActiveMenu(PlayerInputHandler player, GameObject menuRoot, GameObject firstSelection)
    {
        player.SetPlayerActiveMenu(menuRoot, firstSelection);
        menuRoot.SetActive(true);
    }   

    public void OpenPauseMenu(PlayerInputHandler player)
    {
        SetPlayerActiveMenu(player, pauseMenu, pauseMenu.GetComponent<MenuInfo>().GetFirstObjectSelected());
    }

    public void OpenOptionMenu(PlayerInputHandler player)
    {
        SetPlayerActiveMenu(player, optionMenu, optionMenu.GetComponent<MenuInfo>().GetFirstObjectSelected());
    }

    public void OpenMenu(PlayerInputHandler player, GameObject menu)
    {
        SetPlayerActiveMenu(player, menu, menu.GetComponent<MenuInfo>().GetFirstObjectSelected());
    }
}
