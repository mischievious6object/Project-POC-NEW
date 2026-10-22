using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private UIButton _openButton;

    public void Open()
    {
        _openButton.SelectButton();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _openButton.UnselectButton();
        gameObject.SetActive(false);
    }

}
