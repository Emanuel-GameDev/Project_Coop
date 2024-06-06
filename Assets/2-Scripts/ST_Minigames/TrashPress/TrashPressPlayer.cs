using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrashPressPlayer : InputReceiver
{

    [Header("Variables")]
    [SerializeField] private int playerHp = 3;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Jump")]
    [SerializeField] float jumpForce;

    [Header("Push")]
    [SerializeField] float pushRange;
    [SerializeField] float pushForce;
    [SerializeField] int pushCooldown;



    Vector2 moveDir;
    Vector2 lastNonZeroDirection;
    SpriteRenderer spriteRenderer;
    Animator animator;
    GroundChecker groundChecker;
    Rigidbody2D rb;
    Pivot pivot;
    bool isMoving;
    bool canPush = true;



    private void Start()
    {
        InitialSetup();
    }
    private void Update()
    {
        Move(moveDir);
    }
    private void InitialSetup()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        pivot = GetComponentInChildren<Pivot>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        TrashPressManager.Instance.AddPlayer(this);
    }

    public override void SetInputHandler(PlayerInputHandler inputHandler)
    {
        base.SetInputHandler(inputHandler);
        if (inputHandler != null)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    #region Actions
    public void Jump()
    {      
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    private bool IsGrounded()
    {
        return groundChecker.isGrounded;
    }

    #region Move
    public void Move(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        isMoving = rb.velocity.x > 0.2f;

        SetSpriteDirection(lastNonZeroDirection);
    }
    public void SetSpriteDirection(Vector2 direction)
    {

        Vector3 scale = pivot.gameObject.transform.localScale;
        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
    }
    #endregion

    public void PushPlayers()
    {
        Debug.LogWarning("SPINGO");
        canPush = false;

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, pushRange, targetLayer);

        foreach (Collider2D col in objectsInRange)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != this.rb)
            {
                if (rb != null)
                {
                    // Calculate direction from the force field center to the object
                    Vector3 direction = col.transform.position - transform.position;

                    // Apply the push force
                    rb.AddForce(direction.normalized * pushForce, ForceMode2D.Impulse);
                }
            }
        }


        StartCoroutine(PushCooldown());


    }
    private IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCooldown);
        canPush = true;
    }
    #endregion



    #region Input
    public override void MoveAnalog(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        if (moveDir != Vector2.zero)
            lastNonZeroDirection = moveDir;

    }
    public override void ButtonSouth(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            Jump();
        }
    }
    public override void ButtonWeast(InputAction.CallbackContext context)
    {
        if (context.performed && canPush)
        {
            PushPlayers();
        }
    }
    public override void MenuInput(InputAction.CallbackContext context)
    {
        if (context.performed && MenuManager.Instance.ActualMenu == null)
            MenuManager.Instance.OpenPauseMenu(playerInputHandler);
    }
    public override void OptionInput(InputAction.CallbackContext context)
    {
        if (context.performed && MenuManager.Instance.ActualMenu == null)
            MenuManager.Instance.OpenOptionMenu(playerInputHandler);
    }

    #region UI

    public override void UIMenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.ClosePauseMenu(playerInputHandler);
    }

    public override void UIOptionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.CloseOptionMenu(playerInputHandler);
    }

    #endregion

    #endregion
}
