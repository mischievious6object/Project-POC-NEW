using UnityEngine;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

public class LizardsManager : MonoBehaviour
{
    public List<LizardData> LizardTypes => _lizardTypes;

    public event Action<LizardController> OnLizardKilled;

    [SerializeField] private List<LizardData> _lizardTypes = new();
    [SerializeField] private int initialMaxLizards = 2;

    [SerializeField] private List<Route> _linearRoutes;
    [SerializeField] private List<Route> _peekingRoutes;

    private int _totalMovementTypes;
    private int _currentMaxLizards;
    private List<LizardController> _currentLizards = new();

    private LizardPool _pool;
    private DifficultyManager _difficultyManager;

    // For testing
    public void SpawnButton() => SpawnLizard();
    public void DespawnButton()
    {
        List<LizardController> lizardsToDespawn = new(_currentLizards);
        foreach (LizardController lizardController in lizardsToDespawn)
        {
            DespawnLizard(lizardController);
        }
    }

    private void Awake()
    {
        _pool = GetComponent<LizardPool>();
        _difficultyManager = FindFirstObjectByType<DifficultyManager>();

        _totalMovementTypes = Enum.GetValues(typeof(LizardMovementType)).Length;
    }

    private void Start()
    {
        _currentMaxLizards = initialMaxLizards;
        _pool.SetupPool(_lizardTypes);
        _difficultyManager.OnDifficultyIncreased += DifficultyIncreased;
    }

    private void OnDestroy()
    {
        _difficultyManager.OnDifficultyIncreased -= DifficultyIncreased;
    }

    private void SpawnLizard()
    {
        LizardData type = _lizardTypes[0];
        LizardController lizard = _pool.GetFromPool(type);

        lizard.gameObject.SetActive(true);
        lizard.Initialize();

        lizard.OnDeath += LizardKilled;
        lizard.OnMovementFinished += DespawnLizard;

        _currentLizards.Add(lizard);

        StartMovement(lizard);
    }

    private void StartMovement(LizardController lizard)
    {
        LizardMovementType type = (LizardMovementType)Random.Range(0, _totalMovementTypes);
        Route route;
        switch (type)
        {
            case LizardMovementType.Peeking:
                int index = Random.Range(0, _peekingRoutes.Count);
                route = _peekingRoutes[index];
                break;
            case LizardMovementType.Linear:
            default:
                index = Random.Range(0, _linearRoutes.Count);
                route = _linearRoutes[index];
                break;
        }
        lizard.StartMoving(type, route);
    }

    private void LizardKilled(LizardController lizard)
    {
        DespawnLizard(lizard);
        OnLizardKilled?.Invoke(lizard);
    }

    private void DespawnLizard(LizardController lizard)
    {
        lizard.OnDeath -= LizardKilled;
        lizard.OnMovementFinished -= DespawnLizard;
        _pool.ReturnToPool(lizard);
        _currentLizards.Remove(lizard);
    }

    private void DifficultyIncreased() => _currentMaxLizards++;

}
