using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public Vector3 rotation;

    void Start()
    {
        

        transform.position = player.position + offset;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
    private void Update()
    {
      
    }
    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);  

    }
}
