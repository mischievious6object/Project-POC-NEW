using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HighlightManager : MonoBehaviour
{
    private List<Dot> _highlightedDots = new();

    public void HighlightAvailableNeighbors(Dot startDot)
    {
        ClearHighlights();

        foreach (Line line in startDot.AdjacentLines.Where(l => !l.IsDrawn))
        {
            Dot targetDot = line.GetOtherDot(startDot);

            targetDot.SetHighlight(true);
            _highlightedDots.Add(targetDot);
        }
    }

    public void ClearHighlights()
    {
        foreach (Dot dot in _highlightedDots)
        {
            dot.SetHighlight(false);
        }

        _highlightedDots.Clear();
    }

}
