using UnityEngine;

[CreateAssetMenu(fileName = "ButtonColors", menuName = "Scriptable Objects/ButtonColors")]
public class ButtonColors : ScriptableObject
{
    [Header("Default Colors")]
    public Color DefaultTextColor;
    public Color DefaultBackgroundColor;
    [Header("Select Colors")]
    public Color SelectTextColor;
    public Color SelectBackgroundColor;
}
