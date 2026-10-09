using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlightController : MonoBehaviour
    {
        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform cameraPivot;

        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _launchAction;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = config.linearDamping;
            _rigidbody.angularDamping = config.angularDamping;
            // Rotation is driven by mouse look each frame, not physics torque.
            _rigidbody.freezeRotation = true;

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _moveAction = flightMap.FindAction("Move");
            _lookAction = flightMap.FindAction("Look");
            _launchAction = flightMap.FindAction("Launch");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _lookAction.Enable();
            _launchAction.Enable();
            _launchAction.performed += OnLaunchPerformed;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            _launchAction.performed -= OnLaunchPerformed;
            _moveAction.Disable();
            _lookAction.Disable();
            _launchAction.Disable();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            Vector2 look = _lookAction.ReadValue<Vector2>() * config.mouseSensitivity;
            _yaw += look.x;
            _pitch = Mathf.Clamp(_pitch - look.y, config.minPitch, config.maxPitch);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void FixedUpdate()
        {
            Vector2 move = _moveAction.ReadValue<Vector2>();
            Transform facing = cameraPivot != null ? cameraPivot : transform;
            Vector3 direction = facing.forward * move.y + facing.right * move.x;

            _rigidbody.AddForce(direction * config.thrustForce, ForceMode.Force);
        }

        private void OnLaunchPerformed(InputAction.CallbackContext context)
        {
            Transform facing = cameraPivot != null ? cameraPivot : transform;
            _rigidbody.AddForce(facing.forward * config.launchImpulse, ForceMode.Impulse);
        }
    }
}
