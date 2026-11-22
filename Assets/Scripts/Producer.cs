using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Producer : MonoBehaviour
{
    public string resource = "Coins";
    public int amount = 10;
    public float everySeconds = 30f;
    public bool perWaveOnly = false;

    public static event Action<Producer, string, int> OnProduced;
    public static Dictionary<string, int> totals = new Dictionary<string, int>();

    float t;

    void Update()
    {
        if (perWaveOnly || everySeconds <= 0f) return;
        t += Time.deltaTime;
        if (t >= everySeconds)
        {
            t -= everySeconds;
            Produce();
        }
    }

    public void Produce()
    {
        if (!totals.ContainsKey(resource)) totals[resource] = 0;
        totals[resource] += amount;
        if (OnProduced != null) OnProduced(this, resource, amount);
    }

    public static void EndOfWave()
    {
        foreach (Producer p in FindObjectsByType<Producer>(FindObjectsSortMode.None))
            if (p.perWaveOnly) p.Produce();
    }
}
