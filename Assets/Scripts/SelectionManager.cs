using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    private int _outlineLayer;
    private int _defaultLayer;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        _outlineLayer = LayerMask.NameToLayer("Outline");
        _defaultLayer = LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) { ToggleOutlineOnHit(); }
    }
    
    void ToggleOutlineOnHit()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);
    
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            clickedObject.layer = (clickedObject.layer == _outlineLayer) ? _defaultLayer : _outlineLayer;
         }
    }
}