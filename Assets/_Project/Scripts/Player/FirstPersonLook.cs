using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    /// <summary>
    /// Mouse look and cursor lock, shared by both flying and ground walking.
    /// Stays enabled continuously across mode switches so looking around never cuts out.
    /// </summary>
    public class FirstPersonLook : MonoBehaviour
    {
        [SerializeField] private FlightConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform cameraPivot;

        private InputAction _lookAction;
        private float _yaw;
        private float _pitch;

        public Vector3 Up { get; private set; } = Vector3.up;

        private void Awake()
        {
            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _lookAction = flightMap.FindAction("Look");
        }

        private void OnEnable()
        {
            _lookAction.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            _lookAction.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            Vector2 look = _lookAction.ReadValue<Vector2>() * config.mouseSensitivity;
            _yaw += look.x;
            _pitch = Mathf.Clamp(_pitch - look.y, config.minPitch, config.maxPitch);

            // Base orientation aligns world-up to the current surface's Up (identity
            // while flying or standing on a flat floor), yaw then turns around that
            // same Up axis so landings on non-flat surfaces orient correctly later.
            Quaternion baseRotation = Quaternion.FromToRotation(Vector3.up, Up);
            transform.rotation = Quaternion.AngleAxis(_yaw, Up) * baseRotation;

            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        public void SetUp(Vector3 up)
        {
            Up = up;
        }
    }
}
