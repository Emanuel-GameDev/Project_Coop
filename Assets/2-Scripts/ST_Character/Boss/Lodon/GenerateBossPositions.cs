using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateBossPositions : MonoBehaviour
{
    [SerializeField]
    GameObject platformPrefab;
    [SerializeField]
    int rows = 5;
    [SerializeField]
    int columns = 5;
    [SerializeField]
    float spacing = 0;
   
    public List<Transform> PlatformsCenterPositions { get; private set; } = new List<Transform>();
    public List<Transform> WaterCenterPositions { get; private set; } = new List<Transform>();
    public List<Transform> AllCenterPositions => PlatformsCenterPositions.Concat(WaterCenterPositions).ToList();

    private void Awake()
    {
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

        float spriteWidth = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;
        float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;

        // Calcolo della posizione iniziale per centrare la griglia isometrica
        Vector3 startPosition = new Vector3(
            -(columns - 1) * (spriteWidth + spacing) / 2,
            -(rows - 1) * (spriteHeight + spacing) / 2,
            0
        );

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Calcola la posizione isometrica di ciascuna piattaforma
                float posX = (column - row) * (spriteWidth / 2 + spacing / 2);
                float posY = (column + row) * (spriteHeight / 2 + spacing / 2);
                Vector3 position = startPosition + new Vector3(posX, posY, 0);

                // Istanzia la piattaforma
                GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);

                // Aggiungi il Transform della piattaforma alla lista delle posizioni centrali delle piattaforme
                PlatformsCenterPositions.Add(platform.transform);
            }
        }

        // Se necessario, aggiungi qui la logica per generare le posizioni dell'acqua
    }
}
