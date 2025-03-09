using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    public enum SoldierType { Melee, Ranged }
    public enum SoldierFaction { Scientist, Priest }
    public SoldierType soldierType;
    public SoldierFaction soldierFaction;

    public Transform targetPosition; // Ana hedef (düþman yoksa gidilecek yer)
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public int damage = 10;

    public GameObject projectilePrefab; // Ranged askerlerin attýðý ok prefabý
    public Transform firePoint; // Okun fýrlatýlacaðý nokta
    public float projectileSpeed = 15f; // Ok hýzý ayarlanabilir

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
            MoveToTarget(); // Düþman yoksa hedefe ilerlemeye devam et
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
            SoldierAI possibleEnemy = col.GetComponent<SoldierAI>();
            if (possibleEnemy != null && possibleEnemy.soldierFaction != soldierFaction)
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
        currentEnemy = null; // Düþman öldüðünde hedefi sýfýrla
        MoveToTarget(); // Tekrar hedefe ilerlemeye baþla
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null && currentEnemy != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Projectile projectileScript = projectile.AddComponent<Projectile>();
                projectileScript.SetDamage(damage);
                projectileScript.SetFaction(soldierFaction);
                projectileScript.Launch(currentEnemy.position, projectileSpeed);
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
    private int damage;
    private SoldierAI.SoldierFaction shooterFaction;
    public float speed = 15f;
    public float lifeTime = 3f;
    public float arcHeight = 2f;
    private Vector3 targetPosition;
    private float startTime;
    private Vector3 startPos;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetFaction(SoldierAI.SoldierFaction faction)
    {
        shooterFaction = faction;
    }

    public void Launch(Vector3 target, float projectileSpeed)
    {
        targetPosition = target;
        startTime = Time.time;
        startPos = transform.position;
        speed = projectileSpeed;
    }

    void Update()
    {
        float timeSinceStarted = Time.time - startTime;
        float journeyFraction = timeSinceStarted / (lifeTime / speed);

        if (journeyFraction >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 nextPos = Vector3.Lerp(startPos, targetPosition, journeyFraction);
        nextPos.y += Mathf.Sin(journeyFraction * Mathf.PI) * arcHeight;
        transform.position = nextPos;
    }

    void OnTriggerEnter(Collider other)
    {
        SoldierAI soldier = other.GetComponent<SoldierAI>();
        if (soldier != null && soldier.soldierFaction != shooterFaction)
        {
            soldier.TakeDamage(damage);
            Debug.Log(other.name + " took projectile damage: " + damage);
            Destroy(gameObject);
        }
    }
}
