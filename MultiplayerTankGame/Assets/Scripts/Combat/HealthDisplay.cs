using System;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        #region Childs

        [SerializeField] private Health _health = null;
        [SerializeField] private GameObject _healthBarParent = null;
        [SerializeField] private Image _healthBarImage = null;

        #endregion

        #region Awake || Start || Update

        private void Awake()
        {
            _health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        #endregion

        #region OnDestroy

        private void OnDestroy()
        {
            _health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        #endregion

        #region OnMouseEnter

        private void OnMouseEnter()
        {
            _healthBarParent.SetActive(true);
        }

        #endregion

        #region OnMouseExit

        private void OnMouseExit()
        {
            _healthBarParent.SetActive(false);
        }

        #endregion

        #region Event: HandleHealthUpdated

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            _healthBarImage.fillAmount = (float) currentHealth / maxHealth;
        }

        #endregion
    }
}