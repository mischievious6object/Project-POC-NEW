using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _currentScore;

    private SpawnManager _spawnManager;

    private void Awake()
    {
        _spawnManager = FindFirstObjectByType<SpawnManager>();
    }

    private void Start()
    {
        _spawnManager.OnLizardKilled += LizardKilled;
    }

    private void OnDestroy()
    {
        _spawnManager.OnLizardKilled -= LizardKilled;
    }

    public void ResetScore() => UpdateScore(0);

    private void AddScore(int amount)
    {
        int newScore = _currentScore + amount;
        UpdateScore(newScore);
    }

    private void UpdateScore(int score)
    {
        _currentScore = score;
        _scoreText.text = $"{score}";
        //show the score to the display
        TVDisplay.Instance.scoreAmount.text = score.ToString();
    }

    private void LizardKilled(LizardController lizard) => AddScore(lizard.Data.Score);
}
