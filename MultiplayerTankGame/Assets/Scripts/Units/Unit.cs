using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    #region EventHandler || Action
    public static event Action<Unit> ServerOnUnitSpawned; 
    public static event Action<Unit> ServerOnUnitDespawned; 
    public static event Action<Unit> AuthorityOnUnitSpawned; 
    public static event Action<Unit> AuthorityOnUnitDespawned; 

    #endregion
    
    #region Childs

    [SerializeField] 
    private UnitMovement _unitMovement = null;

    public UnitMovement UnitMovement => _unitMovement;

    [SerializeField] 
    private Targeter _targeter = null;

    public Targeter Targeter => _targeter;

    [SerializeField] 
    private Health _health = null;

    [SerializeField] 
    private int _resourceCost = 10;

    public int ResourceCost => _resourceCost;
    
    [SerializeField] 
    private UnityEvent onSelected = null;
    
    [SerializeField] 
    private UnityEvent onDeselected = null;

    #endregion

    #region Server

    #region Start || Stop

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        _health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #endregion

    #region Client

    #region Start || Stop

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Unit: Select || Deselect

    [Client]
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        onDeselected?.Invoke();
    }

    #endregion

    #endregion

    #region Event: ServerHandleDie

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
