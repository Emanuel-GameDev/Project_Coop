using UnityEngine.InputSystem;
using UnityEngine;

public class LabirintPlayer : LabirintMovment
{
    public void MoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = context.ReadValue<Vector2>();
        moveDir = Utility.YtoZ(SetVector01(inputDirection));
        //Debug.Log($"inputDir: {inputDirection}, MoveDir: {moveDir}");
    }
    public void StartInput(InputAction.CallbackContext context)
    {

    }

    public void SelectInput(InputAction.CallbackContext context)
    {

    }
}
