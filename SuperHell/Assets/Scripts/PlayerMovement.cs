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

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public LayerMask resetMask; 

    private bool isInResetZone = false; // Variable para la zona Reset
    private bool isGrounded;
    private float x, y;

    // Variables para el cooldown de pérdida de vida
    private bool canLoseLife = true;
    public float lifeCooldown = 1f; // 1 segundo de cooldown

    // Variable para controlar los saltos
    private int jumpCount = 0;
    private const int maxJumpCount = 2; // Salto inicial + doble salto

    void Update()
    {
        // Procesar entradas de movimiento
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);
        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        // Verificar si el personaje está en el suelo
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Si acaba de aterrizar, resetear jumpCount
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        // Manejar la teletransportación
        if (Input.GetKey("q")) 
        {
            TeleportToOrigin();
        }

        // Manejar el salto y el doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && jumpCount < 1)
            {
                // Salto inicial
                animator.Play("Jump");
                Jump();
                jumpCount++;
            }
            else if (!isGrounded && jumpCount < maxJumpCount)
            {
                // Doble salto
                animator.Play("Jump");
                Jump();
                jumpCount++;
            }
        }
        else if (!isGrounded && jumpCount == 0)
        {
            animator.Play("Falling");
        }
    }

    private void Jump()
    {
        // Añadir fuerza para el salto
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Resetear la velocidad vertical antes de saltar
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    // Añadimos OnTriggerEnter y OnTriggerExit para la zona Reset
    private void OnTriggerEnter(Collider other)
    {
        // Manejar colisión con objetos de la capa "Reset" solo si es el collider de los pies
        if (((1 << other.gameObject.layer) & resetMask) != 0 && other.CompareTag("Reset"))
        {
            Debug.Log("Jugador ha tocado la zona Reset con los pies.");
            // Verificar si ya podemos perder una vida
            if (canLoseLife)
            {
                isInResetZone = true;
                canLoseLife = false; // Inhabilitar pérdida de vida hasta que pase el cooldown

                // Llamar al GameManager para perder una vida
                if (GameManager.instance != null)
                {
                    GameManager.instance.PerderVida();
                    Debug.Log("Vida perdida. Vidas restantes: " + GameManager.instance.vidas);
                }
                else
                {
                    Debug.LogError("GameManager no está asignado en la escena.");
                }

                // Teletransportar al jugador a la posición de origen
                TeleportToOrigin();
                Debug.Log("Jugador teletransportado al origen.");

                // Iniciar el cooldown para permitir perder otra vida después de cierto tiempo
                StartCoroutine(LifeCooldown());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Manejar la salida de la zona Reset solo si es el collider de los pies
        if (((1 << other.gameObject.layer) & resetMask) != 0 && other.CompareTag("Reset"))
        {
            isInResetZone = false;
            Debug.Log("Salió de la zona Reset.");
        }
    }

    private IEnumerator LifeCooldown()
    {
        yield return new WaitForSeconds(lifeCooldown);
        canLoseLife = true;
        Debug.Log("Cooldown completado. Ahora puedes perder otra vida.");
    }

    private void TeleportToOrigin()
    {
        // Define una posición segura fuera de la capa Reset
        Vector3 safeOrigin = new Vector3(0, 5, 0); // Ajusta según tu escena
        transform.position = safeOrigin;
        rb.velocity = Vector3.zero; // Resetear la velocidad para evitar que el jugador siga moviéndose
        Debug.Log("Jugador teletransportado a la posición segura: " + safeOrigin);
    }
}
