using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpSystem : MonoBehaviour
{
    public float pickUpRange = 3f; // Nesneyi almak için gereken mesafe
    public Transform player; // Karakterin Transformu

    void Update()
    {
        // Eðer C tuþuna basýlmýþsa
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Sahnedeki tüm Pickable etiketli nesneleri al
            GameObject[] pickableItems = GameObject.FindGameObjectsWithTag("Pickable");

            GameObject closestItem = GetClosestItem(pickableItems); // En yakýn nesneyi bul
            if (closestItem != null)
            {
                PickUp(closestItem); // Nesneyi al
            }
        }
    }

    // En yakýn nesneyi bulmak için
    GameObject GetClosestItem(GameObject[] items)
    {
        GameObject closestItem = null;
        float closestDistance = pickUpRange;

        foreach (GameObject item in items)
        {
            // Karakter ile nesne arasýndaki mesafeyi hesapla
            float distanceToItem = Vector3.Distance(player.position, item.transform.position);

            // Eðer mesafe pickUpRange'den küçükse ve en yakýn nesneyse
            if (distanceToItem <= pickUpRange && distanceToItem < closestDistance)
            {
                closestDistance = distanceToItem;
                closestItem = item;
            }
        }

        return closestItem;
    }

    // Yakýndaki bir nesneyi almak için
    void PickUp(GameObject item)
    {
        Debug.Log("Nesne alýndý: " + item.name);
        Destroy(item); // Nesneyi yok et
    }
}
