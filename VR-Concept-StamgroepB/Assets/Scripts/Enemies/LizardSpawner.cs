using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LizardSpawner : MonoBehaviour
{

    [SerializeField] private List<LizardData> _lizardTypes = new();

    private int _countPerType = 10;
    private Dictionary<LizardData, List<LizardController>> _lizardsPool = new();

    
    private void Start()
    {
        SetupPool();
    }

    public LizardController SpawnLizard(LizardData type)
    {
        if (!_lizardsPool[type].Any())
        {
            CreateAndAddToPool(type);
        }

        LizardController lizard = _lizardsPool[type].First();
        _lizardsPool[type].Remove(lizard);
        lizard.gameObject.SetActive(true);
        lizard.Initialize();
        return lizard;
    }

    public void ReturnToPool(LizardController lizard)
    {
        lizard.gameObject.SetActive(false);
        _lizardsPool[lizard.Data].Add(lizard);
    }

    private void SetupPool()
    {
        foreach (LizardData type in _lizardTypes)
        {
            _lizardsPool[type] = new();
            for(int i = 0; i < _countPerType; i++)
            {
                CreateAndAddToPool(type);
            }
        }
    }

    private void CreateAndAddToPool(LizardData type)
    {
        LizardController lizard = Instantiate(type.Prefab, gameObject.transform);
        _lizardsPool[type].Add(lizard);
        lizard.gameObject.SetActive(false);
    }

}
