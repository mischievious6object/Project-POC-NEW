using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public event Action OnTimerFinished;

    private float _currentTime;
    private bool _isRunning = false;

    private void Update()
    {
        if (!_isRunning) return;
        UpdateTimer();
    }

    public void StartTimer(float duration)
    {
        _currentTime = duration;
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    private void UpdateTimer()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            _isRunning = false;

            OnTimerFinished?.Invoke();
        }
    }

}
