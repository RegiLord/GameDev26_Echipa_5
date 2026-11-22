using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreIdle : MonoBehaviour
{
    public Transform crystal;
    public Transform ringA;
    public Transform ringB;
    public Transform shards;
    public float crystalSpeed = 18f;
    public float ringSpeed = 35f;
    public float bobHeight = 0.18f;
    public float bobSpeed = 0.8f;

    Vector3 crystalStart;
    Vector3 shardsStart;

    void Start()
    {
        if (crystal == null) crystal = FindDeep(transform, "Core_Crystal");
        if (ringA == null) ringA = FindDeep(transform, "Core_Ring_A");
        if (ringB == null) ringB = FindDeep(transform, "Core_Ring_B");
        if (shards == null) shards = FindDeep(transform, "Core_Shards");
        if (crystal != null) crystalStart = crystal.position;
        if (shards != null) shardsStart = shards.position;
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

    void Update()
    {
        float b = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        if (crystal != null)
        {
            crystal.Rotate(Vector3.up, crystalSpeed * Time.deltaTime, Space.World);
            crystal.position = crystalStart + Vector3.up * b;
        }
        if (ringA != null) ringA.Rotate(Vector3.up, ringSpeed * Time.deltaTime, Space.World);
        if (ringB != null) ringB.Rotate(Vector3.up, -ringSpeed * 0.7f * Time.deltaTime, Space.World);
        if (shards != null)
        {
            shards.Rotate(Vector3.up, -crystalSpeed * 0.6f * Time.deltaTime, Space.World);
            shards.position = shardsStart + Vector3.up * (b * 0.5f);
        }
    }
}
