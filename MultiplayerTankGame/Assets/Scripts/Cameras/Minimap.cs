using System;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        #region EventHandler || Action

        

        #endregion

        #region Childs

        [SerializeField] private RectTransform _minimapRect = null;
        [SerializeField] private float _mapScale = 20f;
        [SerializeField] private float _offset = -6f;

        #endregion

        #region Fields

        private Transform _playerCamTransform;
        
        #endregion

        #region Awake || Start || Update

        private void Update()
        {
            if (_playerCamTransform != null)
            {
                return;
            }

            if (NetworkClient.connection.identity == null)
            {
                return;
            }

            _playerCamTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().CameraTransform;
        }

        #endregion

        #region MoveCamera

        private void MoveCamera()
        {
            var mousePosition = Mouse.current.position.ReadValue();

            string a = "asd";
            
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_minimapRect, mousePosition, null, out var localPoint))
            {
                return;
            }

            var lerp = new Vector2((localPoint.x - _minimapRect.rect.x) / _minimapRect.rect.width, (localPoint.y - _minimapRect.rect.y) / _minimapRect.rect.height);

            var newCamPosition = new Vector3(Mathf.Lerp(-_mapScale, _mapScale, lerp.x), _playerCamTransform.position.y, Mathf.Lerp(-_mapScale, _mapScale, lerp.y));

            _playerCamTransform.position = newCamPosition + new Vector3(0f, 0f, _offset);
        }

        #endregion

        #region Event: OnPointerDown

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        #endregion

        #region Event: OnDrag

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }

        #endregion
    }
}