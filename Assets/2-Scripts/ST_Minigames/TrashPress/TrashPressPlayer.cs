using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class TrashPressPlayer : InputReceiver
{
     
    [SerializeField] int playerHp = 3;
    [SerializeField] float jumpForce;
    [SerializeField] float pushForce;
    [SerializeField] private float moveSpeed = 10f;
    


    private SpriteRenderer spriteRenderer;
    private Animator animator;
    Vector2 moveDir;
    Rigidbody2D rb;
    bool isMoving;
    Vector2 lastNonZeroDirection;
    Pivot pivot;


    private void Start()
    {
        InitialSetup();
    }
    private void Update()
    {
        
    }
    private void InitialSetup()
    {    
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();   
        rb = GetComponentInChildren<Rigidbody2D>();
        pivot = GetComponentInChildren<Pivot>();
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

    }

    #region Move
    public void Move(Vector2 direction)
    {
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * moveSpeed;
        isMoving = rb.velocity.magnitude > 0.2f;

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

    }
    #endregion

  

    #region Input
    public override void MoveMinigameInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        moveDir.y = 0;
       
    }
    public override void ButtonSouth(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }
    public override void ButtonWeast(InputAction.CallbackContext context)
    {
        if(context.performed)
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
