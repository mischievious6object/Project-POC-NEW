using UnityEngine;

public class ShotgunShell : MonoBehaviour
{
    private bool consumed;

    public bool IsConsumed => consumed;

    public void Consume()
    {
        if (consumed)
            return;

        consumed = true;

        Destroy(gameObject);
    }
}