using System;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    // More of a data container than a component -> might change at some moment ( for example when a unit is created a unit data is created
    public bool debugMode = false;
    
    public Collider Collider { get; private set; }
    public HealthModule HealthModule { get; private set; }
    public AttackModule AttackModule { get; private set; }
    public MovementModule MovementModule { get; private set; }
    public AIController AIController { get; private set; }
    public TeamID TeamID { get; private set; }
    public CellData CellData { get; private set; }
    
    private Renderer[] _childRenderers;
    public int OriginalLayer { get; private set; }
    
    void Awake()
    {
        Collider = GetComponent<Collider>();
        HealthModule = GetComponent<HealthModule>();
        AttackModule = GetComponent<AttackModule>();
        MovementModule = GetComponent<MovementModule>();    
        AIController = GetComponent<AIController>();

        _childRenderers = GetComponentsInChildren<Renderer>(true);
        OriginalLayer = gameObject.layer;
        
        if (AIController != null)
        {
            TeamID = AIController.teamID;
        }
    }

    void Start()
    {
        if (CellData == null)
        {
            CellData = new CellData(this, 0, 0, null, null);
            GridManager.Instance.AddCell(CellData);
        }
    }

    private void OnEnable()
    {
        if (CellData != null)
            GridManager.Instance.AddCell(CellData);
    }

    void LateUpdate()
    {
        GridManager.Instance.UpdateCell(CellData);

        if (debugMode && CellData != null)
        {
            Debug.Log(transform.position + " -> " + "(" + CellData.x + " " + CellData.y + ")");
        }
    }

    private void OnDisable()
    {
        if (CellData != null)
            GridManager.Instance.RemoveCell(CellData);
    }

    
    public void SetLayer(int targetLayer)
    {
        for (int i = 0; i < _childRenderers.Length; i++)
            _childRenderers[i].gameObject.layer = targetLayer;
        
        gameObject.layer = targetLayer; 
    }
}
