using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protector.Combat
{
    public class SwordAttack : MonoBehaviour
    {
        public int damage = 20;
        private void Start()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Target enemy = other.GetComponent<Target>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

            }
        }
    }
}

