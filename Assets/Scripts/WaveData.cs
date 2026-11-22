using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WaveGroup
{
    public string spawn;
    public string lane;
    public string enemy;
    public int count;
    public float delay_s;
    public float interval_s;
}

[Serializable]
public class WaveEntry
{
    public int wave;
    public string teaching_goal;
    public string[] sectors;
    public WaveGroup[] groups;
}

[Serializable]
public class WaveFile
{
    public string note;
    public WaveEntry[] waves;
}

public class WaveData : MonoBehaviour
{
    public TextAsset wavesJson;
    public WaveFile file;

    void Awake()
    {
        Load();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (wavesJson == null) return;
        file = JsonUtility.FromJson<WaveFile>(wavesJson.text);
    }

    public WaveEntry Get(int wave)
    {
        if (file == null || file.waves == null) return null;
        return file.waves.FirstOrDefault(w => w.wave == wave);
    }
}
