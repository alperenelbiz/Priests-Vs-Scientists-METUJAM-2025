using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    public class EnemyDestroying : MonoBehaviour
    {
        public int damage = 2;
        private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}

