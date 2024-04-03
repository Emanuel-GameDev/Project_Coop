using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesForWave = 2;
    [SerializeField] private int enemyWaves = 1;
    [SerializeField] private float timerEnemyWaves = 5;
    [SerializeField] private float spawnRange;
    [SerializeField] private List<GameObject> enemiesPrefab;
    private float timer = 0;
    private int currentWave;
   [HideInInspector] public bool canSpawn;


    private void Update()
    {
        if (canSpawn)
        {
            if (timer >= timerEnemyWaves && currentWave <= enemyWaves)
            {
                SpawnEnemies();
                currentWave++;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesForWave; i++)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(0, spawnRange), Random.Range(0, spawnRange));
            Instantiate(enemiesPrefab[Random.Range(0, enemiesPrefab.Count)], spawnPoint, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
