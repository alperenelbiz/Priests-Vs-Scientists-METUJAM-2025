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
   

    List<GameObject> cardObjectList = new List<GameObject>();
    //public List<Sprite> kartImageList = new List<Sprite>();
    //public GameObject playerSoldier;
    //public GameObject enemySoldier;
    void Awake()
    {
        
        eventSystem = GameObject.Find("Event System");
        //levelControl = eventSystem.GetComponent<LevelControl>();

        Kart radyasyonKartýOluþtur = new()
        {
            ad = "RADYASYON",
            aciklama = "Radyasyon yay",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,
            
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };
        
        kartListesi.Add(radyasyonKartýOluþtur);
        Kart okSaptýrKartýOluþtur = new()
        {
            ad = "Ok saptýr",
            aciklama = "Okun yönünü saptýr",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,

            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(okSaptýrKartýOluþtur);

        Kart hýzlandýrmaKartýOluþtur = new()
        {
            ad = "Zamaný Hýzlandýr",
            aciklama = "Ben hýzým",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,
            cost = 2,
            OnDestroy = (kart) => ZamanDelay(kart.ad, 2.0f, 5.0f)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };
       
        kartListesi.Add(hýzlandýrmaKartýOluþtur);

        Kart yavaþlatmaKartýOluþtur = new()
        {
            ad = "Yavaþlat",
            aciklama = "Ben hýz deðilim",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,

            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(yavaþlatmaKartýOluþtur);
        

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

        List<int> kullanilanIndexler = new List<int>(); // Kullanýlan kart indekslerini tutmak için liste

        for (int i = 0; i < Coordinates.Count; i++)
        {
            // Kartý Instantiate et ve gerekli özellikleri ayarla
            Kart secilenKart = SecilenKartiGetir(kullanilanIndexler);
            GameObject cardObject = Instantiate(cardBack, Coordinates[i].position, Quaternion.identity, gameObject.transform);
           // cardObject.transform.SetParent(Coordinates[i]);
            
            cardObjectList.Add(cardObject);

            CardController cardController = cardObject.AddComponent<CardController>();
            cardController.kart = secilenKart;

            if (secilenKart == null)
            {
                continue; // Diðer Kartlara geç
            }
            TextMeshProUGUI[] textBox = cardObject.GetComponentsInChildren<TextMeshProUGUI>();
            if (textBox.Length >= 2)
            {
                textBox[0].text = secilenKart.ad; // Ýlk(Title) TextBox'ý doldur
                textBox[1].text = secilenKart.aciklama; // Ýkinci(Description) TextBox'ý doldur
                textBox[2].text = secilenKart.cost.ToString(); // Üçücüncü(Cost) TextBox'ý doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamadý!");
            }

            Image[] image = cardObject.GetComponentsInChildren<Image>();
            if (image.Length >= 2)
            {
                image[1].sprite = secilenKart.gorsel; // Ýlk(Title) TextBox'ý doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamadý!");
            }
            // Seçilen kartý aktif kartlara ekle
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
            return null; // Kullanýlabilir kart yoksa null döndür
        }
        // Kartlarýn olasýlýklarýný hesapla
        float toplamOlasilik = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplamOlasilik += kart.olasilik;
        }

        // Rastgele bir olasýlýk deðeri seç
        float rastgeleOlasilik = Random.Range(0f, toplamOlasilik);

        float toplam = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplam += kart.olasilik;
            if (rastgeleOlasilik <= toplam)
            {
                // Kartýn kalan adetini kontrol et

                if (kart.kalanAdet == 0)
                {
                    kart.aktiflik = false;
                }
                return kart;
            }
        }

        return null;
    }
    void ZamanDelay(string name, float multiplier, float duration) // asker özellikleri arttýr!!!!
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

    
}