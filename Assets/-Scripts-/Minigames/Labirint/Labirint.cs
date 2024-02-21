using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Labirint : MonoBehaviour
{
    [SerializeField]
    Tilemap objectsTilemap;
    [SerializeField]
    TileBase playerSpawnPoints;
    [SerializeField]
    TileBase keySpawnPoints;
    [SerializeField]
    TileBase enemySpawnPoints;

    public List<Vector3Int> GetPlayerSpawnPoints() => FindTilesOfType(objectsTilemap, playerSpawnPoints);
    public List<Vector3Int> GetKeySpawnPoints() => FindTilesOfType(objectsTilemap, keySpawnPoints);
    public List<Vector3Int> GetEnemySpawnPoints() => FindTilesOfType(objectsTilemap, enemySpawnPoints);

    public void DisableObjectMap()
    {
        objectsTilemap.gameObject.SetActive(false);
    }

    List<Vector3Int> FindTilesOfType(Tilemap tilemap, TileBase targetTile)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.GetTile(pos) == targetTile)
            {
                positions.Add(pos);
            }
        }

        return positions;
    }


}
