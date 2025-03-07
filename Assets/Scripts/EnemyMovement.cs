using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform target; 
    public float speed = 5f; 

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                speed = 0f;
            }
        }
    }
}
