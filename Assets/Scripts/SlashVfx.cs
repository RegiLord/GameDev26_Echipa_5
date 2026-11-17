using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlashVfx : MonoBehaviour
{
    public Renderer[] arcs;
    public float[] start = { 0.33f, 0.32f, 0.45f };
    public float[] window = { 0.17f, 0.18f, 0.17f };
    public Color color = new Color(1f, 0.25f, 0.3f, 1f);
    public float intensity = 1f;
    public Renderer blade;
    public Color glowColor = new Color(1f, 0.05f, 0.08f, 1f);
    public float glowIdle = 0.15f;
    public float glowPeak = 2f;

    Material[] mats;
    Material bladeMat;

    void Awake()
    {
        mats = new Material[arcs.Length];
        for (int i = 0; i < arcs.Length; i++)
        {
            mats[i] = arcs[i].material;
            arcs[i].enabled = false;
        }
        SetBlade(blade);
    }

    public void SetBlade(Renderer r)
    {
        blade = r;
        bladeMat = null;
        if (r == null) return;
        bladeMat = r.material;
        bladeMat.EnableKeyword("_EMISSION");
        SetGlow(glowIdle);
    }

    public void SetLevel(int level)
    {
        intensity = 1f + 0.15f * (level - 1);
    }

    public void Play(int i)
    {
        if (i < 0 || i >= arcs.Length) return;
        StartCoroutine(Run(i));
    }

    IEnumerator Run(int i)
    {
        yield return new WaitForSeconds(start[i]);
        Renderer r = arcs[i];
        Material m = mats[i];
        float w = window[i];
        float t = 0f;
        r.enabled = true;
        while (t < 2f * w)
        {
            float tau = t / w;
            float a = tau < 1f ? 1f : Mathf.Pow(Mathf.Clamp01(2f - tau), 1.5f);
            m.mainTextureOffset = new Vector2(1f - tau, 0f);
            SetColor(m, color * (intensity * a));
            float g = Mathf.Exp(-Mathf.Pow((tau - 0.7f) / 0.9f, 2f));
            SetGlow(Mathf.Lerp(glowIdle, glowPeak * intensity, g));
            t += Time.deltaTime;
            yield return null;
        }
        r.enabled = false;
        SetGlow(glowIdle);
    }

    void SetColor(Material m, Color c)
    {
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
        if (m.HasProperty("_TintColor")) m.SetColor("_TintColor", c);
    }

    void SetGlow(float v)
    {
        if (bladeMat != null) bladeMat.SetColor("_EmissionColor", glowColor * v);
    }
}
