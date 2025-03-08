using UnityEngine;

public class CardController : MonoBehaviour
{
    public Kart kart;

    private void OnDestroy()
    {
        // Invoke the OnDestroy delegate
        kart.OnDestroy?.Invoke(kart);
    }
}