using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuInputPlayer : InputReceiver
{
    public override void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           MenuManager.Instance.GoBack(playerInputHandler);
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
