using System;
using System.Collections.Generic;
using Combat;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        #region EventHandler || Action

        public static event Action<UnitBase> ServerOnBaseSpawned; 
        public static event Action<UnitBase> ServerOnBaseDespawned;

        public static event Action<int> ServerOnPlayerDie;

        #endregion
        
        #region Childs

        [SerializeField] private Health _health = null;

        #endregion

        #region Fields

        

        #endregion

        #region Server

        public override void OnStartServer()
        {
            _health.ServerOnDie += ServerHandleDie;
            
            ServerOnBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBaseDespawned?.Invoke(this);
            
            _health.ServerOnDie -= ServerHandleDie;
        }

        #endregion

        #region Client



        #endregion

        #region Event: ServerHandleDie

        [Server]
        private void ServerHandleDie()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            
            NetworkServer.Destroy(gameObject);
        }

        #endregion
    }
}