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
    private Challenge challengeParent;
   [HideInInspector] public bool canSpawn;

    private void Start()
    {
        if(challengeParent == null)
        {
            challengeParent = GetComponentInParent<Challenge>();
        }
        timer = 1000;
    }
    private void Update()
    {
        if (canSpawn)
        {
            if (timer >= timerEnemyWaves && currentWave < enemyWaves)
            {
                SpawnEnemies();
                currentWave++;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void SpawnEnemies()
    {
        challengeParent.enemySpawned = true;
        for (int i = 0; i < enemiesForWave; i++)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(transform.position.x-spawnRange, transform.position.x+spawnRange), Random.Range(transform.position.y-spawnRange, transform.position.y +spawnRange));
            GameObject tempObject = Instantiate(enemiesPrefab[Random.Range(0, enemiesPrefab.Count)], spawnPoint, Quaternion.identity,challengeParent.gameObject.transform);
            tempObject.TryGetComponent<EnemyCharacter>(out EnemyCharacter tempEnemy);
            if (tempEnemy != null)
                challengeParent.AddToSpawned(tempEnemy);
               
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
