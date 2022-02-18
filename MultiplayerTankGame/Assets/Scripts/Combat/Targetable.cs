using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targetable : NetworkBehaviour
    {
        #region Childs

        [SerializeField] 
        private Transform _aimAtPoint = null;

        public Transform AimAtPoint => _aimAtPoint;

        #endregion
    }
}