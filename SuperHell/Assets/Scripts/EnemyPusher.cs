using UnityEngine;

public class EnemyPusher : MonoBehaviour
{
    public float pushForce = 500f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
			GameManager.instance.PerderVida();
			Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 direction = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(direction * pushForce);
            }
        }
    }
}
