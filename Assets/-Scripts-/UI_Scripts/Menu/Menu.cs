using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Selectable firstSelected;
    [SerializeField] CanvasGroup tables;

    [SerializeField] List<Canvas> table;

    [SerializeField] GameObject shopGroup;

    private void Start()
    {
        shopGroup.SetActive(false);
    }

    public void OpenMenu()
    {
        int i = 0;


        shopGroup.SetActive(true);

        foreach(PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            
            ih.SetPlayerActiveMenu(tables.gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

            ih.MultiplayerEventSystem.SetSelectedGameObject(table[i].GetComponentInChildren<Selectable>().gameObject);
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            //actions.Disable();
            actions.FindActionMap("Player").Disable();
            actions.FindActionMap("UI").Enable();

            actions.FindAction("Menu").performed += Menu_performed;

            i++;
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

            actions.FindActionMap("Player").Enable();
            //actions.Enable();
            actions.FindActionMap("UI").Disable();

            actions.FindAction("Menu").performed -= Menu_performed;
        }
        shopGroup.SetActive(false); 
    }
}
