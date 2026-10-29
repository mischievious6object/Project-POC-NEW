
using UnityEngine;
using TMPro;

public class TVDisplay : MonoBehaviour
{
    public static TVDisplay Instance { get; private set; }

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text scoreAmount;

    [Header("Time")]
    public TMP_Text timeText;
    public TMP_Text timeAmount;

    [Header("Ammo")]
    public TMP_Text ammoText;
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
