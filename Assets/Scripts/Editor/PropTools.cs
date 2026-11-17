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
        string folder = PropsFolder();
        if (folder == null) { EditorUtility.DisplayDialog("Props", "Props folder not found (looking for Pine_A).", "OK"); return; }
        Material pal = FindMat("Env_Palette");
        Material glow = FindMat("Env_Glow");
        if (pal == null || glow == null) { EditorUtility.DisplayDialog("Props", "Extract the materials of Aetherhold_Wildlands.fbx first (Env_Palette and Env_Glow).", "OK"); return; }
        int n = 0;
        foreach (string g in AssetDatabase.FindAssets("t:Model", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
            if (mi == null) continue;
            mi.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "Env_Palette"), pal);
            mi.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "Env_Glow"), glow);
            mi.SaveAndReimport();
            n++;
        }
        Debug.Log("Linked materials on " + n + " prop models in " + folder);
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
}
