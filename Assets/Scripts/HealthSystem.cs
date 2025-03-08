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
        //Debug.Log(gameObject.name + " hasar ald�! Yeni Can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        //Debug.Log(gameObject.name + " sa�l�k kazand�! Yeni Can: " + currentHealth);
    }

    void Die()
    {
        //Debug.Log(gameObject.name + " �ld�!");
        spawner.OnEnemyDeath(gameObject);
    }
}
