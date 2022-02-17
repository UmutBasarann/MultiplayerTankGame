using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] 
    private GameObject _unitPrefab = null;

    [SerializeField] 
    private Transform _spawnPoint = null;

    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        var unitInstance = Instantiate(_unitPrefab, _spawnPoint.position, _spawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }
        
        CmdSpawnUnit();
    }

    #endregion
}
