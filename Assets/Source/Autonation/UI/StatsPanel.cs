using System;
using Spectral.Autonation.Managers;
using TMPro;
using UnityEngine;

namespace Spectral.Autonation.UI
{
    public class StatsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _oxygenText;
        [SerializeField] private TMP_Text _hydrogenText;


        private void Update()
        {
            _oxygenText.SetText("Oxygen: {0}", ResourcesManager.Instance.oxygen);
            _hydrogenText.SetText("Hydrogen: {0}", ResourcesManager.Instance.hydrogen);
        }
    }
}