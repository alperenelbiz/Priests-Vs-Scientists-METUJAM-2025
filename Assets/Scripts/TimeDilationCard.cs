using System;
using UnityEngine;

public class TimeDilationCard : MonoBehaviour
{
    [Header("Time Dilation Settings")]
    [SerializeField] private float slowDuration = 3f; // Duration of slow effect
    [SerializeField] private float slowMultiplier = 0.5f; // Slows enemy movement (0.5 = 50% speed)

    [Header("Speed Boost Settings")]
    [SerializeField] private float boostDuration = 3f; // Duration of speed boost
    [SerializeField] private float boostMultiplier = 1.5f; // Increases speed (1.5 = 50% faster)
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.T) )
        {
            print("Pressed slow key");
            ApplyEffectToAllEnemies(slowMultiplier, slowDuration, "Scientist");
            ApplyArrowSpeedEffect(slowMultiplier, slowDuration, "Scientist");

        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            print("Pressed boost key");
            ApplyEffectToAllEnemies(boostMultiplier, boostDuration, "Papaz");
            ApplyArrowSpeedEffect(boostMultiplier, boostDuration, "Papaz");
        }
    }

    private void ApplyEffectToAllEnemies(float multiplier, float duration, string targetTag)
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag(targetTag))
            {
                //Debug.Log($"✅ Applying {multiplier}x speed effect to {enemy.gameObject.name} ({targetTag})");
                enemy.ApplySpeedEffect(multiplier, duration);
            }
                
        }
    }
    
    private void ApplyArrowSpeedEffect(float multiplier, float duration, string targetTag)
    {
        if (targetTag == "Scientist")
        {
            Debug.Log("Applying arrow speed effect to Scientists");
            ScientistArrowSpawner[] scientistSpawners = FindObjectsOfType<ScientistArrowSpawner>();
            foreach (var spawner in scientistSpawners)
            {
                spawner.SetArrowSpeedMultiplier(multiplier, duration);
            }
        }
        else if (targetTag == "Papaz")
        {
            Debug.Log("Applying arrow speed effect to Papaz");
            PapazArrowSpawner[] papazSpawners = FindObjectsOfType<PapazArrowSpawner>();
            foreach (var spawner in papazSpawners)
            {
                spawner.SetArrowSpeedMultiplier(multiplier, duration);
            }
        }
    }
    
}
