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
        private const string LandableTag = "Landable";
        private const float LandingCooldown = 0.5f;

        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody _rigidbody;
        private FirstPersonLook _look;
        private FlightController _flightController;
        private SurfaceWalker _surfaceWalker;
        private InputAction _launchAction;

        private bool _isGrounded;
        private float _cooldownUntil;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _look = GetComponent<FirstPersonLook>();
            _flightController = GetComponent<FlightController>();
            _surfaceWalker = GetComponent<SurfaceWalker>();

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _launchAction = flightMap.FindAction("Launch");
        }

        private void OnEnable()
        {
            _launchAction.Enable();
            _launchAction.performed += OnLaunchPerformed;

            // Start standing, per the design spec for this scene.
            SetGrounded(true, Vector3.up);
        }

        private void OnDisable()
        {
            _launchAction.performed -= OnLaunchPerformed;
            _launchAction.Disable();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isGrounded) return;
            if (Time.time < _cooldownUntil) return;
            if (!collision.collider.CompareTag(LandableTag)) return;

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
