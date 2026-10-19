using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Player CurrentPlayer => _currentPlayer;

    [SerializeField] private TextMeshProUGUI _currentPlayerText;

    private Player _currentPlayer;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    public void EndTurn()
    {
        if (_gameManager.FirstPlayer == _currentPlayer)
        {
            StartTurn(_gameManager.SecondPlayer);
        }
        else
        {
            StartTurn(_gameManager.FirstPlayer);
        }
    }

    public void StartTurn(Player player)
    {
        _currentPlayer = player;
        _currentPlayerText.text = player.Name;
        _currentPlayerText.color = player.Color;
    }

}
