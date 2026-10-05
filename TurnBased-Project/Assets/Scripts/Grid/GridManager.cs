using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _length = 10;
    [SerializeField] private float _spacing = 25f;

    [SerializeField] private Dot _dotPrefab;

    [SerializeField] private Transform _dotsContainer;

    private Dot[,] _dots;

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        CreateDots();
    }

    private void CreateDots()
    {
        _dots = new Dot[_length, _length];

        for (int y = 0; y < _length; y++)
        {
            float yPosition = y * _spacing;
            for (int x = 0; x < _length; x++)
            {
                float xPosition = x * _spacing;

                Dot dot = Instantiate(_dotPrefab, _dotsContainer);
                dot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
                dot.Initialize(x, y);

                _dots[x, y] = dot;
            }
        }
    }

}
