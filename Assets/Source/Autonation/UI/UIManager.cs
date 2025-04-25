using Spectral.Autonation.Managers;
using Spectral.Autonation.MonoBehaviours;
using Spectral.Core;
using UnityEngine.UIElements;

namespace Spectral.Autonation.UI
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        private VisualElement _root;


        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.Q("OxygenLabel").dataSource = ResourcesManager.Instance;
            _root.Q("HydrogenLabel").dataSource = ResourcesManager.Instance;
            _root.Query<Button>("BuildingButton").ForEach(btn => btn.clicked += () => Player.Instance.EnterBuildMode());
        }
    }
}