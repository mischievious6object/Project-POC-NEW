using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Lizard Data")]
public class LizardData : ScriptableObject
{
    public int MaxHealth => _maxHealth;
    public int Score => _score;
    public float Speed => _speed;
    public LizardController Prefab => _prefab;

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _score;
    [SerializeField] private float _speed;
    [SerializeField] private LizardController _prefab;
}
