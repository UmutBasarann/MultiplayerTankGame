using System;
using System.Collections.Generic;
using Buildings;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        #region EventHandler || Action

        public event Action<int> ClientOnResourcesUpdated; 

        #endregion
        
        #region Childs

        [SerializeField] private Building[] _buildings = new Building[0];

        #endregion
        
        #region Fields
        
        private List<Unit> _myUnits = new List<Unit>();
        public List<Unit> MyUnits => _myUnits;

        private List<Building> _myBuildings = new List<Building>();
        public List<Building> MyBuildings => _myBuildings;

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int _resources = 500;

        public int Resources
        {
            get => _resources;
            set => _resources = value;
        }

        #endregion

        #region Server: Start || Stop

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        #region Server

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 spawnPoint)
        {
            Building buildingToPlace = null;
            
            foreach (var building in _buildings)
            {
                if (building.Id == buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (buildingToPlace == null)
            {
                return;
            }

            var buildingInstance = Instantiate(buildingToPlace.gameObject, spawnPoint, buildingToPlace.transform.rotation);
            
            NetworkServer.Spawn(buildingInstance, connectionToClient);
        }

        #endregion

        #endregion

        #region Client: Start || Stop

        public override void OnStartAuthority()
        {
            if (NetworkServer.active)
            {
                return;
            }

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority)
            {
                return;
            }
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            
            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
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
            _myUnits.Add(unit);
        }

        #endregion

        #region Event: AuthorityHandleUnitDespawned
        
        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            _myUnits.Remove(unit);
        }

        #endregion

        #region Event: ServerHandleBuildingSpawned

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }
            
            _myBuildings.Add(building);
        }

        #endregion

        #region Event: ServerHandleBuildingDespawned

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            _myBuildings.Remove(building);
        }

        #endregion

        #region Event: AuthorityHandleBuildingSpawned

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            _myBuildings.Add(building);
        }

        #endregion

        #region Event: AuthorityHandleBuildingDespawned

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            _myBuildings.Remove(building);
        }

        #endregion
    }
}