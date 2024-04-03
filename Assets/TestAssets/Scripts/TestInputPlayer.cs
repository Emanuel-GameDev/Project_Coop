using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputPlayer : InputReceiver
{
    public override void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.CloseMenu();
        }
    }

    public override void MenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.OpenPauseMenu(playerInputHandler);
        }
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        
    }

    public override void OptionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.OpenOptionMenu(playerInputHandler);
        }
    }

    public override void NextInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.GoNextTab(playerInputHandler);
    }

    public override void PreviousInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.GoPreviousTab(playerInputHandler);
    }

    public override void ScrollWheel(InputAction.CallbackContext context)
    {
        
    }

    public override void Submit(InputAction.CallbackContext context)
    {
        
    }

    public override void UIMenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.ClosePauseMenu();
    }

    public override void UIOptionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.CloseOptionMenu();
    }


}
