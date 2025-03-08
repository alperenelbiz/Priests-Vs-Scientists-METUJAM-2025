using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;


public class PapazArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject blackHolePrefab;
    public float fireRate = 2f;
    public float shootForce = 10f;
    public float minDistanceToAttack =7f;
    private float arrowSpeedMultiplier = 1f;
    
    public bool isMarieCurieModeActive = false;
    public HealthSystem healthSystem; // Sa�l�k sistemine eri�im
    public RadioationEffect radiationEffect; // Par�ac�k efekti

    public float blackHoleDuration = 5f;
    public float pushForce = 10f;
    public bool isHawkingModeActive = false;

    public float detectionRadius = 10f; // Bilim insanlarını algılama yarıçapı




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
            //ActivateHawkingMode();
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
        if (target == null) return;
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
    public void ActivateMarieCurieMode()
    {
        isMarieCurieModeActive = true;
        radiationEffect.ActivateRadiation(); // Par�ac�klar� ba�lat
        //StartCoroutine(HealOverTime());
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

    /*public void ActivateHawkingMode()
    {
        Transform nearestScientist = DetectScientist();
        if (nearestScientist == null)
        {
            Debug.LogWarning("⚠ En yakın Scientist bulunamadı!");
            return;
        }

        isHawkingModeActive = true;

        // **Kara delik oluştur**
        Vector3 spawnPosition = (transform.position + nearestScientist.position) / 2f;
        spawnPosition.y -= 1f;
        GameObject blackHole = Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);

        // **Scientist’i kara delikten uzağa it**
        Rigidbody rb = nearestScientist.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 pushDirection = (nearestScientist.position - blackHole.transform.position).normalized;
            rb.AddForce(pushDirection * pushForce * rb.mass, ForceMode.Impulse); // Aniden itme
            Debug.Log($"💥 Scientist {pushDirection} yönüne itildi!");
        }

        StartCoroutine(BlackHoleEffect(blackHole, nearestScientist));
    }*/





    /*IEnumerator BlackHoleEffect(GameObject blackHole, Transform scientist)
    {
        float elapsedTime = 0f;
        Rigidbody rb = scientist.GetComponent<Rigidbody>();

        while (elapsedTime < blackHoleDuration)
        {
            if (scientist != null && rb != null)
            {
                Vector3 pushDirection = (scientist.position - blackHole.transform.position).normalized;

                // **Her 0.1 saniyede bir ek itme uygula (Kademeli güç veriyoruz)**
                rb.AddForce(pushDirection * (pushForce * 0.1f) * rb.mass, ForceMode.Force);

                Debug.Log($"Scientist yavaş yavaş uzaklaşıyor! Kuvvet: {pushForce * 0.1f}");
            }

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(blackHole);
        isHawkingModeActive = false;
    }

    Transform DetectScientist()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Scientist")) // Sadece Scientistleri kontrol et
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTarget = col.transform;
                }
            }
        }

        return nearestTarget; // Eğer bilim insanı yoksa null döner
    }*/




}