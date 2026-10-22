using UnityEngine;
using UnityEngine.UI;

public class GrabHintUI : MonoBehaviour
{

    [SerializeField] private Image _crosshair;

    [SerializeField] private Color _defaultCrosshairColor;
    [SerializeField] private Color _hintCrosshairColor;

    public void Show()
    {
        _crosshair.color = _hintCrosshairColor;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            _crosshair.color = _defaultCrosshairColor;
            gameObject.SetActive(false);
        }
    }

}
