using UnityEngine;

public class PelletProjectile : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 7f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with other pellets
        if (collision.gameObject.CompareTag("Pellet"))
            return;

        Debug.Log("Pellet hit: " + collision.gameObject.name);

        Destroy(gameObject);
    }
}