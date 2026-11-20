using System;
using UnityEngine;

public class Core : MonoBehaviour
{
    private new Collider collider;
    private UnitData unitData;
    
    public UnitData UnitData => unitData;
    public Collider Coll => collider;
    
    
    static public Core Instance = null;
    
    
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(this);
        
        collider = GetComponent<Collider>();
        unitData = GetComponent<UnitData>();
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
}
