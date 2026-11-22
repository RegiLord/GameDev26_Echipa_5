using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public static Dictionary<int, SelectableUnit> Units = new();
    private Renderer[] _childRenderers;

    public int OriginalLayer { get; private set; }

    // event functions
    void Awake()
    {
        _childRenderers = GetComponentsInChildren<Renderer>(true);

        OriginalLayer = gameObject.layer;
        Units.Add(gameObject.GetInstanceID(), this);
    }

    private void OnDestroy()
    {
        Units.Remove(gameObject.GetInstanceID());
    }

    
    
    // regular functions
    public void SetLayer(int targetLayer)
    {
        for (int i = 0; i < _childRenderers.Length; i++)
            _childRenderers[i].gameObject.layer = targetLayer;
        
        gameObject.layer = targetLayer; 
    }
}