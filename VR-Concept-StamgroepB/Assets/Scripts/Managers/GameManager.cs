using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DifficultyManager _difficultyManager;
    private SpawnManager _spawnManager;
    private ScoreManager _scoreManager;

    private bool _gameRunning;

    public void StartGame()
    {
        if (_gameRunning)
            return;

        _gameRunning = true;

        _difficultyManager = FindFirstObjectByType<DifficultyManager>();
        _spawnManager = FindFirstObjectByType<SpawnManager>();
        _scoreManager = FindFirstObjectByType<ScoreManager>();

        _difficultyManager.StartDifficultyTimer();
        _spawnManager.StartSpawn();
        _scoreManager.ResetScore();
    }

    public void EndGame()
    {
        if (!_gameRunning)
            return;

        _gameRunning = false;

        _difficultyManager.ResetDifficulty();
        _spawnManager.ResetSpawner();
    }
}