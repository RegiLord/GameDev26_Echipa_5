using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroAnimLink : MonoBehaviour
{
    public AttackModule attackModule;
    public MovementModule movementModule;
    public HeroCombat combat;
    public SwordDraw swordDraw;
    public HeroLocomotion loco;
    public WeaponEquip equip;
    public Animator anim;
    public bool drawOnAttack = true;
    public bool matchRunSpeed = true;
    public float clipSpeed = 2.9f;
    public float sheatheAfter = 0f;

    bool pending;
    float lastAttack;

    void Awake()
    {
        if (attackModule == null) attackModule = GetComponent<AttackModule>();
        if (movementModule == null) movementModule = GetComponent<MovementModule>();
        if (combat == null) combat = GetComponent<HeroCombat>();
        if (swordDraw == null) swordDraw = GetComponent<SwordDraw>();
        if (loco == null) loco = GetComponent<HeroLocomotion>();
        if (equip == null) equip = GetComponent<WeaponEquip>();
        if (anim == null) anim = GetComponent<Animator>();
        if (loco != null)
        {
            loco.useInput = false;
            loco.moveTransform = false;
        }
    }

    void OnEnable()
    {
        if (attackModule != null) attackModule.OnAttack += HandleAttack;
    }

    void OnDisable()
    {
        if (attackModule != null) attackModule.OnAttack -= HandleAttack;
    }

    void HandleAttack(float dmg)
    {
        lastAttack = Time.time;
        if (equip != null && !equip.inHand && swordDraw != null)
        {
            if (!drawOnAttack) return;
            swordDraw.Draw();
            pending = true;
            return;
        }
        if (combat != null) combat.Attack();
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (movementModule != null && movementModule.IsMoving)
        {
            dir = movementModule.Destination - transform.position;
            dir.y = 0f;
        }
        if (loco != null) loco.Move(dir);

        if (pending && swordDraw != null && !swordDraw.busy)
        {
            if (equip == null || equip.inHand)
            {
                pending = false;
                if (combat != null) combat.Attack();
            }
            else swordDraw.Draw();
        }

        if (matchRunSpeed && anim != null)
        {
            bool run = loco != null && loco.moving && movementModule != null;
            anim.speed = run ? Mathf.Clamp(movementModule.Speed / clipSpeed, 0.5f, 2f) : 1f;
        }

        if (sheatheAfter > 0f && equip != null && equip.inHand && swordDraw != null && !swordDraw.busy && !pending)
        {
            bool idle = loco == null || !loco.moving;
            if (idle && Time.time - lastAttack > sheatheAfter) swordDraw.Sheathe();
        }
    }
}
