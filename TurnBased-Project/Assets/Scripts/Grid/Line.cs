using UnityEngine;

public class Line : MonoBehaviour
{
    public bool IsDrawn = false;
    public Dot DotA;
    public Dot DotB;

    public void Initialize(Dot dotA, Dot dotB)
    {
        DotA = dotA;
        DotB = dotB;

        IsDrawn = false;
        this.gameObject.SetActive(false);
    }
}
