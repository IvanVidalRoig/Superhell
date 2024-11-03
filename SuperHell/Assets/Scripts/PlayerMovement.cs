using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables públicas para ajustar en el Inspector
    public float speed = 5f;           
    public float runSpeed = 8f;         
    public float rotationSpeed= 250;
    public Animator animator;
    public Rigidbody rb;
    public float jumpHeight = 3;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    bool isGrounded;

    private float x,y;
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0,0, y * Time.deltaTime * runSpeed);
        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (Input.GetKey("space") && isGrounded){
            animator.Play("Jump");
            Invoke("Jump", 0);
        }


    }
    public void Jump(){
            rb.AddForce(Vector3.up*jumpHeight, ForceMode.Impulse);    
    }     
}
