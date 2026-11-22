using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public string buildingId;
    public float padding = 0.3f;
    public static List<Placeable> all = new List<Placeable>();

    void OnEnable()
    {
        if (!all.Contains(this)) all.Add(this);
    }

    void OnDisable()
    {
        all.Remove(this);
    }

    public Bounds WorldBounds()
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(transform.position, Vector3.one);
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    public static bool OverlapXZ(Bounds a, Bounds b, float pad)
    {
        return a.min.x < b.max.x + pad && a.max.x > b.min.x - pad && a.min.z < b.max.z + pad && a.max.z > b.min.z - pad;
    }

    void OnDrawGizmosSelected()
    {
        Bounds b = WorldBounds();
        Gizmos.color = new Color(0.3f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y + 0.05f, b.center.z), new Vector3(b.size.x, 0.1f, b.size.z));
    }
}
