using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    public Transform head;
    public Transform firePoint;
    public Transform target;
    public float turnSpeed = 180f;
    public float idleSweep = 25f;

    Quaternion startRot;
    Vector3 startDir;

    void Start()
    {
        if (head == null) head = FindEnds(transform, "_Head");
        if (firePoint == null) firePoint = FindEnds(transform, "_FirePoint");
        if (head == null) { enabled = false; return; }
        startRot = head.rotation;
        Vector3 d = firePoint != null ? firePoint.position - head.position : head.forward;
        d.y = 0f;
        startDir = d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.forward;
    }

    Transform FindEnds(Transform t, string end)
    {
        if (t.name.EndsWith(end)) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform r = FindEnds(t.GetChild(i), end);
            if (r != null) return r;
        }
        return null;
    }

    public Vector3 AimDirection()
    {
        Vector3 d = firePoint != null ? firePoint.position - head.position : head.forward;
        d.y = 0f;
        return d.normalized;
    }

    void Update()
    {
        float want;
        if (target != null)
        {
            Vector3 d = target.position - head.position;
            d.y = 0f;
            if (d.sqrMagnitude < 0.0001f) return;
            want = Vector3.SignedAngle(startDir, d.normalized, Vector3.up);
        }
        else want = Mathf.Sin(Time.time * 0.4f) * idleSweep;
        Quaternion goal = Quaternion.AngleAxis(want, Vector3.up) * startRot;
        head.rotation = Quaternion.RotateTowards(head.rotation, goal, turnSpeed * Time.deltaTime);
    }
}
