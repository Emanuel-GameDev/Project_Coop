using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] protected Selectable firstSelected;
    [SerializeField] CanvasGroup tables;

    [SerializeField] List<Canvas> table;

    [SerializeField] protected GameObject shopGroup;

    [HideInInspector]public  bool canClose;

    public virtual void Start()
    {
        shopGroup.SetActive(false);
    }

    public virtual void OpenMenu()
    {
        int i = 0;
        canClose = true;

        shopGroup.SetActive(true);

        foreach(PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            
            ih.SetPlayerActiveMenu(tables.gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

            ih.MultiplayerEventSystem.SetSelectedGameObject(table[i].GetComponentInChildren<Selectable>().gameObject);
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Disable();
            actions.FindActionMap("UI").Enable();
            actions.FindActionMap("UI").FindAction("Menu").Disable();
            actions.FindAction("Cancel").performed += Menu_performed;

            i++;
        }
    }
    protected virtual void Menu_performed(InputAction.CallbackContext obj)
    {
        if(canClose)
            CloseMenu();
    }

    public virtual void CloseMenu()
    {
        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Enable();
            
            actions.FindActionMap("UI").Disable();
            actions.FindActionMap("UI").FindAction("Menu").Enable();

            actions.FindAction("Cancel").performed -= Menu_performed;
        }
        shopGroup.SetActive(false); 
    }
}
