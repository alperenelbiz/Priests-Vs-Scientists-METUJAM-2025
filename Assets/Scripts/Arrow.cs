using Attack;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 20f; 
    void Start()
    {
        // "ArrowLayer" numaras�n� al
        int arrowLayer = LayerMask.NameToLayer("ArrowLayer");

        // "ArrowLayer" ile "ArrowLayer" aras�ndaki �arp��may� kapat
        Physics.IgnoreLayerCollision(arrowLayer, arrowLayer, true);
    }
    private GameObject shooter; // Oku atan nesne

    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // E�er ok kendisini atan nesneye �arpt�ysa, hi�bir �ey yapma
        if (other.gameObject == shooter)
        {
            return;
        }
        if (other.gameObject.CompareTag("Arrow"))
        {
            return;
        }

        // E�er bir Scientist veya Papaz'a �arpt�ysa, oku yok et
        if (other.CompareTag("Scientist") || other.CompareTag("Papaz"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage); // Can�n� d���r
            }

            Destroy(gameObject); // Ok yok olsun
        }
    }
}
