using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageSystem : MonoBehaviour
{
    public float pushForce = 10f;
    

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 forceDirection = -contact.normal;
            rb.AddForce(forceDirection * pushForce, ForceMode.Impulse);
        }
    }

    

    
}
