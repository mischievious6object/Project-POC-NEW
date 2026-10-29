using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public event Action OnDifficultyIncreased;

    [SerializeField] private int _killsForDifficultyIncrease = 5;
    [SerializeField] private float _timeForDiffucultyIncrease = 15f;

    private int _killsCounter;

    private Timer _timer;
    private SpawnManager _lizardsManager;

    private void Awake()
    {
        _timer = GetComponent<Timer>();
        _lizardsManager = FindFirstObjectByType<SpawnManager>();
    }

    private void Start()
    {
        _lizardsManager.OnLizardKilled += LizardKilled;
        _timer.OnTimerFinished += TimerFinished;
    }

    private void OnDestroy()
    {
        _lizardsManager.OnLizardKilled -= LizardKilled;
        _timer.OnTimerFinished -= TimerFinished;
    }

    public void StartDifficultyTimer()
    {
        _timer.StartTimer(_timeForDiffucultyIncrease);
    }

    public void ResetDifficulty()
    {
        _timer.StopTimer();
        _killsCounter = 0;
    }

    private void LizardKilled(LizardController controller)
    {
        _killsCounter++;
        if(_killsCounter == _killsForDifficultyIncrease)
        {
            _killsCounter = 0;
            OnDifficultyIncreased?.Invoke();
        }
    }

    private void TimerFinished()
    {
        OnDifficultyIncreased?.Invoke();
        _timer.StartTimer(_timeForDiffucultyIncrease);
    }

}
