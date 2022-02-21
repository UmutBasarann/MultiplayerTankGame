using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    #region Childs

    [SerializeField] 
    private Unit _unitPrefab = null;

    [SerializeField] 
    private Transform _spawnPoint = null;

    [SerializeField]
    private Health _health = null;

    [SerializeField] 
    private TMP_Text _txtRemainingUnit = null;

    [SerializeField] 
    private Image _unitProgressImage = null;
    
    [SerializeField] 
    private int _maxUnitQueue = 5;
    
    [SerializeField] 
    private float _spawnMoveRange = 7f;
    
    [SerializeField] 
    private float _unitSpawnDuration = 5f;

    #endregion

    #region Fields

    private float _progressImageVelocity;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int _queuedUnits;
    
    [SyncVar]
    private float _unitTimer;

    #endregion

    #region Awake || Start || Update

    private void Update()
    {
        if (isServer)
        {
            ProduceUnit();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #endregion

    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
    }

    private void ProduceUnit()
    {
        if (_queuedUnits == 0)
        {
            return;
        }

        _unitTimer += Time.deltaTime;

        if (_unitTimer < _unitSpawnDuration)
        {
            return;
        }
        
        var unitInstance = Instantiate(_unitPrefab.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance, connectionToClient);

        var spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _spawnPoint.position.y;

        var unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(_spawnPoint.position + spawnOffset);

        _queuedUnits--;
        _unitTimer = 0;
    }

    #region Unit: CmdSpawnUnit

    [Command]
    private void CmdSpawnUnit()
    {
        if (_queuedUnits == _maxUnitQueue)
        {
            return;
        }

        var rtsPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();
        if (rtsPlayer.Resources < _unitPrefab.ResourceCost)
        {
            return;
        }

        _queuedUnits++;

        rtsPlayer.Resources -= _unitPrefab.ResourceCost;
    }

    #endregion

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        var newProgress = _unitTimer / _unitSpawnDuration;

        if (newProgress < _unitProgressImage.fillAmount)
        {
            _unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            _unitProgressImage.fillAmount = Mathf.SmoothDamp(_unitProgressImage.fillAmount, newProgress, ref _progressImageVelocity, .1f);
        }
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        _txtRemainingUnit.text = newUnits.ToString();
    }

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

    #region Event: ServerHandleDie

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
