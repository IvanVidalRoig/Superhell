using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 250;
    public Animator animator;
    public Rigidbody rb;
    public float jumpHeight = 3;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool isJumping = false;
    private bool isFalling = false;
    private float x, y;

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);
        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        // Verificar si el personaje está en el suelo
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !isJumping && Input.GetKey("space"))
        {
            // Iniciar el salto y activar la animación
            isJumping = true;
            isFalling = false;
            animator.Play("Jump");
            Jump();
        }
        else if (!isGrounded && !isJumping && !isFalling)
        {
            // Iniciar el retraso de la caída si acaba de dejar el suelo
            StartCoroutine(FallDelay(0.3f)); // Espera 1 segundo antes de activar la caída
        }

        // Si el personaje aterriza, detener la animación de caída
        if (isGrounded && isFalling)
        {
            isFalling = false;
            animator.Play("Landing"); // Puedes cambiar "Landing" por una animación de aterrizaje, o "Idle" si prefieres
        }
    }

    private void Jump()
    {
        // Añadir fuerza para el salto
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        
        // Iniciar corrutina para desactivar el salto después de 3 segundos
        StartCoroutine(JumpCooldown(1f));
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isJumping = false;
    }

    private IEnumerator FallDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Activar la animación de caída solo si sigue en el aire después del retraso
        if (!isGrounded)
        {
            isFalling = true;
            animator.Play("Falling");
        }
    }
}
