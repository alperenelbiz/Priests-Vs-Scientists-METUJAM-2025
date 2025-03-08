using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hasar ald�! Yeni Can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " �ld�!");
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log(gameObject.name + " sa�l�k kazand�! Yeni Can: " + currentHealth);
    }
    
}
