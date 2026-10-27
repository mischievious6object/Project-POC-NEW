using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;

    [Header("Shotgun")]
    [SerializeField] private int pelletsPerShot = 8;
    [SerializeField] private float pelletSpeed = 35f;
    [SerializeField] private float spreadAngle = 6f;
    [SerializeField] private float fireCooldown = 0.6f;

    [Header("Ignore Collisions With")]
    [SerializeField] private Collider[] ignoredColliders;

    private float nextFireTime;

    public void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireCooldown;

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        for (int i = 0; i < pelletsPerShot; i++)
        {
            FirePellet();
        }
    }

    private void FirePellet()
    {
        Quaternion spreadRotation =
            muzzle.rotation *
            Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            );

        // Spawn slightly in front of the muzzle
        Vector3 spawnPosition = muzzle.position + muzzle.forward * 0.08f;

        GameObject pellet = Instantiate(
            pelletPrefab,
            spawnPosition,
            spreadRotation
        );

        Rigidbody rb = pellet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = spreadRotation * Vector3.forward * pelletSpeed;
        }

        Collider pelletCollider = pellet.GetComponent<Collider>();

        if (pelletCollider != null)
        {
            foreach (Collider ignored in ignoredColliders)
            {
                if (ignored != null)
                {
                    Physics.IgnoreCollision(
                        pelletCollider,
                        ignored,
                        true
                    );
                }
            }
        }
    }
}