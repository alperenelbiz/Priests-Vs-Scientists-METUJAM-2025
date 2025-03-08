using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    public Transform targetPoint;
    public int enemyCount = 10;

    void Start()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        randomPosition += transform.position;

        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        EnemyMovement enemyController = enemy.GetComponent<EnemyMovement>();
        enemyController.targetPosition = targetPoint;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
