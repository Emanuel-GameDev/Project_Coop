using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character
{
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up, -1);

    Vector2 lookDir;
    Vector2 moveDir;

    public Vector2 MoveDirection => moveDir;


    private void Update()
    {
        Move(moveDir);
    }

    #region Input

    #region Look
    public void LookInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            lookDir = context.ReadValue<Vector2>();
    }

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

    #endregion

    #region SwitchCharacters

    public void SwitchUp(InputAction.CallbackContext context)
    {
        GameManager.Instance.coopManager.SwitchCharacter(this, 0);
    }

    public void SwitchRight(InputAction.CallbackContext context)
    {
        GameManager.Instance.coopManager.SwitchCharacter(this, 1);
    }

    public void SwitchDown(InputAction.CallbackContext context)
    {
        GameManager.Instance.coopManager.SwitchCharacter(this, 2);
    }
    public void SwitchLeft(InputAction.CallbackContext context)
    {
        GameManager.Instance.coopManager.SwitchCharacter(this, 3);
    }

    #endregion

    public void AttackInput(InputAction.CallbackContext context)
    {
        Attack(context);
    }

    public void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        UseUniqueAbility(context);
    }

    public void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        UseExtraAbility(context);
    }

    public void DefenseInput(InputAction.CallbackContext context)
    {
        Defend(context);
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    #endregion

}
