using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public event Action OnDifficultyIncreased;

    [SerializeField] private int _killsForDifficultyIncrease = 5;
    [SerializeField] private float _timeForDiffucultyIncrease = 15f;

    private int _killsCounter;

    private Timer _timer;
    private LizardsManager _lizardsManager;

    private void Awake()
    {
        _timer = GetComponent<Timer>();
        _lizardsManager = FindFirstObjectByType<LizardsManager>();
    }

    private void Start()
    {
        _lizardsManager.OnLizardKilled += LizardKilled;
        _timer.OnTimerFinished += TimerFinished;

        _timer.StartTimer(_timeForDiffucultyIncrease);
    }

    private void OnDestroy()
    {
        _lizardsManager.OnLizardKilled -= LizardKilled;
        _timer.OnTimerFinished -= TimerFinished;
    }

    private void LizardKilled(LizardController controller)
    {
        _killsCounter++;
        if(_killsCounter == _killsForDifficultyIncrease)
        {
            _killsCounter--;
            OnDifficultyIncreased?.Invoke();
        }
    }

    private void TimerFinished()
    {
        OnDifficultyIncreased?.Invoke();
        _timer.StartTimer(_timeForDiffucultyIncrease);
    }

}
