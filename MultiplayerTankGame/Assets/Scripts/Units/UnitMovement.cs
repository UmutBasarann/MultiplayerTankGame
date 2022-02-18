using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    #region Childs

    [SerializeField] 
    private NavMeshAgent _agent = null;

    #endregion

    #region Awake || Start || Update

    [ServerCallback]
    private void Update()
    {
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

    #region Unit: CmdMove

    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        _agent.SetDestination(hit.position);
    }

    #endregion

    #endregion
}
