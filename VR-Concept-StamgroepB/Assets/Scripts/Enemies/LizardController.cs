using System;
using UnityEngine;

public class LizardController : MonoBehaviour
{
    public bool IsAlive => _health.IsAlive;
    public LizardData Data => _data;
    public event Action<int, LizardController> OnDeath;
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
        _linearMovement.OnDestinationReached += NotifyMovementFinished;
        _peekingMovement.OnMovementFinished += NotifyMovementFinished;
    }

    private void OnDisable()
    {
        _health.OnDeath -= NotifyDied;
        _linearMovement.OnDestinationReached -= NotifyMovementFinished;
        _peekingMovement.OnMovementFinished -= NotifyMovementFinished;
    }

    public void Initialize()    
    {
        _health.SetHealth(_data.MaxHealth);
    }

    private void NotifyMovementFinished() => OnMovementFinished?.Invoke(this);
    private void NotifyDied()
    {
        _linearMovement.StopMoving();
        _peekingMovement.StopPeeking();
        OnDeath?.Invoke(_data.Score, this);
    }

}
