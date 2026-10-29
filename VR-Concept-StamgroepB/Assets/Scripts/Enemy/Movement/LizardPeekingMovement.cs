using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class LizardPeekingMovement : MonoBehaviour
{
    public event Action OnMovementFinished;

    private Coroutine _peekingCoroutine;
    private LizardController _controller;

    private void Awake()
    {
        _controller = GetComponent<LizardController>();
    }

    public void StartMoving(Route route)
    {
        _peekingCoroutine = StartCoroutine(PeekingRoutine(route));
    }

    public void StopPeeking()
    {
        if (_peekingCoroutine == null) return;
        StopCoroutine(_peekingCoroutine);
    }

    private IEnumerator PeekingRoutine(Route route)
    {
        int peekTimes = Random.Range(_controller.Data.MinPeeks, _controller.Data.MaxPeeks + 1);
        while (peekTimes > 0)
        {
            Vector3 targetPosition = GetPositionToPeek(route.Waypoints);
            yield return Wait();
            yield return MoveToPosition(targetPosition);
            yield return Wait();
            yield return MoveToPosition(route.StartPosition);

            peekTimes--;
        }

        OnMovementFinished?.Invoke();
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _controller.Data.PeekingSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private Vector3 GetPositionToPeek(List<Vector3> peekPositions)
    {
        int index = Random.Range(0, peekPositions.Count);
        return peekPositions[index];
    }

    private IEnumerator Wait()
    {
        float waitTime = Random.Range(_controller.Data.MinWaitTime, _controller.Data.MaxWaitTime);
        float timer = 0f;
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

}
