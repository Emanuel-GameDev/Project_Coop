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
        PlayerCharacter playerCharacter = PlayerCharacterPoolManager.Instance.GetCharacter(character, transform);
        if (playerCharacter != null)
        {
            SetPlayerCharacter(playerCharacter);
        }

    }

    public override void SetInputHandler(PlayerInputHandler inputHandler)
    {
        base.SetInputHandler(inputHandler);
        if (inputHandler.currentCharacter != ePlayerCharacter.EmptyCharacter)
            SetCharacter(inputHandler.currentCharacter);
        else if (inputHandler.startingCharacter != ePlayerCharacter.EmptyCharacter)
            SetCharacter(inputHandler.startingCharacter);
        else
        {
            SetPlayerCharacter(PlayerCharacterPoolManager.Instance.GetFreeRandomCharacter());
            inputHandler.SetStartingCharacter(this.character);
        }

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

    public override void MenuInput(InputAction.CallbackContext context)
    {
        //if (actualCharacter != null)
        //    actualCharacter.MenuInput(context);

        //APRIRE IL MENU SENZA PASSARE DAL PLAYERCHARACTER
    }

    public override void OptionInput(InputAction.CallbackContext context)
    {
        //if (actualCharacter != null)
        //    actualCharacter.OptionInput(context);

        //APRIRE LE OPZIONI SENZA PASSARE DAL PLAYERCHARACTER
    }

    #endregion
}
