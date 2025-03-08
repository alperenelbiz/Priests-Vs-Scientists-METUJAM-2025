using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour
{
    public Transform targetPosition; // Set in Inspector
    [SerializeField] private float duration = 3f; // Time to reach target
    [SerializeField] private float detectionRadius = 5f; // Detection range
    [SerializeField] private string enemyTag; // Object to detect (e.g., "Scientist" or "Papaz")
    [SerializeField] private float stopThreshold = 0.5f; // Distance to stop at

    [Header("Movement Boundaries")] 
    [SerializeField] private Vector3 minBounds = new Vector3(-5f, 0f, -5f); // Minimum boundary
    [SerializeField] private Vector3 maxBounds = new Vector3(5f, 0f, 5f);  // Maximum boundary

    private Tween moveTween; // Store movement tween
    private bool isPaused = false;
    private float fixedY; // Stores the Y position to keep it constant
    private bool hasShotArrow = false; // Prevents multiple arrow shots
    private bool isScientistShooting = false; // Prevents multiple scientist attacks

    private PapazArrowSpawner arrowSpawner; 

    void Start()
    {
        fixedY = transform.position.y;
        arrowSpawner = GetComponent<PapazArrowSpawner>();
        MoveToTarget();
    }

    void Update()
    {
        DetectEnemy();
        CheckForStop();
    }

    void MoveToTarget()
    {
        if (targetPosition == null)
        {
            Debug.LogError(gameObject.name + ": Target position is missing!");
            return;
        }

        Debug.Log("🚀 " + gameObject.name + " moving to target: " + targetPosition.position);

        // Generate intermediate waypoints with slight randomness
        Vector3[] path = new Vector3[3];
        path[0] = transform.position;
        path[1] = ClampToBounds(new Vector3(
            (transform.position.x + targetPosition.position.x) / 2 + Random.Range(-0.5f, 0.5f),
            fixedY,
            (transform.position.z + targetPosition.position.z) / 2 + Random.Range(-0.5f, 0.5f)
        ));
        path[2] = ClampToBounds(targetPosition.position);

        moveTween = transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            
            .OnComplete(() =>
            {
               
                moveTween = null;
                TryShootArrow();
            });
        hasShotArrow = false;
        isScientistShooting = false;
    }

    void DetectEnemy()
    {
        if (string.IsNullOrEmpty(enemyTag)) return; // Prevent errors

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool enemyNearby = false;

        foreach (var col in colliders)
        {
            Debug.Log($"👀 {gameObject.name} detected: {col.gameObject.name} with tag {col.tag}");

            if (col.CompareTag(enemyTag))
            {
                Debug.Log($"⏸ {gameObject.name} stopping (Detected {enemyTag})");
                enemyNearby = true;
                break;
            }
        }

        if (enemyNearby && moveTween != null && !isPaused)
        {
            moveTween.Pause();
            isPaused = true;
            TryShootArrow();
        }
        else if (!enemyNearby && isPaused)
        {
            moveTween.Play();
            isPaused = false;
            hasShotArrow = false;
            isScientistShooting = false;
        }
    }

    void CheckForStop()
    {
        if (moveTween == null) return;

        float distance = Vector3.Distance(transform.position, targetPosition.position);
        if (distance <= stopThreshold)
        {
            Debug.Log($"🛑 {gameObject.name} reached its target, stopping.");
            moveTween.Kill();
            moveTween = null;
            TryShootArrow();
            TriggerScientistAttack();

        }
    }
    
    void TriggerScientistAttack()
    {
        GameObject[] scientists = GameObject.FindGameObjectsWithTag("Scientist"); // Find all scientists
        Transform nearestScientist = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject scientist in scientists)
        {
            float distance = Vector3.Distance(transform.position, scientist.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestScientist = scientist.transform;
            }
        }

        if (nearestScientist != null)
        {
            ScientistArrowSpawner scientistSpawner = nearestScientist.GetComponent<ScientistArrowSpawner>();
            if (scientistSpawner != null)
            {
                Debug.Log($"🏹 Scientist {nearestScientist.name} is shooting an arrow at {gameObject.name}");
                scientistSpawner.ShootArrow(transform); // Make the scientist shoot at this enemy
                isScientistShooting = true;
            }
        }
    }
    
    void TryShootArrow()
    {
        if (arrowSpawner != null && !hasShotArrow)
        {
            Transform nearestTarget = arrowSpawner.FindNearestTarget("Scientist"); // Find target
            if (nearestTarget != null)
            {
                arrowSpawner.ShootArrow(nearestTarget); // Shoot arrow at nearest scientist
                Debug.Log($"🏹 {gameObject.name} shot an arrow at {nearestTarget.name}");
                hasShotArrow = true; // Prevent multiple shots while stopped
            }
        }
    }
    
    private Vector3 ClampToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            fixedY, // Keep the Y position constant
            Mathf.Clamp(position.z, minBounds.z, maxBounds.z)
        );
    }
    
    // Time Dilation Card Integration
    public void ApplySpeedEffect(float multiplier, float duration)
    {
        if (moveTween != null)
        {
            moveTween.timeScale = multiplier; // Adjust movement speed
        }

        StartCoroutine(RestoreSpeed(duration));
    }

    private IEnumerator RestoreSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (moveTween != null)
        {
            moveTween.timeScale = 1f; // Reset to normal speed
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
