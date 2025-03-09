using System.Collections;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{
    public GameObject prefabA; // Ýlk savaþçý
    public GameObject prefabB; // Ýkinci savaþçý
    public int warriorsPerLine = 2; // Her çizgiye kaç savaþçý spawn edilecek
    public float spawnInterval = 3f; // Kaç saniyede bir spawn olacaðý

    public Transform leftSpawnPoint;  // Sol tarafýn baþlangýç noktasý
    public Transform rightSpawnPoint; // Sað tarafýn baþlangýç noktasý

    public int lineCount = 4; // Kaç tane paralel çizgi olacak
    public float lineSpacing = 2f; // Çizgiler arasýndaki mesafe

    private int spawnIndex = 0; // Sýrayla noktalarý kullanmak için

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

                // Sýrayla prefablarý kullan
                GameObject selectedPrefab = (spawnIndex % 2 == 0) ? prefabA : prefabB;

                Instantiate(selectedPrefab, leftPos, Quaternion.identity);
                Instantiate(selectedPrefab, rightPos, Quaternion.identity);
            }

            spawnIndex++; // Bir sonraki spawn'da farklý prefab kullan
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

            // Çizgi çiz (Spawn hatlarý)
            Gizmos.DrawLine(leftPos, rightPos);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftSpawnPoint.position, 0.3f);
        Gizmos.DrawSphere(rightSpawnPoint.position, 0.3f);
    }
}
