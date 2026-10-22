using UnityEngine;

public class SettingController : MonoBehaviour
{

    public void ShowSettings() => gameObject.SetActive(true);
    public void HideSettings()
    {
        gameObject.SetActive(false);
    }

}
