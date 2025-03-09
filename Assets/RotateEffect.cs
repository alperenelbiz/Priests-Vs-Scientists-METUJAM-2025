using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public bool clockwise;
    void Update()
    {
        if(clockwise)
            transform.Rotate(0,0 ,rotationSpeed * Time.deltaTime );
        else
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

    }
}
