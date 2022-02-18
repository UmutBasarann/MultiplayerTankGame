﻿using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        #region Fields

        [SerializeField]
        private List<Unit> _myUnits = new List<Unit>();
        public List<Unit> MyUnits => _myUnits;

        #endregion

        #region Server: Start || Stop

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        #endregion

        #region Client: Start || Stop

        public override void OnStartClient()
        {
            if (!isClientOnly)
            {
                return;
            }
        }

        public override void OnStopClient()
        {
            if (!isClientOnly)
            {
                return;
            }
        }

        #endregion

        #region Event: ServerHandleUnitSpawned

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }
            
            _myUnits.Add(unit);
        }

        #endregion

        #region Event: ServerHandleUnitDespawn

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            _myUnits.Remove(unit);
        }

        #endregion

        #region Event: AuthorityHandleUnitSpawned

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }
            
            _myUnits.Add(unit);
        }

        #endregion

        #region Event: AuthorityHandleUnitDespawned
        
        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }
            
            _myUnits.Remove(unit);
        }

        #endregion
    }
}