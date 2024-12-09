using UnityEngine;
using System.Collections.Generic;

public class PlatformDetector : MonoBehaviour
{
    private List<EnemyAI> enemiesOnPlatform = new List<EnemyAI>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Cambiar el estado de todos los enemigos en esta plataforma a Chase
            foreach (var enemy in enemiesOnPlatform)
            {
                enemy.SetState(EnemyState.Chase);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Cambiar el estado de todos los enemigos en esta plataforma a Patrol
            foreach (var enemy in enemiesOnPlatform)
            {
                enemy.SetState(EnemyState.Patrol);
            }
        }
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (!enemiesOnPlatform.Contains(enemy))
        {
            enemiesOnPlatform.Add(enemy);
        }
    }
}
