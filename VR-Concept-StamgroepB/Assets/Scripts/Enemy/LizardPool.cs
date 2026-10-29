using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LizardPool : MonoBehaviour
{

    private int _countPerType = 25;
    private Dictionary<LizardData, List<LizardController>> _lizardsPool = new();

    public LizardController GetFromPool(LizardData type)
    {
        if (!_lizardsPool[type].Any())
        {
            CreateAndAddToPool(type);
        }

        LizardController lizard = _lizardsPool[type].First();
        _lizardsPool[type].Remove(lizard);
        return lizard;
    }

    public void ReturnToPool(LizardController lizard)
    {
        lizard.gameObject.SetActive(false);
        _lizardsPool[lizard.Data].Add(lizard);
    }

    public void SetupPool(List<LizardData> types)
    {
        foreach (LizardData type in types)
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
