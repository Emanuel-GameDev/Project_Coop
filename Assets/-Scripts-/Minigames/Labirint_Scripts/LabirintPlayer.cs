using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;

public class LabirintPlayer : InputReceiver
{
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField, Range(0, 1f)]
    private float directionTreshold = 0.2f;
    private Grid grid;
    private Tilemap wallTilemap;

    Vector2 moveDir;
    Vector2 destination;
    Vector3Int previousPosition;
    public int pickedKeys { get; private set; } = 0;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField]
    private SpriteLibrary spriteLibrary;


    void Start()
    {
        InitialSetup();
    }

    private void InitialSetup()
    {
        destination = transform.position;
        pickedKeys = 0;
        grid = LabirintManager.Instance.Grid;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        spriteLibrary = GetComponentInChildren<SpriteLibrary>();
        LabirintManager.Instance.AddPlayer(this);
        previousPosition = grid.WorldToCell(transform.position);
    }

    void Update()
    {
        Move();
        ManageAnimation();
    }

    private void ManageAnimation()
    {
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0; 
            animator.SetBool("isMoving", isMoving); 

            if (moveDir.x > 0)
            {
                spriteRenderer.flipX = true; 
            }
            else if (moveDir.x < 0)
            {
                spriteRenderer.flipX = false; 
            }
        }
    }

    public override void SetInputHandler(PlayerInputHandler inputHandler)
    {
        base.SetInputHandler(inputHandler);
        if (inputHandler != null) 
        { 
            if(spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (inputHandler.currentCharacter != ePlayerCharacter.EmptyCharacter)
            {
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
            if(character != ePlayerCharacter.EmptyCharacter)
            {
                bool isFree = true;
                foreach(PlayerInputHandler inputHandler in CoopManager.Instance.GetActiveHandlers())
                {
                    if (inputHandler.currentCharacter == character)
                        isFree = false;
                }
                if(isFree)
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
            return GameManager.Instance.GetCharacterData(character).PixelAnimations;
        else
            return null;
    }

    #region Movement
    protected void Move()
    {
        if (wallTilemap == null)
            return;

        if (moveDir != Vector2.zero)
        {
            if (HasReachCenter())
            {
                ChangeDirection();
            }
            else if (IsSameAxis())
            {
                ChangeDestination(transform.position, moveDir);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        Vector3 lastDirection = (grid.WorldToCell(transform.position) - previousPosition);
        lastDirection.Normalize();
        Vector2 lastDirection2D;
        if (Mathf.Abs(lastDirection.x) >= Mathf.Abs(lastDirection.y))
        {
            lastDirection2D = (lastDirection.x >= 0) ? Vector2.right : Vector2.left;
        }
        else
        {
            lastDirection2D = (lastDirection.y >= 0) ? Vector2.up : Vector2.down;
        }

        CheckAndSetDestination(lastDirection2D);
    }

    private void CheckAndSetDestination(Vector2 lastDirection2D)
    {
        Vector2 direction = Vector2Int.zero;
        Vector2 directionX = moveDir.x > 0 ? Vector2.right : Vector2.left;
        Vector2 directionY = moveDir.y > 0 ? Vector2.up : Vector2.down;
        bool isXAxis = lastDirection2D.x != 0;
        bool isYAxis = !isXAxis;

        if ((isXAxis && moveDir.y != 0) || (isYAxis && moveDir.x != 0))
        {
            direction = (isXAxis ? directionY : directionX);
            if (!ChangeDestination(transform.position, direction) && ((isXAxis && moveDir.x != 0) || (isYAxis && moveDir.y != 0)))
            {
                direction = (isXAxis ? directionX : directionY);
                ChangeDestination(transform.position, direction);
            }
        }
        else if ((isXAxis && moveDir.x != 0) || (isYAxis && moveDir.y != 0))
        {
            direction = (isXAxis ? directionX : directionY);
            ChangeDestination(transform.position, direction);
        }

    }

    private bool ChangeDestination(Vector2 startingCheckPosition, Vector2 direction)
    {
        Vector3Int currentCell = grid.WorldToCell(startingCheckPosition);
        Vector3Int destinationCell = currentCell + Vector3Int.RoundToInt(direction);
        Vector2 cellCentralPosition = grid.GetCellCenterWorld(destinationCell);
        if (!wallTilemap.HasTile(destinationCell))
        {
            destination = cellCentralPosition;
            previousPosition = currentCell == grid.WorldToCell(destination) ? previousPosition : currentCell;
            return true;
        }
        return false;
    }

    private bool IsSameAxis()
    {
        Vector2 directionToDestination = (destination - (Vector2)transform.position).normalized;
        bool isSameAxis = ((directionToDestination.x == 0 && moveDir.x == 0) || (directionToDestination.y == 0 && moveDir.y == 0));
        return isSameAxis;
    }

    private bool HasReachCenter()
    {
        return Vector2.Distance(transform.position, destination) < 0.01f;
    }

    public void Inizialize()
    {
        wallTilemap = LabirintManager.Instance.GetWallMap();
        destination = transform.position;
        pickedKeys = 0;
    }

    #endregion

    #region Player management
    internal void PickKey()
    {
        pickedKeys++;
        LabirintManager.Instance.PickedKey(character, pickedKeys);
    }
    internal void Killed()
    {
        gameObject.SetActive(false);
        LabirintManager.Instance.PlayerDead();
    }

    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);
        SetCharacterSpriteAnimation(character);
    }

    private void SetCharacterSpriteAnimation(ePlayerCharacter character)
    {
        spriteLibrary.spriteLibraryAsset = GameManager.Instance.GetCharacterData(character).PixelAnimations;
    }
    #endregion

    #region Input
    public override void MoveMinigameInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        moveDir.Normalize();
        moveDir.x = MathF.Abs(moveDir.x) > directionTreshold ? moveDir.x : 0;
        moveDir.y = MathF.Abs(moveDir.y) > directionTreshold ? moveDir.y : 0;
    }

    public override void MenuInput(InputAction.CallbackContext context)
    {
        if(context.performed && MenuManager.Instance.ActualMenu == null)
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

