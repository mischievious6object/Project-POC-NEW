using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DifficultyManager _difficultyManager;
    private SpawnManager _spawnManager;
    private ScoreManager _scoreManager;

    public void StartGame()
    {
        _difficultyManager = FindFirstObjectByType<DifficultyManager>();
        _spawnManager = FindFirstObjectByType<SpawnManager>();
        _scoreManager = FindFirstObjectByType<ScoreManager>();

        _difficultyManager.StartDifficultyTimer();
        _spawnManager.StartSpawn();
        _scoreManager.ResetScore();
    }

    public void EndGame()
    {
        _difficultyManager.ResetDifficulty();
        _spawnManager.ResetSpawner();
    }
}
