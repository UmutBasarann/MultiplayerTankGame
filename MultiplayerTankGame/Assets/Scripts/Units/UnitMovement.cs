using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    #region Childs

    [SerializeField] 
    private NavMeshAgent _agent = null;

    [SerializeField] 
    private Targeter _targeter = null;

    [SerializeField] 
    private float chaseRange = 10f;

    #endregion

    #region Awake || Start || Update

    [ServerCallback]
    private void Update()
    {
        var target = _targeter.Target;
        
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                _agent.SetDestination(_targeter.Target.transform.position);
            }
            else if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            
            return;
        }
        
        if (!_agent.hasPath)
        {
            return;
        }
        
        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            return;
        }
        
        _agent.ResetPath();
    }

    #endregion

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    #region Unit: CmdMove

    [Command]
    public void CmdMove(Vector3 position)
    {
        _targeter.ClearTarget();
        
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        _agent.SetDestination(hit.position);
    }

    #endregion

    #endregion

    #region Event: ServerHandleGameOver

    [Server]
    private void ServerHandleGameOver()
    {
        _agent.ResetPath();
    }

    #endregion
}
