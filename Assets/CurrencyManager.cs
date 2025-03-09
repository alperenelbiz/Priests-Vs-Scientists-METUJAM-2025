using UnityEngine;
using UnityEngine.UI;  // TextMeshPro'nun gerekli olması için bunu eklemelisin
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public int currency = 0;  // Para miktarı
    public TextMeshProUGUI currencyText;  // TextMeshPro öğesi (TextMeshPro'yu doğru şekilde referans alıyoruz)

    void Start()
    {
        InvokeRepeating("IncreaseCurrencyPerSecond", 1f, 1f);  // Her saniyede bir para artacak
        UpdateCurrencyText();  // Başlangıçta para miktarını güncelle
    }

    // Para miktarını ekrana yansıtan fonksiyon
    void UpdateCurrencyText()
    {
        currencyText.text = "Currency: " + currency.ToString();  // Ekranda gösterilecek metin
    }

    // Saniyede bir para artırma fonksiyonu
    void IncreaseCurrencyPerSecond()
    {
        currency++;  // Para miktarını artır
        UpdateCurrencyText();  // Metni güncelle
    }

    // Düşman öldüğünde para ekleme fonksiyonu
    public void AddCurrency(int amount)
    {
        currency += amount;  // Verilen miktarda para ekle
        UpdateCurrencyText();  // Metni güncelle
    }
}
