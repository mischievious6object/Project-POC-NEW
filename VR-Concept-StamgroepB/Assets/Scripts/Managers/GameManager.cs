using UnityEngine;

public class GameManager : MonoBehaviour
{

    private DifficultyManager _difficultyManager;
    private SpawnManager _spawnManager;

    private void Awake()
    {
        _difficultyManager = FindFirstObjectByType<DifficultyManager>();
        _spawnManager = FindFirstObjectByType<SpawnManager>();
    }

    public void StartGame()
    {
        _difficultyManager.StartDifficultyTimer();
        _spawnManager.StartSpawn();
    }

    public void EndGame()
    {
        _difficultyManager.ResetDifficulty();
        _spawnManager.ResetSpawner();
    }
}
