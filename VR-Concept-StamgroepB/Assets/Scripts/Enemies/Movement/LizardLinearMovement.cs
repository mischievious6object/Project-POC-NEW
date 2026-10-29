using System;
using System.Collections.Generic;
using UnityEngine;

public class LizardLinearMovement : MonoBehaviour
{
    public event Action OnMovementFinished;

    private bool _isMoving = false;
    private int _currentIndex = 0;
    private Route _route;
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

    public void StartMoving(Route route)
    {
        _route = route;
        _currentIndex = 0;
        _isMoving = true;
    }

    public void StopMoving()
    {
        if (!_isMoving) return;
        _isMoving = false;
    }

    private void Move()
    {
        Vector3 targetPosition = _route.Waypoints[_currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _controller.Data.Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            _currentIndex++;

            if (_currentIndex >= _route.Waypoints.Count)
            {
                _isMoving = false;
                OnMovementFinished?.Invoke();
            }
        }
    }
}