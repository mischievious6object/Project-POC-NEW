using UnityEngine;

public class PelletProjectile : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 7f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore other pellets
        if (collision.gameObject.CompareTag("Pellet"))
            return;

        // Look for Health on the object we hit OR one of its parents.
        Health health = collision.gameObject.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);

            Debug.Log(
                "Pellet hit " +
                health.gameObject.name +
                " for " +
                damage +
                " damage."
            );
        }
        else
        {
            Debug.Log(
                "Pellet hit: " +
                collision.gameObject.name
            );
        }

        Destroy(gameObject);
    }
}