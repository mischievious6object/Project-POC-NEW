using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class LizardsManager : MonoBehaviour
{
    public List<LizardData> LizardTypes => _lizardTypes;

    public event Action<LizardController> OnLizardKilled;

    [SerializeField] private List<LizardData> _lizardTypes = new();

    [Header("Routes")]
    [SerializeField] private List<Route> _linearRoutes;
    [SerializeField] private List<Route> _peekingRoutes;

    [Header("Lizard Spawn")]
    [SerializeField] private float _minSpawnTime = 1f;
    [SerializeField] private float _maxSpawnTime = 3f;
    [SerializeField] private int _initialMaxLizards = 2;


    private int _totalMovementTypes;
    private int _currentMaxLizards;
    private List<LizardController> _currentLizards = new();
    private Coroutine _spawnRoutine;

    private LizardPool _pool;
    private DifficultyManager _difficultyManager;

    private void Awake()
    {
        _pool = GetComponent<LizardPool>();
        _difficultyManager = FindFirstObjectByType<DifficultyManager>();

        _totalMovementTypes = Enum.GetValues(typeof(LizardMovementType)).Length;
    }

    private void Start()
    {
        _currentMaxLizards = _initialMaxLizards;
        _pool.SetupPool(_lizardTypes);
        _difficultyManager.OnDifficultyIncreased += DifficultyIncreased;
    }

    private void OnDestroy()
    {
        _difficultyManager.OnDifficultyIncreased -= DifficultyIncreased;
    }

    public void StartSpawning()
    {
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        StopCoroutine(_spawnRoutine);
    }

    public void Reset()
    {
        _currentMaxLizards = _initialMaxLizards;
        List<LizardController> lizardsToDespawn = new(_currentLizards);
        foreach (LizardController lizardController in lizardsToDespawn)
        {
            DespawnLizard(lizardController);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(_minSpawnTime, _maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if(_currentLizards.Count < _currentMaxLizards)
            {
                SpawnLizard();
            }
        }
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
        Debug.Log(type);
        Route route;
        switch (type)
        {
            case LizardMovementType.Peeking:
                route = GetRandomRoute(_peekingRoutes);
                break;
            case LizardMovementType.Linear:
                route = GetRandomRoute(_linearRoutes);
                break;
            default:
                route = GetRandomRoute(_linearRoutes);
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
    
    private Route GetRandomRoute(List<Route> routes)
    {
        int index = Random.Range(0, routes.Count);
        return routes[index];
    }

}
