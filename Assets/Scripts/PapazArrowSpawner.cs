using System;
using UnityEngine;
using System.Collections;

public class PapazArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject blackHolePrefab;
    public float fireRate = 2f;
    public float shootForce = 10f;
    public float minDistanceToAttack =7f;
    private float arrowSpeedMultiplier = 1f;
    
    private bool isMarieCurieModeActive = false;
    public HealthSystem healthSystem; // Sa�l�k sistemine eri�im
    public RadioationEffect radiationEffect; // Par�ac�k efekti

    public float blackHoleDuration = 5f;
    public float pushForce = 10f;
    private bool isHawkingModeActive = false;



    void Start()
    {
        StartCoroutine(FireArrows());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isMarieCurieModeActive)
        {
            ActivateMarieCurieMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !isHawkingModeActive) 
        {
            ActivateHawkingMode();
        }
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate * arrowSpeedMultiplier);
            Transform nearestTarget = FindNearestTarget("Scientist");

            if (nearestTarget != null)
            {
                ShootArrow(nearestTarget);
            }
        }
    }
    
    public void SetArrowSpeedMultiplier(float multiplier, float duration)
    {
        arrowSpeedMultiplier = multiplier;
        StartCoroutine(ResetArrowSpeed(duration));
    }

    IEnumerator ResetArrowSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        arrowSpeedMultiplier = 1f;
    }

    public Transform FindNearestTarget(string enemyTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance && distance < minDistanceToAttack)
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
    void ActivateMarieCurieMode()
    {
        isMarieCurieModeActive = true;
        radiationEffect.ActivateRadiation(); // Par�ac�klar� ba�lat
        StartCoroutine(HealOverTime());
    }

    IEnumerator HealOverTime()
    {
        float duration = 5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            healthSystem.Heal(10f * Time.deltaTime); // Yava� sa�l�k art���
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DeactivateMarieCurieMode();
    }

    void DeactivateMarieCurieMode()
    {
        isMarieCurieModeActive = false;
        radiationEffect.DeactivateRadiation(); // Par�ac�klar� durdur
    }

    void ActivateHawkingMode()
    {
        Transform nearestScientist = FindNearestTarget("Scientist");
        if (nearestScientist == null) return;

        isHawkingModeActive = true;

        // **Papaz ile Scientist�in tam ortas�nda kara delik olu�tur**
        Vector3 spawnPosition = (transform.position + nearestScientist.position) / 2f;
        spawnPosition.y = spawnPosition.y - 1f;
        GameObject blackHole = Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);

        StartCoroutine(BlackHoleEffect(blackHole, nearestScientist));
    }

    IEnumerator BlackHoleEffect(GameObject blackHole, Transform scientist)
    {
        float elapsedTime = 0f;
        Rigidbody rb = scientist.GetComponent<Rigidbody>();

        while (elapsedTime < blackHoleDuration)
        {
            if (scientist != null && rb != null)
            {
                Vector3 pushDirection = (scientist.position - blackHole.transform.position).normalized;
                
                // **Daha g��l� bir itme kuvveti uygula**
                rb.AddForce(pushDirection * pushForce * rb.mass, ForceMode.Impulse);

                Debug.Log("Scientist itildi! Y�n: " + pushDirection);
            }

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f); // Daha s�k kontrol ederek etkiyi art�r
        }

        Destroy(blackHole);
        isHawkingModeActive = false;
    }


}