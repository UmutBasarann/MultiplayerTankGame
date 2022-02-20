using System.Collections;
using System.Collections.Generic;
using Buildings;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    #region Childs

    [SerializeField] 
    private GameObject _unitSpawnerPrefab = null;

    [SerializeField] 
    private GameOverHandler _gameOverHandlerPrefab = null;

    #endregion

    #region Event: OnServerAddPlayer

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerTransform = conn.identity.transform;

        var unitSpawnerInstance = Instantiate(_unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
        
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    #endregion

    #region Event: OnServerSceneChanged

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            var gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);
            
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }

    #endregion
}
