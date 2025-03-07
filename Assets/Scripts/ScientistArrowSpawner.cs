using UnityEngine;
using System.Collections;

public class ScientistArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float fireRate = 2f;
    public float shootForce = 10f;
    private bool isSpaceMode = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isSpaceMode = !isSpaceMode;
            Debug.Log("Mod değişti: " + (isSpaceMode ? "Uzay Modu" : "Normal Mod"));
        }
    }
    void Start()
    {
        Debug.Log("a");
        StartCoroutine(FireArrows());
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Transform nearestTarget = FindNearestTarget("Papaz");
            Debug.Log("b");
            if (nearestTarget != null)
            {
                Debug.Log("c");
                ShootArrow(nearestTarget);
            }
        }
    }

    Transform FindNearestTarget(string enemyTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            if (target.transform == transform) continue; // Kendisini hedef almamasını sağlar

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target.transform;
            }
        }
        return nearestTarget;
    }


    void ShootArrow(Transform target)
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
            if (isSpaceMode)
            {
                // **UZAY MODU: Ok atıldığı yönün 60° yukarısına gider**
                Vector3 spaceDirection = Quaternion.AngleAxis(-60, transform.right) * transform.forward;
                rb.velocity = spaceDirection * shootForce;
                rb.useGravity = false; // Uzayda yerçekimi olmayacağı için kapat

                Debug.Log("Uzaya ok fırlatıldı! Yön: " + spaceDirection);
            }
            else
            {
                // **NORMAL MOD: Eğimli ok**
                Vector3 launchVelocity = CalculateLaunchVelocity(target);
                rb.velocity = launchVelocity;
                rb.useGravity = true;

                Debug.Log("Eğimli ok fırlatıldı! Hedef: " + target.name + " | Hız: " + rb.velocity);
            }
        }
    }
    Vector3 CalculateLaunchVelocity(Transform target)
    {
        Vector3 start = transform.position; // Kapsülün pozisyonu
        Vector3 end = target.position; // Hedefin pozisyonu
        float gravity = Mathf.Abs(Physics.gravity.y); // Yerçekimi

        // Hedefin yatay uzaklığını hesapla
        Vector3 horizontalDirection = new Vector3(end.x - start.x, 0, end.z - start.z);
        float horizontalDistance = horizontalDirection.magnitude;

        // Hedefin yüksekliği farkı
        float heightDifference = end.y - start.y;

        // İlk hızın yukarı bileşenini hesapla
        float initialVelocityY = Mathf.Sqrt(2 * gravity * heightDifference + gravity * horizontalDistance);

        // Uçuş süresini hesapla
        float time = (initialVelocityY / gravity) * 2;

        // XZ yönündeki hız bileşenini hesapla
        Vector3 velocityXZ = horizontalDirection.normalized * (horizontalDistance / time);

        // Son hız vektörünü belirle
        return velocityXZ + Vector3.up * initialVelocityY;
    }



}
