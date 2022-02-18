using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    #region Childs

    [SerializeField] 
    private GameObject _unitPrefab = null;

    [SerializeField] 
    private Transform _spawnPoint = null;

    #endregion

    #region Server

    #region Unit: CmdSpawnUnit

    [Command]
    private void CmdSpawnUnit()
    {
        var unitInstance = Instantiate(_unitPrefab, _spawnPoint.position, _spawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #endregion

    #region Client

    #region Event: OnPointerClick

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

    #endregion
}
