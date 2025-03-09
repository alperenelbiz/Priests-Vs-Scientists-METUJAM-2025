using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    public enum SoldierType { Melee, Ranged }
    public SoldierType soldierType;

    public Transform targetPosition; // Ana hedef (d��man yoksa gidilecek yer)
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public int damage = 10;
    public string enemyTag = "Enemy";

    public GameObject projectilePrefab; // Ranged askerlerin att��� ok prefab�
    public Transform firePoint; // Okun f�rlat�laca�� nokta

    private NavMeshAgent agent;
    private Transform currentEnemy;
    private bool isAttacking;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        MoveToTarget();
        Debug.Log(name + " started moving to target position. Health: " + health + ", Damage: " + damage);
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
            agent.SetDestination(targetPosition.position);
            animator.SetBool("isWalking", true);
            Debug.Log(name + " is walking towards target position.");
        }
    }

    void SearchForEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(enemyTag))
            {
                currentEnemy = col.transform;
                Debug.Log(name + " found an enemy: " + currentEnemy.name);
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
            Debug.Log(name + " is attacking " + currentEnemy.name);
            Attack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentEnemy.position);
            animator.SetBool("isWalking", true);
            Debug.Log(name + " is moving towards " + currentEnemy.name);
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
                Debug.Log(name + " hit " + currentEnemy.name + " for " + damage + " damage.");
            }
            else if (soldierType == SoldierType.Ranged)
            {
                ShootProjectile();
            }
            yield return new WaitForSeconds(attackCooldown);
        }
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null && currentEnemy != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (currentEnemy.position - firePoint.position).normalized;
                rb.velocity = direction * 1f; // Okun h�z�n� ayarla
                projectile.AddComponent<Projectile>(); // Okun �arpt���nda hasar vermesi i�in Projectile scripti ekleniyor
                Debug.Log(name + " fired a projectile at " + currentEnemy.name);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log(name + " took " + amount + " damage. Remaining health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " has died.");
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

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 10f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        SoldierAI soldier = other.GetComponent<SoldierAI>();
        if (soldier != null && !other.CompareTag("Enemy"))
        {
            soldier.TakeDamage(damage);
            Debug.Log(other.name + " took projectile damage: " + damage);
            Destroy(gameObject);
        }
    }
}
