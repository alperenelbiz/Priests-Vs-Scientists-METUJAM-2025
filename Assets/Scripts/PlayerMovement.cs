using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float normalSpeed = 2.0f;  
    public float runSpeed = 4.0f; 
    public float jumpForce = 5.0f;
    
    private Rigidbody rb;

    private Animator animator;  

    public float rotationSmoothness = 0.1f;  
    public float rotationSpeed = 5.0f;  
    private float speed;  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        speed = normalSpeed;  
    }

    void FixedUpdate()
    {
        
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        if (movement.magnitude > 0)
        {
            // Karakterin bakt��� y�ne g�re hareketi hesapla
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.fixedDeltaTime * rotationSpeed);
        }

        // Ko�ma kontrol� (Ctrl tu�una bas�l�nca h�z artar)
        if (Input.GetKey(KeyCode.V))
        {
            speed = runSpeed;  // Ctrl bas�l�yken ko�ma h�z�
        }
        else
        {
            speed = normalSpeed;  // Aksi halde normal y�r�me h�z�
        }

        // Karakteri hareket ettir
        Vector3 moveDirection = transform.forward * movement.magnitude * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        UpdateAnimator(movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }
    }


    private void UpdateAnimator(Vector3 movement)
    {
        float movementSpeed = movement.magnitude;
        animator.SetFloat("speed", movementSpeed * speed);  // Animator'a h�z de�erini g�ncelleme
    }

    /* private void OnCollisionEnter(Collision collision)
     {
         if (collision.gameObject.CompareTag("Ground"))
         {
             isGrounded = true;
         }
     }*/
}
