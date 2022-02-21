using System;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class CameraController : NetworkBehaviour
    {
        #region EventHandler || Action

        

        #endregion
        
        #region Childs

        [SerializeField] private Transform _playerCameraTransform = null;
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _screenBorderThickness = 10f;
        [SerializeField] private Vector2 _screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 _screenZlimits = Vector2.zero;

        #endregion

        #region Fields

        private Controls _controls;
        private Vector2 _previousInput;

        #endregion

        #region Awake || Start || Update

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused)
            {
                return;
            }
            
            UpdateCameraPosition();
        }

        #endregion

        #region Server



        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            _playerCameraTransform.gameObject.SetActive(true);

            _controls = new Controls();

            _controls.Player.MoveCamera.performed += SetPreviousInput;
            _controls.Player.MoveCamera.canceled += SetPreviousInput;
            
            _controls.Enable();
        }

        #endregion

        #region SetPreviousInput

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            _previousInput = ctx.ReadValue<Vector2>();
        }

        #endregion

        #region UpdateCameraPosition

        private void UpdateCameraPosition()
        {
            var position = _playerCameraTransform.position;

            if (_previousInput == Vector2.zero)
            {
                var cursorMovement = Vector3.zero;

                var cursorPosition = Mouse.current.position.ReadValue();

                if (cursorPosition.y >= Screen.height - _screenBorderThickness)
                {
                    cursorMovement.z += 1;
                }
                else if (cursorPosition.y <= _screenBorderThickness)
                {
                    cursorMovement.z -= 1;
                }

                if (cursorPosition.x >= Screen.width - _screenBorderThickness)
                {
                    cursorMovement.x += 1;
                }
                else if (cursorPosition.x <= _screenBorderThickness)
                {
                    cursorMovement.x -= 1;
                }

                position += cursorMovement.normalized * _speed * Time.deltaTime;
            }
            else
            {
                position += new Vector3(_previousInput.x, 0f, _previousInput.y) * _speed * Time.deltaTime;
            }

            position.x = Mathf.Clamp(position.x, _screenXLimits.x, _screenXLimits.y);
            position.z = Mathf.Clamp(position.z, _screenZlimits.x, _screenZlimits.y);

            _playerCameraTransform.position = position;
        }

        #endregion
    }
}