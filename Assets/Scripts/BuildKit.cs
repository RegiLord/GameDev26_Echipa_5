using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildKit : MonoBehaviour
{
    public List<string> builtAtStart = new List<string>();
    public bool hideAllOnAwake = true;

    Dictionary<string, GameObject> pieces = new Dictionary<string, GameObject>();

    void Awake()
    {
        Collect(transform);
        if (!hideAllOnAwake) return;
        foreach (var kv in pieces) kv.Value.SetActive(builtAtStart.Contains(kv.Key));
    }

    void Collect(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform c = t.GetChild(i);
            if (c.name.StartsWith("Kit_") || c.name.StartsWith("Gate_")) pieces[c.name] = c.gameObject;
            else Collect(c);
        }
    }

    public bool Has(string piece)
    {
        return pieces.ContainsKey(piece);
    }

    public bool IsBuilt(string piece)
    {
        return pieces.ContainsKey(piece) && pieces[piece].activeSelf;
    }

    public void SetBuilt(string piece, bool on)
    {
        if (!pieces.ContainsKey(piece))
        {
            Debug.LogWarning("unknown kit piece: " + piece);
            return;
        }
        pieces[piece].SetActive(on);
    }

    public void SetBuiltPrefix(string prefix, bool on)
    {
        foreach (var kv in pieces)
            if (kv.Key.StartsWith(prefix)) kv.Value.SetActive(on);
    }

    public List<string> AllPieces()
    {
        return pieces.Keys.OrderBy(k => k).ToList();
    }

    [ContextMenu("Build Everything")]
    void BuildAll()
    {
        foreach (var kv in pieces) kv.Value.SetActive(true);
        SetBuilt("Kit_Gatehouse_E_Ruined", false);
        SetBuilt("Gate_E_ScrapBarricade", false);
        SetBuilt("Kit_Wall_SW_Damaged", false);
    }

    [ContextMenu("Hide Everything")]
    void HideAll()
    {
        foreach (var kv in pieces) kv.Value.SetActive(false);
    }
}
