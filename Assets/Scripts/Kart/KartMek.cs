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

        Kart radyasyonKart�Olu�tur = new()
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
        
        kartListesi.Add(radyasyonKart�Olu�tur);
        Kart okSapt�rKart�Olu�tur = new()
        {
            ad = "Ok sapt�r",
            aciklama = "Okun y�n�n� sapt�r",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,

            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(okSapt�rKart�Olu�tur);

        Kart h�zland�rmaKart�Olu�tur = new()
        {
            ad = "Zaman� H�zland�r",
            aciklama = "Ben h�z�m",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,
            cost = 2,
            OnDestroy = (kart) => ZamanDelay(kart.ad, 2.0f, 5.0f)
            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };
       
        kartListesi.Add(h�zland�rmaKart�Olu�tur);

        Kart yava�latmaKart�Olu�tur = new()
        {
            ad = "Yava�lat",
            aciklama = "Ben h�z de�ilim",
            aktiflik = true,
            kalanAdet = 3,
            olasilik = 0.5f,
            minLevel = 1,
            maxLevel = 21,

            cost = 2,

            //gorsel = kartImageList.FirstOrDefault(x => x.name == ("OkcuKulesiOlusturma_0"))

        };

        kartListesi.Add(yava�latmaKart�Olu�tur);
        

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

        List<int> kullanilanIndexler = new List<int>(); // Kullan�lan kart indekslerini tutmak i�in liste

        for (int i = 0; i < Coordinates.Count; i++)
        {
            // Kart� Instantiate et ve gerekli �zellikleri ayarla
            Kart secilenKart = SecilenKartiGetir(kullanilanIndexler);
            GameObject cardObject = Instantiate(cardBack, Coordinates[i].position, Quaternion.identity, gameObject.transform);
           // cardObject.transform.SetParent(Coordinates[i]);
            
            cardObjectList.Add(cardObject);

            CardController cardController = cardObject.AddComponent<CardController>();
            cardController.kart = secilenKart;

            if (secilenKart == null)
            {
                continue; // Di�er Kartlara ge�
            }
            TextMeshProUGUI[] textBox = cardObject.GetComponentsInChildren<TextMeshProUGUI>();
            if (textBox.Length >= 2)
            {
                textBox[0].text = secilenKart.ad; // �lk(Title) TextBox'� doldur
                textBox[1].text = secilenKart.aciklama; // �kinci(Description) TextBox'� doldur
                textBox[2].text = secilenKart.cost.ToString(); // ���c�nc�(Cost) TextBox'� doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamad�!");
            }

            Image[] image = cardObject.GetComponentsInChildren<Image>();
            if (image.Length >= 2)
            {
                image[1].sprite = secilenKart.gorsel; // �lk(Title) TextBox'� doldur
            }
            else
            {
                Debug.Log("TextBoxlar bulunamad�!");
            }
            // Se�ilen kart� aktif kartlara ekle
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
            return null; // Kullan�labilir kart yoksa null d�nd�r
        }
        // Kartlar�n olas�l�klar�n� hesapla
        float toplamOlasilik = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplamOlasilik += kart.olasilik;
        }

        // Rastgele bir olas�l�k de�eri se�
        float rastgeleOlasilik = Random.Range(0f, toplamOlasilik);

        float toplam = 0f;
        foreach (Kart kart in kullanilabilirKartlar)
        {
            toplam += kart.olasilik;
            if (rastgeleOlasilik <= toplam)
            {
                // Kart�n kalan adetini kontrol et

                if (kart.kalanAdet == 0)
                {
                    kart.aktiflik = false;
                }
                return kart;
            }
        }

        return null;
    }
    void ZamanDelay(string name, float multiplier, float duration) // asker �zellikleri artt�r!!!!
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
        
        Debug.Log(kart.ad + " se�ildi");
    }

    
}