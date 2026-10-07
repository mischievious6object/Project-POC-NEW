using UnityEngine;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public event Action<int> OnLineDrawn;

    [SerializeField] private int _length = 10;
    [SerializeField] private float _lineLength = 25f;

    [SerializeField] private Dot _dotPrefab;
    [SerializeField] private Line _linePrefab;
    [SerializeField] private Square _squarePrefab;

    [SerializeField] private Transform _dotsContainer;
    [SerializeField] private Transform _linesContainer;
    [SerializeField] private Transform _squaresContainer;

    private Dot[,] _dots;
    private Line[,] _verticalLines;
    private Line[,] _horizontalLines;
    private Square[,] _squares;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        CreateGrid();
    }

    public void TryToDrawLine(Dot dotA, Dot dotB)
    {
        int deltaX = Mathf.Abs(dotA.XPos - dotB.XPos);
        int deltaY = Mathf.Abs(dotA.YPos - dotB.YPos);

        if (deltaX + deltaY != 1) return;

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
            int drawnSquares = TryToDrawAdjacentSquares(targetLine);
            OnLineDrawn?.Invoke(drawnSquares);
        }
    }

    private int TryToDrawAdjacentSquares(Line line)
    {
        int drawnSquares = 0;

        foreach(Square adjacentSquare in line.AdjacentSquares)
        {
            if (adjacentSquare.IsCompleted)
            {
                adjacentSquare.Draw(_gameManager.CurrentPlayerColor);
                drawnSquares++;
            }
        }
        
        return drawnSquares;
    }


    public void CreateGrid()
    {
        CreateDots();
        CreateLines();
        CreateSquares();
    }

    private void CreateDots()
    {
        _dots = new Dot[_length, _length];

        for (int y = 0; y < _length; y++)
        {
            float yPos = y * _lineLength;
            for (int x = 0; x < _length; x++)
            {
                float xPos = x * _lineLength;
                Vector2 position = new Vector2(xPos, yPos);
                CreateDot(x, y, position);
            }
        }
    }

    private void CreateDot(int x, int y, Vector2 position)
    {
        Dot dot = Instantiate(_dotPrefab, _dotsContainer);
        dot.GetComponent<RectTransform>().anchoredPosition = position;
        dot.Initialize(x, y, _gameManager.InputHandler);

        _dots[x, y] = dot;
    }

    private void CreateLines()
    {
        int linesInRow = _length - 1;
        _verticalLines = new Line[_length, linesInRow];
        _horizontalLines = new Line[linesInRow, _length];

        for (int y = 0; y < _length; y++)
        {
            float verticalYPos = (_lineLength / 2) + (_lineLength * y);
            float horizontalYPos = y * _lineLength;

            for (int x = 0; x < _length; x++)
            {
                if (y != linesInRow)
                {
                    float  verticalXPos = x * _lineLength;
                    Vector2 position = new(verticalXPos, verticalYPos);
                    CreateVerticalLine(x, y, position);
                }

                if (x != linesInRow)
                {
                    float horizontalXPos = (_lineLength / 2) + (_lineLength * x);
                    Vector2 position = new(horizontalXPos, horizontalYPos);
                    CreateHorizontalLine(x, y, position);
                }
            }
        }
    }

    private void CreateVerticalLine(int x, int y, Vector2 position)
    {
        Line line = Instantiate(_linePrefab, _linesContainer);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _lineLength);
        rectTransform.anchoredPosition = position;

        Dot dotA = _dots[x, y];
        dotA.AdjacentLines.Add(line);
        Dot dotB = _dots[x, y + 1];
        dotB.AdjacentLines.Add(line);
        line.Initialize(dotA, dotB);

        _verticalLines[x, y] = line;
    }

    private void CreateHorizontalLine(int x, int y, Vector2 position)
    {
        Line line = Instantiate(_linePrefab, _linesContainer);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(_lineLength, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = position;

        Dot dotA = _dots[x, y];
        dotA.AdjacentLines.Add(line);
        Dot dotB = _dots[x + 1, y];
        dotB.AdjacentLines.Add(line);
        line.Initialize(dotA, dotB);

        _horizontalLines[x, y] = line;
    }

    private void CreateSquares()
    {
        int squaresInRow = _length - 1;
        _squares = new Square[squaresInRow, squaresInRow];

        for (int y = 0; y < squaresInRow; y++)
        {
            float yPos = (y * _lineLength) + (_lineLength / 2);
            for (int x = 0; x < squaresInRow; x++)
            {
                float xPos = (x * _lineLength) + (_lineLength / 2);
                Vector2 position = new Vector2(xPos, yPos);
                CreateSquare(x, y, position);
            }
        }
    }

    private void CreateSquare(int x, int y, Vector2 position)
    {
        Square square = Instantiate(_squarePrefab, _squaresContainer);

        RectTransform rectTransform = square.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(_lineLength, _lineLength);

        List<Line> borders = GetSquareLines(x, y);
        foreach (Line line in borders) line.AdjacentSquares.Add(square);

        square.Initialize(borders);
        _squares[x, y] = square;
    }

    private List<Line> GetSquareLines(int x, int y)
    {
        List<Line> borders = new()
        {
            _horizontalLines[x, y],
            _horizontalLines[x, y + 1],
            _verticalLines[x, y],
            _verticalLines[x + 1, y],
        };

        return borders;
    }

}
