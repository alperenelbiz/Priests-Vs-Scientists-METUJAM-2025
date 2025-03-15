using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
public class KartMek : MonoBehaviour
{
    public GameObject cardBack;
    GameObject eventSystem;
    public List<Kart> kartListesi = new List<Kart>();
    private List<Kart> aktifKartlar = new List<Kart>();
    //LevelControl levelControl;
    [SerializeField] public List<Transform> Coordinates = new List<Transform>();

    public List<GameObject> scientist;
    public List<GameObject> priest;
    List<GameObject> cardObjectList = new List<GameObject>();
    public List<Sprite> kartImageList = new List<Sprite>();
    public Currency currency;
    //public GameObject playerSoldier;
    //public GameObject enemySoldier;
    public float blackHoleDuration = 3f;
    void Awake()
    {

        eventSystem = GameObject.Find("Event System");
        //levelControl = eventSystem.GetComponent<LevelControl>();

        Kart radyasyonKartıOluştur = new()
        {
            ad = "Radiation",
            aciklama = "Radyasyon yay",
            aktiflik = true,
           
            olasilik = 0.2f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => radyasyon(kart.ad, true),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("radyasyon"))

        };

        kartListesi.Add(radyasyonKartıOluştur);
        Kart okSaptırKartıOluştur = new()
        {
            ad = "Arrow Deflect",
            aciklama = "Okun yönünü saptır",
            aktiflik = true,
            
            olasilik = 0.4f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => okSaptır(kart.ad, true),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("newton"))

        };

        kartListesi.Add(okSaptırKartıOluştur);
        Kart karaDelikOluştur = new()
        {
            ad = "Black Hole",
            aciklama = "blackniga",
            aktiflik = true,
            
            olasilik = 0.1f,
            minLevel = 1,
            maxLevel = 21,

            cost = 5,
            OnDestroy = (kart) => karaDelik(kart.ad, true),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("karadelik"))

        };

        kartListesi.Add(karaDelikOluştur);

        Kart hızlandırmaKartıOluştur = new()
        {
            ad = "Time Speed",
            aciklama = "Ben hızım",
            aktiflik = true,
            
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,
            cost = 2,
            OnDestroy = (kart) => MovementDelay(kart.ad, 2.0f, 5.0f, "Papaz"),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("hızlan"))

        };

        kartListesi.Add(hızlandırmaKartıOluştur);

        Kart yavaşlatmaKartıOluştur = new()
        {
            ad = "Time Slow",
            aciklama = "Ben hız değilim",
            aktiflik = true,
           
            olasilik = 0.4f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => MovementDelay(kart.ad, 0.5f, 5.0f, "Scientist"),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("yavaşlat"))

        };

        kartListesi.Add(yavaşlatmaKartıOluştur);
        Kart okYavaşlatmaKartıOluştur = new()
        {
            ad = "Arrow Slow",
            aciklama = "Ben hız değilim",
            aktiflik = true,
            
            olasilik = 0.2f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => OkDelay(kart.ad, 2f, 5.0f, "Scientist"),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("yavaşlat"))

        };

        kartListesi.Add(okYavaşlatmaKartıOluştur);
        Kart okHızlandırmaKartıOluştur = new()
        {
            ad = " Arrow Speed",
            aciklama = "Ben hız değilim",
            aktiflik = true,
            
            olasilik = 0.2f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => OkDelay(kart.ad, 0.5f, 5.0f, "Papaz"),
            gorsel = kartImageList.FirstOrDefault(x => x.name == ("hızlan"))

        };

        kartListesi.Add(okHızlandırmaKartıOluştur);

    }
    private void Start()
    {
        DisplayCards();
    }
    private void Update()
    {
        if (aktifKartlar.Count == 0) { DisplayCards(); }
    }
    public void DisplayCards()
    {

        foreach (var item in cardObjectList)
        {
            Destroy(item);
        }
        aktifKartlar.Clear();

        List<int> kullanilanIndexler = new List<int>(); // Kullanılan kart indekslerini tutmak için liste

        for (int i = 0; i < Coordinates.Count; i++)
        {
            // Kartı Instantiate et ve gerekli özellikleri ayarla
            Kart secilenKart = SecilenKartiGetir(kullanilanIndexler);
            GameObject cardObject = Instantiate(cardBack, Coordinates[i].position, Quaternion.identity, gameObject.transform);
            // cardObject.transform.SetParent(Coordinates[i]);

            cardObjectList.Add(cardObject);

            CardController cardController = cardObject.AddComponent<CardController>();
            cardController.kart = secilenKart;

            if (secilenKart == null)
            {
                continue; // Diğer Kartlara geç
            }
            TextMeshProUGUI[] textBox = cardObject.GetComponentsInChildren<TextMeshProUGUI>();
            
                textBox[0].text = secilenKart.ad; // İlk(Title) TextBox'ı doldur
                
               
            
            

            Image[] image = cardObject.GetComponentsInChildren<Image>();
            if (image.Length >= 2)
            {
                image[1].sprite = secilenKart.gorsel; // İlk(Title) TextBox'ı doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamadı!");
            }
            // Seçilen kartı aktif kartlara ekle
            aktifKartlar.Add(secilenKart);
            kullanilanIndexler.Add(secilenKart.indeks);

        }
    }
    Kart SecilenKartiGetir(List<int> kullanilanIndexler)
    {
        List<Kart> kullanilabilirKartlar = new List<Kart>();

        foreach (Kart kart in kartListesi)
        {
            if (kart.aktiflik&& !kullanilanIndexler.Contains(kart.indeks))
            {
                kullanilabilirKartlar.Add(kart);
            }
        }

        if (kullanilabilirKartlar.Count == 0)
        {
            return null; // Kullanılabilir kart yoksa null döndür
        }
        // Kartların olasılıklarını hesapla
        float toplamOlasilik = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplamOlasilik += kart.olasilik;
        }

        // Rastgele bir olasılık değeri seç
        float rastgeleOlasilik = Random.Range(0f, toplamOlasilik);

        float toplam = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplam += kart.olasilik;
            if (rastgeleOlasilik <= toplam)
            {
                // Kartın kalan adetini kontrol et

               
                return kart;
            }
        }

        return null;
    }
    //void ZamanDelay(string name, float multiplier, float duration, string targetTag) // asker özellikleri arttır!!!!
    //{
    //    Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
    //    if (kart != null)
    //    {
    //        //CoinUpdate(kart);
    //        kart.kalanAdet--;
    //        if (kart.kalanAdet == 0)
    //        {
    //            kart.aktiflik = false;
    //        }
    //        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();

    //        foreach (var enemy in enemies)
    //        {
    //            if (enemy.CompareTag(targetTag))
    //            {
    //                //Debug.Log($"✅ Applying {multiplier}x speed effect to {enemy.gameObject.name} ({targetTag})");
    //                enemy.ApplySpeedEffect(multiplier, duration);
    //            }

    //        }
    //        aktifKartlar.Remove(kart);
    //    }

    //    Debug.Log(kart.ad + " seçildi");

    //}

    void MovementDelay(string name, float multiplier, float duration, string targetTag)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log(kart.ad + " seçildi");

            

            // **Belirtilen Tag'e Sahip Karakterleri Bul**
            SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
            foreach (SoldierAI soldier in allSoldiers)
            {
                if (soldier.CompareTag(targetTag)) // 🔥 SADECE belirtilen TAG'e sahip karakterleri etkilesin
                {
                    soldier.ApplySpeedEffect(multiplier, duration); // ⚡ Hız değiştir
                    Debug.Log(soldier.name + " hız çarpanı uygulandı: " + multiplier);
                }
            }
            currency.SpendCurrency(kart.cost);
            aktifKartlar.Remove(kart);
        }
    }



    //void OkDelay(string name, float multiplier, float duration, string targetTag) // asker özellikleri arttır!!!!
    //{
    //    Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
    //    if (kart != null)
    //    {
    //        //CoinUpdate(kart);
    //        kart.kalanAdet--;
    //        if (kart.kalanAdet == 0)
    //        {
    //            kart.aktiflik = false;
    //        }
    //        if (targetTag == "Scientist")
    //        {
    //            Debug.Log("Applying arrow speed effect to Scientists");
    //            ScientistArrowSpawner[] scientistSpawners = FindObjectsOfType<ScientistArrowSpawner>();
    //            foreach (var spawner in scientistSpawners)
    //            {
    //                spawner.SetArrowSpeedMultiplier(multiplier, duration);
    //            }
    //        }
    //        else if (targetTag == "Papaz")
    //        {
    //            Debug.Log("Applying arrow speed effect to Papaz");
    //            PapazArrowSpawner[] papazSpawners = FindObjectsOfType<PapazArrowSpawner>();
    //            foreach (var spawner in papazSpawners)
    //            {
    //                spawner.SetArrowSpeedMultiplier(multiplier, duration);
    //            }
    //        }
    //        aktifKartlar.Remove(kart);
    //    }

    //    Debug.Log(kart.ad + " seçildi");

    //}
    void OkDelay(string name, float multiplier, float duration, string targetTag) // asker özellikleri arttır!!!!
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            //CoinUpdate(kart);
            
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
            currency.SpendCurrency(kart.cost);
            aktifKartlar.Remove(kart);
        }

        Debug.Log(kart.ad + " seçildi");

    }

    void okSaptır(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log($"🎴 Kart Kullanıldı: {kart.ad}");

           

            // **1️⃣ SADECE "Scientist" TAG'İNE SAHİP OLAN RANGED ASKERLERİ BUL VE SPACE MODE'U AÇ**
            SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
            foreach (SoldierAI soldier in allSoldiers)
            {
                if (soldier.soldierType == SoldierAI.SoldierType.Ranged && soldier.CompareTag("Scientist"))
                {
                    soldier.isSpaceMode = true; // 🔥 Space Mode AÇ
                    Debug.Log($"✅ {soldier.name} için Space Mode AKTİF");
                }
            }

            // **2️⃣ Belirtilen süre sonra hepsini kapat**
            StartCoroutine(DisableSpaceModeAfter(3f)); // 3 saniye sonra kapat
            currency.SpendCurrency(kart.cost);
            aktifKartlar.Remove(kart);
        }
    }
    IEnumerator DisableSpaceModeAfter(float duration)
    {
        yield return new WaitForSeconds(duration); // ⏳ Süre kadar bekle

        // **Sahnede olan tüm askerleri bul ve Space Mode'u kapat**
        SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
        foreach (SoldierAI soldier in allSoldiers)
        {
            soldier.isSpaceMode = false; // ❌ Space Mode KAPAT
            Debug.Log($"🛑 {soldier.name} için Space Mode KAPANDI");
        }
    }




    void radyasyon(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log(kart.ad + " seçildi");

            

            // **SADECE "Priest" TAG'İNE SAHİP RANGED ASKERLERİ BUL VE MODU AKTİF ET**
            SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
            foreach (SoldierAI soldier in allSoldiers)
            {
                if (soldier.CompareTag("Papaz"))
                {
                    soldier.ActivateMarieCurieMode(); // ☢️ **Marie Curie Modu Açılıyor**
                    Debug.Log(soldier.name + " için Marie Curie Mode AKTİF (Sadece Priest)");
                }
            }

            // **Belirli süre sonra tekrar kapat**
            StartCoroutine(SetForSeconds(saptir, 3f, kart));
            currency.SpendCurrency(kart.cost);
            aktifKartlar.Remove(kart);
        }
    }

    void karaDelik(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log($"🎴 Kara Delik Kartı Kullanıldı: {kart.ad}");

            // **Sadece "Papaz" Tag'ine Sahip Askerleri Bul ve "Hawking Mode" Başlat**
            SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
            foreach (SoldierAI soldier in allSoldiers)
            {
                if (soldier.CompareTag("Papaz"))
                {
                    soldier.ActivateHawkingMode(); // 🌀 Hawking Mode Aç
                    Debug.Log($"🌌 {soldier.name} için Hawking Mode AKTİF!");
                }
            }

            // **Belirli süre sonra kapat**
            StartCoroutine(DisableHawkingModeAfter(blackHoleDuration));
            currency.SpendCurrency(kart.cost);
            aktifKartlar.Remove(kart);
        }
    }
    IEnumerator DisableHawkingModeAfter(float duration)
    {
        yield return new WaitForSeconds(duration);

        // **Sahnede olan tüm "Papaz" askerleri bul ve "Hawking Mode" kapat**
        SoldierAI[] allSoldiers = FindObjectsOfType<SoldierAI>();
        foreach (SoldierAI soldier in allSoldiers)
        {
            if (soldier.CompareTag("Papaz"))
            {
                soldier.DeactivateHawkingMode(); // ❌ Hawking Mode Kapat
                Debug.Log($"🛑 {soldier.name} için Hawking Mode KAPANDI.");
            }
        }
    }

    private IEnumerator SetForSeconds(bool saptir, float duration, Kart kart)
    {


        yield return new WaitForSeconds(duration);

        saptir = false;
        if (kart.ad == "RADYASYON")
        {
            foreach(GameObject pri in priest)
            {
                pri.GetComponent<SoldierAI>().isMarieCurieModeActive = saptir;
            }
            
        }
        else if (kart.ad == "Ok saptır")
        {
           foreach(GameObject sci in scientist)
            {
                sci.GetComponent<SoldierAI>().isSpaceMode = saptir;
            }            
        }
            
        else
            foreach(GameObject pri in priest)
            {
                pri.GetComponent<PapazArrowSpawner>().isHawkingModeActive = saptir;
            }
            
        Debug.Log("saptir set to false");
    }

}