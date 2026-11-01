using System;
using UnityEngine;


[System.Serializable]
public class Stat
{
    // The base value of the stat
    public float BaseValue { get; set; }
    // The percentage with which the stat is modified
    public float Percentage { get; private set; }
    // The multiplier of the percentage changes
    public float UpdateMultiplier { get; set; }
    public StatType Type { get; private set; }
    
    public float Value => BaseValue * Percentage;

    /// <summary>
    /// Increases the stat value by percentage of base value.
    /// </summary>
    /// <param name="percentage"></param>
    public void IncreasePercentageBy(float percentage)
    {
        this.Percentage += percentage * UpdateMultiplier;
    }

    /// <summary>
    /// Decreases the stat value by percentage of base value.
    /// </summary>
    /// <param name="percentage"></param>
    public void DecreasePercentageBy(float percentage)
    {
        this.Percentage -= percentage * UpdateMultiplier;
    }
    
    
    public Stat() {}
    public Stat(float baseValue, float percentage, float updateMultiplier, StatType type)
    {
        BaseValue = baseValue;
        Percentage = percentage;
        UpdateMultiplier = updateMultiplier;
        Type = type;
    }

    public Stat(float baseValue, StatType type)
    {
        BaseValue = baseValue;
        Type = type;
        Percentage = 1;
        UpdateMultiplier = 1;
    }

}

/// <summary>
/// An enum for every stat type available 
/// </summary>
/// <example> <code> Attack, Armor, Hp, HpRegen, AttackSpeed,
/// MoveSpeed, CritChance, CritDmg, DmgReduction,
/// PhysicalDmg, MagicalDmg, CooldownReduction, Size, ExpGain, PickupRadius </code> </example>
public enum StatType
{
    // Player and Enemy Stats
    CurrentHp,
    Hp,
    HpRegen,
    Armor,
    DmgReduction,
    Range,

    AttackSpeed,
    CooldownReduction,
    Size,

    MoveSpeed,
    ExpGain,
    PickupRadius,

    Attack,    
    CritChance,
    CritDmg,
    PhysicalDmg,
    MagicalDmg,
    //<--------->
    
    // Weapon Stats
    ProjectileCount
}