using System;
using UnityEngine.UI;

namespace Spectral.Autonation.UI
{
    public class AutonationButton : Button
    {
        protected override void Start()
        {
            base.Start();
            onClick.AddListener(InvokeOnClickAction);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onClick.RemoveListener(InvokeOnClickAction);
        }

        public event Action OnClickAction;

        private void InvokeOnClickAction()
        {
            OnClickAction?.Invoke();
        }
    }
}