using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Labirint : MonoBehaviour
{
    [SerializeField]
    List<GameObject> playerSpawnPoints;
    [SerializeField]
    List<GameObject> keySpawnPoints;
    [SerializeField]
    List<GameObject> enemySpawnPoints;

    public List<GameObject> GetPlayerSpawnPoints() => playerSpawnPoints;
    public List<GameObject> GetKeySpawnPoints() => keySpawnPoints;
    public List<GameObject> GetEnemySpawnPoints() => enemySpawnPoints;

}
