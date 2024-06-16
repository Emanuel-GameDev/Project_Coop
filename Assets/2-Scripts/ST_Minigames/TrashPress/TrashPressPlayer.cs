using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class TrashPressPlayer : InputReceiver
{

    [SerializeField]
    private SpriteLibrary spriteLibrary;

    [Header("Variables")]
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float inAirSpeed = 5f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallAcceleration = 1.5f;


    [Header("Push")]
    [SerializeField] float pushRange;
    [SerializeField] float pushForce;
    [SerializeField] int pushCooldown;

    [Header("Audio")]
    [SerializeField] AudioClip onHitSound;
    [SerializeField] AudioClip onDeathSound;
    [SerializeField] AudioClip onPushSound;
    [SerializeField] AudioClip onJumpSound;


    public float surviveTime;
    int currentHp;
    Vector2 moveDir;
    Vector2 lastNonZeroDirection;
    SpriteRenderer spriteRenderer;
    Animator animator;
    GroundChecker groundChecker;
    Rigidbody2D rb;
    Pivot pivot;
    bool isMoving;
    bool canPush = true;
    private bool isDead;
    private float speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Trash>() != null)
        {
            TakeDamage();
        }
        if (collision.GetComponentInParent<Press>() != null)
        {
            TakeDamage();
        }
    }

    private void Start()
    {
        InitialSetup();
    }
    private void Update()
    {
        Move(moveDir, speed);
        if (!IsGrounded() && rb.velocity.y < 0)
        {
            //fall faster
            animator.SetBool("IsFalling",true);
            rb.velocity += Vector2.down * fallAcceleration * Time.deltaTime;           
        }
        if (IsGrounded())
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsFalling", false);
        }
        if(!isDead)
        {
            surviveTime += Time.deltaTime;
        }
    }
    private void InitialSetup()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        pivot = GetComponentInChildren<Pivot>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        TrashPressManager.Instance.AddPlayer(this);
        currentHp = maxHp;
        speed = moveSpeed;
        surviveTime = 0;
        spriteLibrary = GetComponentInChildren<SpriteLibrary>();
    }
    public override void SetInputHandler(PlayerInputHandler inputHandler)
    {
        base.SetInputHandler(inputHandler);
        if (inputHandler != null)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (inputHandler.currentCharacter != ePlayerCharacter.EmptyCharacter)
            {
                Debug.Log(spriteLibrary);
                spriteLibrary.spriteLibraryAsset = GetSpriteAnimations(inputHandler.currentCharacter);
            }
            else
            {
                SpriteLibraryAsset sprite = GetFreeSpriteAnimation();
                if (sprite != null)
                    spriteLibrary.spriteLibraryAsset = sprite;
            }

        }
    }
    private SpriteLibraryAsset GetFreeSpriteAnimation()
    {
        ePlayerCharacter free = ePlayerCharacter.EmptyCharacter;
        foreach (ePlayerCharacter character in Enum.GetValues(typeof(ePlayerCharacter)))
        {
            if (character != ePlayerCharacter.EmptyCharacter)
            {
                bool isFree = true;
                foreach (PlayerInputHandler inputHandler in CoopManager.Instance.GetActiveHandlers())
                {
                    if (inputHandler.currentCharacter == character)
                        isFree = false;
                }
                if (isFree)
                    free = character;
            }
        }
        SetCharacter(free);
        playerInputHandler.SetStartingCharacter(free);
        return GetSpriteAnimations(free);
    }

    private SpriteLibraryAsset GetSpriteAnimations(ePlayerCharacter character)
    {
        if (character != ePlayerCharacter.EmptyCharacter)
            return GameManager.Instance.GetCharacterData(character).TrashPressAnimations;
        else
            return null;
    }

    #region Player Management
    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);
        SetCharacterSpriteAnimation(character);
    }
    private void SetCharacterSpriteAnimation(ePlayerCharacter character)
    {
        spriteLibrary.spriteLibraryAsset = GameManager.Instance.GetCharacterData(character).TrashPressAnimations;
    }
    private void TakeDamage()
    {
        currentHp--;
        AudioManager.Instance.PlayAudioClip(onHitSound);
        if (currentHp <= 0)
        {
            currentHp = 0;
           gameObject.SetActive(false);
            TrashPressManager.Instance.PlayerDead();
          
           
        }
    }
    #endregion

    #region Actions
    public void Jump()
    {
        animator.SetTrigger("IsJumping");

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
    }
    private bool IsGrounded()
    {
        if(groundChecker.isGrounded)
        {
            speed = moveSpeed;
           
        }
        else
        {
            speed = inAirSpeed;
        }

        
        return groundChecker.isGrounded;
    }

    #region Move
    public void Move(Vector2 direction, float horizontalSpeed)
    {
        rb.velocity = new Vector2(direction.x * horizontalSpeed, rb.velocity.y);
        isMoving = rb.velocity.x != 0;
        animator.SetBool("IsMoving", isMoving);

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
