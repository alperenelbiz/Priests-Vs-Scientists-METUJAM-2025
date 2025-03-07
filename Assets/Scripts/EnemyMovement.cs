using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // Assign the destination in the Inspector
    [SerializeField] private float duration = 2f; // Time in seconds for the movement
    [SerializeField] private float detectionRadius = 3f; // Radius to check for the player
    [SerializeField] private float deviationAmount = 0.5f; // How much deviation from the straight line
    [SerializeField] private float stopThreshold = 0.5f; // How close to stop the movement

    [Header("Movement Boundaries")] 
    [SerializeField] private Vector3 minBounds = new Vector3(-5f, 0f, -5f); // Minimum map boundary
    [SerializeField] private Vector3 maxBounds = new Vector3(5f, 0f, 5f);  // Maximum map boundary

    private Tween moveTween; // Store the tween
    private bool isPaused = false;
    private float fixedY; // Stores the Y position to keep it constant

    void Start()
    {
        fixedY = transform.position.y; // Store initial Y position
        MoveToTarget();
    }

    void Update()
    {
        CheckForPlayer();
        CheckForStop();
    }

    void MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetPosition.position;

        // Ensure Y-axis stays fixed
        startPosition.y = fixedY;
        endPosition.y = fixedY;

        // Generate intermediate waypoints with slight randomness (only on X & Z axes)
        Vector3[] path = new Vector3[3];
        path[0] = startPosition;
        path[1] = ClampToBounds(new Vector3(
            (startPosition.x + endPosition.x) / 2 + Random.Range(-deviationAmount, deviationAmount),
            fixedY, // Keep Y-axis fixed
            (startPosition.z + endPosition.z) / 2 + Random.Range(-deviationAmount, deviationAmount)
        ));
        path[2] = ClampToBounds(endPosition); // Ensure the final position stays inside bounds

        moveTween = transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => moveTween = null);
    }

    void CheckForPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool playerNearby = false;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerNearby = true;
                break;
            }
        }

        if (playerNearby && moveTween != null && !isPaused)
        {
            moveTween.Pause();
            isPaused = true;
        }
        else if (!playerNearby && isPaused)
        {
            moveTween.Play();
            isPaused = false;
        }
    }
    
    void CheckForStop()
    {
        if (moveTween == null) return;

        float currentX = transform.position.x;
        float targetX = targetPosition.position.x;

        if (Mathf.Abs(currentX - targetX) <= stopThreshold) // Stop when close enough on X
        {
            moveTween.Kill(); // Stop movement
            moveTween = null;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw map boundaries
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((minBounds + maxBounds) / 2, maxBounds - minBounds);
    }
}
