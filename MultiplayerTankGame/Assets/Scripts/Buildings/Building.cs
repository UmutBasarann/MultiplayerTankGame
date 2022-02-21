using System;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class Building : NetworkBehaviour
    {
        #region EventHandler || Action

        public static event Action<Building> ServerOnBuildingSpawned; 
        public static event Action<Building> ServerOnBuildingDespawned;
        
        public static event Action<Building> AuthorityOnBuildingSpawned; 
        public static event Action<Building> AuthorityOnBuildingDespawned; 

        #endregion
        
        #region Childs

        [SerializeField] 
        private Sprite _icon = null;

        public Sprite Icon => _icon;
        
        [SerializeField] 
        private int _id = -1;

        public int Id => _id;
        
        [SerializeField] 
        private int _price = 100;

        public int Price => _price;

        [SerializeField] private GameObject _buildingPreview = null;
        public GameObject BuildingPreview => _buildingPreview;

        #endregion

        #region Server

        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority)
            {
                return;
            }
            
            AuthorityOnBuildingDespawned?.Invoke(this);
        }

        #endregion
    }
}