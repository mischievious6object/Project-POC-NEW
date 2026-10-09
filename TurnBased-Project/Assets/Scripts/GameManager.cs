using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager GridManager { get; private set; }
    public HighlightManager HighlightManager { get; private set; }
    public InputManager InputHandler { get; private set; }
    public TurnManager TurnManager { get; private set; }
    public ScoreManager ScoreManager { get; private set; }

    public Player FirstPlayer;
    public Player SecondPlayer;

    [SerializeField] private Player _firstPlayer;
    [SerializeField] private Player _secondPlayer;

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
        _firstPlayer.Initialize("Player A", _firstPlayerColor);
        _secondPlayer.Initialize("Player B", _secondPlayerColor);

        GridManager.CreateGrid();
        TurnManager.StartTurn(_firstPlayer);
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

        Player winner = ScoreManager.GetWinner(_firstPlayer, _secondPlayer);
        _endGamePopup.Show(winner);
    }


}
