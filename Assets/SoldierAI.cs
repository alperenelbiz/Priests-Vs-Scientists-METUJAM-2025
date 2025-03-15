using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    public enum SoldierType { Melee, Ranged }
    public enum SoldierFaction { Scientist, Priest }

    public SoldierType soldierType;
    public SoldierFaction soldierFaction;

    public Transform targetPosition;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public int damage = 10;

    public GameObject arrowPrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;
    public bool isSpaceMode = false;
    public float fireRate = 2f;
    private float arrowSpeedMultiplier = 1f;

    private NavMeshAgent agent;
    private Transform currentEnemy;
    private bool isAttacking;
    private Animator animator;

    public float shootForce = 10f;

    public RadioationEffect radiationEffect;
    public bool isMarieCurieModeActive = false;

    private float speedMultiplier = 1f;
    public float soldierSpeed;
    public static GameObject BlackHoleInstance; // Tek bir Black Hole olacak
    public GameObject blackHolePrefab;
    public Transform blackHoleSpawnPoint; // Karadeliğin çıkacağı yer
    public bool isHawkingModeActive = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        MoveToTarget();

        if (soldierType == SoldierType.Ranged)
        {
            StartCoroutine(FireArrows());
        }
        if (BlackHoleInstance == null && blackHolePrefab != null)
        {
            BlackHoleInstance = Instantiate(blackHolePrefab);
            BlackHoleInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        if (currentEnemy == null)
        {
            MoveToTarget();
            SearchForEnemy();
        }
        else
        {
            EngageEnemy();
        }
    }

    void MoveToTarget()
    {
        if (targetPosition != null)
        {
            agent.isStopped = false;
            agent.speed = speedMultiplier*soldierSpeed;
            agent.SetDestination(targetPosition.position);
            animator.SetBool("isWalking", true);
        }
    }

    public void ApplySpeedEffect(float multiplier, float duration)
    {
        StartCoroutine(SpeedEffectCoroutine(multiplier, duration));
    }

    IEnumerator SpeedEffectCoroutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier; // ⚡ Hız çarpanını uygula
        agent.speed = 3.5f * speedMultiplier; // Yeni hız ayarla
        Debug.Log(name + " hız değiştirildi: " + agent.speed);

        yield return new WaitForSeconds(duration); // ⏳ Belirtilen süre bekle

        speedMultiplier = 1.0f; // 🛑 Hızı sıfırla
        agent.speed = 3.5f * speedMultiplier;
        Debug.Log(name + " hızı normale döndü.");
    }

    public void ApplyArrowSpeedEffect(float multiplier, float duration)
    {
        StartCoroutine(ArrowSpeedEffectCoroutine(multiplier, duration));
    }

    IEnumerator ArrowSpeedEffectCoroutine(float multiplier, float duration)
    {
        arrowSpeedMultiplier = multiplier; // 🏹 Ok hız çarpanını değiştir
        Debug.Log(name + " ok hız değiştirildi: " + arrowSpeedMultiplier);

        yield return new WaitForSeconds(duration); // ⏳ Süre kadar bekle

        arrowSpeedMultiplier = 1.0f; // 🔄 Normale dön
        Debug.Log(name + " ok hızı normale döndü.");
    }

    void SearchForEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider col in colliders)
        {
            SoldierAI possibleEnemy = col.GetComponent<SoldierAI>();
            if (possibleEnemy != null && possibleEnemy.soldierFaction != soldierFaction)
            {
                currentEnemy = col.transform;
                break;
            }
        }
    }

    void EngageEnemy()
    {
        if (currentEnemy == null) return;

        float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.position);

        if (distanceToEnemy <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            Attack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentEnemy.position);
            animator.SetBool("isWalking", true);
        }
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        while (currentEnemy != null && health > 0)
        {
            if (soldierType == SoldierType.Melee && currentEnemy.GetComponent<SoldierAI>())
            {
                currentEnemy.GetComponent<SoldierAI>().TakeDamage(damage);
            }
            else if (soldierType == SoldierType.Ranged)
            {
                ShootArrow(currentEnemy);
            }
            yield return new WaitForSeconds(attackCooldown);
        }
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        currentEnemy = null;
        MoveToTarget();
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate * arrowSpeedMultiplier);
            Transform nearestTarget = FindNearestTarget(soldierFaction == SoldierFaction.Scientist ? "Papaz" : "Scientist");

            if (nearestTarget != null)
            {
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
            if (target.transform == transform) continue;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance && distance < detectionRange)
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
    public void ActivateMarieCurieMode()
    {
        if (CompareTag("Papaz")) // ☢️ Sadece "Papaz" olan askerler etkilenecek
        {
            isMarieCurieModeActive = true;
            radiationEffect.ActivateRadiation(); // Görsel efekti başlat

            StartCoroutine(HealOverTime(2f, 5)); // 2 saniye boyunca, saniyede 5 can artışı
            Debug.Log($"{name} Marie Curie Mode'a girdi! ☢️");
        }
    }

    // **Belirtilen süre boyunca can yenileme efekti**
    IEnumerator HealOverTime(float duration, int healPerSecond)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            health += healPerSecond; // Her saniye 5 can arttır
            Debug.Log($"{name} can kazandı! Yeni Can: {health}");

            elapsedTime += 1f; // 1 saniye bekle
            yield return new WaitForSeconds(1f);
        }

        DeactivateMarieCurieMode(); // Süre bitince modu kapat
    }

    public void DeactivateMarieCurieMode()
    {
        isMarieCurieModeActive = false;
        radiationEffect.DeactivateRadiation(); // Görsel efekti kapat

        Debug.Log($"{name} Marie Curie Mode'dan çıktı! ☢️");
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

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    public void ActivateHawkingMode()
    {
        isHawkingModeActive = true;
        Debug.Log($"{name} Hawking Mode'a girdi! 🌀");

        // **Papazlar kendini klonlasın**
        if (CompareTag("Papaz"))
        {
            CloneSelf();
        }

        // **Black Hole belirlenen noktada açılacak**
        if (BlackHoleInstance != null && blackHoleSpawnPoint != null)
        {
            BlackHoleInstance.transform.position = blackHoleSpawnPoint.position;
            BlackHoleInstance.SetActive(true);
            Debug.Log($"🌌 Black Hole aktif edildi! Pozisyon: {BlackHoleInstance.transform.position}");
        }

        // **1 saniye sonra Black Hole'u gizle**
        StartCoroutine(HideBlackHoleAfter(1f));
    }

    GameObject CloneSelf()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        GameObject clone = Instantiate(gameObject, transform.position + randomOffset, Quaternion.identity);
        clone.name = gameObject.name + "_Clone";
        Debug.Log($"{gameObject.name} klonlandı! 🌀");

        return clone;
    }

    IEnumerator HideBlackHoleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (BlackHoleInstance != null)
        {
            BlackHoleInstance.SetActive(false);
            Debug.Log("🌌 Black Hole gizlendi!");
        }
    }

    public void DeactivateHawkingMode()
    {
        isHawkingModeActive = false;
        Debug.Log($"{name} Hawking Mode'dan çıktı! 🛑");

        if (BlackHoleInstance != null)
        {
            BlackHoleInstance.SetActive(false);
        }
    }


    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
