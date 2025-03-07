using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovment : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // Assign the destination in the Inspector
    [SerializeField] private float duration = 2f; // Time in seconds for the movement
    [SerializeField] private float detectionRadius = 3f; // Radius to check for the player
    [SerializeField] private float deviationAmount = 0.5f;
    
    private Tween moveTween; // Store the tween
    private bool isPaused = false;

    void Start()
    {
        MoveToTarget();
    }

    void Update()
    {
        CheckForPlayer();
    }

    void MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetPosition.position;

        // Generate intermediate waypoints with slight randomness
        Vector3[] path = new Vector3[3];
        path[0] = startPosition;
        path[1] = new Vector3(
            (startPosition.x + endPosition.x) / 2 + Random.Range(-deviationAmount, deviationAmount),
            (startPosition.y + endPosition.y) / 2,
            (startPosition.z + endPosition.z) / 2 + Random.Range(-deviationAmount, deviationAmount)
        );
        path[2] = endPosition;

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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
