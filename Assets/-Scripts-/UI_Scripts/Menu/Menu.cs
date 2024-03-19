using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Selectable firstSelected;

    private void Start()
    {
        OpenMenu();
    }

    public void OpenMenu()
    {
        CoopManager.Instance.GetComponentInChildren<PlayerInputHandler>().MultiplayerEventSystem.SetSelectedGameObject(firstSelected.gameObject);


        foreach(PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.Disable();
            actions.FindActionMap("UI").Enable();

            actions.FindAction("Menu").performed += Menu_performed;
        }
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        CloseMenu();
    }

    public void CloseMenu()
    {
        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            //actions.FindAction("Player").Enable();
            actions.Enable();
            actions.FindActionMap("UI").Disable();

            actions.FindAction("Menu").performed -= Menu_performed;
        }
        gameObject.SetActive(false);
    }
}
