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

    }

    private Vector2 ReadInput()
    {
        Vector2 moveInput = playerInputSystem.Player.Move.ReadValue<Vector2>();
        return moveInput;
    }

    // informasi sulla look
    public Vector3 ReadLook()
    {
        Vector3 lookInput = playerInputSystem.Player.Look.ReadValue<Vector2>();
        return new Vector3(lookInput.x, 0, lookInput.y).normalized;
    }




    private void Attack_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }

}
