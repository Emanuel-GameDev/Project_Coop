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

    public void MenuButton(LabirintPlayer player)
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

    public void SubmitButton(LabirintPlayer player)
    {
        if (currentActiveMenu != null)
        { 
            currentActiveMenu.SubmitButton(player); 
        }
    }

    public void CancelButton(LabirintPlayer player)
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.CancelButton(player);
        }
    }

    public void NavigateButton(Vector2 value, LabirintPlayer player)
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.NavigateButton(value, player);
        }
    }

    public void SetActiveMenu(MinigameMenu minigameMenu, InputReceiver activeReceiver)
    {
        if(minigameMenu != null)
        {
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

    public void ExitMenu()
    {
        currentActiveMenu.gameObject.SetActive(false);
        currentActiveMenu = null;
        SetPlayersActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
    }

    public void StartFirstMenu()
    {
        SetActiveMenu(firstMenu, CoopManager.Instance.GetActiveHandlers()[0].CurrentReceiver);
    }
}
