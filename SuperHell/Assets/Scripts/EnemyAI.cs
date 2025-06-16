using UnityEngine;
using System.Collections.Generic;

public enum EnemyState
{
    Patrol,
    Chase
}

public class EnemyAI : MonoBehaviour
{
    [Header("Platform Reference")]
    public GameObject platformObject;

    [Header("Movement Settings")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 8f;
    public float margin = 11f; // Ajusta para controlar cuanto "hacia adentro" van los waypoints

    [Header("Waypoints (auto-generated)")]
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;

    [Header("Player Reference")]
    public Transform player;
    private Rigidbody playerRb;
    public EnemyState currentState = EnemyState.Patrol;
    private Animator anim;
    public float attackRange = 2f;
    public float pushForce = 10f; // Ajusta la fuerza del empuje

    private float minX, maxX, minZ, maxZ;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        if (platformObject != null)
        {
            GenerateWaypoints();

            // Registrar el enemigo en su plataforma
            PlatformDetector pd = platformObject.GetComponentInChildren<PlatformDetector>();
            if (pd != null)
            {
                pd.RegisterEnemy(this);
            }
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                player = p.transform;
                playerRb = player.GetComponent<Rigidbody>();
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            SetState(EnemyState.Chase);
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                if (distanceToPlayer < attackRange)
                {
                    AttackPlayer();
                }
                Chase();
                break;
        }
    }

    void AttackPlayer()
    {
        anim.SetTrigger("Hit");

        if (playerRb != null)
        {
            Vector3 pushDirection = (player.position - transform.position).normalized;

            // Ajustar la dirección del empuje para que el jugador sea empujado hacia fuera de la plataforma
            pushDirection.y = 0.2f; // Pequeño empuje hacia arriba
            pushDirection += transform.forward * 1.5f; // Asegura que el jugador vaya hacia adelante

            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }

    void Patrol()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; 

        float rotationSpeed = 10f; 
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        anim.SetFloat("Walk", 1f);

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    void Chase()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        // Calcular posición siguiente
        Vector3 nextPosition = transform.position + direction * chaseSpeed * Time.deltaTime;

        // Verificar si está dentro del área de movimiento permitido
        if (nextPosition.x < minX || nextPosition.x > maxX || nextPosition.z < minZ || nextPosition.z > maxZ)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        anim.SetFloat("Walk", 2f);

        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.Translate(direction * chaseSpeed * Time.deltaTime, Space.World);
    }

    public void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    void GenerateWaypoints()
    {
        waypoints.Clear();

        Transform planeTransform = platformObject.transform.Find("Plane");
        if (planeTransform != null)
        {
            Renderer rend = planeTransform.GetComponent<Renderer>();
            if (rend != null)
            {
                Bounds platformBounds = rend.bounds;
                Vector3 center = platformBounds.center;
                Vector3 extents = platformBounds.extents;

                Vector3 corner1 = new Vector3(center.x + extents.x - margin, center.y + 1f, center.z + extents.z - margin);
                Vector3 corner2 = new Vector3(center.x - extents.x + margin, center.y + 1f, center.z + extents.z - margin);
                Vector3 corner3 = new Vector3(center.x - extents.x + margin, center.y + 1f, center.z - extents.z + margin);
                Vector3 corner4 = new Vector3(center.x + extents.x - margin, center.y + 1f, center.z - extents.z + margin);

                waypoints.Add(CreateWaypointAt(corner1, "Waypoint1"));
                waypoints.Add(CreateWaypointAt(corner2, "Waypoint2"));
                waypoints.Add(CreateWaypointAt(corner3, "Waypoint3"));
                waypoints.Add(CreateWaypointAt(corner4, "Waypoint4"));

                minX = Mathf.Min(corner1.x, corner2.x, corner3.x, corner4.x);
                maxX = Mathf.Max(corner1.x, corner2.x, corner3.x, corner4.x);
                minZ = Mathf.Min(corner1.z, corner2.z, corner3.z, corner4.z);
                maxZ = Mathf.Max(corner1.z, corner2.z, corner3.z, corner4.z);
            }
        }
    }

    Transform CreateWaypointAt(Vector3 position, string name)
    {
        GameObject wp = new GameObject(name);
        wp.transform.position = position;
        return wp.transform;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Gizmos.color = Color.green;

        foreach (Transform wp in waypoints)
        {
            if (wp != null)
            {
                Gizmos.DrawSphere(wp.position, 0.2f);
            }
        }

        for (int i = 0; i < waypoints.Count; i++)
        {
            Transform current = waypoints[i];
            Transform next = waypoints[(i + 1) % waypoints.Count]; 
            if (current != null && next != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }
}
