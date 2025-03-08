using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float normalSpeed = 2.0f;
    [SerializeField] float minZ = -5.0f; // Minimum boundary
    [SerializeField] float maxZ = 5.0f;  // Maximum boundary

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // A and D keys for horizontal movement
        float newZ = transform.position.z + moveHorizontal * normalSpeed * Time.deltaTime;

        // Clamp the position within boundaries
        newZ = Mathf.Clamp(newZ, minZ, maxZ);

        // Apply the new position
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, minZ),
                        new Vector3(transform.position.x, transform.position.y, maxZ));
    }
}
