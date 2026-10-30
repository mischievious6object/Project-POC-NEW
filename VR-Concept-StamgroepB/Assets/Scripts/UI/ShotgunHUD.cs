using TMPro;
using UnityEngine;

public class ShotgunHUD : MonoBehaviour
{
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private TMP_Text ammoAmount;

    private void Update()
    {
        if (shotgun == null || ammoAmount == null)
            return;

        ammoAmount.text = shotgun.CurrentAmmo.ToString();
    }
}