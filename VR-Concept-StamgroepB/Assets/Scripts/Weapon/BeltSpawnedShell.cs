using UnityEngine;

public class BeltSpawnedShell : MonoBehaviour
{
    private BeltShellSpawner spawner;
    private bool notified;

    public void Initialize(BeltShellSpawner source)
    {
        spawner = source;
    }

    private void OnDestroy()
    {
        if (notified)
            return;

        notified = true;

        if (spawner != null)
        {
            spawner.NotifyShellDestroyed();
        }
    }
}