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
    [SerializeField] private float spreadAngle = 2.5f;
    [SerializeField] private float fireCooldown = 0.35f;

    [Header("State")]
    [SerializeField] private bool chambered = true;

    [Header("Ignore Collisions With")]
    [SerializeField] private Collider[] ignoredColliders;

    private float nextFireTime;

    public bool NeedsPump => !chambered;

    public void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        if (!chambered)
        {
            Debug.Log("Shotgun needs to be pumped.");
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        chambered = false;

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
        float pitch = Random.Range(-spreadAngle, spreadAngle);
        float yaw = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation =
            muzzle.rotation *
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 spawnPosition =
            muzzle.position + muzzle.forward * 0.12f;

        GameObject pellet = Instantiate(
            pelletPrefab,
            spawnPosition,
            spreadRotation
        );

        Rigidbody rb = pellet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity =
                spreadRotation * Vector3.forward * pelletSpeed;
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

    public void CompletePumpCycle()
    {
        if (chambered)
            return;

        chambered = true;

        Debug.Log("Shotgun chambered.");
    }
}