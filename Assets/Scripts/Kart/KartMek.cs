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
using UnityEngine.WSA;
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

    public GameObject scientist;
    public GameObject priest;
    List<GameObject> cardObjectList = new List<GameObject>();
    //public List<Sprite> kartImageList = new List<Sprite>();
    //public GameObject playerSoldier;
    //public GameObject enemySoldier;
    void Awake()
    {

        eventSystem = GameObject.Find("Event System");
        //levelControl = eventSystem.GetComponent<LevelControl>();

        Kart radyasyonKartıOluştur = new()
        {
            ad = "RADYASYON",
            aciklama = "Radyasyon yay",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => radyasyon(kart.ad, true)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(radyasyonKartıOluştur);
        Kart okSaptırKartıOluştur = new()
        {
            ad = "Ok saptır",
            aciklama = "Okun yönünü saptır",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => okSaptır(kart.ad, true)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(okSaptırKartıOluştur);
        Kart karaDelikOluştur = new()
        {
            ad = "Kara delik",
            aciklama = "blackniga",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => karaDelik(kart.ad, true)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(karaDelikOluştur);

        Kart hızlandırmaKartıOluştur = new()
        {
            ad = "Zamanı Hızlandır",
            aciklama = "Ben hızım",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,
            cost = 2,
            OnDestroy = (kart) => ZamanDelay(kart.ad, 2.0f, 5.0f, "Papaz")
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(hızlandırmaKartıOluştur);

        Kart yavaşlatmaKartıOluştur = new()
        {
            ad = "Yavaşlat",
            aciklama = "Ben hız değilim",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => ZamanDelay(kart.ad, 0.5f, 5.0f, "Scientist")
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(yavaşlatmaKartıOluştur);
        Kart okYavaşlatmaKartıOluştur = new()
        {
            ad = "Ok yavaslat",
            aciklama = "Ben hız değilim",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => OkDelay(kart.ad, 2.0f, 5.0f, "Scientist")
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(okYavaşlatmaKartıOluştur);
        Kart okHızlandırmaKartıOluştur = new()
        {
            ad = " Ok Hizlandir",
            aciklama = "Ben hız değilim",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            OnDestroy = (kart) => OkDelay(kart.ad, 0.5f, 5.0f, "Papaz")
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(okHızlandırmaKartıOluştur);

    }
    private void Start()
    {
        DisplayCards();
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
            if (textBox.Length >= 2)
            {
                textBox[0].text = secilenKart.ad; // İlk(Title) TextBox'ı doldur
                textBox[1].text = secilenKart.aciklama; // İkinci(Description) TextBox'ı doldur
                textBox[2].text = secilenKart.cost.ToString(); // Üçücüncü(Cost) TextBox'ı doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamadı!");
            }

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
            if (kart.aktiflik && kart.kalanAdet > 0 && !kullanilanIndexler.Contains(kart.indeks))
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

                if (kart.kalanAdet == 0)
                {
                    kart.aktiflik = false;
                }
                return kart;
            }
        }

        return null;
    }
    void ZamanDelay(string name, float multiplier, float duration,string targetTag) // asker özellikleri arttır!!!!
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            //CoinUpdate(kart);
            kart.kalanAdet--;
            if (kart.kalanAdet == 0)
            {
                kart.aktiflik = false;
            }
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

        Debug.Log(kart.ad + " seçildi");

    }
    void OkDelay(string name, float multiplier, float duration, string targetTag) // asker özellikleri arttır!!!!
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            //CoinUpdate(kart);
            kart.kalanAdet--;
            if (kart.kalanAdet == 0)
            {
                kart.aktiflik = false;
            }
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

        Debug.Log(kart.ad + " seçildi");

    }

    void okSaptır(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log(kart.ad + " seçildi");
            //CoinUpdate(kart);
            kart.kalanAdet--;
            if (kart.kalanAdet == 0)
            {
                kart.aktiflik = false;
            }
            scientist.GetComponent<ScientistArrowSpawner>().isSpaceMode = saptir;
            Debug.Log(saptir);
            // Start the coroutine to handle the saptir bool
            StartCoroutine(SetForSeconds(saptir, 3f, kart));

        }

        Debug.Log(kart.ad + " seçildi");


    }
    void radyasyon(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log(kart.ad + " seçildi");
            //CoinUpdate(kart);
            kart.kalanAdet--;
            if (kart.kalanAdet == 0)
            {
                kart.aktiflik = false;
            }
            //priest.GetComponent<PapazArrowSpawner>().isMarieCurieModeActive = saptir;
            //Debug.Log(saptir);
            // Start the coroutine to handle the saptir bool

            //StartCoroutine(SetForSeconds(saptir, 0.8f, kart));
            priest.GetComponent<PapazArrowSpawner>().ActivateMarieCurieMode();

        }

        Debug.Log(kart.ad + " seçildi");


    }
    void karaDelik(string name, bool saptir)
    {
        Kart kart = kartListesi.FirstOrDefault(x => x.ad == name);
        if (kart != null)
        {
            Debug.Log(kart.ad + " seçildi");
            //CoinUpdate(kart);
            kart.kalanAdet--;
            if (kart.kalanAdet == 0)
            {
                kart.aktiflik = false;
            }
            //priest.GetComponent<PapazArrowSpawner>().isHawkingModeActive = saptir;
            //Debug.Log(saptir);
            // Start the coroutine to handle the saptir bool
            //StartCoroutine(SetForSeconds(saptir, 5f, kart));
            //priest.GetComponent<PapazArrowSpawner>().ActivateHawkingMode();
        }

        Debug.Log(kart.ad + " seçildi");


    }
    private IEnumerator SetForSeconds(bool saptir, float duration, Kart kart)
    {


        yield return new WaitForSeconds(duration);

        saptir = false;
        if (kart.ad == "RADYASYON")
        {
            priest.GetComponent<PapazArrowSpawner>().isMarieCurieModeActive = saptir;
        }
        else if (kart.ad == "Ok saptır")
            scientist.GetComponent<ScientistArrowSpawner>().isSpaceMode = saptir;
        else
            priest.GetComponent<PapazArrowSpawner>().isHawkingModeActive = saptir;
        Debug.Log("saptir set to false");
    }

}