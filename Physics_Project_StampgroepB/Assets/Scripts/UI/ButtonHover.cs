using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Background")]
    [SerializeField] private Image _background;
    [SerializeField] private Color _defaultBackgroundColor;
    [SerializeField] private Color _hoverBackgroundColor;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _defaultText;
    [SerializeField] private Color _defaultTextColor;
    [SerializeField] private Color _hoverTextColor;


    private void Start()
    {
        ResetButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _background.color = _hoverBackgroundColor;
        _text.color = _hoverTextColor;
        _text.text = $"> {_defaultText}";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    private void OnEnable()
    {
        ResetButton();
    }

    private void ResetButton()
    {
        _background.color = _defaultBackgroundColor;
        _text.color = _defaultTextColor;
        SetDefaultText();
    }

    private void SetDefaultText() => _text.text = $"  {_defaultText}";
}
