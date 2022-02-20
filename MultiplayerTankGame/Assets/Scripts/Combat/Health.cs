using System;
using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        #region EventHandler || Action

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #endregion
        
        #region Childs

        [SerializeField] 
        private int _maxHealth = 100;

        #endregion

        #region Fields

        [SyncVar(hook = nameof(HandleHealthUpdate))] 
        private int _currentHealth;

        #endregion

        #region Server

        public override void OnStartServer()
        {
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
            
            _currentHealth = _maxHealth;
        }

        [Server]
        private void ServerHandlePlayerDie(int connectionId)
        {
            if (connectionId != connectionToClient.connectionId)
            {
                return;
            }
            
            DealDamage(_currentHealth);
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth == 0)
            {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

            if (_currentHealth != 0)
            {
                return;
            }
            
            ServerOnDie?.Invoke();
            
            Debug.Log("We died");
        }

        #endregion

        #region Client

        private void HandleHealthUpdate(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, _maxHealth);
        }

        #endregion
    }
}