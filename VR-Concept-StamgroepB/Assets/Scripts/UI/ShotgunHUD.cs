
using UnityEngine;
using TMPro;

public class ShotgunHUD : MonoBehaviour
{
    public static ShotgunHUD Instance { get; private set; }

    public TMP_Text ammoAmount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
