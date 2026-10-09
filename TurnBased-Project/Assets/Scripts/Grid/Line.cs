using UnityEngine;
using System.Collections.Generic;

public class Line : MonoBehaviour
{
    public bool IsDrawn { get; private set; }
    public Dot DotA { get; private set; }
    public Dot DotB { get; private set; }
    public List<Square> AdjacentSquares { get; private set; } = new();

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
