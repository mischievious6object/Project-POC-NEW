using System;
using UnityEngine;

public class LizardController : MonoBehaviour
{
    public bool IsAlive => _health.IsAlive;
    public LizardData Data => _data;
    public event Action<LizardController> OnDeath;
    public event Action<LizardController> OnMovementFinished;

    [SerializeField] private LizardData _data;

    private Health _health;
    private LizardLinearMovement _linearMovement;
    private LizardPeekingMovement _peekingMovement;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _linearMovement = GetComponent<LizardLinearMovement>();
        _peekingMovement = GetComponent<LizardPeekingMovement>();
    }

    private void OnEnable()
    {
        _health.OnDeath += NotifyDied;
        _linearMovement.OnMovementFinished += NotifyMovementFinished;
        _peekingMovement.OnMovementFinished += NotifyMovementFinished;
    }

    private void OnDisable()
    {
        _health.OnDeath -= NotifyDied;
        _linearMovement.OnMovementFinished -= NotifyMovementFinished;
        _peekingMovement.OnMovementFinished -= NotifyMovementFinished;
    }

    public void Initialize()    
    {
        _health.SetHealth(_data.MaxHealth);
    }

    public void StartMoving(LizardMovementType type, Route route)
    {
        transform.position = route.StartPosition;
        switch (type)
        {
            case LizardMovementType.Linear:
                _linearMovement.StartMoving(route);
                break;
            case LizardMovementType.Peeking:
                _peekingMovement.StartMoving(route);
                break;
        }
    }

    private void NotifyMovementFinished() => OnMovementFinished?.Invoke(this);
    private void NotifyDied()
    {
        _linearMovement.StopMoving();
        _peekingMovement.StopPeeking();
        OnDeath?.Invoke(this);
    }

}
