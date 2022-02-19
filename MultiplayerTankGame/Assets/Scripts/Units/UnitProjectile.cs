using System;
using Combat;
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
        [SerializeField] private int _damageToDeal = 20;

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

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out var networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient)
                {
                    return;
                }
            }

            if (other.TryGetComponent<Health>(out var health))
            {
                health.DealDamage(_damageToDeal);
            }
            
            DestroySelf();
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