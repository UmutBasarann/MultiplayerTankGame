using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    #region Childs

    [SerializeField] 
    private GameObject _unitSpawnerPrefab = null;

    #endregion

    #region Event: Server: OnServerAddPlayer

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerTransform = conn.identity.transform;

        var unitSpawnerInstance = Instantiate(_unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
        
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    #endregion
}
