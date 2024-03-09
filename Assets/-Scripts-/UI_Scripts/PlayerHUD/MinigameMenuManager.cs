using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameMenuManager : MonoBehaviour
{
    [SerializeField]
    private List<MinigameMenu> menus = new List<MinigameMenu>();

    private List<MinigameMenu> currentActiveMenus;


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

    public void MenuButton(InputAction.CallbackContext context, LabirintPlayer player)
    {

    }

    public void SubmitButton(InputAction.CallbackContext context, LabirintPlayer player)
    {

    }

    public void CancelButton(InputAction.CallbackContext context, LabirintPlayer player)
    {

    }

    public void NavigateButton(InputAction.CallbackContext context, LabirintPlayer player)
    {

    }

    public void AddMinigameMenu(MinigameMenu minigameMenu)
    {
        foreach(MinigameMenu menu in menus)
        {
            if (menu.GetType() == minigameMenu.GetType())
                return;
        }

        menus.Add(minigameMenu);
    }

    public void SetActiveMenu(MinigameMenu minigameMenu)
    {
        throw new NotImplementedException();
    }

    public void DisactivateMenu(MinigameMenu minigameMenu)
    {
        throw new NotImplementedException();
    }
}
