using UnityEngine;

public class TimeDilationCard : MonoBehaviour
{
    [Header("Time Dilation Settings")]
    [SerializeField] private float slowDuration = 3f; // Duration of slow effect
    [SerializeField] private float slowMultiplier = 0.5f; // Slows enemy movement (0.5 = 50% speed)

    [Header("Speed Boost Settings")]
    [SerializeField] private float boostDuration = 3f; // Duration of speed boost
    [SerializeField] private float boostMultiplier = 1.5f; // Increases speed (1.5 = 50% faster)

    [Header("Key Bindings")]
    [SerializeField] private KeyCode slowKey = KeyCode.T; // Press 'T' to slow down enemies
    [SerializeField] private KeyCode boostKey = KeyCode.Y; // Press 'Y' to speed up enemies

    void Update()
    {
        if (Input.GetKeyDown(slowKey))
        {
            Debug.Log("Slow down");
            ApplyEffectToAllEnemies(slowMultiplier, slowDuration);
        }

        if (Input.GetKeyDown(boostKey))
        {
            Debug.Log("Boost");
            ApplyEffectToAllEnemies(boostMultiplier, boostDuration);
        }
    }

    private void ApplyEffectToAllEnemies(float multiplier, float duration)
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();

        foreach (var enemy in enemies)
        {
            enemy.ApplySpeedEffect(multiplier, duration);
        }
    }
}
