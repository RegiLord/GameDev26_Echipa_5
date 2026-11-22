using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroLocomotion : MonoBehaviour
{
    public Animator anim;
    public WeaponEquip equip;
    public string runArmed = "Run";
    public string runSheathed = "Run_Sheathed";
    public string idleArmed = "Idle";
    public string idleSheathed = "Idle_Sheathed";
    public string[] lockStates = { "Attack_1", "Attack_2", "Attack_3", "Draw", "Sheathe" };
    public float speed = 2.9f;
    public float turnSpeed = 720f;
    public float blend = 0.12f;
    public bool moveTransform = true;
    public bool useInput = true;

    public bool moving;
    string cur;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (equip == null) equip = GetComponent<WeaponEquip>();
    }

    void Update()
    {
        if (!useInput) return;
        Vector3 d = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Move(d);
    }

    bool Locked()
    {
        AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nx = anim.GetNextAnimatorStateInfo(0);
        for (int i = 0; i < lockStates.Length; i++)
        {
            if (st.IsName(lockStates[i]) && st.normalizedTime < 0.9f) return true;
            if (anim.IsInTransition(0) && nx.IsName(lockStates[i])) return true;
        }
        return false;
    }

    public void Move(Vector3 dir)
    {
        if (Locked())
        {
            moving = false;
            cur = null;
            return;
        }
        dir.y = 0f;
        moving = dir.sqrMagnitude > 0.01f;
        bool armed = equip == null || equip.inHand;
        if (moving)
        {
            dir.Normalize();
            Quaternion q = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.deltaTime);
            if (moveTransform) transform.position += dir * speed * Time.deltaTime;
        }
        string want = moving ? (armed ? runArmed : runSheathed) : (armed ? idleArmed : idleSheathed);
        if (want != cur)
        {
            anim.CrossFadeInFixedTime(want, blend);
            cur = want;
        }
    }
}
