using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public Transform handPosition;
    private GameObject pickedUpSword = null;
    private bool hasSword = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Sword") && pickedUpSword == null)
        {
            PickUpSword(other.gameObject);
        }
    }
    void PickUpSword(GameObject sword)
    {
        sword.transform.position = handPosition.position;
        sword.transform.rotation = handPosition.rotation;
        sword.transform.parent = handPosition;
        pickedUpSword = sword;
        
        hasSword = true;
    }
    public bool HasSword()
    {
        return hasSword;
    }
}
