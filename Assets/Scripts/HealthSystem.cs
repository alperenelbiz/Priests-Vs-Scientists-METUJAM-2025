using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public EnemySpawner spawner;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hasar aldý! Yeni Can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log(gameObject.name + " saðlýk kazandý! Yeni Can: " + currentHealth);
    }

    void Die()
    {
        Debug.Log(gameObject.name + " öldü!");
        spawner.OnEnemyDeath(gameObject);
    }
}
