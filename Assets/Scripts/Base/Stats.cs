using System.Collections.Generic;

public class Stats
{
    public Dictionary<StatType, float> stats;

    public Stats(Dictionary<StatType, float> stats)
    {
        this.stats = stats;
    }
    
    public void AddStat(StatType type, float value)
    {
        stats[type] = value;
    }
    
    public float this[StatType type]
    {
        get => stats[type];
        set => stats[type] = value;
    }
}
