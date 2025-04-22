using Spectral.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spectral.Autonation.MonoBehaviours
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
        private GameObject _objectToSpawn;

        private void Start()
        {
            _objectToSpawn = Resources.Load<GameObject>("Prefabs/ENTT Electrolyzer");
            _leftClickAction = InputSystem.actions.FindAction("Left Button");
            _mainCamera = Camera.main;
            _cameraController = GetComponent<CameraController>();
            _layerMask = LayerMask.GetMask("Surface");
            _layerMask2 = LayerMask.GetMask("Selection");
        }

        private void Update()
        {
            if (_leftClickAction.WasPressedThisFrame())
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                switch (_currentState)
                {
                    case State.Selection when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask2.value):
                        _cameraController.SetFocusedObject(_hit.collider.transform);
                        return;
                    case State.Building when Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask.value):
                        Instantiate(_objectToSpawn, _hit.point, Quaternion.LookRotation(-_hit.normal) * Quaternion.Euler(-90, 0, 0));
                        SetState(State.Selection);
                        return;
                }
            }
        }

        public void SetState(State state)
        {
            _currentState = state;
        }
    }
}