using UnityEngine;

public class ShotgunReloadPort : MonoBehaviour
{
    [SerializeField] private Shotgun shotgun;

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
            shell.Consume();
        }
    }
}