using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    public Animator anim;
    public SlashVfx vfx;
    public string[] states = { "Attack_1", "Attack_2", "Attack_3" };
    public float[] lengths = { 1.1f, 1.15f, 1.4f };
    public float cancelAt = 0.62f;
    public float comboWindow = 0.35f;
    public float blend = 0.08f;

    int step;
    float busyUntil;
    float lastEnd;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (vfx == null) vfx = GetComponentInChildren<SlashVfx>();
    }

    public bool Attack()
    {
        if (Time.time < busyUntil) return false;
        if (Time.time > lastEnd + comboWindow) step = 0;
        anim.CrossFadeInFixedTime(states[step], blend);
        if (vfx != null) vfx.Play(step);
        busyUntil = Time.time + lengths[step] * cancelAt;
        lastEnd = Time.time + lengths[step];
        step = (step + 1) % states.Length;
        return true;
    }

    [ContextMenu("Attack")]
    void TestAttack()
    {
        Attack();
    }
}
