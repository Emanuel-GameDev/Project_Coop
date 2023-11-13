using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.Rendering;

public class PlayerCharacter : Character
{
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up,-1);

    Vector2 lookDir;
    Vector2 moveDir;

    public Vector2 MoveDirection => moveDir;


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
            screenPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);

                worldPosition = (worldPosition - transform.position).normalized;
            }

            Debug.Log(worldPosition);
            return new Vector3(worldPosition.x, 0, worldPosition.z);
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
