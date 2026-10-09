using UnityEngine;

namespace Lightbound
{
    [RequireComponent(typeof(SphereCollider))]
    public class LanternBatteryStation : MonoBehaviour
    {
        [SerializeField] private StationConfig config;

        private SphereCollider _zone;
        private LanternChargeTracker _tracker;

        private void Awake()
        {
            _zone = GetComponent<SphereCollider>();
            _zone.isTrigger = true;
            _zone.radius = config.zoneRadius;

            _tracker = new LanternChargeTracker(config.chargeTime);
        }

        private void OnTriggerStay(Collider other)
        {
            var player = other.GetComponentInParent<PlayerRingEnergy>();
            if (player == null) return;

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
