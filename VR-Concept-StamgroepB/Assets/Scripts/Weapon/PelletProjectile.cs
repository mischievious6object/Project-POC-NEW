using UnityEngine;

public class PelletProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 7f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Pellet hit: " + collision.gameObject.name);

        Destroy(gameObject);
    }
}