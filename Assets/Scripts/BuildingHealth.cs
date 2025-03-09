using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth { get; private set; }

    public HealthBar healthBar;
    public GameObject winnerCanvas;
    public TextMeshProUGUI winnerText; // Reference to Winner Text UI

    private static bool gameEnded = false; // Ensures only one building triggers the end

    private CameraShake cameraShake;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        if (winnerCanvas != null)
        {
            winnerCanvas.SetActive(false); // Hide winner UI initially
        }

        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void TakeDamage(float damage)
    {
        if (gameEnded) return; // Prevent further damage after the game ends

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    private IEnumerator DieSequence()
    {
        if (gameEnded) yield break; // Stop if the game has already ended

        gameEnded = true; // Mark game as ended

        Debug.Log(gameObject.name + " öldü!");

        // Shake the camera
        if (cameraShake != null)
        {
            yield return StartCoroutine(cameraShake.Shake(0.5f, 0.5f)); // Shake for 0.5 seconds
        }

        // Find the remaining building
        GameObject remainingBuilding = FindRemainingBuilding();

        // Stop the game
        Time.timeScale = 0f;

        // Display the Winner UI
        if (winnerCanvas != null)
        {
            winnerCanvas.SetActive(true);

            if (winnerText != null)
            {
                string message = gameObject.name + " Destroyed!";
                
                if (remainingBuilding != null)
                {
                    message += "\n" + remainingBuilding.name + " Wins!";
                }

                winnerText.text = message;
            }
        }
    }

    private GameObject FindRemainingBuilding()
    {
        // Find all objects with the BuildingHealth component
        BuildingHealth[] allBuildings = FindObjectsOfType<BuildingHealth>();

        foreach (BuildingHealth building in allBuildings)
        {
            if (building != this && building.currentHealth > 0) // Find the alive building
            {
                return building.gameObject;
            }
        }

        return null; // No building found
    }
}
