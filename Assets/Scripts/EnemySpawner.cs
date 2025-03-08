using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    public Transform targetPoint;
    public int enemyCount = 10; // Kaç tane düşman spawn edilecek
    public float spawnInterval = 30f; // Kaç saniyede bir tekrar spawn edilecek

    void Start()
    {
        StartCoroutine(SpawnEnemiesLoop());
    }

    IEnumerator SpawnEnemiesLoop()
    {
        while (true) // Sonsuz döngüde sürekli spawn et
        {
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(1f); // Her düşman için 1 saniye bekleyerek doğal akış sağla
            }

            Debug.Log($"⏳ {spawnInterval} saniye bekleniyor...");
            yield return new WaitForSeconds(spawnInterval); // 30 saniye bekle ve yeniden spawn et
        }
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        randomPosition += transform.position; // Spawner'ın bulunduğu konumu merkez al

        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        EnemyMovement enemyController = enemy.GetComponent<EnemyMovement>();

        if (enemyController != null)
        {
            enemyController.targetPosition = targetPoint;
        }

        Debug.Log($"👹 Yeni düşman spawn edildi: {enemy.name} Konum: {randomPosition}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
