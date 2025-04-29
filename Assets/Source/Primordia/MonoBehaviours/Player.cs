using Primordia.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Primordia.MonoBehaviours
{
    [RequireComponent(typeof(CameraController))]
    public class Player : SingletonBehaviour<Player>
    {
        public enum State
        {
            Selection,
            Building
        }

        private CameraController _cameraController;
        private State _currentState;
        private RaycastHit _hit;
        private LayerMask _layerMask;
        private LayerMask _layerMask2;
        private InputAction _leftClickAction;
        private Camera _mainCamera;
        private InputAction _rightClickAction;
        private GameObject _entityToSpawn;

        private void Start()
        {
            _leftClickAction = InputSystem.actions.FindAction("Left Button");
            _rightClickAction = InputSystem.actions.FindAction("Right Button");
            _mainCamera = Camera.main;
            _cameraController = GetComponent<CameraController>();
            _layerMask = LayerMask.GetMask("Surface");
            _layerMask2 = LayerMask.GetMask("Selection");
            _entityToSpawn = Resources.Load<GameObject>("Prefabs/ENTT TestCube");

        }

        private void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (_leftClickAction.WasPressedThisFrame())
                switch (_currentState)
                {
                    case State.Selection when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask2.value):
                        _cameraController.SetFocusedObject(_hit.collider.transform);
                        return;
                    case State.Building when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask.value):
                        Instantiate(_entityToSpawn, _hit.point, Quaternion.LookRotation(-_hit.normal));
                        ExitBuildMode();
                        return;
                }

            if (_rightClickAction.WasPressedThisFrame())
                switch (_currentState)
                {
                    case State.Selection when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask2.value):
                        Destroy(_hit.collider.gameObject);
                        return;
                    case State.Building when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask.value):
                        return;
                }
        }

        public void EnterBuildMode()
        {
            SetState(State.Building);
        }

        public void ExitBuildMode()
        {
            SetState(State.Selection);
        }

        private void SetState(State state)
        {
            _currentState = state;
        }
    }
}