using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlightController : MonoBehaviour
    {
        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private InputAction _launchAction;
        private InputAction _ascendAction;
        private InputAction _descendAction;
        private float _fallBoostUntil;

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
            _ascendAction = flightMap.FindAction("Ascend");
            _descendAction = flightMap.FindAction("Descend");
        }

        private void OnEnable()
        {
            // Move/Launch's global enabled state is owned by SurfaceAttach, since
            // both actions are shared with SurfaceWalker/SurfaceAttach too and
            // InputAction enable/disable isn't reference-counted across listeners.
            // Ascend/Descend are exclusive to this script, so it's safe to own them
            // directly here.
            _ascendAction.Enable();
            _descendAction.Enable();
            _launchAction.performed += OnLaunchPerformed;
        }

        private void OnDisable()
        {
            _launchAction.performed -= OnLaunchPerformed;
            _ascendAction.Disable();
            _descendAction.Disable();
        }

        private void FixedUpdate()
        {
            float decay = Mathf.Pow(config.stopDragFactor, Time.fixedDeltaTime);
            Vector3 v = _rigidbody.linearVelocity;

            Vector2 move = _moveAction.ReadValue<Vector2>();
            if (move.sqrMagnitude > 0.0001f)
            {
                // Yaw-only facing (not the camera's pitch) — looking up/down no
                // longer affects horizontal movement, matching SurfaceWalker.
                Vector3 direction = transform.forward * move.y + transform.right * move.x;
                _rigidbody.AddForce(direction * config.thrustForce, ForceMode.Force);
            }
            else
            {
                // Active brake on horizontal drift once Move releases.
                v = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = new Vector3(v.x * decay, v.y, v.z * decay);
            }

            bool ascend = _ascendAction.IsPressed();
            bool descend = _descendAction.IsPressed();
            if (ascend || descend)
            {
                float vertical = (ascend ? 1f : 0f) - (descend ? 1f : 0f);
                _rigidbody.AddForce(Vector3.up * vertical * config.thrustForce, ForceMode.Force);
            }
            else if (Time.time >= _fallBoostUntil)
            {
                // Same brake on vertical drift once Ascend/Descend release, mirroring
                // horizontal — except while the fall-to-ground kick below is still
                // active, so it isn't killed the instant you let go of Shift.
                v = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = new Vector3(v.x, v.y * decay, v.z);
            }
        }

        private void OnLaunchPerformed(InputAction.CallbackContext context)
        {
            // While flying, double-tap Shift drops you fast until the existing
            // auto-landing takes over (SurfaceAttach handles the same action's
            // grounded-state case — takeoff — separately). Protected from the
            // vertical brake for a bit so it's a real drop, not an instant stop.
            Vector3 v = _rigidbody.linearVelocity;
            _rigidbody.linearVelocity = new Vector3(v.x, -config.fallSpeed, v.z);
            _fallBoostUntil = Time.time + config.fallBoostDuration;
        }
    }
}
