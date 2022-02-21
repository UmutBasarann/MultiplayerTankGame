using System;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;

namespace Resources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        #region Childs

        [SerializeField] private TMP_Text _txtResources = null;

        #endregion

        #region Fields

        private RTSPlayer _rtsPlayer;

        #endregion

        #region Awake || Start || Update

        private void Update()
        {
            if (_rtsPlayer == null)
            {
                _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if (_rtsPlayer != null)
                {
                    ClientHandleResourcesUpdated(_rtsPlayer.Resources);
                    
                    _rtsPlayer.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }
            }
        }

        #endregion

        #region OnDestroy

        private void OnDestroy()
        {
            _rtsPlayer.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }

        #endregion

        #region Event: ClientHandeResourcesUpdated

        private void ClientHandleResourcesUpdated(int resources)
        {
            _txtResources.text = $"Resources: {resources}";
        }

        #endregion
    }
}