using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbound
{
    [RequireComponent(typeof(SphereCollider))]
    public class LanternBatteryStation : MonoBehaviour
    {
        [SerializeField] private StationConfig config;
        [SerializeField] private InputActionAsset inputActions;

        private SphereCollider _zone;
        private LanternChargeTracker _tracker;
        private InputAction _interactAction;

        private void Awake()
        {
            _zone = GetComponent<SphereCollider>();
            _zone.isTrigger = true;
            _zone.radius = config.zoneRadius;

            _tracker = new LanternChargeTracker(config.chargeTime);

            var flightMap = inputActions.FindActionMap("Flight", throwIfNotFound: true);
            _interactAction = flightMap.FindAction("Interact");
        }

        private void OnEnable()
        {
            _interactAction.Enable();
        }

        private void OnDisable()
        {
            _interactAction.Disable();
        }

        private void OnTriggerStay(Collider other)
        {
            var player = other.GetComponentInParent<PlayerRingEnergy>();
            if (player == null) return;

            if (player.Ring.Current >= player.Ring.Max)
            {
                _tracker.CancelCharge(player);
                return;
            }

            if (!_interactAction.IsPressed())
            {
                _tracker.CancelCharge(player);
                return;
            }

            if (_tracker.Tick(player, Time.fixedDeltaTime))
            {
                player.Ring.SetFull();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponentInParent<PlayerRingEnergy>();
            if (player == null) return;

            _tracker.CancelCharge(player);
        }

        public float GetProgress01(PlayerRingEnergy player)
        {
            return _tracker.GetProgress01(player);
        }
    }
}
