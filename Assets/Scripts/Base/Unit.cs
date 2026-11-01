using System;
using UnityEngine;
using System.Collections.Generic;
using Base;

public class Unit : MonoBehaviour
{
    public List<StatType> mainStat;

    public event Action OnDeath;

    private void Start()
    {
        OnDeath += DestroySelf;
    }

    public void Death()
    {
        OnDeath?.Invoke();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
