using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : InputReceiver
{
    public PlayerCharacter ActualPlayerCharacter => actualPlayerCharacter;
    private PlayerCharacter actualPlayerCharacter;

    public void SetPlayerCharacter(PlayerCharacter character)
    {
        if (character == null)
            return;

        actualPlayerCharacter = character;
        character.characterController = this;
        playerInputHandler.SetCurrentCharacter(character.Character);
        if (character.Character != this.character)
            this.character = character.Character;
    }

    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);

        // Set pos del personaggio effettivo
        PlayerCharacter playerCharacter = PlayerCharacterPoolManager.Instance.GetCharacter(character, SpawnPositionManager.Instance.GetFreePos().spawnPos);

        if (playerCharacter != null)
        {
            SetPlayerCharacter(playerCharacter);
        }
    }

    public override void SetInputHandler(PlayerInputHandler inputHandler)
    {
        base.SetInputHandler(inputHandler);

        if(inputHandler.currentCharacter == ePlayerCharacter.EmptyCharacter && inputHandler.startingCharacter == ePlayerCharacter.EmptyCharacter)
        {
            character = PlayerCharacterPoolManager.Instance.GetFreeRandomCharacter();
            inputHandler.SetStartingCharacter(this.character);
        }


        //if (inputHandler.currentCharacter != ePlayerCharacter.EmptyCharacter)
        //    SetCharacter(inputHandler.currentCharacter);
        //else if (inputHandler.startingCharacter != ePlayerCharacter.EmptyCharacter)
        //    SetCharacter(inputHandler.startingCharacter);
        //else
        //{
        //    //Da

        //    //SetPlayerCharacter(PlayerCharacterPoolManager.Instance.GetFreeRandomCharacter());

        //    character = PlayerCharacterPoolManager.Instance.GetFreeRandomCharacter();
        //    inputHandler.SetStartingCharacter(this.character);
        //}

    }

    #region Input
    public override void MoveInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.MoveInput(context);
    }

    public override void LookInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.LookInput(context);
    }

    public override void MouseLookInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.LookInputMouse(context);
    }

    public override void AttackInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.AttackInput(context);
    }

    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.DefenseInput(context);
    }

    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.UniqueAbilityInput(context);
    }

    public override void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.ExtraAbilityInput(context);
    }

    public override void InteractInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.InteractInput(context);
    }

    public override void CancelInteractInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.CancelInteractInput(context);
    }

    public override void SwitchUpInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.SwitchUpInput(context);
    }

    public override void SwitchRightInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.SwitchRightInput(context);
    }

    public override void SwitchDownInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.SwitchDownInput(context);
    }

    public override void SwitchLeftInput(InputAction.CallbackContext context)
    {
        if (actualPlayerCharacter != null)
            actualPlayerCharacter.SwitchLeftInput(context);
    }

    public override void LockTargetInput(InputAction.CallbackContext context)
    {
        if(actualPlayerCharacter != null)
            actualPlayerCharacter.LockTargetInput(context);
    }

    #region UI
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
        if (context.performed)
            MenuManager.Instance.GoNextSubTab(playerInputHandler);
    }

    public override void SubPreviousInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.GoPreviousSubTab(playerInputHandler);
    }

    public override void ChangeVisualizationInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.ChangeVisualization(playerInputHandler);
    }
    #endregion

    #endregion
}
