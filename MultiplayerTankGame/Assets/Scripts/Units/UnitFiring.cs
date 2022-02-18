using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitFiring : NetworkBehaviour
    {
        #region Childs

        [SerializeField] 
        private Targeter _targeter = null;

        [SerializeField] 
        private GameObject _projectilePrefab = null;

        [SerializeField] 
        private Transform _projectileSpawnPoint = null;

        [SerializeField] 
        private float _fireRange = 12f;

        [SerializeField] 
        private float _fireRate = 1f;

        [SerializeField] 
        private float _rotationSpeed = 20f;

        #endregion

        #region Fields

        private float _lastFireTime;

        #endregion

        #region Awake || Start || Update

        [ServerCallback]
        private void Update()
        {
            if (_targeter.Target == null)
            {
                return;
            }
            
            if (!CanFireAtTarget())
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(_targeter.Target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / _fireRate) + _lastFireTime)
            {
                var projectileRotation = Quaternion.LookRotation(_targeter.Target.AimAtPoint.position - _projectileSpawnPoint.position);
                
                var projectileInstance = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, projectileRotation);
                
                NetworkServer.Spawn(projectileInstance, connectionToClient);
                
                _lastFireTime = Time.time;
            }
        }

        #endregion

        #region Server

        [Server]
        private bool CanFireAtTarget()
        {
            return (_targeter.Target.transform.position - transform.position).sqrMagnitude <= _fireRange * _fireRange;
        }

        #endregion
    }
}