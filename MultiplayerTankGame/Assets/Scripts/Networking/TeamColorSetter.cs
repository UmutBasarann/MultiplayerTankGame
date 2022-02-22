using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        #region Childs

        [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

        #endregion

        #region Fields

        [SyncVar(hook = nameof(HandleTeamColorUpdated))] 
        private Color _teamColor = new Color();

        #endregion

        #region Server

        public override void OnStartServer()
        {
            var rtsPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();

            _teamColor = rtsPlayer.TeamColor;
        }

        #endregion

        #region Client

        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (var renderer in colorRenderers)
            {
                renderer.material.SetColor("_BaseColor", newColor);
            }
        }

        #endregion
    }
}