using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    #region Childs

    [SerializeField] 
    private LayerMask _layerMask = new LayerMask();

    #endregion

    #region Fields

    private Camera _mainCamera;
    
    private List<Unit> _selectedUnits = new List<Unit>();
    public List<Unit> SelectedUnits => _selectedUnits;

    #endregion

    #region Awake || Start || Update

    void Start()
    {
        _mainCamera = Camera.main;
    }

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

    #endregion

    #region Unit: ClearSelectionArea

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

    #endregion
}
