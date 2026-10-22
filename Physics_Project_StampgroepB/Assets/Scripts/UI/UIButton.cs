using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _defaultText;
    [SerializeField] private ButtonColors _colors;

    public void SelectButton()
    {
        _background.color = _colors.SelectBackgroundColor;
        _text.color = _colors.SelectTextColor;
        _text.text = $"> {_defaultText}";
    }

    public void UnselectButton()
    {
        _background.color = _colors.DefaultBackgroundColor;
        _text.color = _colors.DefaultTextColor;
        _text.text = $"  {_defaultText}";
    }

}
