using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TowerPlacer : MonoBehaviour
{
    public GameObject tower;
    public float interactDistance = 100;
    
    private InputAction interactAction;
    
    private bool placementMode = false;
    private GameObject towerPlacement;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        
        interactAction.performed += InteractPressed;

        towerPlacement = Instantiate(tower);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward * interactDistance, out RaycastHit hit);
        
        if (hit.collider is not null)
            if (hit.collider.CompareTag("Ground"))
            towerPlacement.transform.position = hit.point;
    }

    void InteractPressed(InputAction.CallbackContext context)
    {
        
    }
}
