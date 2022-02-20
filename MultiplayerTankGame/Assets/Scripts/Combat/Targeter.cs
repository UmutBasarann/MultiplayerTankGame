using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        #region Childs

        [SerializeField] 
        private Targetable _target;

        public Targetable Target => _target;

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



        #region CmdSetTarget

        [Command]
        public void CmdSetTarget(Targetable target)
        {
            if (!target.TryGetComponent<Targetable>(out var targetable))
            {
                return;
            }

            _target = targetable;
        }

        #endregion

        #region ClearTarget

        [Server]
        public void ClearTarget()
        {
            _target = null;
        }

        #endregion

        #endregion

        #region Event: ServerHandleGameOver

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }

        #endregion
    }
}