using UnityEngine;
using System.Collections;

public class PapazArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float fireRate = 2f;
    public float shootForce = 10f;

    void Start()
    {
        StartCoroutine(FireArrows());
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Transform nearestTarget = FindNearestTarget("Scientist");

            if (nearestTarget != null)
            {
                ShootArrow(nearestTarget);
            }
        }
    }

    public Transform FindNearestTarget(string enemyTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearestTarget = null;
        float minDistance = 7f;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target.transform;
            }
        }
        return nearestTarget;
    }

    public void ShootArrow(Transform target)
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = newArrow.GetComponent<Rigidbody>();

        // Okun sahibini belirle
        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetShooter(gameObject);
        }

        if (rb != null)
        {
                Vector3 launchVelocity = CalculateLaunchVelocity(target);
                rb.velocity = launchVelocity;
                rb.useGravity = true;
                Debug.Log("E�imli ok f�rlat�ld�! Hedef: " + target.name + " | H�z: " + rb.velocity);
            
        }
    }

    Vector3 CalculateLaunchVelocity(Transform target)
    {
        Vector3 start = transform.position; // Kaps�l�n pozisyonu
        Vector3 end = target.position; // Hedefin pozisyonu
        float gravity = Mathf.Abs(Physics.gravity.y); // Yer�ekimi

        // Hedefin yatay uzakl���n� hesapla
        Vector3 horizontalDirection = new Vector3(end.x - start.x, 0, end.z - start.z);
        float horizontalDistance = horizontalDirection.magnitude;

        // Hedefin y�ksekli�i fark�
        float heightDifference = end.y - start.y;

        // �lk h�z�n yukar� bile�enini hesapla
        float initialVelocityY = Mathf.Sqrt(2 * gravity * heightDifference + gravity * horizontalDistance);

        // U�u� s�resini hesapla
        float time = (initialVelocityY / gravity) * 2;

        // XZ y�n�ndeki h�z bile�enini hesapla
        Vector3 velocityXZ = horizontalDirection.normalized * (horizontalDistance / time);

        // Son h�z vekt�r�n� belirle
        return velocityXZ + Vector3.up * initialVelocityY;
    }


}