using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    public class CoreHealth : MonoBehaviour
    {
        public int maxHealth = 100;
        private int currentHealth;
        private void Start()
        {
            currentHealth = maxHealth;
        }
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

