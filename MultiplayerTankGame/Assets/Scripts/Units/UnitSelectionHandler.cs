using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] 
    private LayerMask _layerMask = new LayerMask();

    [SerializeField] 
    private GameObject _highlight = null;
    
    private Camera _mainCamera;
    private List<Unit> _selectedUnits = new List<Unit>();

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (var selectedUnit in _selectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            _selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
        {
            return;
        }

        if (!hit.collider.TryGetComponent<Unit>(out var unit))
        {
            return;
        }

        if (!unit.hasAuthority)
        {
            return;
        }
        
        _selectedUnits.Add(unit);

        foreach (var selectedUnit in _selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
