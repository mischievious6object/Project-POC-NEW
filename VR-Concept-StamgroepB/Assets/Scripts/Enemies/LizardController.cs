using System;
using UnityEngine;

public class LizardController : MonoBehaviour
{
    public LizardData Data => _data;
    public event Action<int> OnDeath;

    [SerializeField] private LizardData _data;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnDeath += Death;
    }

    private void OnDisable()
    {
        _health.OnDeath -= Death;
    }

    public void Initialize()    
    {
        _health.SetHealth(_data.MaxHealth);
    }

    private void Death()
    {
        _health.OnDeath -= Death;
        OnDeath.Invoke(_data.Score);
    }


}
