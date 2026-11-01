using System;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public Stats stats = new Stats(
        new Dictionary<StatType, float>
        {
            { StatType.Hp, 12.0f },
            { StatType.CurrentHp, 12.0f}
        }
    );
    
    public float currentHp;
    public float maxHp;

    public Transform target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        stats[StatType.CurrentHp] -= 10.0f;
        
        Enum.Parse<StatType>("CurrentHp");
        
        if (stats[StatType.CurrentHp] <= 0)
        {
            stats[StatType.CurrentHp] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
