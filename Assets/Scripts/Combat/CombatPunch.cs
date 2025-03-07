using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protector.Combat
{
    public class CombatPunch : MonoBehaviour
    {
        
        private Animator anim;
        private PlayerMovement playerMovement;  
        private bool isAttacking;
        public int kickDamage = 20;
        public int punchDamage = 10;
        public float forceStrength = 5f;
        private bool isKicking = false;
        private bool isPunching = false;

        private void Start()
        {
            anim = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();  
            isAttacking = false;
        }

        private void Update()
        {
            if (!isAttacking)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    anim.SetTrigger("ComboPunch");
                    isAttacking = true;
                    isPunching = true;
                    isKicking = false;
                    playerMovement.enabled = false;  
                    StartCoroutine(ResetAttack());
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    anim.SetTrigger("Kick");
                    isAttacking = true;
                    isKicking = true;  
                    isPunching = false;
                    playerMovement.enabled = false;  
                    StartCoroutine(ResetAttack());
                }
            }
        }

        private IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(1f);  
            isAttacking = false;
            playerMovement.enabled = true;  
        }

        private void OnTriggerEnter(Collider other)
        {
            
            Protector.Combat.Target target = other.GetComponent<Protector.Combat.Target>();
            if (target != null)
            {
                Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();

                if (isPunching)
                {
                    target.TakeDamage(punchDamage);  
                    ApplyForceToTarget(targetRigidbody);  
                }
                else if (isKicking)
                {
                    target.TakeDamage(kickDamage);  
                    ApplyForceToTarget(targetRigidbody);  
                }
            }
        }

        private void ApplyForceToTarget(Rigidbody targetRigidbody)
        {
            if (targetRigidbody != null)
            {
                
                Vector3 direction = transform.forward;
                
                targetRigidbody.AddForce(direction * forceStrength, ForceMode.Impulse);
            }
        }
    }
}
