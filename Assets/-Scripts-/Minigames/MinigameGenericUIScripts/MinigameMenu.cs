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
        activeReceiver = this.activeReceiver = activeReceiver;
    }

    protected virtual void GoNextMenu()
    {
        MinigameMenuManager.Instance.SetActiveMenu(nextMenu, activeReceiver);
    }

    protected virtual void GoPreviousMenu()
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

    public virtual void MenuButton(LabirintPlayer player)
    {
        if(MinigameMenuManager.Instance.PauseMenu != this)
        {
            SetNextMenu(MinigameMenuManager.Instance.PauseMenu);
            MinigameMenuManager.Instance.PauseMenu.previousMenu = this;
        }
        menuButton?.Invoke();
    }

    public virtual void SubmitButton(LabirintPlayer player)
    {
        submitButton?.Invoke();
    }

    public virtual void CancelButton(LabirintPlayer player)
    {
        cancelButton?.Invoke();
    }

    public virtual void NavigateButton(Vector2 direction, LabirintPlayer player)
    {
        navigateButton?.Invoke(direction);
    }

}
