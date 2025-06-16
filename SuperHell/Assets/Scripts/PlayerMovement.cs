using System.Collections;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 3f;
    public float runSpeed = 5f;
    public float rotationSpeed = 250;

    [Header("Salto")]
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Reset / Vida")]
    public LayerMask resetMask;
    private bool isInResetZone = false;
    private bool canLoseLife = true;
    public float lifeCooldown = 1f; // 1 segundo de cooldown

    [Header("Componentes")]
    public Animator animator;
    public Rigidbody rb;

    // Variables para el conteo de saltos
    private bool isGrounded;
    private int jumpCount = 0;
    private const int maxJumpCount = 2; // Salto inicial + doble salto

    // Nueva variable: Posición de respawn
    [Header("Checkpoint")]
    public Vector3 respawnPosition; // Último punto de reaparición

    void Start()
    {
        // Iniciamos la posición de respawn en un punto seguro (como hacías antes con (0,4,10))
        respawnPosition = new Vector3(0, 4, 10);
    }

    void Update()
    {
        // --- Movimiento ---
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);

        // Actualizar animaciones de movimiento
        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        // --- Comprobar si está en el suelo ---
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		Vector3 moveDirection = transform.forward * y;

		if (isGrounded)
		{
			rb.velocity = new Vector3(moveDirection.x * runSpeed, rb.velocity.y, moveDirection.z * runSpeed);
		}
		else
		{
			// Movimiento reducido en el aire
			rb.velocity = new Vector3(moveDirection.x * (runSpeed * 0.5f), rb.velocity.y, moveDirection.z * (runSpeed * 0.5f));
		}
		if (isGrounded)
		{
			transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
			transform.Translate(0, 0, y * Time.deltaTime * runSpeed);

			// Actualizar animaciones de movimiento solo si está en el suelo
			animator.SetFloat("VelX", x);
			animator.SetFloat("VelY", y);
		}
		else
		{
			// Si está en el aire, detener animaciones de caminar
			animator.SetFloat("VelX", 0);
			animator.SetFloat("VelY", 0);
		}

		// Si acaba de aterrizar, reseteamos el número de saltos
		if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        // --- Teletransportar al origen (para debug) ---
        if (Input.GetKey("q"))
        {
            TeleportToOrigin();
        }

        // --- Manejar el salto y el doble salto ---
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

		if (!isGrounded && rb.velocity.y < 0)
		{
			animator.Play("Falling");
		}
	}

    private void Jump()
    {
        // Resetear velocidad vertical antes de saltar
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        // Añadir fuerza de salto
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Manejar colisión con objetos de la capa "Reset" solo si coinciden con la tag "Reset"
        if (((1 << other.gameObject.layer) & resetMask) != 0 && other.CompareTag("Reset"))
        {
            Debug.Log("Jugador ha tocado la zona Reset con los pies.");

            if (canLoseLife)
            {
                isInResetZone = true;
                canLoseLife = false; // Evitamos perder vida repetidamente

                // Llamamos al GameManager (si existe) para restar vida
                if (GameManager.instance != null)
                {
                    GameManager.instance.PerderVida();
                    Debug.Log("Vida perdida. Vidas restantes: " + GameManager.instance.vidas);
                }
                else
                {
                    Debug.LogError("GameManager no está asignado en la escena.");
                }

                // Teletransportar al jugador al último respawn checkpoint
                TeleportToOrigin();
                Debug.Log("Jugador teletransportado al último checkpoint.");

                // Iniciar el cooldown para poder perder otra vida después de un tiempo
                StartCoroutine(LifeCooldown());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Salir de la zona Reset
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

    // --- Ajustado para que use la respawnPosition en vez de una fija ---
    private void TeleportToOrigin()
    {
        transform.position = respawnPosition;
        rb.velocity = Vector3.zero;
        Debug.Log("Jugador teletransportado al respawn: " + respawnPosition);
    }
}
