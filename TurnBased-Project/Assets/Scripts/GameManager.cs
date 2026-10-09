using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager GridManager;
    public HighlightManager HighlightManager;
    public InputManager InputHandler;
    public TurnManager TurnManager;
    public ScoreManager ScoreManager;

    public Player FirstPlayer;
    public Player SecondPlayer;

    [SerializeField] private Color _firstPlayerColor;
    [SerializeField] private Color _secondPlayerColor;

    [SerializeField] private EndGamePopup _endGamePopup;

    private void Awake()
    {
        GridManager = FindFirstObjectByType<GridManager>();
        HighlightManager = FindFirstObjectByType<HighlightManager>();
        InputHandler = FindFirstObjectByType<InputManager>();
        TurnManager = FindFirstObjectByType<TurnManager>();
        ScoreManager = FindFirstObjectByType<ScoreManager>();


        GridManager.OnLineDrawn += OnLineDrawn;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        FirstPlayer.Initialize("Player A", _firstPlayerColor);
        SecondPlayer.Initialize("Player B", _secondPlayerColor);

        GridManager.CreateGrid();
        TurnManager.StartTurn(FirstPlayer);
    }

    private void OnLineDrawn(int drawnSquares)
    {
        if (drawnSquares <= 0)
        {
            TurnManager.EndTurn();
        }
        else
        {
            ScoreManager.AddScoreToPlayer(TurnManager.CurrentPlayer, drawnSquares);
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        bool hasEmptySquares = GridManager.HasUndrawnSquares();
        if (hasEmptySquares) return;

        Player winner = ScoreManager.GetWinner(FirstPlayer, SecondPlayer);
        _endGamePopup.Show(winner);
    }


}
