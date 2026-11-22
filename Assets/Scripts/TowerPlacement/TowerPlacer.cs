using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }
    // PUBLIC VARIABLES
    public float interactDistance = 100;
    public float rotateAngle = 15f;
    public LayerMask interactableLayer;

    [Header("Materials")] public Material acceptedMaterial;
    public Material rejectedMaterial;

    [Header("Debug")] public bool debug = false;
    public GameObject tower;

    // INPUT VARIABLES
    private InputAction interactAction;
    private InputAction rotateAction;
    private InputAction rejectAction;

    private Camera mainCamera;
    //PUBLIC EVENTS
    public delegate void TemplateFunction();

    public static event TemplateFunction StartedPlacement;
    public static event TemplateFunction ExitedPlacement;
    public static event TemplateFunction TowerPlaced;
    public static event TemplateFunction TowerRejected;

    // TOWER PLACEMENT VARIABLES
    private bool placementMode = false;
    
    private bool? canPlace = null;
    private GameObject towerPlacement;
    private Collider towerCollider;
    private UnitData towerData;
    private AIController towerAI;
    private List<Material> towerMaterials;
    

    void Awake()
    {
        if (Instance is null)
            Instance = this;
        else 
            Destroy(this);
        
        // Initialize InputActions
        interactAction = InputSystem.actions.FindAction("Interact");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        rejectAction = InputSystem.actions.FindAction("Reject");
        
        mainCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (debug)
            EnterPlacementMode(tower);
    }

    // Update is called once per frame
    void Update()
    {
        if (!placementMode)
            return;
        
        if (towerPlacement is null)
        {
            Debug.LogError("In placement mode but no tower!!");
            return;
        }

        // Send ray for next location
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer);
        
        // If it hits the interactableLater, it moves to the hit point
        if (hit.collider is not null)
            towerPlacement.transform.position = hit.point;
        
        // Should now check if it is in an acceptable place or not
        bool newCanPlace = true;
        if (towerCollider is not null)
        {
            var colliderBounds = towerCollider.bounds;

            Collider[] overlapResults = new Collider[2];
            var colliders = Physics.OverlapBoxNonAlloc(
                colliderBounds.center, 
                colliderBounds.extents, 
                overlapResults, 
                towerPlacement.transform.rotation,
                -1,
                QueryTriggerInteraction.Ignore);
            
            if (colliders > 0)
                newCanPlace = false;
        }
        
        
        UpdateTowerMaterials(newCanPlace);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (placementMode && towerPlacement is not null && canPlace == true)
            AcceptPlacement();
    }

    private void OnReject(InputAction.CallbackContext context)
    {
        if (placementMode && towerPlacement is not null)
            RejectPlacement();
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        if (placementMode && towerPlacement is not null)
            towerPlacement.transform.Rotate(0, rotateAngle, 0);
    }
    

    private void EnterPlacementMode(GameObject tower)
    {
        placementMode = true;
        Debug.Log("ENTERED PLACEMENT MODE");

        towerPlacement = Instantiate(tower);
        towerCollider = towerPlacement.GetComponent<Collider>();
        towerData = towerPlacement.GetComponent<UnitData>();
        towerAI = towerPlacement.GetComponent<AIController>();
        towerData.enabled = false;
        towerAI.enabled = false;
        
        var meshRenderer = towerPlacement.GetComponent<MeshRenderer>();
        
        
        
        towerMaterials = new List<Material>();
        
        if (meshRenderer is not null)
        {
            towerMaterials = new List<Material>(meshRenderer.materials);
            
            var newMaterials = Enumerable.Repeat(acceptedMaterial, towerMaterials.Count);
            meshRenderer.materials = newMaterials.ToArray();
        }
        
        // Subscribe input events
        interactAction.performed += OnInteract;
        rotateAction.performed += OnRotate;
        rejectAction.performed += OnReject;
        
        if (StartedPlacement is not null)
            StartedPlacement();
    }

    private void ExitPlacementMode()
    {
        Debug.Log("EXITED PLACEMENT MODE");
        // Remove input events
        interactAction.performed -= OnInteract;
        rotateAction.performed -= OnRotate;
        rejectAction.performed -= OnReject;
        
        if (ExitedPlacement is not null)
            ExitedPlacement();
    }
    private void AcceptPlacement()
    {
        placementMode = false;
        
        if (towerCollider is not null)
            towerCollider.isTrigger = false;
        
        towerData.enabled = false;
        towerAI.enabled = false;
        
        var meshRenderer = towerPlacement.GetComponent<MeshRenderer>();
        
        if (meshRenderer is not null) 
            meshRenderer.materials = towerMaterials.ToArray();
        
        if (TowerPlaced is not null)
            TowerPlaced();

        towerPlacement = null;
        
        ExitPlacementMode();
    }
    private void RejectPlacement()
    {
        placementMode = false;
        
        Destroy(towerPlacement);
        
        if (TowerRejected is not null)
            TowerRejected();
        
        towerPlacement = null;
        
        ExitPlacementMode();
    }

    private void UpdateTowerMaterials(bool newCanPlace)
    {
        if (canPlace == newCanPlace)
            return;
        
        canPlace = newCanPlace;
        var meshRenderer = towerPlacement.GetComponent<MeshRenderer>();
        if (meshRenderer is not null)
        {
            if (canPlace == true)
                meshRenderer.materials = Enumerable.Repeat(acceptedMaterial, towerMaterials.Count).ToArray();
            else
                meshRenderer.materials = Enumerable.Repeat(rejectedMaterial, towerMaterials.Count).ToArray();
        }
    }
}
