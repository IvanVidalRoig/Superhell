using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuraci�n del Checkpoint")]
    [Tooltip("Si true, el cofre solo se abrir� la primera vez que el jugador lo toque.")]
    public bool onlyOnce = true;
    [Tooltip("Altura extra para colocar el punto de respawn sobre el cofre.")]
    public float offsetAboveChest = 1f;

    private Animator chestAnimator; // Se obtiene autom�ticamente en Awake()
    private bool alreadyOpened = false;

    void Awake()
    {
        // Si el Animator est� en este mismo GameObject
        chestAnimator = GetComponent<Animator>();
        if (chestAnimator == null)
        {
            Debug.LogError("No se encontr� un Animator en este GameObject. Aseg�rate de que el cofre tenga un Animator aqu�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta al jugador y comprueba si este cofre se puede volver a abrir
        if (other.CompareTag("Player") && (!alreadyOpened || !onlyOnce))
        {
            // 1) Actualizar respawn del jugador
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Define el nuevo punto de respawn un poco por encima del cofre
                Vector3 newRespawn = transform.position + Vector3.up * offsetAboveChest;
                playerMovement.respawnPosition = newRespawn;
                Debug.Log("Checkpoint activado en cofre. Nuevo respawn: " + newRespawn);
            }

            // 2) Disparar la animaci�n de apertura
            if (chestAnimator != null)
            {
                // Aseg�rate de tener un par�metro Trigger en tu Animator Controller llamado "OpenChest"
                chestAnimator.SetTrigger("OpenChest");
                Debug.Log("Cofre: disparando animaci�n de apertura.");
            }

            // 3) Marcar como abierto, si solo queremos que se abra la primera vez
            alreadyOpened = true;
        }
    }
}
