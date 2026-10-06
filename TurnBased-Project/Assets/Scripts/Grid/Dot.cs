using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Dot : MonoBehaviour, IPointerDownHandler
{
    public int XPos;
    public int YPos;
    public List<Line> AdjacentLines = new();

    [SerializeField] private GameObject _highlight;

    private InputManager _inputHandler;
    
    public void Initialize(int x, int y, InputManager inputHandler)
    {
        XPos = x; 
        YPos = y;
        _inputHandler = inputHandler;
    }

    public void SetHighlight(bool value)
    {
        _highlight.SetActive(value);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _inputHandler.OnDotPressed(this);
    }

}
