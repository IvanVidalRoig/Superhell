using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    public EnemyAI enemyAI; // Asigna aquí el enemigo al que vas a notificar

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.SetState(EnemyState.Chase);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.SetState(EnemyState.Patrol);
        }
    }
}
