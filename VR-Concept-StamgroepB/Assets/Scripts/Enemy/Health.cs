using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action OnDeath;

    public bool IsAlive => _health > 0;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    private int _health;
    private bool isDead;

    public void SetHealth(int health)
    {
        _health = health;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (deathSound != null)
        {
            float volume = audioSource != null
                ? audioSource.volume
                : 1f;

            AudioSource.PlayClipAtPoint(
                deathSound,
                transform.position,
                volume
            );
        }

        OnDeath?.Invoke();
    }
}