using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputReceiver : MonoBehaviour
{
    protected ePlayerCharacter character;
    protected PlayerInputHandler playerInputHandler;
    
    public virtual GameObject GetGameObject() => gameObject;

    public virtual void SetCharacter(ePlayerCharacter character) => this.character = character;

    public virtual ePlayerCharacter GetCharacter() => character;

    public virtual void SetInputHandler(PlayerInputHandler inputHandler) => playerInputHandler = inputHandler;

    public virtual PlayerInputHandler GetInputHandler() => playerInputHandler;

    public virtual void Dismiss()
    {
       
    }

    // Lista di tutti gli input di tutte le possibili mappe 
    #region MapInput

    #region Player
    public virtual void MoveInput(InputAction.CallbackContext context)
    {
       
    }

    public virtual void LookInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void AttackInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void DefenseInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void InteractInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void JoinInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void SwitchUpInput(InputAction.CallbackContext context)
    {
       
    }

    public virtual void SwitchRightInput(InputAction.CallbackContext context)
    {
       
    }

    public virtual void SwitchDownInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void SwitchLeftInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void MenuInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void OptionInput(InputAction.CallbackContext context)
    {
        
    }
    #endregion

    #region Minigame
    public virtual void MoveMinigameInput(InputAction.CallbackContext context)
    {
        
    }
    public virtual void ButtonEast(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonNorth(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonWeast(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonSouth(InputAction.CallbackContext context)
    {

    }

    #endregion

    #region UI

    public virtual void Navigate(InputAction.CallbackContext context)
    {
    }

    public virtual void Submit(InputAction.CallbackContext context)
    {
    }

    public virtual void Cancel(InputAction.CallbackContext context)
    {
    }

    public virtual void ScrollWheel(InputAction.CallbackContext context)
    {
    }

    public virtual void RandomSelection(InputAction.CallbackContext context)
    {
    }

    public virtual void UIMenuInput(InputAction.CallbackContext context)
    {
    }

    public virtual void UIOptionInput(InputAction.CallbackContext context)
    {
    }

    #endregion

    #endregion
}
