using UnityEngine;
using UnityEngine.InputSystem;


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
        lookDir = context.ReadValue<Vector2>();
    }

    // informasi sulla look
    public Vector3 ReadLook()
    {
        return new Vector3(lookDir.x, 0, lookDir.y).normalized;
    }

    public void Attack_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }

    public void UniqueAbility_performed(InputAction.CallbackContext context)
    {
        UseUniqueAbility();
    }

    public void ExtraAbility_performed(InputAction.CallbackContext context)
    {
        UseExtraAbility();
    }

    public void Defense_performed(InputAction.CallbackContext context)
    {
        Defend();
    }

    public void Move_performed(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
}
