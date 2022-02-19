using System;
using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    #region Childs

    [SerializeField] 
    private LayerMask _layerMask = new LayerMask();

    [SerializeField] 
    private RectTransform _unitSelectionArea = null;

    #endregion

    #region Fields

    private RTSPlayer _rtsPlayer;
    
    private Camera _mainCamera;
    
    private List<Unit> _selectedUnits = new List<Unit>();
    public List<Unit> SelectedUnits => _selectedUnits;

    private Vector2 _startPosition = Vector2.zero;

    #endregion

    #region Awake || Start || Update

    void Start()
    {
        _mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    void Update()
    {
        if (_rtsPlayer == null)
        {
            _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (var selectedUnit in _selectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            _selectedUnits.Clear();    
        }

        _unitSelectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();
        
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        var areaWidth = mousePosition.x - _startPosition.x;
        var areaHeight = mousePosition.y - _startPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    #endregion

    #region OnDestroy

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    #endregion

    #region Unit: ClearSelectionArea

    private void ClearSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(false);

        #region SingleSelection

        if (_unitSelectionArea.sizeDelta.magnitude == 0)
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

        #region MultiSelection

        var min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        var max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (var unit in _rtsPlayer.MyUnits)
        {
            if (_selectedUnits.Contains(unit))
            {
                continue;
            }
            
            var screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x && 
                screenPosition.x < max.x && 
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                _selectedUnits.Add(unit);
                unit.Select();
            }
        }

        #endregion
    }

    #endregion

    #region Event: AuthorityHandleUnitDespawned

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        _selectedUnits.Remove(unit);
    }

    #endregion
}
