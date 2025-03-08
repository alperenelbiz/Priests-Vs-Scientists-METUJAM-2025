using System.Collections;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{
    public GameObject prefabA; // �lk sava���
    public GameObject prefabB; // �kinci sava���
    public int warriorsPerLine = 2; // Her �izgiye ka� sava��� spawn edilecek
    public float spawnInterval = 3f; // Ka� saniyede bir spawn olaca��

    public Transform leftSpawnPoint;  // Sol taraf�n ba�lang�� noktas�
    public Transform rightSpawnPoint; // Sa� taraf�n ba�lang�� noktas�

    public int lineCount = 4; // Ka� tane paralel �izgi olacak
    public float lineSpacing = 2f; // �izgiler aras�ndaki mesafe

    private int spawnIndex = 0; // S�rayla noktalar� kullanmak i�in

    void Start()
    {
        StartCoroutine(SpawnWarriors());
    }

    IEnumerator SpawnWarriors()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            for (int i = 0; i < lineCount; i++)
            {
                Vector3 leftPos = leftSpawnPoint.position + Vector3.forward * (i * lineSpacing);
                Vector3 rightPos = rightSpawnPoint.position + Vector3.forward * (i * lineSpacing);

                // S�rayla prefablar� kullan
                GameObject selectedPrefab = (spawnIndex % 2 == 0) ? prefabA : prefabB;

                Instantiate(selectedPrefab, leftPos, Quaternion.identity);
                Instantiate(selectedPrefab, rightPos, Quaternion.identity);
            }

            spawnIndex++; // Bir sonraki spawn'da farkl� prefab kullan
        }
    }

    void OnDrawGizmos()
    {
        if (leftSpawnPoint == null || rightSpawnPoint == null) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < lineCount; i++)
        {
            Vector3 leftPos = leftSpawnPoint.position + Vector3.forward * (i * lineSpacing);
            Vector3 rightPos = rightSpawnPoint.position + Vector3.forward * (i * lineSpacing);

            // �izgi �iz (Spawn hatlar�)
            Gizmos.DrawLine(leftPos, rightPos);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftSpawnPoint.position, 0.3f);
        Gizmos.DrawSphere(rightSpawnPoint.position, 0.3f);
    }
}
