using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BuildingPlacer : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundMask = ~0;
    public float groundY = 0f;
    public float grid = 1f;
    public float rotateStep = 90f;
    public Transform buildingsRoot;
    public MapLayout map;
    public float laneClearance = 2.5f;
    public float coreClearance = 11f;
    public bool keepPlacingWithShift = true;
    public Color okColor = new Color(0.4f, 1f, 0.5f, 1f);
    public Color badColor = new Color(1f, 0.35f, 0.35f, 1f);
    public GameObject[] hotbar;

    public event Action<GameObject> OnPlaced;
    public bool IsPlacing { get { return ghost != null; } }

    GameObject prefab;
    GameObject ghost;
    float yaw;
    bool canPlace;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        mpb = new MaterialPropertyBlock();
    }

    public void Begin(GameObject p)
    {
        Cancel();
        if (p == null) return;
        prefab = p;
        ghost = Instantiate(p);
        ghost.name = p.name + "_Ghost";
        foreach (Collider c in ghost.GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (MonoBehaviour m in ghost.GetComponentsInChildren<MonoBehaviour>()) m.enabled = false;
    }

    public void Cancel()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        prefab = null;
    }

    void Update()
    {
        if (hotbar != null)
            for (int i = 0; i < hotbar.Length && i < 9; i++)
                if (KeyNum(i + 1)) Begin(hotbar[i]);
        if (ghost == null) return;
        if (KeyRotate()) yaw += rotateStep;
        if (KeyCancel() || MouseDown(1)) { Cancel(); return; }
        Vector3 pos;
        if (!MouseGround(out pos)) return;
        if (grid > 0f) { pos.x = Mathf.Round(pos.x / grid) * grid; pos.z = Mathf.Round(pos.z / grid) * grid; }
        ghost.transform.position = pos;
        ghost.transform.rotation = Quaternion.Euler(0f, yaw, 0f) * prefab.transform.rotation;
        canPlace = CanPlace();
        Tint(ghost, canPlace ? okColor : badColor);
        if (MouseDown(0) && canPlace && !OverUI()) Place(pos);
    }

    void Place(Vector3 pos)
    {
        GameObject g = Instantiate(prefab, pos, Quaternion.Euler(0f, yaw, 0f) * prefab.transform.rotation, buildingsRoot);
        g.name = prefab.name;
        Placeable pl = g.GetComponent<Placeable>();
        if (pl == null) pl = g.AddComponent<Placeable>();
        if (string.IsNullOrEmpty(pl.buildingId)) pl.buildingId = prefab.name;
        if (OnPlaced != null) OnPlaced(g);
        if (!(keepPlacingWithShift && KeyShift())) Cancel();
    }

    bool CanPlace()
    {
        Bounds b = GetBounds(ghost);
        foreach (Placeable p in Placeable.all)
            if (p != null && p.gameObject != ghost && Placeable.OverlapXZ(b, p.WorldBounds(), p.padding)) return false;
        if (map != null)
        {
            if (map.core != null)
            {
                Vector3 c = map.core.position;
                if (DistXZ(b, c) < coreClearance) return false;
            }
            float r = Mathf.Max(b.extents.x, b.extents.z) * 0.7f;
            foreach (var kv in map.lanes)
            {
                if (kv.Key.Contains("Ring")) continue;
                List<Transform> w = kv.Value;
                for (int i = 0; i + 1 < w.Count; i++)
                    if (SegDistXZ(b.center, w[i].position, w[i + 1].position) < laneClearance + r) return false;
            }
        }
        return true;
    }

    static float DistXZ(Bounds b, Vector3 p)
    {
        float dx = Mathf.Max(b.min.x - p.x, 0f, p.x - b.max.x);
        float dz = Mathf.Max(b.min.z - p.z, 0f, p.z - b.max.z);
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    static float SegDistXZ(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector2 pp = new Vector2(p.x, p.z), aa = new Vector2(a.x, a.z), bb = new Vector2(b.x, b.z);
        Vector2 ab = bb - aa;
        float t = ab.sqrMagnitude > 0f ? Mathf.Clamp01(Vector2.Dot(pp - aa, ab) / ab.sqrMagnitude) : 0f;
        return Vector2.Distance(pp, aa + ab * t);
    }

    static Bounds GetBounds(GameObject g)
    {
        Renderer[] rs = g.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(g.transform.position, Vector3.one);
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    void Tint(GameObject g, Color c)
    {
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor", c);
            mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);
        }
    }

    bool MouseGround(out Vector3 pos)
    {
        pos = Vector3.zero;
        if (cam == null) return false;
        Ray ray = cam.ScreenPointToRay(MousePos());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5000f, groundMask, QueryTriggerInteraction.Ignore) && (ghost == null || !hit.transform.IsChildOf(ghost.transform)))
        {
            pos = hit.point;
            return true;
        }
        Plane pl = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        float d;
        if (pl.Raycast(ray, out d)) { pos = ray.GetPoint(d); return true; }
        return false;
    }

    static bool OverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    static Vector3 MousePos() { return Mouse.current != null ? (Vector3)Mouse.current.position.ReadValue() : Vector3.zero; }
    static bool MouseDown(int b) { if (Mouse.current == null) return false; return b == 0 ? Mouse.current.leftButton.wasPressedThisFrame : Mouse.current.rightButton.wasPressedThisFrame; }
    static bool KeyRotate() { return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame; }
    static bool KeyCancel() { return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame; }
    static bool KeyShift() { return Keyboard.current != null && Keyboard.current.shiftKey.isPressed; }
    static bool KeyNum(int n) { return Keyboard.current != null && Keyboard.current[(Key)((int)Key.Digit1 + n - 1)].wasPressedThisFrame; }
#else
    static Vector3 MousePos() { return Input.mousePosition; }
    static bool MouseDown(int b) { return Input.GetMouseButtonDown(b); }
    static bool KeyRotate() { return Input.GetKeyDown(KeyCode.R); }
    static bool KeyCancel() { return Input.GetKeyDown(KeyCode.Escape); }
    static bool KeyShift() { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); }
    static bool KeyNum(int n) { return Input.GetKeyDown(KeyCode.Alpha1 + n - 1); }
#endif
}
