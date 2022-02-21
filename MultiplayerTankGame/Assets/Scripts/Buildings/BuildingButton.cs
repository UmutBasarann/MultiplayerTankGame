using System;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region EventHandler || Action

        

        #endregion

        #region Childs

        [SerializeField] private Building _building = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _txtPrice = null;
        [SerializeField] private LayerMask _floorMask = new LayerMask();

        #endregion

        #region Fields

        private RTSPlayer _rtsPlayer;
        private Camera _mainCamera;
        private GameObject _buildingPreviewInstance;
        private Renderer _buildingRendererInstance;
        private BoxCollider _buildingCollider;

        #endregion

        #region Awake || Start || Update

        private void Start()
        {
            _mainCamera = Camera.main;

            _icon.sprite = _building.Icon;
            _txtPrice.text = _building.Price.ToString();

            _buildingCollider = _building.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (_rtsPlayer == null)
            {
                _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }

            if (_buildingPreviewInstance == null)
            {
                return;
            }
            
            UpdateBuildingPreview();
        }

        #endregion

        #region UpdateBuildingPreview

        private void UpdateBuildingPreview()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _floorMask))
            {
                return;
            }

            _buildingPreviewInstance.transform.position = hit.point;

            if (!_buildingPreviewInstance.activeSelf)
            {
                _buildingPreviewInstance.SetActive(true);
            }

            var color = _rtsPlayer.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;
            
            _buildingRendererInstance.material.SetColor("_BaseColor", color);
        }

        #endregion

        #region Event: OnPointerDown

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (_rtsPlayer.Resources < _building.Price)
            {
                return;
            }

            _buildingPreviewInstance = Instantiate(_building.BuildingPreview);
            _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();
            
            _buildingPreviewInstance.SetActive(false);
        }

        #endregion

        #region Event: OnPointerUp

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_buildingPreviewInstance = null)
            {
                return;
            }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _floorMask))
            {
                _rtsPlayer.CmdTryPlaceBuilding(_building.Id, hit.point);
            }
            
            Destroy(_buildingPreviewInstance);
        }

        #endregion
    }
}