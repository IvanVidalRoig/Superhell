using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que entra es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtenemos el script PlayerMovement
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Actualizamos el respawn del jugador a la posición de este checkpoint
                // Puedes sumarle algo de altura para evitar colisionar con el suelo:
                // transform.position + Vector3.up
                playerMovement.respawnPosition = transform.position;

                Debug.Log("Checkpoint activado. Nuevo respawn: " + transform.position);
            }
        }
    }
}
