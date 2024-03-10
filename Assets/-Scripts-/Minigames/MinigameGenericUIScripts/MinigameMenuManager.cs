using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameMenuManager : MonoBehaviour
{
    [SerializeField]
    private MinigameMenu pauseMenu;
    public MinigameMenu PauseMenu => pauseMenu;

    [SerializeField]
    private MinigameMenu firstMenu;

    private MinigameMenu currentActiveMenu;
    public MinigameMenu CurrentActiveMenu => currentActiveMenu;

    private static MinigameMenuManager _instance;
    public static MinigameMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MinigameMenuManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("MinigameMenuManager");
                    _instance = singletonObject.AddComponent<MinigameMenuManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void MenuButton(InputReceiver player)
    {
        if(currentActiveMenu != null)
        {
            currentActiveMenu.MenuButton(player);
        }
        else
        {
            SetActiveMenu(pauseMenu, player);
        }
    }

    public void SubmitButton(InputReceiver player)
    {
        if (currentActiveMenu != null)
        { 
            currentActiveMenu.SubmitButton(player); 
        }
    }

    public void CancelButton(InputReceiver player)
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.CancelButton(player);
        }
    }

    public void NavigateButton(Vector2 value, InputReceiver player)
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.NavigateButton(value, player);
        }
    }
    public void SetActiveMenu(MinigameMenu minigameMenu)
    {
        SetActiveMenu(minigameMenu, GetFirstPlayer());
    }

    public void SetActiveMenu(MinigameMenu minigameMenu, InputReceiver activeReceiver)
    {
        if(minigameMenu != null)
        {
            CloseMenu();
            currentActiveMenu = minigameMenu;
            currentActiveMenu.gameObject.SetActive(true);
            currentActiveMenu.Inizialize(activeReceiver);
            SetPlayersActionMap(eInputMap.UI);
        }
        else
            ExitMenu();

    }

    private static void SetPlayersActionMap(eInputMap map)
    {
        foreach (PlayerInputHandler player in CoopManager.Instance.GetActiveHandlers())
        {
            player.SetActionMap(map);
        }
    }

    private void CloseMenu()
    {
        if(currentActiveMenu != null)
            currentActiveMenu.gameObject.SetActive(false);
        currentActiveMenu = null;
    }

    public void ExitMenu()
    {
        CloseMenu();
        SetPlayersActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
    }

    public void StartFirstMenu()
    {
        SetActiveMenu(firstMenu);
    }

    public InputReceiver GetFirstPlayer()
    {
        return CoopManager.Instance.GetActiveHandlers()[0].CurrentReceiver;
    }

}
