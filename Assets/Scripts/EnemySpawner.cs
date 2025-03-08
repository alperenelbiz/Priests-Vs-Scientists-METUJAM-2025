using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    public Transform targetPoint;
    public int maxActiveEnemies = 5; // Limit active enemies at a time
    public float spawnInterval = 2f; // Time between spawns

    private Queue<GameObject> inactiveEnemies = new Queue<GameObject>(); // Queue for reusing enemies
    private int activeEnemyCount = 0; // Track active enemies

    void Start()
    {
        // Store all inactive enemies in the queue
        foreach (GameObject enemy in enemyPrefabs)
        {
            enemy.SetActive(false);
            inactiveEnemies.Enqueue(enemy);
        }

        // Start spawning enemies with a delay
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            if (activeEnemyCount < maxActiveEnemies && inactiveEnemies.Count > 0)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (inactiveEnemies.Count == 0) return;

        GameObject enemy = inactiveEnemies.Dequeue(); // Get an inactive enemy
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        randomPosition += transform.position;

        enemy.transform.position = randomPosition;
        enemy.SetActive(true);
        activeEnemyCount++;

        // Assign target position for movement
        EnemyMovement enemyController = enemy.GetComponent<EnemyMovement>();
        if (enemyController != null)
        {
            enemyController.targetPosition = targetPoint;
        }

        // Assign spawner reference to HealthSystem
        HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.spawner = this;
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        enemy.SetActive(false);
        inactiveEnemies.Enqueue(enemy);
        activeEnemyCount--;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
