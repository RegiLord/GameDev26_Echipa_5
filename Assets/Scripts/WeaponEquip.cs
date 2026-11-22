using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponEquip : MonoBehaviour
{
    public GameObject swordPrefab;
    public GameObject sheathPrefab;
    public string handSocket = "Weapon.R";
    public string backSocket = "Sheath.Back";
    public Vector3 handPos;
    public Vector3 handRot;
    public Vector3 backPos;
    public Vector3 backRot;
    public Vector3 backSpin = new Vector3(0f, 0f, 180f);
    public bool drawOnStart = true;
    public bool inHand;

    [HideInInspector] public GameObject sword;
    [HideInInspector] public GameObject sheath;
    Transform hand;
    Transform back;

    void Awake()
    {
        hand = Find(transform, handSocket);
        back = Find(transform, backSocket);
        if (hand == null) Debug.LogWarning("hand socket not found: " + handSocket);
        if (back == null) Debug.LogWarning("back socket not found: " + backSocket);
        if (sheathPrefab != null && back != null)
        {
            sheath = Instantiate(sheathPrefab);
            Attach(sheath, back, backPos, backRot);
            Spin(sheath);
        }
        if (swordPrefab != null) Equip(swordPrefab, drawOnStart);
    }

    Transform Find(Transform t, string n)
    {
        if (t.name == n) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform r = Find(t.GetChild(i), n);
            if (r != null) return r;
        }
        return null;
    }

    void Attach(GameObject g, Transform s, Vector3 p, Vector3 r)
    {
        g.transform.SetParent(s, false);
        g.transform.localPosition = p;
        g.transform.localRotation = Quaternion.Euler(r);
        g.transform.localScale = Vector3.one;
    }

    void Spin(GameObject g)
    {
        g.transform.localRotation = Quaternion.Euler(backRot) * Quaternion.Euler(backSpin);
    }

    public void Equip(GameObject prefab, bool draw = true)
    {
        if (sword != null) Destroy(sword);
        sword = Instantiate(prefab);
        if (draw) Draw(); else Sheathe();
    }

    public void Unequip()
    {
        if (sword != null) Destroy(sword);
        sword = null;
        inHand = false;
    }

    public void Draw()
    {
        if (sword == null || hand == null) return;
        Attach(sword, hand, handPos, handRot);
        inHand = true;
    }

    public void Sheathe()
    {
        if (sword == null || back == null) return;
        Attach(sword, back, backPos, backRot);
        Spin(sword);
        inHand = false;
    }

    [ContextMenu("Toggle Draw")]
    void Toggle()
    {
        if (inHand) Sheathe(); else Draw();
    }
}
