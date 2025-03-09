using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    public int currencyAmount = 5; // Düşman öldüğünde verilecek para miktarı

    private void OnMouseDown()
    {
        // CurrencyManager'ı bul ve para ekle
        FindObjectOfType<CurrencyManager>().AddCurrency(currencyAmount);  // 5 para ekler
        
        Destroy(gameObject); // Enemy'yi yok et
    }
}

  