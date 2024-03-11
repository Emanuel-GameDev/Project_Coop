using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public abstract class MinigameMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    protected bool allowMultipleUsers;
    public bool AllowMultipleUsers => allowMultipleUsers;

    [SerializeField]
    protected MinigameMenu nextMenu;
    [SerializeField]
    protected MinigameMenu previousMenu;

    [Header("Events")]
    [SerializeField]
    protected UnityEvent menuButton;
    [SerializeField]
    protected UnityEvent submitButton;
    [SerializeField]
    protected UnityEvent cancelButton;
    [SerializeField]
    protected UnityEvent<Vector2> navigateButton;

    protected InputReceiver activeReceiver;



    public virtual void Inizialize(InputReceiver activeReceiver)
    {
        this.activeReceiver = activeReceiver;
    }

    public virtual void GoNextMenu()
    {
        MinigameMenuManager.Instance.SetActiveMenu(nextMenu, activeReceiver);
    }

    public virtual void GoPreviousMenu()
    {
        MinigameMenuManager.Instance.SetActiveMenu(previousMenu, activeReceiver);
    }

    protected virtual void ExitMenu()
    {
        MinigameMenuManager.Instance.ExitMenu();
    }

    public virtual void SetpreviousMenu(MinigameMenu previousMenu)
    {
        this.previousMenu = previousMenu;
    }

    public virtual void SetNextMenu(MinigameMenu nextMenu)
    {
        this.nextMenu = nextMenu;
    }

    public virtual void MenuButton(InputReceiver player)
    {
        if(MinigameMenuManager.Instance.PauseMenu != this)
        {
            MinigameMenuManager.Instance.SetActiveMenu(MinigameMenuManager.Instance.PauseMenu, player);
            MinigameMenuManager.Instance.PauseMenu.previousMenu = this;
        }
        else if (CheckPlayer(player))
            menuButton?.Invoke();
    }

    public virtual void SubmitButton(InputReceiver player)
    {
        if(CheckPlayer(player))
            submitButton?.Invoke();
    }

    protected virtual bool CheckPlayer(InputReceiver player)
    {
        if (!allowMultipleUsers)
            return activeReceiver == player;
        else 
            return true;    
    }

    public virtual void CancelButton(InputReceiver player)
    {
        if (CheckPlayer(player))
            cancelButton?.Invoke();
    }

    public virtual void NavigateButton(Vector2 direction, InputReceiver player)
    {
        if (CheckPlayer(player))
            navigateButton?.Invoke(direction);
    }

}
