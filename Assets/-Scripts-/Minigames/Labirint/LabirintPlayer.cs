using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LabirintPlayer : DefaultInputReceiver
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
        LabirintManager.Instance.AddPlayer(this);
        previousPosition = grid.WorldToCell(transform.position);
    }

    void Update()
    {
        Move();
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
    }

    #endregion

    #region Player management
    internal void PickKey()
    {
        pickedKeys++;
        LabirintManager.Instance.PickedKey();
    }
    internal void Killed()
    {
        gameObject.SetActive(false);
        LabirintManager.Instance.PlayerDead();
    }

    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);
        SetCharacterSprite(character);
    }

    private void SetCharacterSprite(ePlayerCharacter character)
    {
        spriteRenderer.sprite = GameManager.Instance.GetCharacterData(character).PixelSprite;
    }
    #endregion

    #region Input
    public override void MoveMinigameInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public override void MenuInput(InputAction.CallbackContext context)
    {

    }

    #endregion

}

