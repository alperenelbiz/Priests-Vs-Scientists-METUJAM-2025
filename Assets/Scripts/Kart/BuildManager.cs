using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    static public bool isBuilt=false;

    public GameObject[] areas;
    public GameObject turretCursor;

    //public void Start()
    //{
    //    areas = FindObjectsOfType<PlacementArea>();
    //    turretCursor = FindObjectOfType<TurretFollow>();
    //}

    //public void CreateArea()
    //{
    //    turretCursor.gameObject.SetActive(true);

    //    foreach (var area in areas)
    //    {
    //        area.gameObject.SetActive(true);
    //    }

    //}



    public void DestroyArea()
    {
        StartCoroutine(DestroyWithDelay());
    }

    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var area in areas)
        {
            if (area != null)
                Destroy(area);
               
        }

       
        Destroy(turretCursor);
    }

    
}
