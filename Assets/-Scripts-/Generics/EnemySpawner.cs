using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesForWave = 2;
    [SerializeField] private int enemyWaves = 1;
    [SerializeField] private float timerEnemyWaves = 5;
    [SerializeField] private Transform spawnDestination;
    [SerializeField] private float destinationRange;
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
            GameObject tempObject = Instantiate(enemiesPrefab[Random.Range(0, enemiesPrefab.Count)], transform.position, Quaternion.identity,challengeParent.gameObject.transform);
            tempObject.TryGetComponent<BasicEnemy>(out BasicEnemy tempEnemy);
            if (tempEnemy != null)
            {
                challengeParent.AddToSpawned(tempEnemy);                
                Vector2 randomSpawnDestination = new Vector2(Random.Range(spawnDestination.position.x - destinationRange, spawnDestination.position.x + destinationRange), Random.Range(spawnDestination.position.y - destinationRange, spawnDestination.position.y + destinationRange));
                tempEnemy.entryDestination = randomSpawnDestination;
                tempEnemy.canGoIdle = false;
                tempEnemy.stateMachine.SetState(tempEnemy.entryState);


            }

            tempObject.TryGetComponent<BossCharacter>(out BossCharacter tempBossCharacter);
            if(tempBossCharacter != null)
            {
                challengeParent.AddToSpawned(tempBossCharacter);
                
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnDestination.position, destinationRange);
    }
}
