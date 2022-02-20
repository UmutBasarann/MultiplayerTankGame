using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class GameOverHandler : NetworkBehaviour
    {
        #region EventHandler || Action

        public static event Action<string> ClientOnGameOver; 

        #endregion
        
        #region Fields

        private List<UnitBase> _bases = new List<UnitBase>();

        #endregion
        
        #region Server

        public override void OnStartServer()
        {
            UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion

        #region Event: ServerHandleBaseSpawned

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            _bases.Add(unitBase);
        }

        #endregion

        #region Event: ServerHandleBaseDespawned

        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitBase)
        {
            _bases.Remove(unitBase);

            if (_bases.Count != 1)
            {
                return;
            }

            var playerId = _bases[0].connectionToClient.connectionId;
            
            RpcGameOver($"Player {playerId}");
        }

        #endregion
    }
}