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

    public GameObject arrow;
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
            //OnDestroy = (kart) => okSaptır(kart.ad,true)
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
            OnDestroy = (kart) => ZamanDelay(kart.ad, 2.0f, 5.0f)
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
            OnDestroy = (kart) => ZamanDelay(kart.ad, 2.0f, 5.0f)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(yavaşlatmaKartıOluştur);
        

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
    void ZamanDelay(string name, float multiplier, float duration) // asker özellikleri arttır!!!!
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
                enemy.ApplySpeedEffect(multiplier, duration);
            }

        }
        
        Debug.Log(kart.ad + " seçildi");
       
    }

    void okSaptır(string name,bool saptir)
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
            arrow.GetComponent<ScientistArrowSpawner>().isSpaceMode = saptir;
            Debug.Log(saptir);
            // Start the coroutine to handle the saptir bool
            StartCoroutine(SetSaptirForSeconds(saptir, 0.8f));
            
        }

        Debug.Log(kart.ad + " seçildi");
        
        
    }

    private IEnumerator SetSaptirForSeconds(bool saptir, float duration)
    {
        

        yield return new WaitForSeconds(duration);

        saptir = false;
            arrow.GetComponent<ScientistArrowSpawner>().isSpaceMode = saptir;
        Debug.Log("saptir set to false");
    }

}