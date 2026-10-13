using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    /// <summary>
    /// Grounded WASD movement. Uses the root transform's yaw-only forward/right
    /// (not the camera's pitch) so looking up/down doesn't walk you into the air
    /// or the ground. Enabled only while standing; SurfaceAttach toggles it.
    /// Also owns the single-tap-Space jump: a brief ballistic hop with a forward
    /// momentum boost, using a locally-simulated "gravity" since flight is
    /// otherwise zero-g and nothing would normally pull the player back down.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SurfaceWalker : MonoBehaviour
    {
        [SerializeField] private FlightConfig config;

        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private bool _isJumping;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            var actions = GetComponent<PlayerInputActions>().Actions;
            _moveAction = actions.FlightAction("Move");
            _jumpAction = actions.FlightAction("Jump");
        }

        // Move's global enabled state is owned by SurfaceAttach (shared with
        // FlightController too) — this script just reads it while it's enabled.
        // Jump is exclusive to this script, so it's safe to own directly here.

        private void OnEnable()
        {
            _jumpAction.Enable();
            _jumpAction.performed += OnJumpPerformed;
            _isJumping = false;
        }

        private void OnDisable()
        {
            _jumpAction.performed -= OnJumpPerformed;
            _jumpAction.Disable();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (_isJumping) return;

            Vector2 move = _moveAction.ReadValue<Vector2>();
            Vector3 direction = move.sqrMagnitude > 0.0001f
                ? (transform.forward * move.y + transform.right * move.x).normalized
                : transform.forward;

            _rigidbody.linearVelocity = direction * (config.walkSpeed + config.jumpForwardBoost) + Vector3.up * config.jumpSpeed;
            _isJumping = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isJumping && collision.collider.CompareTag(GameTags.Landable))
            {
                _isJumping = false;
            }
        }

        private void FixedUpdate()
        {
            if (_isJumping)
            {
                // No global gravity (zero-g) — simulate a local pull back down just
                // for the duration of the jump's arc.
                _rigidbody.AddForce(Vector3.down * config.jumpGravity, ForceMode.Acceleration);
                return;
            }

            Vector2 move = _moveAction.ReadValue<Vector2>();
            Vector3 direction = transform.forward * move.y + transform.right * move.x;
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            _rigidbody.linearVelocity = direction * config.walkSpeed;
        }
    }
}
