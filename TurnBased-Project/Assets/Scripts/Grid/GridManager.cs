using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private int _length = 10;
    [SerializeField] private float _spacing = 25f;

    [SerializeField] private Dot _dotPrefab;
    [SerializeField] private Line _linePrefab;

    [SerializeField] private Transform _dotsContainer;
    [SerializeField] private Transform _linesContainer;

    private Dot[,] _dots;
    private Line[,] _verticalLines;
    private Line[,] _horizontalLines;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        CreateGrid();
    }

    public bool TryToDrawLine(Dot dotA, Dot dotB)
    {
        int deltaX = Mathf.Abs(dotA.XPos - dotB.XPos);
        int deltaY = Mathf.Abs(dotA.YPos - dotB.YPos);

        if (deltaX + deltaY != 1)
        {
            return false;
        }

        Line targetLine;
        if(deltaX == 0)
        {
            int yPos = Mathf.Min(dotA.YPos, dotB.YPos);
            targetLine = _verticalLines[dotA.XPos, yPos];
        }
        else
        {
            int xPos = Mathf.Min(dotA.XPos, dotB.XPos);
            targetLine = _horizontalLines[xPos, dotA.YPos];
        }

        if (!targetLine.IsDrawn)
        {
            targetLine.Draw();
            return true;
        }

        return false;
    }

    public void CreateGrid()
    {
        CreateDots();
        CreateLines();
    }

    private void CreateDots()
    {
        _dots = new Dot[_length, _length];

        for (int y = 0; y < _length; y++)
        {
            float yPos = y * _spacing;
            for (int x = 0; x < _length; x++)
            {
                float xPos = x * _spacing;

                Dot dot = Instantiate(_dotPrefab, _dotsContainer);
                dot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
                dot.Initialize(x, y, _gameManager.InputHandler);

                _dots[x, y] = dot;
            }
        }
    }

    private void CreateLines()
    {
        int linesInRow = _length - 1;
        _verticalLines = new Line[_length, linesInRow];
        _horizontalLines = new Line[linesInRow, _length];

        for (int y = 0; y < _length; y++)
        {
            float verticalYPos = (_spacing / 2) + (_spacing * y);
            float horizontalYPos = y * _spacing;

            for (int x = 0; x < _length; x++)
            {
                if (y != linesInRow)
                {
                    float  verticalXPos = x * _spacing;
                    Vector2 linePos = new(verticalXPos, verticalYPos);
                    CreateVerticalLine(x, y, linePos);
                }

                if (x != linesInRow)
                {
                    float horizontalXPos = (_spacing / 2) + (_spacing * x);
                    Vector2 linePos = new(horizontalXPos, horizontalYPos);
                    CreateHorizontalLine(x, y, linePos);
                }
            }
        }
    }

    private void CreateVerticalLine(int x, int y, Vector2 linePos)
    {
        Line line = Instantiate(_linePrefab, _linesContainer);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _spacing);
        rectTransform.anchoredPosition = linePos;

        Dot dotA = _dots[x, y];
        dotA.AdjacentLines.Add(line);
        Dot dotB = _dots[x, y + 1];
        dotB.AdjacentLines.Add(line);
        line.Initialize(dotA, dotB);

        _verticalLines[x, y] = line;
    }

    private void CreateHorizontalLine(int x, int y, Vector2 linePos)
    {
        Line line = Instantiate(_linePrefab, _linesContainer);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(_spacing, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = linePos;

        Dot dotA = _dots[x, y];
        dotA.AdjacentLines.Add(line);
        Dot dotB = _dots[x + 1, y];
        dotB.AdjacentLines.Add(line);
        line.Initialize(dotA, dotB);

        _horizontalLines[x, y] = line;
    }

    

}
