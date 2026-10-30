using UnityEngine;

public class ShotgunReloadPort : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shotgun shotgun;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shellInsertSound;

    private void OnTriggerEnter(Collider other)
    {
        ShotgunShell shell =
            other.GetComponentInParent<ShotgunShell>();

        if (shell == null)
            return;

        if (shell.IsConsumed)
            return;

        if (!shotgun.CanLoadShell())
            return;

        if (shotgun.LoadShell())
        {
            if (audioSource != null && shellInsertSound != null)
            {
                audioSource.PlayOneShot(shellInsertSound);
            }

            shell.Consume();
        }
    }
}