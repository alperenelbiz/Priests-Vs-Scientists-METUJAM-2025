using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protector.Combat
{
    public class Target : MonoBehaviour
    {
        public int health = 100;
        public void TakeDamage(int damage)
        {
            health -= damage;
            if(health <= 0)
            {
                Die();
            }
        }
        void Die()
        {
            Destroy(gameObject);
        }
    }
}

