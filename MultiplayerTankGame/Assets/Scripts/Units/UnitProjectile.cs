using System;
using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitProjectile : NetworkBehaviour
    {
        #region Childs

        [SerializeField] private Rigidbody _rigidbody = null;
        [SerializeField] private float _destroyAfterSeconds = 5f;
        [SerializeField] private float _launchForce = 10f;

        #endregion

        #region Awake || Start || Update

        private void Start()
        {
            _rigidbody.velocity = transform.forward * _launchForce;
        }

        #endregion

        #region Server

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Event: OnStartServer

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), _destroyAfterSeconds);
        }

        #endregion
    }
}