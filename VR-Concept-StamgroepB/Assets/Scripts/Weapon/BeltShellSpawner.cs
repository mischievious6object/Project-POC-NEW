using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BeltShellSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Shell Limit")]
    [SerializeField] private int maxActiveShells = 6;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 0.15f;

    private GameObject currentBeltShell;
    private int activeShellCount;

    private void Start()
    {
        SpawnShell();
    }

    private void SpawnShell()
    {
        if (shellPrefab == null)
        {
            Debug.LogError("BeltShellSpawner: No shell prefab assigned.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("BeltShellSpawner: No spawn point assigned.");
            return;
        }

        if (currentBeltShell != null)
            return;

        if (activeShellCount >= maxActiveShells)
            return;

        // Spawn as a CHILD of the belt spawn point.
        currentBeltShell = Instantiate(
            shellPrefab,
            spawnPoint
        );

        currentBeltShell.transform.localPosition = Vector3.zero;
        currentBeltShell.transform.localRotation = Quaternion.identity;

        activeShellCount++;

        // Freeze it onto the belt until grabbed.
        Rigidbody rb =
            currentBeltShell.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        XRGrabInteractable grab =
            currentBeltShell.GetComponent<XRGrabInteractable>();

        if (grab == null)
        {
            Debug.LogError(
                "BeltShellSpawner: Shell prefab has no XRGrabInteractable."
            );

            return;
        }

        grab.selectEntered.AddListener(OnShellGrabbed);

        BeltSpawnedShell tracker =
            currentBeltShell.GetComponent<BeltSpawnedShell>();

        if (tracker == null)
        {
            tracker =
                currentBeltShell.AddComponent<BeltSpawnedShell>();
        }

        tracker.Initialize(this);

        Debug.Log(
            "Belt shell spawned. Active shells: " +
            activeShellCount +
            "/" +
            maxActiveShells
        );
    }

    private void OnShellGrabbed(SelectEnterEventArgs args)
    {
        GameObject grabbedShell =
            args.interactableObject.transform.gameObject;

        XRGrabInteractable grab =
            grabbedShell.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnShellGrabbed);
        }

        // Detach it from the player's belt.
        grabbedShell.transform.SetParent(null, true);

        // Give physics back to the shell.
        Rigidbody rb =
            grabbedShell.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        currentBeltShell = null;

        CancelInvoke(nameof(SpawnShell));

        Invoke(
            nameof(SpawnShell),
            respawnDelay
        );
    }

    public void NotifyShellDestroyed()
    {
        activeShellCount--;

        if (activeShellCount < 0)
            activeShellCount = 0;

        if (currentBeltShell == null)
        {
            CancelInvoke(nameof(SpawnShell));

            Invoke(
                nameof(SpawnShell),
                respawnDelay
            );
        }
    }
}