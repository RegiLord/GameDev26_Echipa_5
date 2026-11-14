using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    
    public RectTransform selectionBox;
    
    private int _outlineLayer, _defaultLayer;
    private Camera _mainCamera;

    private bool _isMouseDown, _isDragging;
    private Vector2 _mouseStartPos;

    private readonly List<SelectableUnit> _currentlySelected = new();

    
    
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
            }

            if (_isDragging)
            {
                Vector2 newPos = mousePos - _mouseStartPos;
                selectionBox.sizeDelta = new Vector2(Mathf.Abs(newPos.x), Mathf.Abs(newPos.y));
                selectionBox.anchoredPosition = (_mouseStartPos + mousePos) / 2;
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (_isDragging)
                MultiSelect(_mouseStartPos, mousePos);
            else
                SingleSelect(mousePos);

            _isMouseDown = false;
            _isDragging = false;
            selectionBox.gameObject.SetActive(false);
        }
    }
    
    
    
    // regular functions
    void SingleSelect(Vector2 mousePos)
    {
        foreach (SelectableUnit un in _currentlySelected)
        {
            un.SetLayer(un.OriginalLayer);
        }
        _currentlySelected.Clear();

        Ray ray = _mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << _defaultLayer))
        {
            if (SelectableUnit.Units.TryGetValue(hit.collider.gameObject.GetInstanceID(), out SelectableUnit unit))
            {
                unit.SetLayer(_outlineLayer);
                _currentlySelected.Add(unit);
            }
        }
    }
    
    void MultiSelect(Vector2 startPos, Vector2 endPos)
    {
        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);
        Rect selectionRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

        foreach (SelectableUnit unit in SelectableUnit.Units.Values)
        {
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPos.z > 0 && selectionRect.Contains(new Vector2(screenPos.x, screenPos.y)))
            {
                unit.SetLayer(_outlineLayer);
                _currentlySelected.Add(unit);
            }
            else
            {
                unit.SetLayer(_defaultLayer);
                _currentlySelected.Remove(unit);
            }
        }
    }
}