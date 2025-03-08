using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f; // Maksimum can
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Baþlangýçta tam can
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

    void Die()
    {
        Debug.Log(gameObject.name + " öldü!");
        Destroy(gameObject); // Karakter yok olur
    }
}
