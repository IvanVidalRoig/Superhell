using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 250;
    public Animator animator;
    public Rigidbody rb;
    public float jumpHeight = 3f;
    public float climbHeight = 1f;
    public float climbForwardForce = 1000f; 

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public LayerMask resetMask; 
    public LayerMask climbMask;

    // Eliminamos isInClimbZone basado en colisiones
    // private bool isInClimbZone = false;

    // Añadimos una referencia para el ClimbTrigger si es necesario
    // Opcional: si tienes múltiples triggers, considera usar una lista
    private bool isInClimbZone = false;

    private bool isGrounded;
    private bool isClimbing;
    private bool isJumping = false;
    private float x, y;

void Update()
{
    // Verificar si el jugador está escalando
    if (!isClimbing)
    {
        // Procesar entradas de movimiento solo si NO está escalando
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);
        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);
    }
    else
    {
        // Opcional: Resetear los parámetros de animación de movimiento
        animator.SetFloat("VelX", 0);
        animator.SetFloat("VelY", 0);
    }

    // Verificar si el personaje está en el suelo
    bool wasGrounded = isGrounded;
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    if (Input.GetKey("q")) 
    {
        TeleportToOrigin();
    }

    if (isGrounded && !isJumping && Input.GetKey("space"))
    {
        // Iniciar el salto y activar la animación
        isJumping = true;
        animator.Play("Jump");
        Jump();
    }
    else if (!isGrounded && !isJumping && !isClimbing)
    {
        animator.Play("Falling");
    }
    
    if (Input.GetKey("f") && isInClimbZone) 
    {
        isClimbing = true;
        animator.Play("Climbing");
        Climb();
    }
}


    private void Jump()
    {
        // Añadir fuerza para el salto
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        
        // Iniciar corrutina para desactivar el salto después de 0.9 segundos
        StartCoroutine(JumpCooldown(0.9f));
    }

    private void Climb()
    {
        // Calcular la fuerza hacia arriba
        float upwardForce = Mathf.Sqrt(climbHeight * -3f * Physics.gravity.y);

        // Aplicar la fuerza hacia arriba
        rb.velocity = new Vector3(rb.velocity.x, upwardForce, rb.velocity.z);

        // Aplicar fuerza hacia adelante si es necesario
        rb.AddForce(transform.forward * climbForwardForce * Time.deltaTime);

        // Iniciar corrutina para desactivar la escalada después de 1.5 segundos
        StartCoroutine(ClimbCooldown(1.5f));
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isJumping = false;
    }

    private IEnumerator ClimbCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isClimbing = false;
    }

    private IEnumerator ApplyForwardForceWithDelay(float delay)
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(delay);

        // Aplicar la fuerza hacia adelante
        ApplyForwardForce(climbForwardForce);
    }


    // Añadimos OnTriggerEnter y OnTriggerExit para la escalada
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si la layer del objeto es "borde" para escalada
        if (((1 << other.gameObject.layer) & climbMask) != 0)
        {
            isInClimbZone = true;
        }

        // Opcional: manejar otras capas como "Reset"
        if (((1 << other.gameObject.layer) & resetMask) != 0)
        {
            TeleportToOrigin();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verificar si la layer del objeto es "borde" para escalada
        if (((1 << other.gameObject.layer) & climbMask) != 0)
        {
            isInClimbZone = false;
        }
    }

    private void TeleportToOrigin()
    {
        // Teletransportar al jugador a la posición (0, 0, 0)
        rb.position = Vector3.zero;
        rb.velocity = Vector3.zero; // Resetear la velocidad para evitar que el jugador siga moviéndose
    }

    private void ApplyForwardForce(float force)
    {
        // Aplica una fuerza hacia adelante basada en la dirección actual del personaje
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
}
