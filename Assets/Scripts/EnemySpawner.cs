using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    public Transform targetPoint;
    public int maxActiveEnemies = 5;
    public float spawnInterval = 2f;

    private Queue<GameObject> inactiveEnemies = new Queue<GameObject>();
    private int activeEnemyCount = 0;

    void Start()
    {
        // Add all enemies to the queue
        foreach (GameObject enemy in enemyPrefabs)
        {
            enemy.SetActive(false);
            inactiveEnemies.Enqueue(enemy);
        }

       // StartCoroutine(SpawnEnemiesRoutine());
    }

    /*IEnumerator SpawnEnemiesRoutine()
    {
        while (inactiveEnemies.Count > 0 || activeEnemyCount > 0) // Stop when all enemies are dead
        {
            if (activeEnemyCount < maxActiveEnemies && inactiveEnemies.Count > 0)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("All enemies have been defeated. Spawning has stopped.");
    }*/

    void SpawnEnemy()
    {
        if (inactiveEnemies.Count == 0) return;

        GameObject enemy = inactiveEnemies.Dequeue();
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        enemy.transform.position = transform.position + randomPosition;
        enemy.SetActive(true);
        activeEnemyCount++;

        EnemyMovement enemyController = enemy.GetComponent<EnemyMovement>();
        if (enemyController != null)
        {
            enemyController.targetPosition = targetPoint;
        }

        HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.spawner = this;
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount--;

        // Stop spawning when all enemies are used up
        if (activeEnemyCount == 0 && inactiveEnemies.Count == 0)
        {
            StopAllCoroutines();
            Debug.Log("No more enemies to spawn. Spawning has permanently stopped.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
