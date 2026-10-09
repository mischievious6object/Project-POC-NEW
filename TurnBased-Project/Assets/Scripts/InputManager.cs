using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    private Dot _selectedDot;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Update()
    {
        if (_selectedDot == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            FinishSelection(Mouse.current.position.ReadValue());
        }
    }

    public void OnDotPressed(Dot dot)
    {
        _selectedDot = null;
        _gameManager.HighlightManager.ClearHighlights();

        _selectedDot = dot;
        _gameManager.HighlightManager.HighlightAvailableNeighbors(dot);
    }


    private void FinishSelection(Vector2 releasePosition)
    {
        Dot targetDot = GetDotUnderPointer(releasePosition);

        if (targetDot != null && targetDot != _selectedDot)
        {
            _gameManager.GridManager.TryToDrawLine(_selectedDot, targetDot);
        }

        _selectedDot = null;
        _gameManager.HighlightManager.ClearHighlights();
    }

    private Dot GetDotUnderPointer(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<Dot>(out Dot dot))
            {
                return dot;
            }
        }

        return null;
    }

}