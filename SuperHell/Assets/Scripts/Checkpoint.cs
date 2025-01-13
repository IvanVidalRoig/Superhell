using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuraci�n del Checkpoint")]
    [Tooltip("Si true, el cofre solo se abrir� la primera vez que el jugador lo toque.")]
    public bool onlyOnce = true;
    [Tooltip("Altura extra para colocar el punto de respawn sobre el cofre.")]
    public float offsetAboveChest = 1f;

    private Animator chestAnimator;
    private bool alreadyOpened = false;

    void Awake()
    {
        chestAnimator = GetComponent<Animator>();
        if (chestAnimator == null)
        {
            Debug.LogError("No se encontr� un Animator en este GameObject. Aseg�rate de que el cofre tenga un Animator aqu�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (!alreadyOpened || !onlyOnce))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector3 newRespawn = transform.position + Vector3.up * offsetAboveChest;
                playerMovement.respawnPosition = newRespawn;
                Debug.Log("Checkpoint activado en cofre. Nuevo respawn: " + newRespawn);
            }

            if (chestAnimator != null)
            {
                chestAnimator.SetTrigger("OpenChest");
                Debug.Log("Cofre: disparando animaci�n de apertura.");
            }

            alreadyOpened = true;
        }
    }
}
