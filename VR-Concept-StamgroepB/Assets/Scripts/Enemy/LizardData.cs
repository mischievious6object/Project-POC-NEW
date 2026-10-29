using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Lizard Data")]
public class LizardData : ScriptableObject
{
    public int MaxHealth => _maxHealth;
    public int Score => _score;
    public float Speed => _speed;
    public LizardController Prefab => _prefab;

    public float PeekingSpeed => _peekingSpeed;
    public int MinPeeks => _minPeeksTimes;
    public int MaxPeeks => _maxPeeksTimes;
    public float MinWaitTime => _minWaitTime;
    public float MaxWaitTime => _maxWaitTime;

    [Header("General stats")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _score;
    [SerializeField] private float _speed;
    [SerializeField] private LizardController _prefab;

    [Header("Peeking movement")]
    [SerializeField] private float _peekingSpeed;
    [SerializeField] private int _minPeeksTimes;
    [SerializeField] private int _maxPeeksTimes;
    [SerializeField] private float _minWaitTime;
    [SerializeField] private float _maxWaitTime;

}
