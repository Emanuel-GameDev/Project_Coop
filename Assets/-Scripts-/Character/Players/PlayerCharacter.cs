using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.Rendering;

public class PlayerCharacter : Character
{
    Vector2 lookDir;
    Vector2 moveDir;

    private void Update()
    {
        Move(moveDir);
    }

    public void Look_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
            lookDir = context.ReadValue<Vector2>();
    }

    // informasi sulla look
    public Vector3 ReadLook()
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            //perndo la look dal player.input utilizzando il gamepad
            return new Vector3(lookDir.x, 0, lookDir.y).normalized;           
        }
        else
        {
            //prendo la look con un raycast dal mouse
            return new Vector3(lookDir.x,0,lookDir.y).normalized;
        }
       
    }

    public void Attack_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
            Attack();
    }

    public void UniqueAbility_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
            UseUniqueAbility();
    }

    public void ExtraAbility_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
            UseExtraAbility();
    }

    public void Defense_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
            Defend();
    }

    public void Move_performed(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
}
