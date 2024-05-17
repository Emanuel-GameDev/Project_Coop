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

    [SerializeField] internal List<Canvas> table;

    [SerializeField] protected GameObject shopGroup;

    [HideInInspector]public  bool canClose;

    public virtual void Start()
    {
        shopGroup.SetActive(false);
    }

    public virtual void OpenMenu()
    {
        closeQueueInt = 0;
        int i = 0;
        canClose = true;

        shopGroup.SetActive(true);

        foreach(PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            
            //ih.SetPlayerActiveMenu(tables.gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

            //ih.MultiplayerEventSystem.SetSelectedGameObject(table[i].GetComponentInChildren<Selectable>().gameObject);
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Disable();
            actions.FindActionMap("UI").Enable();
            actions.FindActionMap("UI").FindAction("Menu").Disable();
            actions.FindAction("Cancel").performed += Menu_performed;

            i++;
        }
    }

    internal List<PlayerInputHandler> closeQueue;
    internal int closeQueueInt = 0;
    protected virtual void Menu_performed(InputAction.CallbackContext obj)
    {
        if (canClose)
        {
            //closeQueueInt++;

            //if (closeQueueInt >= CoopManager.Instance.GetActiveHandlers().Count)
            //{
                CloseMenu();
            //}

        }
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
