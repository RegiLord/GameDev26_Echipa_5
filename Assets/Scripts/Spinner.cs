using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Transform target;
    public string targetName = "Windmill_Sails";
    public bool axisAwayFromRoot = true;
    public Vector3 localAxis = Vector3.up;
    public float speed = 40f;
    public float swingAngle = 0f;
    public float swingSpeed = 1.5f;
    public bool autoSwingAxis = true;

    Vector3 axisW;
    Quaternion startRot;

    void Start()
    {
        if (target == null) target = FindDeep(transform, targetName);
        if (target == null) { enabled = false; return; }
        startRot = target.rotation;
        if (axisAwayFromRoot)
        {
            Vector3 d = target.position - transform.position;
            d.y = 0f;
            axisW = d.sqrMagnitude > 0.0001f ? d.normalized : target.TransformDirection(localAxis).normalized;
        }
        else axisW = target.TransformDirection(localAxis).normalized;
        if (swingAngle > 0f && autoSwingAxis)
        {
            Renderer r = target.GetComponentInChildren<Renderer>();
            if (r != null) axisW = r.bounds.size.x >= r.bounds.size.z ? Vector3.right : Vector3.forward;
        }
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
        if (swingAngle > 0f)
            target.rotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * swingSpeed) * swingAngle, axisW) * startRot;
        else
            target.Rotate(axisW, speed * Time.deltaTime, Space.World);
    }
}
