using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager GridManager;
    public HighlightManager HighlightManager;
    public InputManager InputHandler;
    private void Awake()
    {
        GridManager = FindFirstObjectByType<GridManager>();
        HighlightManager = FindFirstObjectByType<HighlightManager>();
        InputHandler = FindFirstObjectByType<InputManager>();
    }
}
