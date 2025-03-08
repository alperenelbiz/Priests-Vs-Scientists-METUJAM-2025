using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public int attackDamage = 20;
    public LayerMask enemyLayer;

    public void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out HealthSystem health))
            {
                health.TakeDamage(attackDamage);
            }
        }
    }

    // For Debugging - Draw the attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}