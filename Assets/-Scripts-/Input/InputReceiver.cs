using UnityEngine;
using UnityEngine.InputSystem;

public interface InputReceiver
{
    void SetCharacter(ePlayerCharacter character);
    ePlayerCharacter GetCharacter();

    GameObject GetReceiverObject();

    void SetInputHandler(PlayerInputHandler inputHandler);
    PlayerInputHandler GetInputHandler();

    void Dismiss();

    // Lista di tutti gli input di tutte le possibili mappe 
    #region MapInput

    #region Player
    void MoveInput(InputAction.CallbackContext context);

    void LookInput(InputAction.CallbackContext context);

    void AttackInput(InputAction.CallbackContext context);

    void DefenseInput(InputAction.CallbackContext context);

    void UniqueAbilityInput(InputAction.CallbackContext context);

    void ExtraAbilityInput(InputAction.CallbackContext context);

    void InteractInput(InputAction.CallbackContext context);

    void JoinInput(InputAction.CallbackContext context);

    void SwitchUpInput(InputAction.CallbackContext context);

    void SwitchRightInput(InputAction.CallbackContext context);

    void SwitchDownInput(InputAction.CallbackContext context);

    void SwitchLeftInput(InputAction.CallbackContext context);

    void MenuInput(InputAction.CallbackContext context);

    void OptionInput(InputAction.CallbackContext context);
    #endregion

    #region Minigame
    void MoveMinigameInput(InputAction.CallbackContext context);
    #endregion

    #region UI

    void Navigate(InputAction.CallbackContext context);

    void Submit(InputAction.CallbackContext context);

    void RandomSelection(InputAction.CallbackContext context);

    void Cancel(InputAction.CallbackContext context);

    void ScrollWheel(InputAction.CallbackContext context);

    #endregion
    #endregion
}
