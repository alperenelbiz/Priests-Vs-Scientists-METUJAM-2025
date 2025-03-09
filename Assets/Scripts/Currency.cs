using System.Collections;
using UnityEngine;
using TMPro;

public class Currency : MonoBehaviour
{
    public TextMeshProUGUI currencyText; // TMP Text Referansı
    public float currency = 0; // Başlangıç Para Değeri
    public float increaseRate = 1f; // Her saniye kaç artacak

    void Start()
    {
        if (currencyText == null)
        {
            Debug.LogError("⚠ Currency: TMP Referansı Eksik!");
            return;
        }

        // Currency'yi düzenli olarak arttıran bir Coroutine başlat
        StartCoroutine(IncreaseCurrencyOverTime());
    }

    IEnumerator IncreaseCurrencyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle
            currency += increaseRate; // Para miktarını artır
            UpdateCurrencyUI(); // UI'yı güncelle
        }
    }

    void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "CURRENCY : " + Mathf.FloorToInt(currency).ToString(); // Tam sayı göster
        }
    }

    // Para eklemek için metod
    public void AddCurrency(float amount)
    {
        currency += amount;
        UpdateCurrencyUI();
    }

    // Para çıkarmak için metod
    public bool SpendCurrency(float amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            UpdateCurrencyUI();
            return true;
        }
        return false; // Yetersiz bakiye
    }
}
