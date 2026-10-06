using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class InputHandler : MonoBehaviour
{
    private HighlightManager _highlightManager;
    private GridManager _gridManager;

    private Dot _selectedDot;

    private void Awake()
    {
        _highlightManager = FindFirstObjectByType<HighlightManager>();
        _gridManager = FindFirstObjectByType<GridManager>();
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
        if (_selectedDot == null)
        {
            _selectedDot = dot;
            _highlightManager.HighlightAvailableNeighbors(dot);
        }
    }


    private void FinishSelection(Vector2 releasePosition)
    {
        Dot targetDot = GetDotUnderPointer(releasePosition);

        if (targetDot != null && targetDot != _selectedDot)
        {
            _gridManager.TryToDrawLine(_selectedDot, targetDot);
        }

        _selectedDot = null;
        _highlightManager.ClearHighlights();
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