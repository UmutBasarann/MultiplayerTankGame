using System;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        #region EventHandler || Action

        public event Action ServerOnDie;

        #endregion
        
        #region Childs

        [SerializeField] 
        private int _maxHealth = 100;

        #endregion

        #region Fields

        [SyncVar] 
        private int _currentHealth;

        #endregion

        #region Server

        public override void OnStartServer()
        {
            _currentHealth = _maxHealth;
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

        

        #endregion
    }
}