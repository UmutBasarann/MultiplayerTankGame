using System;
using Buildings;
using Mirror;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class GameOverDisplay : MonoBehaviour
    {
        #region Childs

        [SerializeField] private GameObject _gameOverDisplayParent = null;
        [SerializeField] private TMP_Text _txtWinnerName = null;

        #endregion
        
        #region Awake || Start || Update

        private void Start()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }
        
        #endregion

        #region OnDestroy

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        #endregion

        #region Event: ClientHandleGameOver

        private void ClientHandleGameOver(string winner)
        {
            _txtWinnerName.text = $"{winner} Has Won!";
            
            _gameOverDisplayParent.SetActive(true);
        }

        #endregion

        #region Event: LeaveGame

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        #endregion
    }
}