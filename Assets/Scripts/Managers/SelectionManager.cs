using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    
    public RectTransform selectionBox;
    
    private int _outlineLayer, _groundLayer, _defaultLayer;
    private Camera _mainCamera;

    private bool _isMouseDown, _isDragging;
    private Vector2 _mouseStartPos;

    private readonly List<UnitData> _currentlySelected = new();
    private Vector3 _dragStartPos;
    
    
    // event functions
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    void Start()
    {
        _isMouseDown = false;
        _isDragging = false;
        _mainCamera = Camera.main;
        _outlineLayer = LayerMask.NameToLayer("Outline");
        _defaultLayer = LayerMask.NameToLayer("Default");
        _groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isMouseDown = true;
            _mouseStartPos = mousePos;
        }

        if (_isMouseDown)
        {
            if (Vector2.Distance(mousePos, _mouseStartPos) > 1 && !_isDragging)
            {
                _isDragging = true;
                selectionBox.gameObject.SetActive(true);
                
                Ray ray = _mainCamera.ScreenPointToRay(_mouseStartPos);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << _groundLayer))
                    _dragStartPos = hit.point;
            }

            if (_isDragging)
            {
                Vector2 newPos = mousePos - _mouseStartPos;
                selectionBox.sizeDelta = new Vector2(Mathf.Abs(newPos.x), Mathf.Abs(newPos.y));
                selectionBox.anchoredPosition = (_mouseStartPos + mousePos) / 2;
            
                MultiSelect(mousePos); 
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (!_isDragging) 
                SingleSelect(mousePos);

            // Reset variables
            _isMouseDown = false;
            _isDragging = false;
            _dragStartPos = Vector3.zero;
            selectionBox.gameObject.SetActive(false);
        }
    }
    
    
    
    // regular functions
    void SingleSelect(Vector2 mousePos)
    {
        ClearSelected();

        Ray ray = _mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit)) //, Mathf.Infinity, 1 << _defaultLayer
        {
            if (hit.collider.gameObject.TryGetComponent(out UnitData ud))
            {
                if (ud.TeamID == TeamID.Player)
                {
                    ud.SetLayer(_outlineLayer);
                    _currentlySelected.Add(ud);
                }
            }
        }
    }
        
    void MultiSelect(Vector2 endPos)
    {
        // Asta de modificat, la multi select poate selecta doar TeamID Player
        // De asemenea sa foloseasca grid manager-ul petnru a lua unituri
        // Cum? (Tragi un ray de la start pos la ground si endPos la ground si iei toate uniturile din acel rectangle
        // Poti creea tu functia dupa ce intelegi grid manager-ul
    
        // Multi-Select sa fie doar pe Player ID + asa scapam si de acest dictionar si de taota clasa SelectableUnit
        // Care va fi inlocuita de UnitData 
        
        Vector3 dragEndPos = Vector3.zero;
        Ray ray = _mainCamera.ScreenPointToRay(endPos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << _groundLayer))
            dragEndPos = hit.point;
        
        Vector3 min = Vector3.Min(_dragStartPos, dragEndPos);
        Vector3 max = Vector3.Max(_dragStartPos, dragEndPos);
        min.y -= 10f; max.y += 10f;

        Bounds selectionBounds = new Bounds();
        selectionBounds.SetMinMax(min, max);
    
        //Debug.Log($"Selection: {_dragStartPos} -> {dragEndPos}");

        ClearSelected();
        var units = GridManager.Instance.GetUnits(_dragStartPos, dragEndPos);

        foreach (UnitData ud in units)
        {
            if (ud.TeamID != TeamID.Player) continue;

            if (ud.TryGetComponent(out Collider col) && selectionBounds.Intersects(col.bounds))
            {
                ud.SetLayer(_outlineLayer);
                _currentlySelected.Add(ud);
            }
        }
    
        // foreach (SelectableUnit unit in SelectableUnit.Units.Values)
        // {
        //     Vector3 screenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);
        //
        //     if (screenPos.z > 0 && selectionRect.Contains(new Vector2(screenPos.x, screenPos.y)))
        //     {
        //         unit.SetLayer(_outlineLayer);
        //         _currentlySelected.Add(unit);
        //     }
        //     else
        //     {
        //         unit.SetLayer(_groundLayer);
        //     }
        // }
    }

    private void ClearSelected()
    {
        foreach (UnitData un in _currentlySelected)
        {
            if (un) un.SetLayer(un.OriginalLayer);
        }
        _currentlySelected.Clear();
    }
}