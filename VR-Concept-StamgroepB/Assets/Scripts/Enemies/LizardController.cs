using System;
using UnityEngine;

public class LizardController : MonoBehaviour
{
    public event Action<int> OnDeath;

    [SerializeField] private LizardData _data;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDeath += Death;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _health.SetHealth(_data.MaxHealth);
    }

    private void Death()
    {
        _health.OnDeath -= Death;
        Destroy(gameObject);
        OnDeath.Invoke(_data.Score);
    }


}
