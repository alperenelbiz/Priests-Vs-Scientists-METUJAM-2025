using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public Vector3 spawnArea = new Vector3(5, 1, 5);

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            spawnArea.y,
            Random.Range(-spawnArea.z, spawnArea.z)
        );

        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }
}
