using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitCommander : MonoBehaviour
    {
        #region Childs

        [SerializeField] 
        private UnitSelectionHandler _unitSelectionHandler = null;

        [SerializeField] 
        private LayerMask _layerMask;

        #endregion

        #region Fields

        private Camera _mainCamera;

        #endregion

        #region Awake || Start || Update

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame)
            {
                return;
            }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
            {
                return;
            }

            TryMove(hit.point);
        }

        #endregion

        #region Unit: TryMove

        private void TryMove(Vector3 hitPoint)
        {
            foreach (var selectedUnit in _unitSelectionHandler.SelectedUnits)
            {
                selectedUnit.UnitMovement.CmdMove(hitPoint);
            }
        }

        #endregion
    }
}