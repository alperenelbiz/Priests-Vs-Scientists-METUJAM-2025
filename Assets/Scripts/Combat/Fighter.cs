using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Protector.Combat
{
    public class Fighter : MonoBehaviour
    {
        private Animator animator;
        private float MaxTeleportDistance = 2f;
        private PlayerPickUp playerPickUp;
        private float rotationSpeed;
        private Target target;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerPickUp = GetComponent<PlayerPickUp>();
            target = GetComponent<Target>();
        }

        private void Update()
        {
            if(playerPickUp.HasSword())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    TeleportToNearestEnemy();
                    animator.SetTrigger("SwordPower");
                }
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetTrigger("SwordAttack");
                }
            }
            
            RotateToEnemy(target);
            
        }
        private void RotateToEnemy(Target target)
        {
            if (target == null) return; 

            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Yükseklik (y) deðerini sýfýrlýyoruz
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        private void TeleportToNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                Debug.Log("No enemies found");
                return;
            }

            GameObject nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            
            if (nearestEnemy != null && nearestDistance <= MaxTeleportDistance)
            {
                transform.position = nearestEnemy.transform.position;
                Debug.Log("Teleported to nearest enemy");
            }
            else
            {
                Debug.Log("No enemy within teleport distance");
            }
        }
    }
}
