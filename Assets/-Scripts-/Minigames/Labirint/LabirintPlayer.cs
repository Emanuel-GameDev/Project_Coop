using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LabirintPlayer : DefaultInputReceiver
{
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField, Range(0,1f)]
    private float directionTreshold = 0.2f;
    private Grid grid;
    private Tilemap wallTilemap;
    //private float tileSize;

    Vector2 moveDir;
    Vector2 lastInput;
    Vector2 destination;
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
        //tileSize = grid.cellSize.x;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        LabirintManager.Instance.AddPlayer(this);
    }

    void Update()
    {
        Move();
    }

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

    #region Movement
    protected void Move()
    {
        if (wallTilemap == null)
            return;

        if (moveDir != Vector2.zero && CheckDirection())
        {
            //CheckDirections();


            Vector3Int currentCell = grid.WorldToCell(transform.position);
            Vector3Int destinationCell = currentCell + Vector3Int.RoundToInt(moveDir);

            Vector2 cellCentralPosition = grid.GetCellCenterWorld(destinationCell);

            if (!wallTilemap.HasTile(destinationCell))
            {
                destination = cellCentralPosition;
            }

        }

        transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private void CheckDirections()
    {
        Vector2 destinationCell = destination;

        if (Mathf.Abs(moveDir.x) > directionTreshold)
        {
            Vector2Int direction = moveDir.x > 0 ? Vector2Int.right : Vector2Int.left;
            CheckIfCellIsOccupied(direction);
        }

        if (Mathf.Abs(moveDir.y) > directionTreshold)
        {
            Vector2Int direction = moveDir.y > 0 ? Vector2Int.up : Vector2Int.down;
            
        }
    }

    private bool CheckIfCellIsOccupied(Vector2Int direction)
    {
        Vector3Int currentCell = grid.WorldToCell(transform.position);
        Vector3Int destinationCell = currentCell + (Vector3Int)direction;
        return wallTilemap.HasTile(destinationCell);
       
    }

    protected bool CheckDirection()
    {
        bool hasReachCenter = Vector3.Distance(transform.position, destination) < 0.01f;
        Vector2 directionToDestination = (destination - (Vector2)transform.position).normalized;
        bool isSameAxis = ((directionToDestination.x == 0 && moveDir.x == 0) || (directionToDestination.y == 0 && moveDir.y == 0));
        return hasReachCenter || isSameAxis;
    }

    public void Inizialize()
    {
        wallTilemap = LabirintManager.Instance.GetWallMap();
        destination = transform.position;
        lastInput = Vector2.zero;
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

