using System;
using UnityEngine;

namespace Cameras
{
    public class FaceCamera : MonoBehaviour
    {
        #region Fields

        private Transform _mainCameraTransform;

        #endregion

        #region Awake || Start || Update

        private void Start()
        {
            _mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward, _mainCameraTransform.rotation * Vector3.up);
        }

        #endregion
    }
}