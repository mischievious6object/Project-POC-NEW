using UnityEngine;

public class Line : MonoBehaviour
{
    public bool IsDrawn = false;
    public Dot DotA;
    public Dot DotB;

    public Dot GetOtherDot(Dot current) => current == DotA ? DotB : DotA;

    public void Initialize(Dot dotA, Dot dotB)
    {
        DotA = dotA;
        DotB = dotB;

        IsDrawn = false;
        gameObject.SetActive(false);
    }

    public void Draw()
    {
        IsDrawn = true;
        gameObject.SetActive(true);
    }
}
