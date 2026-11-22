using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class PropTools
{
    static string PropsFolder()
    {
        string[] g = AssetDatabase.FindAssets("Pine_A t:Model");
        if (g.Length == 0) return null;
        string p = AssetDatabase.GUIDToAssetPath(g[0]);
        return Path.GetDirectoryName(Path.GetDirectoryName(p)).Replace("\\", "/");
    }

    static string BuildingsFolder()
    {
        string[] g = AssetDatabase.FindAssets("Building_Tavern t:Model");
        if (g.Length == 0) return null;
        return Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(g[0])).Replace("\\", "/");
    }

    static Dictionary<string, GameObject> LoadProps(string folder)
    {
        Dictionary<string, GameObject> d = new Dictionary<string, GameObject>();
        foreach (string g in AssetDatabase.FindAssets("t:Model", new[] { folder }))
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g));
            if (go != null) d[go.name] = go;
        }
        return d;
    }

    static Material FindMat(string n)
    {
        foreach (string g in AssetDatabase.FindAssets(n + " t:Material"))
        {
            Material m = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g));
            if (m != null && m.name == n) return m;
        }
        return null;
    }

    [MenuItem("Tools/Aetherhold/Link Prop Materials")]
    static void LinkMaterials()
    {
        Material pal = FindMat("Env_Palette");
        Material glow = FindMat("Env_Glow");
        if (pal == null || glow == null) { EditorUtility.DisplayDialog("Props", "Extract the materials of Aetherhold_Wildlands.fbx first (Env_Palette and Env_Glow).", "OK"); return; }
        Material core = FindMat("Core_Glow");
        if (core == null)
        {
            core = new Material(glow);
            core.name = "Core_Glow";
            AssetDatabase.CreateAsset(core, Path.GetDirectoryName(AssetDatabase.GetAssetPath(glow)).Replace("\\", "/") + "/Core_Glow.mat");
        }
        List<string> folders = new List<string>();
        string pf = PropsFolder();
        string bf = BuildingsFolder();
        if (pf != null) folders.Add(pf);
        if (bf != null && (pf == null || !bf.StartsWith(pf))) folders.Add(bf);
        if (folders.Count == 0) { EditorUtility.DisplayDialog("Props", "No Props or Buildings folder found.", "OK"); return; }
        int n = 0;
        foreach (string g in AssetDatabase.FindAssets("t:Model", folders.ToArray()))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
            if (mi == null) continue;
            mi.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "Env_Palette"), pal);
            mi.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "Env_Glow"), glow);
            mi.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "Core_Glow"), core);
            mi.SaveAndReimport();
            n++;
        }
        Debug.Log("Linked materials on " + n + " models in " + string.Join(", ", folders));
    }

    [MenuItem("Tools/Aetherhold/Replace Map Props With Prefabs")]
    static void Replace()
    {
        GameObject root = Selection.activeGameObject;
        if (root == null) { EditorUtility.DisplayDialog("Props", "Select the Aetherhold_Wildlands object in the scene first.", "OK"); return; }
        string folder = PropsFolder();
        if (folder == null) { EditorUtility.DisplayDialog("Props", "Props folder not found (looking for Pine_A).", "OK"); return; }
        Dictionary<string, GameObject> props = LoadProps(folder);
        Regex re = new Regex(@"^(.*)_(\d{3})$");
        GameObject outer = PrefabUtility.GetOutermostPrefabInstanceRoot(root);
        if (outer != null) PrefabUtility.UnpackPrefabInstance(outer, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        List<Transform> list = root.GetComponentsInChildren<Transform>(true).Where(t =>
        {
            Match m = re.Match(t.name);
            return m.Success && props.ContainsKey(m.Groups[1].Value);
        }).ToList();
        Undo.SetCurrentGroupName("Replace map props");
        int grp = Undo.GetCurrentGroup();
        int n = 0;
        foreach (Transform t in list)
        {
            string key = re.Match(t.name).Groups[1].Value;
            GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(props[key]);
            Undo.RegisterCreatedObjectUndo(g, "prop");
            Vector3 s0 = g.transform.lossyScale;
            g.transform.SetParent(t.parent, false);
            g.transform.SetPositionAndRotation(t.position, t.rotation);
            Vector3 k = t.parent != null ? t.parent.lossyScale : Vector3.one;
            Vector3 want = Vector3.Scale(s0, t.localScale);
            g.transform.localScale = new Vector3(want.x / k.x, want.y / k.y, want.z / k.z);
            g.transform.SetSiblingIndex(t.GetSiblingIndex());
            g.name = t.name;
            g.isStatic = t.gameObject.isStatic;
            Undo.DestroyObjectImmediate(t.gameObject);
            n++;
        }
        Undo.CollapseUndoOperations(grp);
        Debug.Log("Replaced " + n + " objects with prefab instances from " + folder);
    }

    [MenuItem("Tools/Aetherhold/Make Building Prefabs")]
    static void MakeBuildingPrefabs()
    {
        string bf = BuildingsFolder();
        if (bf == null) { EditorUtility.DisplayDialog("Buildings", "Buildings folder not found (looking for Building_Tavern).", "OK"); return; }
        string outDir = bf + "/Prefabs";
        if (!AssetDatabase.IsValidFolder(outDir)) AssetDatabase.CreateFolder(bf, "Prefabs");
        int n = 0;
        foreach (string g in AssetDatabase.FindAssets("t:Model", new[] { bf }))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            if (!Path.GetFileName(path).StartsWith("Building_")) continue;
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
            Placeable pl = inst.AddComponent<Placeable>();
            pl.buildingId = model.name.Replace("Building_", "");
            if (model.name == "Building_Windmill") inst.AddComponent<Spinner>();
            if (model.name == "Building_Tavern")
            {
                Spinner s = inst.AddComponent<Spinner>();
                s.targetName = "Tavern_Sign";
                s.axisAwayFromRoot = false;
                s.swingAngle = 6f;
                inst.AddComponent<Producer>();
            }
            if (model.name == "Building_Windmill")
            {
                Producer p = inst.AddComponent<Producer>();
                p.resource = "Food";
            }
            if (model.name == "Building_ArcherTower") inst.AddComponent<TurretAim>();
            if (model.name == "Building_Core") inst.AddComponent<CoreIdle>();
            foreach (MeshFilter mf in inst.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.GetComponent<Collider>() != null) continue;
                MeshCollider mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = false;
            }
            PrefabUtility.SaveAsPrefabAsset(inst, outDir + "/" + model.name + ".prefab");
            UnityEngine.Object.DestroyImmediate(inst);
            n++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Created " + n + " building prefabs in " + outDir);
    }
}
