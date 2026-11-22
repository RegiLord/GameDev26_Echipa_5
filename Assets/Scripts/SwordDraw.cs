using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwordDraw : MonoBehaviour
{
    public Animator anim;
    public WeaponEquip equip;
    public SlashVfx vfx;
    public HeroCombat combat;
    public string drawState = "Draw";
    public string sheatheState = "Sheathe";
    public string idleArmed = "Idle";
    public string idleSheathed = "Idle_Sheathed";
    public float drawLen = 1.7f;
    public float sheatheLen = 1.9f;
    public float grabAt = 0.5f;
    public float releaseAt = 1.38f;
    public int drawArc = 3;
    public float drawArcStart = 0.857f;
    public float drawArcWindow = 0.3f;
    public float flashAt = 0.64f;
    public float flashPeak = 1.4f;
    public float blend = 0.1f;
    public KeyCode toggleKey = KeyCode.None;

    public bool busy;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (equip == null) equip = GetComponent<WeaponEquip>();
        if (vfx == null) vfx = GetComponentInChildren<SlashVfx>();
        if (combat == null) combat = GetComponent<HeroCombat>();
        if (vfx != null && vfx.arcs != null && vfx.arcs.Length > drawArc)
        {
            if (vfx.start.Length <= drawArc) Array.Resize(ref vfx.start, drawArc + 1);
            if (vfx.window.Length <= drawArc) Array.Resize(ref vfx.window, drawArc + 1);
            vfx.start[drawArc] = drawArcStart;
            vfx.window[drawArc] = drawArcWindow;
        }
    }

    void Start()
    {
        if (equip != null && !equip.inHand && anim != null) anim.Play(idleSheathed, 0, 0f);
    }

    void Update()
    {
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey)) Toggle();
    }

    public bool Draw()
    {
        if (busy || equip == null || equip.sword == null || equip.inHand) return false;
        StartCoroutine(RunDraw());
        return true;
    }

    public bool Sheathe()
    {
        if (busy || equip == null || equip.sword == null || !equip.inHand) return false;
        StartCoroutine(RunSheathe());
        return true;
    }

    [ContextMenu("Toggle")]
    public bool Toggle()
    {
        if (equip == null) return false;
        return equip.inHand ? Sheathe() : Draw();
    }

    public bool Attack()
    {
        if (busy) return false;
        if (equip != null && !equip.inHand) return Draw();
        return combat != null && combat.Attack();
    }

    IEnumerator RunDraw()
    {
        busy = true;
        anim.CrossFadeInFixedTime(drawState, blend);
        yield return new WaitForSeconds(grabAt);
        equip.Draw();
        if (vfx != null)
        {
            vfx.SetBlade(equip.sword.GetComponentInChildren<Renderer>());
            StartCoroutine(Flash());
            if (vfx.arcs != null && vfx.arcs.Length > drawArc) StartCoroutine(Arc());
        }
        yield return new WaitForSeconds(drawLen - grabAt - blend);
        anim.CrossFadeInFixedTime(idleArmed, blend);
        busy = false;
    }

    IEnumerator RunSheathe()
    {
        busy = true;
        anim.CrossFadeInFixedTime(sheatheState, blend);
        yield return new WaitForSeconds(releaseAt);
        equip.Sheathe();
        if (vfx != null) vfx.SetGlow(0f);
        yield return new WaitForSeconds(sheatheLen - releaseAt - blend);
        anim.CrossFadeInFixedTime(idleSheathed, blend);
        busy = false;
    }

    IEnumerator Arc()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, drawArcStart - grabAt));
        vfx.start[drawArc] = 0f;
        vfx.Play(drawArc);
    }

    IEnumerator Flash()
    {
        float t = grabAt;
        while (t < flashAt + 0.12f)
        {
            float k = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(t - flashAt) / 0.1f), 1.5f);
            vfx.SetGlow(Mathf.Lerp(vfx.glowIdle, flashPeak * vfx.intensity, k));
            t += Time.deltaTime;
            yield return null;
        }
        vfx.SetGlow(vfx.glowIdle);
    }
}
