using System;
using Primordia.Core;
using Primordia.Managers;
using Primordia.MonoBehaviours;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Primordia.UI
{
    public class UIManager : SingletonBehaviour<UIManager>, IDataSourceViewHashProvider
    {
        private Label _hydrogenLabel;
        private float _lastUpdateTime;
        private Label _oxygenLabel;
        private VisualElement _root;
        [CreateProperty] public string oxygenLabel => "Oxygen: " + ResourcesManager.Instance.oxygen;
        [CreateProperty] public string hydrogenLabel => "Hydrogen: " + ResourcesManager.Instance.hydrogen;

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _oxygenLabel = _root.Q<Label>("OxygenLabel");
            _oxygenLabel.SetBinding("text", new DataBinding
            {
                dataSourceType = typeof(string),
                bindingMode = BindingMode.ToTarget,
                dataSource = this,
                dataSourcePath = new PropertyPath(nameof(oxygenLabel)),
                updateTrigger = BindingUpdateTrigger.OnSourceChanged
            });

            _hydrogenLabel = _root.Q<Label>("HydrogenLabel");

            _hydrogenLabel.SetBinding("text", new DataBinding
            {
                dataSourceType = typeof(string),
                dataSource = this,
                bindingMode = BindingMode.ToTarget,
                dataSourcePath = new PropertyPath(nameof(hydrogenLabel)),
                updateTrigger = BindingUpdateTrigger.OnSourceChanged
            });
            _root.Query<Button>("BuildingButton").ForEach(btn => btn.clicked += () => Player.Instance.EnterBuildMode());
        }

        public long GetViewHashCode()
        {
            return HashCode.Combine(ResourcesManager.Instance.oxygen, ResourcesManager.Instance.hydrogen);
        }
    }
}