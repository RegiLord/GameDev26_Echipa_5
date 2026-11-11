using System;
using UnityEngine;

public class Core : MonoBehaviour
{
    static public Core Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(this);
    }
    private void OnDestroy()
    {
        Instance = null;
    }
}
