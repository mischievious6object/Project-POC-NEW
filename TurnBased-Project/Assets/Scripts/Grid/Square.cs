using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public bool IsDrawn { get; private set; }
    public List<Line> Borders { get; private set; } = new();

    public bool IsCompleted => !IsDrawn && Borders.All(b => b.IsDrawn);

    [SerializeField] private Image _body;
    
    public void Initialize(List<Line> borders)
    {
        Borders = borders;
        IsDrawn = false;
        gameObject.SetActive(false);
    }

    public void Draw(Color squareColor)
    {
        IsDrawn = true;
        _body.color = squareColor;
        gameObject.SetActive(true);
    }

}
