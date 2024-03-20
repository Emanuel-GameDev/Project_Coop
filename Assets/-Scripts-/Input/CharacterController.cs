using UnityEngine.InputSystem;

public class CharacterController : InputReceiver
{
    public PlayerCharacter ActualCharacter => actualCharacter;
    private PlayerCharacter actualCharacter;

    public void SetPlayerCharacter(PlayerCharacter character)
    {
        actualCharacter = character;
    }

    public override void MoveInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.MoveInput(context);
    }

    public override void LookInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.LookInput(context);
    }

    public override void AttackInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.AttackInput(context);
    }

    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.DefenseInput(context);
    }

    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.UniqueAbilityInput(context);
    }

    public override void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.ExtraAbilityInput(context);
    }

    public override void InteractInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.InteractInput(context);
    }

    public override void SwitchUpInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.SwitchUpInput(context);
    }

    public override void SwitchRightInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.SwitchRightInput(context);
    }

    public override void SwitchDownInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.SwitchDownInput(context);
    }

    public override void SwitchLeftInput(InputAction.CallbackContext context)
    {
        if (actualCharacter != null)
            actualCharacter.SwitchLeftInput(context);
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
}
