using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformsGenerator : MonoBehaviour
{
    [Header("Referencies")]
    [SerializeField, Tooltip("Se non impostato userà se stesso come centro")]
    Transform startPoint;
    [SerializeField, Tooltip("Deve essere impostato sennò non funziona")]
    GameObject platformPrefab;
    [SerializeField, Tooltip("Se non impostato genererà degli oggetti vuoti")]
    GameObject outerPlatformPrefab;

    [Header("Settings")]
    [SerializeField, Min(1)]
    int rows = 5;
    [SerializeField, Min(1)]
    int columns = 5;
    [SerializeField, Tooltip("Spaziatura tra una piattaforma e l'altra.")]
    float spacing = 0;
    [SerializeField]
    bool mustGenerateOuterPositions = true;
    [SerializeField, Min(1)]
    int outerRows = 1;
    [SerializeField, Min(1)]
    int outerColumns = 1;

    public List<Transform> PlatformsCenterPositions { get; private set; } = new List<Transform>();
    public List<Transform> OuterCenterPositions { get; private set; } = new List<Transform>();
    public List<Transform> AllCenterPositions => PlatformsCenterPositions.Concat(OuterCenterPositions).ToList();

    private void Awake()
    {
        DestroyAllPlatforms();
        GeneratePlatforms();
    }

    private void GeneratePlatforms()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("PlatformPrefab is null");
            return;
        }

        SpriteRenderer spriteRenderer = platformPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("PlatformPrefab does not have a SpriteRenderer component.");
            return;
        }

        float spriteWidth = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit * platformPrefab.transform.localScale.x;
        float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit * platformPrefab.transform.localScale.y;
        float spriteWidthHalf = spriteWidth / 2;
        float spriteHeightHalf = spriteHeight / 2;

        float maxVerticalDifference = (((float)(columns + rows - 2)) / 2) * (spriteHeightHalf + spacing);
        float maxHorizontalDifference = (((float)(columns - rows)) / 2) * (spriteWidthHalf + spacing);
        Vector3 startPosition = (startPoint == null ? transform.position : startPoint.position) + new Vector3(-maxHorizontalDifference, -maxVerticalDifference, 0);

        GeneratePlatformGrid(startPosition, rows, columns, spriteWidthHalf, spriteHeightHalf);

        if (mustGenerateOuterPositions)
        {
            if (outerPlatformPrefab == null)
            {
                outerPlatformPrefab = new GameObject("Outer Position");
            }

            GenerateOuterPlatforms(startPosition, rows, columns, spriteWidthHalf, spriteHeightHalf);
        }
    }

    private void GeneratePlatformGrid(Vector3 startPosition, int rows, int columns, float spriteWidthHalf, float spriteHeightHalf)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Calcola la posizione isometrica di ciascuna piattaforma
                float posX = (column - row) * (spriteWidthHalf + spacing);
                float posY = (column + row) * (spriteHeightHalf + spacing);
                Vector3 position = startPosition + new Vector3(posX, posY, 0);

                // Istanzia la piattaforma
                GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
                platform.transform.SetParent(transform);

                // Aggiungi il Transform della piattaforma alla lista delle posizioni centrali delle piattaforme
                PlatformsCenterPositions.Add(platform.transform);
            }
        }
    }

    private void GenerateOuterPlatforms(Vector3 startPosition, int rows, int columns, float spriteWidthHalf, float spriteHeightHalf)
    {
        for (int row = -outerRows; row < rows + outerRows; row++)
        {
            for (int column = -outerColumns; column < columns + outerColumns; column++)
            {
                bool isOuterRow = row < 0 || row >= rows;
                bool isOuterColumn = column < 0 || column >= columns;
                if (isOuterRow || isOuterColumn)
                {
                    float posX = (column - row) * (spriteWidthHalf + spacing);
                    float posY = (column + row) * (spriteHeightHalf + spacing);
                    Vector3 position = startPosition + new Vector3(posX, posY, 0);

                    // Istanzia la piattaforma
                    GameObject platform = Instantiate(outerPlatformPrefab, position, Quaternion.identity);
                    platform.transform.SetParent(transform);

                    // Aggiungi il Transform della piattaforma alla lista delle posizioni centrali esterne
                    OuterCenterPositions.Add(platform.transform);
                }
            }
        }
    }

    //platform.GetComponent<SpriteRenderer>().color = Color.white;

    public void DestroyAllPlatforms()
    {
        foreach (Transform platform in AllCenterPositions)
        {
            if (platform != null)
                Destroy(platform.gameObject);
        }

        PlatformsCenterPositions.Clear();
        OuterCenterPositions.Clear();
    }

    //DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DestroyAllPlatforms();
            GeneratePlatforms();
        }
    } 



}
