using Attack;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 20f; 
    void Start()
    {
        // "ArrowLayer" numarasýný al
        int arrowLayer = LayerMask.NameToLayer("ArrowLayer");

        // "ArrowLayer" ile "ArrowLayer" arasýndaki çarpýþmayý kapat
        Physics.IgnoreLayerCollision(arrowLayer, arrowLayer, true);
    }
    private GameObject shooter; // Oku atan nesne

    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eðer ok kendisini atan nesneye çarptýysa, hiçbir þey yapma
        if (other.gameObject == shooter)
        {
            return;
        }
        if (other.gameObject.CompareTag("Arrow"))
        {
            return;
        }

        // Eðer bir Scientist veya Papaz'a çarptýysa, oku yok et
        if (other.CompareTag("Scientist") || other.CompareTag("Papaz"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage); // Canýný düþür
            }

            Destroy(gameObject); // Ok yok olsun
        }
    }
}
