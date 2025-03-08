using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HawkingSystem : MonoBehaviour
{
    public GameObject blackHolePrefab; // Kara delik prefab�
    public float effectRadius = 5f; // Kara deli�in etki alan�
    public float pushForce = 10f; // �tme kuvveti
    public float blackHoleDuration = 5f; // Kara deli�in s�resi
    public string targetTag = "Scientist"; // Hangi karakteri etkileyecek

    private GameObject currentBlackHole;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) // 8 tu�una bas�nca kara delik olu�ur
        {
            ActivateHawkingMode();
        }
    }

    void ActivateHawkingMode()
    {
        if (currentBlackHole != null) return; // E�er zaten bir kara delik varsa yeni olu�turma

        Transform nearestScientist = FindNearestScientist();
        if (nearestScientist == null) return; // E�er scientist yoksa devam etme

        Vector3 spawnPosition = GetRandomPositionNearScientist(nearestScientist.position);
        currentBlackHole = Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(BlackHoleEffect());
    }

    Transform FindNearestScientist()
    {
        GameObject[] scientists = GameObject.FindGameObjectsWithTag(targetTag);
        Transform nearestScientist = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject scientist in scientists)
        {
            float distance = Vector3.Distance(transform.position, scientist.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestScientist = scientist.transform;
            }
        }
        return nearestScientist;
    }

    Vector3 GetRandomPositionNearScientist(Vector3 scientistPosition)
    {
        float randomAngle = Random.Range(0, 360); // 360 derece i�inde rastgele bir y�n se�
        float randomDistance = Random.Range(3f, 6f); // 3-6 birim uza�a yerle�tir
        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(randomAngle) * randomDistance,
            0,
            Mathf.Sin(randomAngle) * randomDistance
        );
        return scientistPosition + spawnOffset;
    }

    IEnumerator BlackHoleEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blackHoleDuration)
        {
            Collider[] affectedObjects = Physics.OverlapSphere(currentBlackHole.transform.position, effectRadius);

            foreach (Collider obj in affectedObjects)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && obj.CompareTag(targetTag)) // Sadece Scientist'leri iter
                {
                    Vector3 pushDirection = (obj.transform.position - currentBlackHole.transform.position).normalized;
                    rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                }
            }

            elapsedTime += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(currentBlackHole);
        currentBlackHole = null;
    }
}
