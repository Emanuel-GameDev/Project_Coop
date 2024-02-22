using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class LabirintPlayer : DefaultInputReceiver
{
    public float moveSpeed = 10f;
    private Grid grid;
    private Tilemap wallTilemap;
    private float tileSize;

    Vector3 moveDir;
    Vector3 destination;
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
        wallTilemap = LabirintManager.Instance.GetWallMap();
        tileSize = grid.cellSize.x;
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
        Vector3 direzioneMovimento = moveDir;
        direzioneMovimento.Normalize();

        if (moveDir != Vector3.zero) // && CheckDirection())
        {
            //Vector3 nextCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(transform.position + direzioneMovimento * grid.cellSize.x));
            //if (!IsCellOccupied(nextCellPosition))
            //    destination = nextCellPosition;

            ////Debug.Log($"Destination: {destination}, MoveDir: {moveDir}");

            Vector3Int cellaAttuale = grid.WorldToCell(transform.position);
            Vector3Int cellaDestinazione = cellaAttuale + Vector3Int.RoundToInt(moveDir);

            // Calcola la posizione del centro della cella di destinazione
            Vector3 posizioneCentraleCella = grid.GetCellCenterWorld(cellaDestinazione);


            // Controlla se la destinazione è sulla griglia e non contiene un muro
            if (wallTilemap.HasTile(cellaDestinazione))
            {
                // Non muovere se la destinazione contiene un muro o è fuori dalla griglia
                moveDir = Vector3.zero;
                destination = transform.position;
            }
            else if (Vector3.Distance(transform.position, posizioneCentraleCella) < 0.01f)
            {
                // Se il personaggio è al centro della cella, imposta la posizione di destinazione
                destination = posizioneCentraleCella;
            }

            // Muovi il personaggio verso la destinazione
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        }


        //transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    //protected bool CheckDirection()
    //{
    //    bool hasReachCenter = Vector3.Distance(transform.position, destination) < 0.01f;
    //    Vector3 directionToDestination = (destination - (Vector3)transform.position).normalized;
    //    bool isSameAxis = false;
    //    if ((directionToDestination.x == 0 && moveDir.x == 0) || (directionToDestination.z == 0 && moveDir.z == 0))
    //        isSameAxis = true;
    //    //Debug.Log($"MoveDir: {moveDir}");
    //    //Debug.Log($"has reac Center: {hasReachCenter}, Is Same Axis: {isSameAxis}");
    //    return hasReachCenter || isSameAxis;
    //}

    //protected bool IsCellOccupied(Vector3 cellPosition)
    //{
    //    Collider[] colliders = Physics.OverlapBox(cellPosition, grid.cellSize / 2.1f);

    //    foreach (Collider collider in colliders)
    //    {
    //        if (!collider.isTrigger)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
    #endregion

    #region Input

    public override void MoveMinigameInput(InputAction.CallbackContext context)
    {
        //Vector2 inputDirection = context.ReadValue<Vector2>();
        moveDir = context.ReadValue<Vector2>();
        //Debug.Log($"inputDir: {inputDirection}, MoveDir: {moveDir}");
    }
    #endregion

}

