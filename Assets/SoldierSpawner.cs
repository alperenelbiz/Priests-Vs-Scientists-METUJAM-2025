using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject[] soldierPrefabs;
    public Transform spawnPoint;
    public Transform targetPoint;
    public float spawnInterval = 5f;

    void Start()
    {
        StartCoroutine(SpawnSoldiers());
    }

    IEnumerator SpawnSoldiers()
    {
        while (true)
        {
            GameObject soldierPrefab = soldierPrefabs[Random.Range(0, soldierPrefabs.Length)];
            GameObject soldier = Instantiate(soldierPrefab, spawnPoint.position, Quaternion.identity);
            SoldierAI soldierAI = soldier.GetComponent<SoldierAI>();
            if (soldierAI != null)
            {
                soldierAI.targetPosition = targetPoint;
            }
            Debug.Log("Spawned: " + soldier.name);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
