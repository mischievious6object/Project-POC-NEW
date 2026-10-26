using UnityEngine;
using System;
using System.Collections.Generic;

public class LizardLinearMovement : MonoBehaviour
{
    public event Action OnDestinationReached;

    private bool _isMoving = false;
    private int _currentIndex = 0;
    private List<Vector3> _waypoints = new();
    private LizardController _controller;


    private void Awake()
    {
        _controller = GetComponent<LizardController>();
    }

    private void Update()
    {
        if (!_isMoving || !_controller.IsAlive) return;
        Move();
    }

    public void StartMoving(List<Vector3> waypoints)
    {
        _waypoints = new(waypoints);
        _currentIndex = 0;
        _isMoving = true;
    }

    private void Move()
    {
        Vector3 targetPosition = _waypoints[_currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _controller.Data.Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            _currentIndex++;

            if (_currentIndex >= _waypoints.Count)
            {
                _isMoving = false;
                OnDestinationReached?.Invoke();
            }
        }
    }
}