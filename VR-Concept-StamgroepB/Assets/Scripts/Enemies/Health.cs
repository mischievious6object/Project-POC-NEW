using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action OnDeath;

    private int _health;

    public void SetHealth(int health)
    {
        _health = health;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

}
