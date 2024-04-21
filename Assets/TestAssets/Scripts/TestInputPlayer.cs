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
           MenuManager.Instance.GoBack(playerInputHandler);
        }
    }

    public override void MenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.OpenPauseMenu(playerInputHandler);
        }
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

    public override void UIMenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.ClosePauseMenu(playerInputHandler);
    }

    public override void UIOptionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.CloseOptionMenu(playerInputHandler);
    }

    public override void SubNextInput(InputAction.CallbackContext context)
    {
        if(context.performed)
            MenuManager.Instance.GoNextSubTab(playerInputHandler);
    }

    public override void SubPreviousInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.GoPreviousSubTab(playerInputHandler);
    }

    public override void ChangeVisualizationInput(InputAction.CallbackContext context)
    {
        if(context.performed)
            MenuManager.Instance.ChangeVisualization(playerInputHandler);
    }
}
