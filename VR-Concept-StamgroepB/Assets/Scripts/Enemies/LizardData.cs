using UnityEngine;

[CreateAssetMenu(menuName = "Lizard Data")]
public class LizardData : ScriptableObject
{
    public int MaxHealth => _maxHealth;
    public int Score => _score;
    public float Speed => _speed;

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _score;
    [SerializeField] private float _speed;
}
