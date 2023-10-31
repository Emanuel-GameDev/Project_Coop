using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCharacter : Character
{
    [SerializeField] private PlayerInputSystem playerInputSystem;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        playerInputSystem.Player.Attack.performed += Attack_performed;
    }

    private void Update()
    {
        Move(ReadInput());
        if (Input.GetKeyUp(KeyCode.E))
            UseUniqueAbility();
    }

    private Vector2 ReadInput()
    {
        Vector2 moveInput = playerInputSystem.Player.Move.ReadValue<Vector2>();
        return moveInput;
    }


    private void Attack_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }

}
