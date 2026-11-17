using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLayout : MonoBehaviour
{
    public Transform markers;
    public Transform core;
    public Transform coreCrystal;
    public List<Transform> spawns = new List<Transform>();
    public List<Transform> gates = new List<Transform>();
    public List<Transform> towerPads = new List<Transform>();
    public List<Transform> barricadeSlots = new List<Transform>();
    public List<Transform> heroSlots = new List<Transform>();
    public List<Transform> facilities = new List<Transform>();
    public bool breachOpen = true;
    public bool drawGizmos = true;

    public Dictionary<string, List<Transform>> lanes = new Dictionary<string, List<Transform>>();

    void Awake()
    {
        Collect();
    }

    [ContextMenu("Collect")]
    public void Collect()
    {
        if (markers == null) markers = FindDeep(transform, "Markers");
        if (markers == null)
        {
            Debug.LogWarning("Markers not found under " + name);
            return;
        }
        core = markers.Find("Core");
        coreCrystal = markers.Find("CoreCrystal");
        Fill(spawns, "Spawns", "Spawn_");
        Fill(gates, "Gates", "");
        Fill(towerPads, "BuildPads", "TowerPad_");
        Fill(barricadeSlots, "BuildPads", "BarricadeSlot_");
        Fill(heroSlots, "HeroSlots", "HeroSlot_");
        Fill(facilities, "Facilities", "Facility_");
        lanes.Clear();
        Transform ln = markers.Find("Lanes");
        if (ln == null) return;
        foreach (Transform l in ln)
        {
            List<Transform> wps = new List<Transform>();
            foreach (Transform w in l) wps.Add(w);
            wps = wps.OrderBy(w => w.name).ToList();
            lanes[l.name] = wps;
        }
    }

    void Fill(List<Transform> list, string group, string prefix)
    {
        list.Clear();
        Transform g = markers.Find(group);
        if (g == null) return;
        foreach (Transform t in g)
            if (t.name.StartsWith(prefix)) list.Add(t);
        list.Sort((a, b) => string.Compare(a.name, b.name));
    }

    Transform FindDeep(Transform t, string n)
    {
        if (t.name == n) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform r = FindDeep(t.GetChild(i), n);
            if (r != null) return r;
        }
        return null;
    }

    public Transform GetSpawn(string spawnName)
    {
        return spawns.FirstOrDefault(s => s.name == spawnName);
    }

    public List<Vector3> GetLane(string laneName)
    {
        List<Vector3> pts = new List<Vector3>();
        if (!lanes.ContainsKey(laneName)) return pts;
        foreach (Transform w in lanes[laneName]) pts.Add(w.position);
        return pts;
    }

    public List<Vector3> PathFromSpawn(string spawnName)
    {
        string key = spawnName.Replace("Spawn_", "Lane_");
        if (key == "Lane_W_3" && breachOpen && UnityEngine.Random.value < 0.5f) key = "Lane_W_Breach";
        return GetLane(key);
    }

    public List<Transform> SpawnsInSector(string sector)
    {
        return spawns.Where(s => s.name.StartsWith("Spawn_" + sector + "_")).ToList();
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || markers == null) return;
        if (lanes.Count == 0) Collect();
        foreach (var kv in lanes)
        {
            Gizmos.color = kv.Key.Contains("Ring") ? Color.gray : (kv.Key.Contains("Breach") ? Color.red : Color.yellow);
            for (int i = 0; i + 1 < kv.Value.Count; i++)
                Gizmos.DrawLine(kv.Value[i].position + Vector3.up * 0.3f, kv.Value[i + 1].position + Vector3.up * 0.3f);
        }
        Gizmos.color = new Color(0.7f, 0.3f, 1f);
        foreach (Transform s in spawns) Gizmos.DrawWireSphere(s.position, 2.5f);
        Gizmos.color = Color.cyan;
        foreach (Transform p in towerPads) Gizmos.DrawWireSphere(p.position, 1.7f);
        Gizmos.color = new Color(1f, 0.6f, 0.2f);
        foreach (Transform b in barricadeSlots) Gizmos.DrawWireCube(b.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
        Gizmos.color = Color.green;
        foreach (Transform h in heroSlots) Gizmos.DrawWireSphere(h.position, 0.6f);
        if (core != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(core.position, 6.2f);
        }
    }
}
