using UnityEngine;
using System.Collections;

public class PapazArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject blackHolePrefab;
    public float fireRate = 2f;
    public float shootForce = 10f;

    public bool isMarieCurieModeActive = false;
    public HealthSystem healthSystem; // Saðlýk sistemine eriþim
    public RadioationEffect radiationEffect; // Parçacýk efekti

    public float blackHoleDuration = 5f;
    public float pushForce = 10f;
    private bool isHawkingModeActive = false;



    void Start()
    {
        StartCoroutine(FireArrows());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isMarieCurieModeActive)
        {
            ActivateMarieCurieMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !isHawkingModeActive) 
        {
            ActivateHawkingMode();
        }
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Transform nearestTarget = FindNearestTarget("Scientist");

            if (nearestTarget != null)
            {
                ShootArrow(nearestTarget);
            }
        }
    }

    public Transform FindNearestTarget(string enemyTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearestTarget = null;
        float minDistance = 7f;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target.transform;
            }
        }
        return nearestTarget;
    }

    public void ShootArrow(Transform target)
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = newArrow.GetComponent<Rigidbody>();

        // Okun sahibini belirle
        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetShooter(gameObject);
        }

        if (rb != null)
        {
                Vector3 launchVelocity = CalculateLaunchVelocity(target);
                rb.velocity = launchVelocity;
                rb.useGravity = true;
                
        }
    }

    Vector3 CalculateLaunchVelocity(Transform target)
    {
        Vector3 start = transform.position; // Kapsï¿½lï¿½n pozisyonu
        Vector3 end = target.position; // Hedefin pozisyonu
        float gravity = Mathf.Abs(Physics.gravity.y); // Yerï¿½ekimi

        // Hedefin yatay uzaklï¿½ï¿½ï¿½nï¿½ hesapla
        Vector3 horizontalDirection = new Vector3(end.x - start.x, 0, end.z - start.z);
        float horizontalDistance = horizontalDirection.magnitude;

        // Hedefin yï¿½ksekliï¿½i farkï¿½
        float heightDifference = end.y - start.y;

        // ï¿½lk hï¿½zï¿½n yukarï¿½ bileï¿½enini hesapla
        float initialVelocityY = Mathf.Sqrt(2 * gravity * heightDifference + gravity * horizontalDistance);

        // Uï¿½uï¿½ sï¿½resini hesapla
        float time = (initialVelocityY / gravity) * 2;

        // XZ yï¿½nï¿½ndeki hï¿½z bileï¿½enini hesapla
        Vector3 velocityXZ = horizontalDirection.normalized * (horizontalDistance / time);

        // Son hï¿½z vektï¿½rï¿½nï¿½ belirle
        return velocityXZ + Vector3.up * initialVelocityY;
    }
    void ActivateMarieCurieMode()
    {
        isMarieCurieModeActive = true;
        radiationEffect.ActivateRadiation(); // Parçacýklarý baþlat
        StartCoroutine(HealOverTime());
    }

    IEnumerator HealOverTime()
    {
        float duration = 5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            healthSystem.Heal(10f * Time.deltaTime); // Yavaþ saðlýk artýþý
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DeactivateMarieCurieMode();
    }

    void DeactivateMarieCurieMode()
    {
        isMarieCurieModeActive = false;
        radiationEffect.DeactivateRadiation(); // Parçacýklarý durdur
    }

    void ActivateHawkingMode()
    {
        Transform nearestScientist = FindNearestTarget("Scientist");
        if (nearestScientist == null) return;

        isHawkingModeActive = true;

        // **Papaz ile Scientist’in tam ortasýnda kara delik oluþtur**
        Vector3 spawnPosition = (transform.position + nearestScientist.position) / 2f;
        spawnPosition.y = spawnPosition.y - 1f;
        GameObject blackHole = Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);

        StartCoroutine(BlackHoleEffect(blackHole, nearestScientist));
    }

    IEnumerator BlackHoleEffect(GameObject blackHole, Transform scientist)
    {
        float elapsedTime = 0f;
        Rigidbody rb = scientist.GetComponent<Rigidbody>();

        while (elapsedTime < blackHoleDuration)
    {
        if (scientist != null && rb != null)
        {
            Vector3 pushDirection = (scientist.position - blackHole.transform.position).normalized;
            
            // **Daha güçlü bir itme kuvveti uygula**
            rb.AddForce(pushDirection * pushForce * rb.mass, ForceMode.Impulse);

            Debug.Log("Scientist itildi! Yön: " + pushDirection);
        }

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f); // Daha sýk kontrol ederek etkiyi artýr
    }

        Destroy(blackHole);
        isHawkingModeActive = false;
    }


}