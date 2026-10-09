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
        private InputAction _launchAction;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = config.linearDamping;
            _rigidbody.angularDamping = config.angularDamping;
            // Rotation is driven by FirstPersonLook each frame, not physics torque.
            _rigidbody.freezeRotation = true;
            // Discrete detection can tunnel straight through thin colliders (like the
            // station's capsule) at flight speeds, so sweep for collisions instead.
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _moveAction = flightMap.FindAction("Move");
            _launchAction = flightMap.FindAction("Launch");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _launchAction.Enable();
            _launchAction.performed += OnLaunchPerformed;
        }

        private void OnDisable()
        {
            _launchAction.performed -= OnLaunchPerformed;
            _moveAction.Disable();
            _launchAction.Disable();
        }

        private void FixedUpdate()
        {
            Vector2 move = _moveAction.ReadValue<Vector2>();

            if (move.sqrMagnitude > 0.0001f)
            {
                Transform facing = cameraPivot != null ? cameraPivot : transform;
                Vector3 direction = facing.forward * move.y + facing.right * move.x;
                _rigidbody.AddForce(direction * config.thrustForce, ForceMode.Force);
            }
            else
            {
                // Active brake once input releases, on top of the ambient linear
                // damping used while thrusting. stopDragFactor is the fraction of
                // velocity retained after a full second with no input.
                float decay = Mathf.Pow(config.stopDragFactor, Time.fixedDeltaTime);
                _rigidbody.linearVelocity *= decay;
            }
        }

        private void OnLaunchPerformed(InputAction.CallbackContext context)
        {
            Transform facing = cameraPivot != null ? cameraPivot : transform;
            _rigidbody.AddForce(facing.forward * config.launchImpulse, ForceMode.Impulse);
        }
    }
}
