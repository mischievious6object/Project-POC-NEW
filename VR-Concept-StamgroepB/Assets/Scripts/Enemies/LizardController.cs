using System;
using UnityEngine;

public class LizardController : MonoBehaviour
{
    public bool IsAlive => _health.IsAlive;
    public LizardData Data => _data;
    public event Action<int, LizardController> OnDeath;
    public event Action<LizardController> OnMovementFinished;

    [SerializeField] private LizardData _data;

    private LizardLinearMovement _linearMovement;
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _linearMovement = GetComponent<LizardLinearMovement>();
    }

    private void OnEnable()
    {
        _health.OnDeath += NotifyDied;
        _linearMovement.OnDestinationReached += NotifyMovementFinished;
    }

    private void OnDisable()
    {
        _health.OnDeath -= NotifyDied;
        _linearMovement.OnDestinationReached -= NotifyMovementFinished;
    }

    public void Initialize()    
    {
        _health.SetHealth(_data.MaxHealth);
    }

    private void NotifyDied() => OnDeath?.Invoke(_data.Score, this);
    private void NotifyMovementFinished() => OnMovementFinished?.Invoke(this);

}
