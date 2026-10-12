using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    /// <summary>
    /// Owns the Flying/Grounded state: lands automatically on contact with a
    /// Landable-tagged surface, takes off again on the Launch action (the same
    /// double-tap-Shift impulse used mid-flight). Toggles FlightController and
    /// SurfaceWalker so exactly one movement mode is ever active.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SurfaceAttach : MonoBehaviour
    {
        private const float LandingCooldown = 0.5f;

        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody _rigidbody;
        private FirstPersonLook _look;
        private FlightController _flightController;
        private SurfaceWalker _surfaceWalker;
        private InputAction _moveAction;
        private InputAction _launchAction;

        private bool _isGrounded;
        private float _cooldownUntil;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _look = GetComponent<FirstPersonLook>();
            _flightController = GetComponent<FlightController>();
            _surfaceWalker = GetComponent<SurfaceWalker>();

            _moveAction = inputActions.FlightAction("Move");
            _launchAction = inputActions.FlightAction("Launch");
        }

        private void OnEnable()
        {
            // SurfaceAttach is the single owner of Move/Launch's global enabled
            // state: both are shared between FlightController and SurfaceWalker,
            // and InputAction enable/disable isn't reference-counted, so having
            // more than one owner toggling it causes exactly the kind of "stuck
            // after takeoff" bug this centralizes away.
            _moveAction.Enable();
            _launchAction.Enable();
            _launchAction.performed += OnLaunchPerformed;

            // Start standing, per the design spec for this scene.
            SetGrounded(true, Vector3.up);
        }

        private void OnDisable()
        {
            _launchAction.performed -= OnLaunchPerformed;
            _launchAction.Disable();
            _moveAction.Disable();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isGrounded) return;
            if (Time.time < _cooldownUntil) return;
            if (!collision.collider.CompareTag(GameTags.Landable)) return;

            SetGrounded(true, collision.GetContact(0).normal);
        }

        private void OnLaunchPerformed(InputAction.CallbackContext context)
        {
            if (!_isGrounded) return;

            Vector3 departureDirection = _look.Up;
            SetGrounded(false, Vector3.up);
            _rigidbody.AddForce(departureDirection * config.launchImpulse, ForceMode.Impulse);
            _cooldownUntil = Time.time + LandingCooldown;
        }

        private void SetGrounded(bool grounded, Vector3 up)
        {
            _isGrounded = grounded;
            _rigidbody.linearVelocity = Vector3.zero;
            _look.SetUp(up);
            _flightController.enabled = !grounded;
            _surfaceWalker.enabled = grounded;
        }
    }
}
