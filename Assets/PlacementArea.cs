using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementArea : BuildManager
{
    private Renderer rend;

    public Color hoverColor;
    private Color startColor;
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;
    
    public Transform turretBlueprint;
   

    
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color; 

    }



    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;  
    }



    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //turret butonlarý nodelarýn üstünde olduðunda butona basýnca altýndaki node a turret koymamasý için
            return;
        
        BuildTurret(turretBlueprint);
    }



    void BuildTurret(Transform blueprint)
    {
        if(!isBuilt)
        {
            GameObject _turret = (GameObject)Instantiate(blueprint.gameObject, GetBuildPosition(), Quaternion.identity);
           

            turretBlueprint = blueprint;

            ////GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
            ////Destroy(effect, 5f);



            BuildManager buildManager = FindObjectOfType<BuildManager>(); 
            if (buildManager != null)
            {
                buildManager.DestroyArea();
            }
            else
            {
                Debug.LogWarning("BuildManager not found in the scene!");
            }


        }

        isBuilt = true;
        
    }


    void OnMouseEnter() 
    {
        if(!isBuilt)
        {
            rend.material.color = hoverColor;
        }
    }


    void OnMouseExit()
    {
        rend.material.color = startColor;
    }



}
