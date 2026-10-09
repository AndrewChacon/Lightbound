using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    /// <summary>
    /// Grounded WASD movement. Uses the root transform's yaw-only forward/right
    /// (not the camera's pitch) so looking up/down doesn't walk you into the air
    /// or the ground. Enabled only while standing; SurfaceAttach toggles it.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SurfaceWalker : MonoBehaviour
    {
        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody _rigidbody;
        private InputAction _moveAction;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _moveAction = flightMap.FindAction("Move");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
        }

        private void FixedUpdate()
        {
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
