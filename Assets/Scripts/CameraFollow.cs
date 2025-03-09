using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform cameraFollower;
    public Vector3 offset;
    public Vector3 rotation;

    void Start()
    {
        transform.position = cameraFollower.position + offset;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = cameraFollower.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);  

    }
}
