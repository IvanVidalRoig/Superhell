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
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public float margin = 0.5f; // Ajusta para controlar cuanto "hacia adentro" van los waypoints

    [Header("Waypoints (auto-generated)")]
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;

    [Header("Player Reference")]
    public Transform player;
    public EnemyState currentState = EnemyState.Patrol;
    private Animator anim;
    public int attackRange;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        Debug.Log("Animator encontrado: " + anim.gameObject.name);

        if (platformObject != null)
        {
            GenerateWaypoints();

            // Aquí registramos el enemigo en su plataforma
            PlatformDetector pd = platformObject.GetComponentInChildren<PlatformDetector>();
            if (pd != null)
            {
                pd.RegisterEnemy(this);
            }
            else
            {
                Debug.LogWarning("La plataforma no tiene PlatformDetector asignado");
            }
        }
        else
        {
            Debug.LogError("No se asignó la plataforma al EnemyAI");
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                if (Vector3.Distance(transform.position, player.position) < attackRange)
                    AttackPlayer();
                Chase();
                break;
        }
    }

    void AttackPlayer()
    {
        anim.SetTrigger("Hit");
    }

    void Patrol()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        // Dirección hacia el waypoint
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Asegurarse de que no cambie la inclinación

        // Rotación suave hacia el objetivo
        float rotationSpeed = 5f; // Ajusta según la rapidez de giro que desees
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Movimiento hacia el waypoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        anim.SetFloat("Walk", 1f);
        var aux = anim.GetFloat("Walk");

        // Cambio de waypoint cuando llega
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    void Chase()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Mantener el enemigo erguido sin inclinarlo
        anim.SetFloat("Walk", 1f);

        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position += direction * chaseSpeed * Time.deltaTime;
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
                // Ahora tienes el Renderer específico del objeto "plane"
                Bounds platformBounds = rend.bounds;
                Vector3 center = platformBounds.center;
                Vector3 extents = platformBounds.extents;

                // Añadimos el margen para que no estén en el borde exacto
                Vector3 corner1 = new Vector3(center.x + extents.x - margin, center.y, center.z + extents.z - margin);
                Vector3 corner2 = new Vector3(center.x - extents.x + margin, center.y, center.z + extents.z - margin);
                Vector3 corner3 = new Vector3(center.x - extents.x + margin, center.y, center.z - extents.z + margin);
                Vector3 corner4 = new Vector3(center.x + extents.x - margin, center.y, center.z - extents.z + margin);

                waypoints.Add(CreateWaypointAt(corner1, "Waypoint1"));
                waypoints.Add(CreateWaypointAt(corner2, "Waypoint2"));
                waypoints.Add(CreateWaypointAt(corner3, "Waypoint3"));
                waypoints.Add(CreateWaypointAt(corner4, "Waypoint4"));
            }
            else
            {
                Debug.LogError("El objeto 'plane' no tiene un Renderer.");
            }
        }
        else
        {
            Debug.LogError("No se encontró un objeto hijo llamado 'plane' en la plataforma.");
        }
    }

    Transform CreateWaypointAt(Vector3 position, string name)
    {
        GameObject wp = new GameObject(name);
        wp.transform.position = position;
    //    wp.transform.SetParent(this.transform);
        return wp.transform;
    }

    //void OnValidate()
    //{
    //    if (platformObject != null)
    //    {
    //        GenerateWaypoints(); // Este método generará los waypoints en modo edición
    //    }
    //}


    void OnDrawGizmos()
    {
        // Si no tienes waypoints aún, o si el juego no está corriendo, sal
        if (waypoints == null || waypoints.Count == 0)
            return;

        // Elegir un color para los gizmos
        Gizmos.color = Color.green;

        // Dibujar una pequeña esfera en cada waypoint
        foreach (Transform wp in waypoints)
        {
            if (wp != null)
            {
                Gizmos.DrawSphere(wp.position, 0.2f);
            }
        }

        // Opcional: Dibujar líneas entre los waypoints para ver el recorrido
        for (int i = 0; i < waypoints.Count; i++)
        {
            Transform current = waypoints[i];
            Transform next = waypoints[(i + 1) % waypoints.Count]; // Conexión circular
            if (current != null && next != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }

}
