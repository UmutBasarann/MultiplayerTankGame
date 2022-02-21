using System;
using Buildings;
using Combat;
using Mirror;
using Networking;
using UnityEngine;

namespace Resources
{
    public class ResourceGenerator : NetworkBehaviour
    {
        #region Childs

        [SerializeField] private Health _health = null;
        [SerializeField] private int _resourcesPerInterval = 10;
        [SerializeField] private float _interval = 2f;

        #endregion

        #region Fields

        private float _timer;
        private RTSPlayer _rtsPlayer;

        #endregion

        #region Awake || Start || Update

        [Server]
        private void Update()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                _rtsPlayer.Resources += _resourcesPerInterval;
                
                _timer = _interval;
            }
        }

        #endregion

        #region Server

        public override void OnStartServer()
        {
            _timer = _interval;
            _rtsPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();

            _health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            _health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        #endregion

        #region Client

        

        #endregion

        #region Event: ServerHandleDie

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Event: ServerHandleGameOver

        private void ServerHandleGameOver()
        {
            enabled = false;
        }

        #endregion
    }
}