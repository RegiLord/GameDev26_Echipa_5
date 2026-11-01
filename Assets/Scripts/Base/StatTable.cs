using System;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class StatTable
{
    public Dictionary<StatType, Stat> _stats = new Dictionary<StatType, Stat>();
    
    public StatTable() {}

    public StatTable(float baseValue, float percentage, float updateMultiplier)
    {
        // Maybe some schema things like jsons for basic units etc.
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            _stats.Add(
                statType,
                new Stat(baseValue, percentage, updateMultiplier, statType)
            );
        }
        
    }

    public List<(StatType, float)> GetStats()
    {
        List<(StatType, float)> stats = new List<(StatType, float)>();
        
        foreach (KeyValuePair<StatType, Stat> stat in _stats)
            stats.Add((stat.Key, stat.Value.Value));
        stats.Sort();
        
        return stats;
    }
    public Stat this[StatType type]
    {
        get => _stats[type];
    }
    public Stat this[string name]
    {
        get => _stats[Enum.Parse<StatType>(name)];
    }

    /// <summary>
    /// For debugging. Show all stats.
    /// </summary>
    public void Show()
    {
        foreach (var item in _stats)
        {
            Debug.Log(item.Key + ": " + item.Value.Value);
        }
    }
}