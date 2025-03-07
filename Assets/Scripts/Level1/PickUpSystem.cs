using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpSystem : MonoBehaviour
{
    public float pickUpRange = 3f; // Nesneyi almak i�in gereken mesafe
    public Transform player; // Karakterin Transformu

    void Update()
    {
        // E�er C tu�una bas�lm��sa
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Sahnedeki t�m Pickable etiketli nesneleri al
            GameObject[] pickableItems = GameObject.FindGameObjectsWithTag("Pickable");

            GameObject closestItem = GetClosestItem(pickableItems); // En yak�n nesneyi bul
            if (closestItem != null)
            {
                PickUp(closestItem); // Nesneyi al
            }
        }
    }

    // En yak�n nesneyi bulmak i�in
    GameObject GetClosestItem(GameObject[] items)
    {
        GameObject closestItem = null;
        float closestDistance = pickUpRange;

        foreach (GameObject item in items)
        {
            // Karakter ile nesne aras�ndaki mesafeyi hesapla
            float distanceToItem = Vector3.Distance(player.position, item.transform.position);

            // E�er mesafe pickUpRange'den k���kse ve en yak�n nesneyse
            if (distanceToItem <= pickUpRange && distanceToItem < closestDistance)
            {
                closestDistance = distanceToItem;
                closestItem = item;
            }
        }

        return closestItem;
    }

    // Yak�ndaki bir nesneyi almak i�in
    void PickUp(GameObject item)
    {
        Debug.Log("Nesne al�nd�: " + item.name);
        Destroy(item); // Nesneyi yok et
    }
}
