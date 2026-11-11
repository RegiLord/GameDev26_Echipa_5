using System;
using UnityEngine;

public class Core : MonoBehaviour
{
    private Collider collider;
    public Collider Coll => collider;
    
    static public Core Instance = null;
    
    
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(this);
        
        collider = GetComponent<Collider>();
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
}
