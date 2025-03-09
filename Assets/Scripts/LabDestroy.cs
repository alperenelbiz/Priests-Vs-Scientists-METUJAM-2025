using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabDestroy : MonoBehaviour
{
    public BuildingHealth churchHealth; // Assign this in the Inspector
    public float damagePerSecond = 10f;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Papaz")) // Ensure Papaz has the correct tag
        {
            if (damageCoroutine == null) // Start damage only if not already running
            {
                damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Papaz"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            if (churchHealth != null)
            {
                churchHealth.TakeDamage(damagePerSecond);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
