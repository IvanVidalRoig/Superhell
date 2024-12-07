using UnityEngine;
using System.Collections.Generic;

public enum EnemyState
{
    Patrol,
    Chase
}

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;

    [Header("Waypoints")]
    public List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    [Header("Player Reference")]
    public Transform player;

    // Estado actual
    public EnemyState currentState = EnemyState.Patrol;

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    void Chase()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    public void SetState(EnemyState newState)
    {
        currentState = newState;
    }
}
