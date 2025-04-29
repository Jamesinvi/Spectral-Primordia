using Primordia.Core;
using Primordia.Core.Math;
using Primordia.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Primordia.MonoBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] public float _orbitSpeed = 3f;
        [SerializeField] [Range(0,1)] public float _orbitSmoothTime = 1f;
        [SerializeField] public float _zoomSpeed = 3f;
        [SerializeField] public float _zoomDistance = 3f;
        [SerializeField] public float _minZoomDistance = 1f;
        [SerializeField] public float _maxZoomDistance = 70f;
        [SerializeField] public Transform _focusedObject;
        private Camera _cam;
        private Vector3 _currentVelocity;
        private Entity _focusedEntity;
        private Vector3 _focusedPosition;

        private InputAction _panCameraAction;
        private float _pitch;
        private bool _postStartWasCalled;
        private InputAction _rightClickAction;
        private float _yaw;
        private InputAction _zoomAction;
        private Vector2 _virtualMousePosition;


        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void Start()
        {
            _panCameraAction = InputSystem.actions.FindAction("Rotate Camera");
            _rightClickAction = InputSystem.actions.FindAction("Right Button");
            _zoomAction = InputSystem.actions.FindAction("Zoom");
        }

        private void Update()
        {
            if (!_postStartWasCalled) PostStart();
            if (_rightClickAction.IsPressed())
            {
                Vector2 delta = _panCameraAction.ReadValue<Vector2>().normalized;
                _yaw += delta.x * _orbitSpeed;
                _pitch -= delta.y * _orbitSpeed;
                // Clamp pitch so you can't flip upside‑down
                _pitch = Mathf.Clamp(_pitch, -89, 89);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                // Just exited rotation movement, reset the cursor to the last saved position
                if (_rightClickAction.WasReleasedThisFrame())
                {
                    Mouse.current.WarpCursorPosition(_virtualMousePosition);
                }
                Cursor.lockState = CursorLockMode.None;
                _virtualMousePosition = Pointer.current.position.ReadValue();
            }


            var zoomDelta = _zoomAction.ReadValue<Vector2>();
            _zoomDistance -= zoomDelta.y * _zoomSpeed * Time.deltaTime;
            _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoomDistance, _maxZoomDistance);

            if (_focusedEntity != -1 && EntityDatabase.Instance.boundsComponents.data[_focusedEntity] != null)
                _zoomDistance = Mathf.Clamp(_zoomDistance, Mathf.Max(_minZoomDistance, EntityDatabase.Instance.boundsComponents.data[_focusedEntity].bounds.extents.MaxComponent()) * 1.1f, _maxZoomDistance);
            if (_focusedObject != null) _focusedPosition = _focusedObject.position;
            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rot * Vector3.back * _zoomDistance;
            Vector3 targetPosition = _focusedPosition + offset;
            
            transform.position = Vector3.Slerp(transform.position, targetPosition, _orbitSmoothTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_focusedObject.transform.position - transform.position), _orbitSpeed);
        }

        public void SetFocusedObject(Transform focusedObject)
        {
            _focusedObject = focusedObject;
            _focusedPosition = focusedObject.position;
            if (focusedObject.TryGetComponent(out ConvertToEntity convertToEntity)) _focusedEntity = convertToEntity.generatedEntity;
        }

        private void PostStart()
        {
            SetFocusedObject(_focusedObject);
            _postStartWasCalled = true;
        }
    }
}